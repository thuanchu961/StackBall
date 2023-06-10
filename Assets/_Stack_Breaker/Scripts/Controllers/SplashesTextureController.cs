using System.Collections;
using UnityEngine;

public class SplashesTextureController : MonoBehaviour {

    [SerializeField] private SpriteRenderer spRender = null;
    public void FadeOut(Color color)
    {
        spRender.color = new Color(color.r, color.g, color.b, Random.Range(0.4f, 0.8f));
        StartCoroutine(FadingOut());
    }
    private IEnumerator FadingOut()
    {
        float fadingTime = (Random.Range(1f, 3f));
        Color startColor = spRender.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
        float t = 0;
        while (t < fadingTime)
        {
            t += Time.deltaTime;
            float factor = t / fadingTime;
            spRender.color = Color.Lerp(startColor, endColor, factor);
            yield return null;
        }
        gameObject.SetActive(false);
    }

}
