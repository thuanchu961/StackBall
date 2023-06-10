using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CBGames;
using UnityEngine.SceneManagement;
using System.Linq;

public class IngameManager : MonoBehaviour
{

    public static IngameManager Instance { private set; get; }
    public static event System.Action<IngameState> GameStateChanged = delegate { };
    public static int EnvironmentIndex { private set; get; }

    public IngameState IngameState
    {
        get
        {
            return gameState;
        }
        private set
        {
            if (value != gameState)
            {
                gameState = value;
                GameStateChanged(gameState);
            }
        }
    }


    [Header("Enter a number of level to test. Set back to 0 to disable this feature.")]
    [SerializeField] private int testingLevel = 0;



    [Header("Ingame Config")]
    [SerializeField] private float reviveWaitTime = 5f;
    [SerializeField] private float stackXPosition = 0;
    [SerializeField] private float stackZPosition = 4;
    [SerializeField] private float firstStackYPosition = -1;
    [SerializeField] private float stackSpace = 2;

    [Header("Levels Config")]
    [SerializeField] private List<LevelConfig> listLevelConfig = new List<LevelConfig>();


    [Header("Ingame References")]
    [SerializeField] private CenterPillarController centerPillarControl = null;
    [SerializeField] private Transform bottomPillarTrans = null;
    [SerializeField] private Transform sunTrans = null;
    [SerializeField] private Material backgroundMaterial = null;
    [SerializeField] private Material deadlyStackPartMaterial = null;
    [SerializeField] private ParticleSystem[] completedLevelEffects = null;

    public StackController CurrentHighestStackControl { private set; get; }
    public Transform BottomPillarTrans { get { return bottomPillarTrans; } }
    public float ReviveWaitTime { get { return reviveWaitTime; } }
    public int CurrentLevel { private set; get; }
    public float TimePassed { private set; get; }
    public bool IsRevived { private set; get; }


    private IngameState gameState = IngameState.Ingame_GameOver;
    private LevelConfig currentLevelConfig = null;
    private List<StackData> listStackData = new List<StackData>();
    private List<int> listDeadlyPartIndex = new List<int>();
    private Vector3 nextStackPosition = Vector3.zero;
    private int totalStack = 0;
    private int currentBrokenStack = 0;
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

