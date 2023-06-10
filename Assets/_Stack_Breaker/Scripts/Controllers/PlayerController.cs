using System.Collections;
using UnityEngine;
using CBGames;

public class PlayerController : MonoBehaviour
{

    public static PlayerController Instance { private set; get; }
    public static event System.Action<PlayerState> PlayerStateChanged = delegate { };

    public PlayerState PlayerState
    {
        get
        {
            return playerState;
        }

        private set
        {
            if (value != playerState)
            {
                value = playerState;
                PlayerStateChanged(playerState);
            }
        }
    }


    private PlayerState playerState = PlayerState.Player_Prepare;


    [Header("Player Config")]
    [SerializeField] private float jumpUpVelocity = 15;
    [SerializeField] private float fallDownVelocity = -50;
    [SerializeField] private float minScale = 0.85f;
    [SerializeField] private float maxScale = 1.25f;
    [SerializeField] private float scalingFactor = 2;
    [SerializeField] private float timeCountToEnableImmortalMode = 0.3f;
    [SerializeField] private float immortalModeTime = 3f;

    [Header("Player References")]
    [SerializeField] private MeshRenderer meshRender = null;
    [SerializeField] private Material playerMaterial = null;
    [SerializeField] private GameObject trailEffect = null;
    [SerializeField] private ParticleSystem explodeEffect = null;
    [SerializeField] private ParticleSystem fireEffect = null;

    public float TargetY { private set; get; }
    public float ImmortalModeTime { get { return immortalModeTime; } }


    private StackController closestStackControl = null;
    private Vector3 originalScale = Vector3.zero;
    private float lastSavedYPos = 0;
    private float currentJumpVelocity = 0;
    private float closestYAxis = -1;
    private float currentTimeCount = 0;
    private bool isPaused = false;
    private bool isTouchingScreen = false;
    private bool isImmortal = false;

    private void OnEnable()
    {
        IngameManager.GameStateChanged += GameManager_GameStateChanged;
    }
    private void OnDisable()
    {
        IngameManager.GameStateChanged -= GameManager_GameStateChanged;
    }

