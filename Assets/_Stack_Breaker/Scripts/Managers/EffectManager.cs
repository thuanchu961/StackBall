using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EffectManager : MonoBehaviour {

    public static EffectManager Instance { private set; get; }

    [SerializeField] private GameObject fadingStackPrefab = null;
    [SerializeField] private GameObject splashesTexturePrefab = null;
    [SerializeField] private GameObject splashesDustPrefab = null;

    private List<FadingStackController> listFadingStackControl = new List<FadingStackController>();
    private List<SplashesTextureController> listSplashesTextureControl = new List<SplashesTextureController>();
    private List<ParticleSystem> listSplashesDustParticle = new List<ParticleSystem>();

    void Awake()
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

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }


    /// <summary>
    /// Play the given particle then disable it 
    /// </summary>
    /// <param name="par"></param>
    /// <returns></returns>
    private IEnumerator CRPlayParticle(ParticleSystem par)
    {
        par.Play();
        yield return new WaitForSeconds(par.main.startLifetimeMultiplier);
        par.gameObject.SetActive(false);
    }



    /// <summary>
    /// Create a fading stack effect at given position.
    /// </summary>
    /// <param name="pos"></param>
    public void CreateFadingStackEffect(Vector3 pos)
    {
        //Find in the list
        FadingStackController fadingStack = listFadingStackControl.Where(a => !a.gameObject.activeInHierarchy).FirstOrDefault();

        //Didn't find one -> create new one
        if (fadingStack == null)
        {
            fadingStack = Instantiate(fadingStackPrefab, Vector3.zero, Quaternion.identity).GetComponent<FadingStackController>();
            listFadingStackControl.Add(fadingStack);
        }

        fadingStack.transform.position = pos;
        fadingStack.gameObject.SetActive(true);
        fadingStack.StartFade(Color.white, 2.5f, 0.5f);
    }


    /// <summary>
    /// Create a splashes texture effect at given position.
    /// </summary>
    /// <param name="pos"></param>
    public void CreateSplashesTextureEffect(Vector3 pos, Color color, Transform parent)
    {
        //Find in the list
        SplashesTextureController splashesControl = listSplashesTextureControl.Where(a => !a.gameObject.activeInHierarchy).FirstOrDefault();

        //Didn't find one -> create new one
        if (splashesControl == null)
        {
            splashesControl = Instantiate(splashesTexturePrefab, Vector3.zero, Quaternion.identity).GetComponent<SplashesTextureController>();
            listSplashesTextureControl.Add(splashesControl);
        }

        splashesControl.transform.position = pos;
        splashesControl.transform.eulerAngles = new Vector3(90, Random.Range(0, 360), 0);
        splashesControl.transform.SetParent(parent);
        splashesControl.gameObject.SetActive(true);
        splashesControl.FadeOut(color);
    }


    /// <summary>
    /// Create a splashes dust effect at given position.
    /// </summary>
    /// <param name="pos"></param>
    public void CreateSplashesDustEffect(Vector3 pos)
    {
        //Find in the list
        ParticleSystem splashesDust = listSplashesDustParticle.Where(a => !a.gameObject.activeInHierarchy).FirstOrDefault();

        //Didn't find one -> create new one
        if (splashesDust == null)
        {
            splashesDust = Instantiate(splashesDustPrefab, Vector3.zero, Quaternion.identity).GetComponent<ParticleSystem>();
            listSplashesDustParticle.Add(splashesDust);
        }

        splashesDust.transform.position = pos;
        splashesDust.gameObject.SetActive(true);
        StartCoroutine(CRPlayParticle(splashesDust));
    }
}
