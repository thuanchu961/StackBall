using CBGames;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReviveViewController : MonoBehaviour {

    [SerializeField] private Text reviveCountTxt = null;
    [SerializeField] private RectTransform reviveButtonTrans = null;
    [SerializeField] private RectTransform closeReviveViewButtonTrans = null;


    public void OnShow()
    {
        reviveCountTxt.text = IngameManager.Instance.ReviveWaitTime.ToString();
        StartCoroutine(CROnShow());
    }

    private void OnDisable()
    {
        reviveCountTxt.rectTransform.anchoredPosition = new Vector2(-500, reviveCountTxt.rectTransform.anchoredPosition.y);
        reviveButtonTrans.anchoredPosition = new Vector2(500, reviveButtonTrans.anchoredPosition.y);
        closeReviveViewButtonTrans.localScale = Vector2.zero;
    }



    private IEnumerator CROnShow()
    {
        RectTransform reviveCountDownRect = reviveCountTxt.rectTransform;
        ViewManager.Instance.MoveRect(reviveCountDownRect, reviveCountDownRect.anchoredPosition, new Vector2(0, reviveCountDownRect.anchoredPosition.y), 0.5f);
        ViewManager.Instance.MoveRect(reviveButtonTrans, reviveButtonTrans.anchoredPosition, new Vector2(0, reviveButtonTrans.anchoredPosition.y), 0.5f);
        StartCoroutine(CRReviveCountDown());
        yield return new WaitForSeconds(1f);
        ViewManager.Instance.ScaleRect(closeReviveViewButtonTrans, Vector2.zero, Vector2.one, 0.5f);
    }



    /// <summary>
    /// Start counting down revive wait time.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CRReviveCountDown()
    {
        float t = IngameManager.Instance.ReviveWaitTime;
        float decreaseAmount = 1f;
        while (t > 0)
        {
            if (IngameManager.Instance.IngameState != IngameState.Ingame_Revive)
                yield break;
            t -= decreaseAmount;
            reviveCountTxt.text = t.ToString();
            yield return new WaitForSeconds(decreaseAmount);
        }
        IngameManager.Instance.GameOver();
    }


    public void ReviveBtn()
    {
        ViewManager.Instance.PlayClickButtonSound();
        ServicesManager.Instance.AdManager.ShowRewardedVideoAd();
    }

    public void CloseReviveViewBtn()
    {
        ViewManager.Instance.PlayClickButtonSound();
        IngameManager.Instance.GameOver();
    }
}