    private void GameManager_GameStateChanged(IngameState obj)
    {
        if (obj == IngameState.Ingame_Playing)
        {
            Player_Living();
        }
        else if (obj == IngameState.Ingame_CompletedLevel)
        {
            Player_CompletedLevel();
        }
    }



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(Instance.gameObject);
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }




    private void Start()
    {

        //Fire event
        PlayerState = PlayerState.Player_Prepare;
        playerState = PlayerState.Player_Prepare;

        //Setting parameters
        isImmortal= false;

        //Disable objects
        explodeEffect.gameObject.SetActive(false);
        fireEffect.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (playerState == PlayerState.Player_Died)
        {
            return;
        }

        transform.position = transform.position + Vector3.up * (currentJumpVelocity * Time.deltaTime + fallDownVelocity * Time.deltaTime * Time.deltaTime / 2);

        if (currentJumpVelocity < fallDownVelocity)
            currentJumpVelocity = fallDownVelocity;
        else
            currentJumpVelocity = currentJumpVelocity + fallDownVelocity * Time.deltaTime;

        if (currentJumpVelocity < 0)
        {
            //Adjust scale
            Vector3 scale = transform.localScale;
            if (scale.x > minScale)
            {
                scale.x -= scalingFactor * Time.deltaTime;
            }
            else
                scale.x = minScale;
            transform.localScale = scale;


            //Get the closest platform
            if (closestYAxis == -1)
            {
                closestYAxis = IngameManager.Instance.GetTopYAxisOfClosestPlatform(out closestStackControl);
            }

            //Calculate the distance
            float bottomY = (transform.position + Vector3.down * (meshRender.bounds.size.y / 2f)).y;
            float distance = bottomY - closestYAxis;
            if (distance <= 0.1f)
            {
                if (closestStackControl == null) //Player already gone through all stacks -> completed level
                {
                    currentJumpVelocity = jumpUpVelocity;
                    if (playerState == PlayerState.Player_Living)
                    {
                        Player_CompletedLevel();
                        IngameManager.Instance.CompletedLevel();
                    }
                }
                else
                {
                    TargetY = closestYAxis;

                    if (isImmortal) //Is on immortal mode
                    {
                        if (!isTouchingScreen)
                        {
                            ServicesManager.Instance.SoundManager.PlayOneSound(ServicesManager.Instance.SoundManager.jump);
                            currentJumpVelocity = jumpUpVelocity;

                            //Create splashes
                            Vector3 splashesPos = new Vector3(transform.position.x, closestYAxis + 0.05f, transform.position.z);
                            EffectManager.Instance.CreateSplashesTextureEffect(splashesPos, playerMaterial.color, closestStackControl.transform);
                            EffectManager.Instance.CreateSplashesDustEffect(splashesPos);
                        }
                        else
                        {
                            ServicesManager.Instance.SoundManager.PlayOneSound(ServicesManager.Instance.SoundManager.immortalBreakStack);
                            StackPartController closestStackPart = closestStackControl.GetClosestStackPartController();
                            closestStackControl.ShatterAllParts();

                            //Create effect
                            EffectManager.Instance.CreateFadingStackEffect(closestStackPart.transform.position);

                            closestYAxis = -1;
                            closestStackControl = null;
                        }
                    }
                    else
                    {
                        if (!isTouchingScreen)
                        {
                            ServicesManager.Instance.SoundManager.PlayOneSound(ServicesManager.Instance.SoundManager.jump);
                            currentTimeCount = Mathf.Clamp(currentTimeCount - Time.deltaTime, 0, timeCountToEnableImmortalMode);
                            currentJumpVelocity = jumpUpVelocity;

                            //Create splashes effects
                            Vector3 splashesPos = new Vector3(transform.position.x, closestYAxis + 0.05f, transform.position.z);
                            EffectManager.Instance.CreateSplashesTextureEffect(splashesPos, playerMaterial.color, closestStackControl.transform);
                            EffectManager.Instance.CreateSplashesDustEffect(splashesPos);
                        }
                        else
                        {
                            StackPartController closestStackPart = closestStackControl.GetClosestStackPartController();
                            if (closestStackPart.IsDeadlyPart)
                            {
                                lastSavedYPos = closestYAxis;
                                Player_Died();
                            }
                            else
                            {
                                currentTimeCount += Time.deltaTime;
                                if (currentTimeCount >= timeCountToEnableImmortalMode)
                                {
                                    currentTimeCount = timeCountToEnableImmortalMode;
                                    StartCoroutine(CRCountingImmortalMode());
                                }

                                ServicesManager.Instance.SoundManager.PlayOneSound(ServicesManager.Instance.SoundManager.normalBreakStack);
                                closestStackControl.ShatterAllParts();
                            }
                            closestYAxis = -1;
                            closestStackControl = null;
                        }
                    }
                }
            }
        }
        else
        {
            //Adjust scale
            Vector3 scale = transform.localScale;
            if (scale.x < maxScale)
            {
                scale.x += scalingFactor * Time.deltaTime;
            }
            else
                scale.x = maxScale;
            transform.localScale = scale;

        }


        if (playerState == PlayerState.Player_Living)
        {
            if (Input.GetMouseButton(0))
            {
                isTouchingScreen = true;
            }

            if (Input.GetMouseButtonUp(0) && isTouchingScreen)
            {
                isTouchingScreen = false;
            }
        }

        ViewManager.Instance.IngameViewController.PlayingViewControl.UpdateImmortalModeTimeView(currentTimeCount, timeCountToEnableImmortalMode);
    }

    private void Player_Living()
    {
        //Fire event
        PlayerState = PlayerState.Player_Living;
        playerState = PlayerState.Player_Living;

        //Other actions
        if (!isPaused)
        {
            if (IngameManager.Instance.IsRevived)
            {
                //Reset values
                meshRender.enabled = true;
                explodeEffect.gameObject.SetActive(false);
                fireEffect.gameObject.SetActive(false);
                trailEffect.SetActive(true);
                currentTimeCount = 0;
                transform.position = new Vector3(transform.position.x, lastSavedYPos, transform.position.z);
            }
        }
    }

    private void Player_Died()
    {
        //Fire event
        PlayerState = PlayerState.Player_Died;
        playerState = PlayerState.Player_Died;

        //Other actions
        ServicesManager.Instance.SoundManager.PlayOneSound(ServicesManager.Instance.SoundManager.playerDied);
        isTouchingScreen = false;
        meshRender.enabled = false;
        explodeEffect.gameObject.SetActive(true);
        explodeEffect.Play();
        trailEffect.SetActive(false);
        currentTimeCount = 0;
        StartCoroutine(CRHandleState_Player_Dead());
    }


    private void Player_CompletedLevel()
    {
        //Fire event
        PlayerState = PlayerState.Player_CompletedLevel;
        playerState = PlayerState.Player_CompletedLevel;
    }

    /// <summary>
    /// Wait a delay time and call Ingame states.
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator CRHandleState_Player_Dead()
    {
        yield return null;
        if (!IngameManager.Instance.IsRevived && ServicesManager.Instance.AdManager.IsRewardedVideoAdReady())
        {
            //Call Revive state 
            IngameManager.Instance.Revive();
        }
        else
        {
            //Call GameOver state
            IngameManager.Instance.GameOver();
        }
    }

    /// <summary>
    /// Counting to disable immortal mode.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CRCountingImmortalMode()
    {
        isImmortal = true;
        fireEffect.gameObject.SetActive(true);
        fireEffect.Play();
        float timeTemp = immortalModeTime;
        while (timeTemp > 0)
        {
            yield return null;
            timeTemp -= Time.deltaTime;
            currentTimeCount = 0;
        }

        currentTimeCount = 0;
        isImmortal = false;
        fireEffect.Stop(true);
        yield return new WaitForSeconds(1f);
        fireEffect.gameObject.SetActive(false);
    }





    /// <summary>
    /// Set player coor by given color.
    /// </summary>
    /// <param name="color"></param>
    public void SetPlayerColor(Color color)
    {
        playerMaterial.color = color;
    }


}
