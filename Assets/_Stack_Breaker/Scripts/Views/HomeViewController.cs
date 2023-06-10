using CBGames;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;


public class HomeViewController : MonoBehaviour {

    [SerializeField] private RectTransform gameNameTrans = null;
    [SerializeField] private RectTransform currentLevelTxtTrans = null;
    [SerializeField] private RectTransform tapToPlayTextTrans = null;
    [SerializeField] private RectTransform settingButtonViewTrans = null;
    [SerializeField] private RectTransform soundButtonsTrans = null;
    [SerializeField] private RectTransform musicButtonsTrans = null;
    [SerializeField] private GameObject soundOnButtonView = null;
    [SerializeField] private GameObject soundOffButtonView = null;
    [SerializeField] private GameObject musicOnButtonView = null;
    [SerializeField] private GameObject musicOffButtonView = null;
    [SerializeField] private Text tapToPlayText = null;
    [SerializeField] private Text currentLevelTxt = null;

    private int settingButtonTurn = 1;
    private int servicesButtonTurn = 1;
    public void OnShow()
    {

        ViewManager.Instance.MoveRect(gameNameTrans, gameNameTrans.anchoredPosition, new Vector2(0, gameNameTrans.anchoredPosition.y), 0.5f);
        ViewManager.Instance.MoveRect(currentLevelTxtTrans, currentLevelTxtTrans.anchoredPosition, new Vector2(0, currentLevelTxtTrans.anchoredPosition.y), 0.5f);
        ViewManager.Instance.ScaleRect(tapToPlayTextTrans, Vector2.zero, Vector2.one, 0.5f);
        ViewManager.Instance.MoveRect(settingButtonViewTrans, settingButtonViewTrans.anchoredPosition, new Vector2(0, settingButtonViewTrans.anchoredPosition.y), 0.5f);


        settingButtonTurn = 1;
        servicesButtonTurn = 1;

        currentLevelTxt.text = "LEVEL: " + PlayerPrefs.GetInt(PlayerPrefsKey.SAVED_LEVEL_PPK, 1).ToString();

        //Update sound btns
        if (ServicesManager.Instance.SoundManager.IsSoundOff())
        {
            soundOnButtonView.gameObject.SetActive(false);
            soundOffButtonView.gameObject.SetActive(true);
        }
        else
        {
            soundOnButtonView.gameObject.SetActive(true);
            soundOffButtonView.gameObject.SetActive(false);
        }

        //Update music btns
        if (ServicesManager.Instance.SoundManager.IsMusicOff())
        {
            musicOffButtonView.gameObject.SetActive(true);
            musicOnButtonView.gameObject.SetActive(false);
        }
        else
        {
            musicOffButtonView.gameObject.SetActive(false);
            musicOnButtonView.gameObject.SetActive(true);
        }

        StartCoroutine(CRFadingTapToPlayText());
    }

    private void OnDisable()
    {
        gameNameTrans.anchoredPosition = new Vector2(-700, gameNameTrans.anchoredPosition.y);
        currentLevelTxtTrans.anchoredPosition = new Vector2(-600, currentLevelTxtTrans.anchoredPosition.y);
        tapToPlayTextTrans.localScale = Vector2.zero;
        settingButtonViewTrans.anchoredPosition = new Vector2(-150, settingButtonViewTrans.anchoredPosition.y);
        soundButtonsTrans.anchoredPosition = new Vector2(-150, soundButtonsTrans.anchoredPosition.y);
        musicButtonsTrans.anchoredPosition = new Vector2(-150, musicButtonsTrans.anchoredPosition.y);
    }

    private IEnumerator CRFadingTapToPlayText()
    {
        Color startColor = new Color(tapToPlayText.color.r, tapToPlayText.color.g, tapToPlayText.color.b, 0.25f);
        Color endColor = new Color(tapToPlayText.color.r, tapToPlayText.color.g, tapToPlayText.color.b, 0.75f);
        float fadingTime = 0.3f;
        float t = 0;
        while (true)
        {
            t = 0;
            while (t < fadingTime)
            {
                t += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(LerpType.Liner, t / fadingTime);
                tapToPlayText.color = Color.Lerp(startColor, endColor, factor);
                yield return null;
            }

            t = 0;
            while (t < fadingTime)
            {
                t += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(LerpType.Liner, t / fadingTime);
                tapToPlayText.color = Color.Lerp(endColor, startColor, factor);
                yield return null;
            }
        }
    }

    //////////////////////////////////////////////////////////////////////UI Functions


    public void PlayBtn()
    {
        ViewManager.Instance.PlayClickButtonSound();
        ViewManager.Instance.LoadScene("Ingame", 0.3f);
    }

    public void NativeShareBtn()
    {
        ViewManager.Instance.PlayClickButtonSound();
    }

    public void SettingBtn()
    {
        settingButtonTurn *= -1;
        StartCoroutine(CRHandleSettingBtn());
    }
    private IEnumerator CRHandleSettingBtn()
    {
        ViewManager.Instance.PlayClickButtonSound();
        if (settingButtonTurn == -1)
        {
            ViewManager.Instance.MoveRect(soundButtonsTrans, soundButtonsTrans.anchoredPosition, new Vector2(0, soundButtonsTrans.anchoredPosition.y), 0.5f);
            yield return new WaitForSeconds(0.08f);
            ViewManager.Instance.MoveRect(musicButtonsTrans, musicButtonsTrans.anchoredPosition, new Vector2(0, musicButtonsTrans.anchoredPosition.y), 0.5f);
            yield return new WaitForSeconds(0.08f);
        }
        else
        {
            ViewManager.Instance.MoveRect(soundButtonsTrans, soundButtonsTrans.anchoredPosition, new Vector2(-150, soundButtonsTrans.anchoredPosition.y), 0.5f);
            yield return new WaitForSeconds(0.08f);
            ViewManager.Instance.MoveRect(musicButtonsTrans, musicButtonsTrans.anchoredPosition, new Vector2(-150, musicButtonsTrans.anchoredPosition.y), 0.5f);
            yield return new WaitForSeconds(0.08f);
        }
    }


    public void ServicesBtn()
    {
        servicesButtonTurn *= -1;
    }

    public void ToggleSound()
    {
        ViewManager.Instance.PlayClickButtonSound();
        ServicesManager.Instance.SoundManager.ToggleSound();
        if (ServicesManager.Instance.SoundManager.IsSoundOff())
        {
            soundOnButtonView.gameObject.SetActive(false);
            soundOffButtonView.gameObject.SetActive(true);
        }
        else
        {
            soundOnButtonView.gameObject.SetActive(true);
            soundOffButtonView.gameObject.SetActive(false);
        }
    }

    public void ToggleMusic()
    {
        ViewManager.Instance.PlayClickButtonSound();
        ServicesManager.Instance.SoundManager.ToggleMusic();
        if (ServicesManager.Instance.SoundManager.IsMusicOff())
        {
            musicOffButtonView.gameObject.SetActive(true);
            musicOnButtonView.gameObject.SetActive(false);
        }
        else
        {
            musicOffButtonView.gameObject.SetActive(false);
            musicOnButtonView.gameObject.SetActive(true);
        }
    }

    public void FacebookShareBtn()
    {
        ViewManager.Instance.PlayClickButtonSound();
    }
    public void TwitterShareBtn()
    {
        ViewManager.Instance.PlayClickButtonSound();
    }
    public void RateAppBtn()
    {
        ViewManager.Instance.PlayClickButtonSound();
    }
    public void CloseSettingViewBtn()
    {
        ViewManager.Instance.PlayClickButtonSound();
    }
}
