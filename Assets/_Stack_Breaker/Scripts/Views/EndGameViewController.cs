using CBGames;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EndGameViewController : MonoBehaviour {

    [SerializeField] private Text levelResultTxt = null;
    [SerializeField] private Text nextLevelTxt = null;
    [SerializeField] private RectTransform tapToNextLevelTextTrans = null;
    [SerializeField] private RectTransform tapToReplayTextTrans = null;
    [SerializeField] private RectTransform homeButtonViewTrans = null;

    public void OnShow()
    {
        if (IngameManager.Instance.IngameState == IngameState.Ingame_CompletedLevel)
        {
            levelResultTxt.text = "LEVEL COMPLETED !";
            levelResultTxt.color = Color.green;

            tapToReplayTextTrans.gameObject.SetActive(false);
            tapToNextLevelTextTrans.gameObject.SetActive(true);
            ViewManager.Instance.ScaleRect(tapToNextLevelTextTrans, Vector2.zero, Vector2.one, 0.75f);

        }
        else if (IngameManager.Instance.IngameState == IngameState.Ingame_GameOver)
        {
            levelResultTxt.text = "LEVEL FAILED !";
            levelResultTxt.color = Color.red;

            tapToReplayTextTrans.gameObject.SetActive(true);
            tapToNextLevelTextTrans.gameObject.SetActive(false);
            ViewManager.Instance.ScaleRect(tapToReplayTextTrans, Vector2.zero, Vector2.one, 0.75f);
        }

        RectTransform levelResultTxtTrans = levelResultTxt.rectTransform;
        ViewManager.Instance.MoveRect(levelResultTxtTrans, levelResultTxtTrans.anchoredPosition, new Vector2(0, levelResultTxtTrans.anchoredPosition.y), 0.5f);
        RectTransform nextLevelTxtTrans = nextLevelTxt.rectTransform;
        ViewManager.Instance.MoveRect(nextLevelTxtTrans, nextLevelTxtTrans.anchoredPosition, new Vector2(0, nextLevelTxtTrans.anchoredPosition.y), 0.5f);
        ViewManager.Instance.MoveRect(homeButtonViewTrans, homeButtonViewTrans.anchoredPosition, new Vector2(30, homeButtonViewTrans.anchoredPosition.y), 0.5f);
        nextLevelTxt.text = "NEXT LEVEL: " + PlayerPrefs.GetInt(PlayerPrefsKey.SAVED_LEVEL_PPK).ToString();
    }


    private void OnDisable()
    {
        levelResultTxt.rectTransform.anchoredPosition = new Vector2(-700, levelResultTxt.rectTransform.anchoredPosition.y);
        nextLevelTxt.rectTransform.anchoredPosition = new Vector2(700, nextLevelTxt.rectTransform.anchoredPosition.y);
        tapToNextLevelTextTrans.localScale = Vector2.zero;
        tapToReplayTextTrans.localScale = Vector2.zero;
        homeButtonViewTrans.anchoredPosition = new Vector2(-150, homeButtonViewTrans.anchoredPosition.y);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject == null)
        {
            ViewManager.Instance.PlayClickButtonSound();
            ViewManager.Instance.LoadScene("Ingame", 0.3f);
        }
    }

    public void HomeBtn()
    {
        ViewManager.Instance.PlayClickButtonSound();
        ViewManager.Instance.LoadScene("Home", 0.3f);
    }

    public void NativeShareBtn()
    {
        ViewManager.Instance.PlayClickButtonSound();
    }
}
