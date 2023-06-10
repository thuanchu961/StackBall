using System.Collections;
using UnityEngine;
using CBGames;

public class CenterPillarController : MonoBehaviour
{
    private int turn = 1;
    public void OnSetup(CenterPillarData centerPillarData)
    {
        turn *= (Random.value <= 0.5f) ? 1 : -1;
        StartCoroutine(CRRotating(centerPillarData));
    }

    private IEnumerator CRRotating(CenterPillarData centerPillarData)
    {
        Vector3 rotateDir = Vector3.zero;
        float timeTemp = 0;
        float rotatingSpeed = 0;
        while (true)
        {
            turn *= -1;
            rotateDir = (turn < 0) ? Vector3.down : Vector3.up;
            timeTemp = Random.Range(centerPillarData.MinRotatingTime, centerPillarData.MaxRotatingTime);
            rotatingSpeed = Random.Range(centerPillarData.MinRotatingSpeed, centerPillarData.MaxRotatingSpeed);
            while (timeTemp > 0)
            {
                timeTemp -= Time.deltaTime;
                yield return null;
                transform.eulerAngles += rotateDir * rotatingSpeed * Time.deltaTime;
            }

            turn *= -1;
            rotateDir = (turn < 0) ? Vector3.down : Vector3.up;
            timeTemp = Random.Range(centerPillarData.MinRotatingTime, centerPillarData.MaxRotatingTime);
            rotatingSpeed = Random.Range(centerPillarData.MinRotatingSpeed, centerPillarData.MaxRotatingSpeed);
            while (timeTemp > 0)
            {
                timeTemp -= Time.deltaTime;
                yield return null;
                transform.eulerAngles += rotateDir * rotatingSpeed * Time.deltaTime;
            }
        }
    }
}
