using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CloudManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float minCreateCloudDelay = 1f;
    [SerializeField] private float maxCreateCloudDelay = 3f;
    [SerializeField] private float minCloudMovingTime = 25f;
    [SerializeField] private float maxCloudMovingTime = 35f;
    [SerializeField] private float minCloudAlphaChannel = 0.25f;
    [SerializeField] private float maxCloudAlphaChannel = 0.75f;
    [SerializeField] private float limitLeft = -4f;
    [SerializeField] private float limitRight = 4f;
    [SerializeField] private float limitTop = 5f;
    [SerializeField] private float limitBottom = -5f;

    [Header("References")]
    [SerializeField] private CloudController[] cloudPrefabs = null;


    private List<CloudController> listCloundControl = new List<CloudController>();


    private void Start()
    {
        StartCoroutine(CRCreatingClouds());
    }
    private IEnumerator CRCreatingClouds()
    {
        while (true)
        {
            string cloudName = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)].ObjectName;

            CloudController cloudControl = listCloundControl.Where(a => (!a.gameObject.activeInHierarchy) && (a.ObjectName.Equals(cloudName))).FirstOrDefault();
            if (cloudControl == null)
            {
                GameObject cloudPrefab = cloudPrefabs.Where(a => a.ObjectName.Equals(cloudName)).FirstOrDefault().gameObject;
                cloudControl = Instantiate(cloudPrefab, Vector3.zero, Quaternion.identity).GetComponent<CloudController>();
                listCloundControl.Add(cloudControl);
            }

            cloudControl.transform.SetParent(transform);
            cloudControl.transform.localEulerAngles = Vector3.zero;
            cloudControl.SetAlpha(Random.Range(minCloudAlphaChannel, maxCloudAlphaChannel));
            if (Random.value < 0.5f) //Create cloud on left and moving right
            {
                Vector3 startPos = new Vector3(limitLeft, Random.Range(limitTop, limitBottom), 0);
                Vector3 endPos = startPos + new Vector3(20, 0, 0);
                cloudControl.gameObject.SetActive(true);
                cloudControl.Move(startPos, endPos, Random.Range(minCloudMovingTime, maxCloudMovingTime));
            }
            else //Create cloud on right and moving left
            {
                Vector3 startPos = new Vector3(limitRight, Random.Range(limitTop, limitBottom), 0);
                Vector3 endPos = startPos - new Vector3(20, 0, 0);
                cloudControl.gameObject.SetActive(true);
                cloudControl.Move(startPos, endPos, Random.Range(minCloudMovingTime, maxCloudMovingTime));
            }

            yield return new WaitForSeconds(Random.Range(minCreateCloudDelay, maxCreateCloudDelay));
        }
       
    }
}
