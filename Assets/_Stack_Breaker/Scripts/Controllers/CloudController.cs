using System.Collections;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRender = null;

    public string ObjectName { get { return gameObject.name.Split('(')[0]; } }


    /// <summary>
    /// Set the alpha chanel for the color of the sprite renderer.
    /// </summary>
    /// <param name="amount"></param>
    public void SetAlpha(float amount)
    {
        Color color = spriteRender.color;
        color.a = amount;
        spriteRender.color = color;
    }


    public void Move(Vector3 startPos, Vector3 endPos, float movingTime)
    {
        StartCoroutine(CRMoving(startPos, endPos, movingTime));
    }
    private IEnumerator CRMoving(Vector3 startPos, Vector3 endPos, float movingTime)
    {
        float t = 0;
        transform.localPosition = startPos;
        while (t < movingTime)
        {
            t += Time.deltaTime;
            float factor = t / movingTime;
            transform.localPosition = Vector3.Lerp(startPos, endPos, factor);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