    // Use this for initialization
    private void Start()
    {

        Application.targetFrameRate = 60;
        ViewManager.Instance.OnLoadingSceneDone(SceneManager.GetActiveScene().name);

        //Setup variables
        IsRevived = false;
        nextStackPosition = new Vector3(stackXPosition, firstStackYPosition, stackZPosition);
        foreach(ParticleSystem o in completedLevelEffects)
        {
            o.gameObject.SetActive(false);
        }
        int turnValue = (Random.value <= 0.5) ? -1 : 1;
        sunTrans.localPosition = new Vector3(sunTrans.localPosition.x * turnValue, sunTrans.localPosition.y, sunTrans.localPosition.z);

        //Set current level
        if (!PlayerPrefs.HasKey(PlayerPrefsKey.SAVED_LEVEL_PPK))
        {
            PlayerPrefs.SetInt(PlayerPrefsKey.SAVED_LEVEL_PPK, 1);
            CurrentLevel = 1;
        }
        else
        {
            CurrentLevel = PlayerPrefs.GetInt(PlayerPrefsKey.SAVED_LEVEL_PPK);
        }

        if (testingLevel != 0)
            CurrentLevel = testingLevel;

        //Load parameters
        foreach (LevelConfig o in listLevelConfig)
        {
            if (CurrentLevel >= o.MinLevel && CurrentLevel < o.MaxLevel)
            {
                //Set variables
                currentLevelConfig = o;

                //Set colors
                deadlyStackPartMaterial.color = o.DeadlyPartColor;
                backgroundMaterial.SetColor("_TopColor", o.BackgroundTopColor);
                backgroundMaterial.SetColor("_BottomColor", o.BackgroundBottomColor);

                //Player parameters
                PlayerController.Instance.SetPlayerColor(o.PlayerColor);

                //Create stack datas
                CreateStackDatas(o.ListStackConfig);

                //Scale the center pillar base on listStackData
                float scaleResult = 0;
                float stackHeight = PoolManager.Instance.GetStackController(StackType.STACK_6_PARTS).GetYSize();
                float stackPackSpace = (listStackData.Count - 1) * (stackSpace - stackHeight);
                for(int i = 0; i < listStackData.Count; i++)
                {
                    float space = (listStackData[i].StackNumber - 1) * (stackSpace - stackHeight);
                    float heightOfAllStacks = listStackData[i].StackNumber * stackHeight;
                    scaleResult += space + heightOfAllStacks;
                }
                scaleResult += centerPillarControl.transform.position.y + stackPackSpace + 5.5f;
                centerPillarControl.transform.localScale = new Vector3(1, scaleResult, 1);
                bottomPillarTrans.localPosition = new Vector3(bottomPillarTrans.position.x, -(scaleResult - 5.5f), bottomPillarTrans.position.z);

                //Rotate the center pillar
                CenterPillarData centerPillarData = new CenterPillarData();
                centerPillarData.SetMinRotatingSpeed(o.MinCenterPillarRotatingSpeed);
                centerPillarData.SetMaxRotatingSpeed(o.MaxCenterPillarRotatingSpeed);
                centerPillarData.SetMinRotatingTime(o.MinCenterPillarRotatingTime);
                centerPillarData.SetMaxRotatingTime(o.MaxCenterPillarRotatingTime);
                centerPillarControl.OnSetup(centerPillarData);

                break;
            }
        }

        //Create the level
        foreach (StackData o in listStackData)
        {
            int stackCount = 0;
            Material normalPath = new Material(Shader.Find("Legacy Shaders/Self-Illumin/Diffuse"));
            normalPath.color = o.StackColor;
            for (int i = 0; i < o.StackNumber; i++)
            {
                //Create new stack
                StackController stackControl = PoolManager.Instance.GetStackController(o.StackType);
                stackControl.transform.position = nextStackPosition;
                stackControl.gameObject.SetActive(true);
                stackControl.transform.SetParent(centerPillarControl.transform);
                float yAngle = o.StackAngle + (stackCount * o.RotationChangeAmount);
                stackControl.InitValues(yAngle, o.ListIndexOfDeadlyPart, normalPath, deadlyStackPartMaterial);
                nextStackPosition = stackControl.transform.position + Vector3.down * stackSpace;
                stackCount++;
            }
        }


        PlayingGame();
    }

    /// <summary>
    /// Actual start the game (call Ingame_Playing event).
    /// </summary>
    public void PlayingGame()
    {
        //Fire event
        IngameState = IngameState.Ingame_Playing;
        gameState = IngameState.Ingame_Playing;

        //Other actions

        if (IsRevived)
        {
            ResumeBackgroundMusic(0.5f);
        }
        else
        {
            PlayBackgroundMusic(0.5f);
        }
    }


    /// <summary>
    /// Call Ingame_Revive event.
    /// </summary>
    public void Revive()
    {
        //Fire event
        IngameState = IngameState.Ingame_Revive;
        gameState = IngameState.Ingame_Revive;

        //Add another actions here
        PauseBackgroundMusic(0.5f);
    }


    /// <summary>
    /// Call Ingame_GameOver event.
    /// </summary>
    public void GameOver()
    {
        //Fire event
        IngameState = IngameState.Ingame_GameOver;
        gameState = IngameState.Ingame_GameOver;

        //Add another actions here
        StopBackgroundMusic(0f);
    }


    /// <summary>
    /// Call Ingame_CompletedLevel event.
    /// </summary>
    public void CompletedLevel()
    {
        //Fire event
        IngameState = IngameState.Ingame_CompletedLevel;
        gameState = IngameState.Ingame_CompletedLevel;

        //Other actions

        StopBackgroundMusic(0f);
        ServicesManager.Instance.SoundManager.PlayOneSound(ServicesManager.Instance.SoundManager.passedLevel);

        //Play effects
        foreach (ParticleSystem o in completedLevelEffects)
        {
            o.gameObject.SetActive(true);
            o.Play();
        }

        //Save level
        if (testingLevel == 0)
        {
            PlayerPrefs.SetInt(PlayerPrefsKey.SAVED_LEVEL_PPK, CurrentLevel + 1);
        }
    }

