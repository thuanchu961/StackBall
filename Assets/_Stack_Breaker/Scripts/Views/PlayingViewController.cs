using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayingViewController : MonoBehaviour {

    [SerializeField] private RectTransform topBarTrans = null;
    [SerializeField] private Text currentLevelTxt = null;
    [SerializeField] private Text nextLevelTxt = null;
    [SerializeField] private Image levelSliderImg = null;
    [SerializeField] private GameObject immortalModeTimeCountView = null;
    [SerializeField] private Image immortalModeTimeSlider = null;

    private bool isCountingImmortalMode = false;
    public void OnShow()
    {
        ViewManager.Instance.MoveRect(topBarTrans, topBarTrans.anchoredPosition, new Vector2(topBarTrans.anchoredPosition.x, 0), 0.5f);

        immortalModeTimeSlider.color = Color.white;
        isCountingImmortalMode = false;

        //Update texts
        currentLevelTxt.text = IngameManager.Instance.CurrentLevel.ToString();
        nextLevelTxt.text = (IngameManager.Instance.CurrentLevel + 1).ToString();

        if (!IngameManager.Instance.IsRevived)
        {
            levelSliderImg.fillAmount = 0;
        }
    }

    private void OnDisable()
    {
        topBarTrans.anchoredPosition = new Vector2(topBarTrans.anchoredPosition.x, 150);
    }

    public void UpdateLevelProgress(float levelProgress)
    {
        levelSliderImg.fillAmount = levelProgress;
    }


    public void UpdateImmortalModeTimeView(float currentTimeCount, float timeCountToEnableImmortalMode)
    {
        if (isCountingImmortalMode)
            return;

        if (currentTimeCount > 0 && !immortalModeTimeCountView.activeInHierarchy)
        {
            immortalModeTimeCountView.SetActive(true);
        }
        else if (currentTimeCount <= 0 && immortalModeTimeCountView.activeInHierarchy)
        {
            immortalModeTimeCountView.SetActive(false);
        }

        immortalModeTimeSlider.fillAmount = currentTimeCount / timeCountToEnableImmortalMode;

        if (immortalModeTimeSlider.fillAmount == 1f)
        {
            immortalModeTimeSlider.color = Color.magenta;
            isCountingImmortalMode = true;
            StartCoroutine(CRCountingImmortalMode());
        }
    }


    private IEnumerator CRCountingImmortalMode()
    {
        float countingTime = PlayerController.Instance.ImmortalModeTime;
        float t = 0;
        while (t < countingTime)
        {
            t += Time.deltaTime;
            float factor = EasyType.MatchedLerpType(LerpType.Liner, t / countingTime);
            immortalModeTimeSlider.fillAmount = Mathf.Lerp(1, 0, factor);
            yield return null;
        }
        immortalModeTimeSlider.color = Color.white;
        immortalModeTimeCountView.SetActive(false);
        isCountingImmortalMode = false;
    }
}
