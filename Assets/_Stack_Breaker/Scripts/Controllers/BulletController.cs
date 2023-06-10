using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public void OnSetup(Vector3 dir, float speed)
    {
        transform.up = dir.normalized;
        StartCoroutine(CRMoving(dir, speed));
    }

    private IEnumerator CRMoving(Vector3 dir, float speed)
    {
        while (true)
        {
            transform.position += dir * speed * Time.deltaTime;
            yield return null;

            Vector2 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
            if (viewportPos.x < -0.05f || viewportPos.x > 1.05f || viewportPos.y < -0.05f || viewportPos.y > 1.05f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