    private void PlayBackgroundMusic(float delay)
    {
        StartCoroutine(CRPlayBGMusic(delay));
    }

    private IEnumerator CRPlayBGMusic(float delay)
    {
        yield return new WaitForSeconds(delay);
        ServicesManager.Instance.SoundManager.PlayMusic(ServicesManager.Instance.SoundManager.background, 0.5f);
    }

    private void StopBackgroundMusic(float delay)
    {
        StartCoroutine(CRStopBGMusic(delay));
    }

    private IEnumerator CRStopBGMusic(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (ServicesManager.Instance.SoundManager.background != null)
            ServicesManager.Instance.SoundManager.StopMusic(0.5f);
    }

    private void PauseBackgroundMusic(float delay)
    {
        StartCoroutine(CRPauseBGMusic(delay));
    }

    private IEnumerator CRPauseBGMusic(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (ServicesManager.Instance.SoundManager.background != null)
            ServicesManager.Instance.SoundManager.PauseMusic();
    }

    private void ResumeBackgroundMusic(float delay)
    {
        StartCoroutine(CRResumeBGMusic(delay));
    }

    private IEnumerator CRResumeBGMusic(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (ServicesManager.Instance.SoundManager.background != null)
            ServicesManager.Instance.SoundManager.ResumeMusic();
    }



    private void CreateStackDatas(List<StackConfig> listStackConfig)
    {
        List<int> listTemp = new List<int>();
        while (listTemp.Count < listStackConfig.Count)
        {
            int index = Random.Range(0, listStackConfig.Count);
            if (!listTemp.Contains(index))
            {
                listTemp.Add(index);
            }
        }

        for (int i = 0; i < listTemp.Count; i++)
        {
            StackConfig stackConfig = listStackConfig[listTemp[i]];
            StackData stackData = new StackData();
            stackData.SetStackNumber(Random.Range(stackConfig.MinStackNumber, stackConfig.MaxStackNumber));
            stackData.SetStackType(stackConfig.StackType);
            stackData.SetStackColor(stackConfig.StackColor);
            stackData.SetListIndexOfDeadlyPart(stackConfig.ListIndexOfDeadlyPart);
            stackData.SetStackAngle(stackConfig.FirstStackAngle);
            stackData.SetRotationChangeAmount(stackConfig.RotationChangeAmount);
            listStackData.Add(stackData);
            totalStack += stackData.StackNumber;
        }
    }



    //////////////////////////////////////Publish functions

    /// <summary>
    /// Continue the game
    /// </summary>
    public void SetContinueGame()
    {
        IsRevived = true;
        PlayingGame();
    }


    /// <summary>
    /// Get the position of the platform closest to player (can only be the stack or the bottom pillar).
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public float GetTopYAxisOfClosestPlatform(out StackController stackController)
    {
        StackController[] stackControllers = FindObjectsOfType<StackController>();
        StackController result = null;
        foreach (StackController o in stackControllers)
        {
            if (!o.IsShattered)
            {
                if (result == null || (o.transform.position.y > result.transform.position.y))
                {
                    result = o;
                }
            }
        }

        if (result == null) //Out of stack -> player reached the end level
        {
            stackController = null;
            return bottomPillarTrans.position.y;
        }
        else
        {
            stackController = result;
            return result.GetTopYAxis();
        }

    }


    /// <summary>
    /// Update the currentBrokenStack and the level process on UI.
    /// </summary>
    public void UpdateCurrentBrokenStack()
    {
        currentBrokenStack++;
        ViewManager.Instance.IngameViewController.PlayingViewControl.UpdateLevelProgress(currentBrokenStack / (float)totalStack);
    }
}
