using UnityEngine.SceneManagement;
using UnityEngine;
using CBGames;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class HomeManager : MonoBehaviour {

    [SerializeField] private MeshRenderer mainCharacterMeshRender = null;
    [SerializeField] private Transform bottomPillarTrans = null;

    private float jumpVelocity = 20;
    private float fallingSpeed = -60;
    private float minScale = 0.85f;
    private float maxScale = 1.25f;
    private float scalingFactor = 2;
    private Vector3 originalScale = Vector3.zero;
    private float currentJumpVelocity = 0;
    private void Start()
    { 
        Application.targetFrameRate = 60;
        ViewManager.Instance.OnLoadingSceneDone(SceneManager.GetActiveScene().name);

    }


    private void Update()
    {
        //Rotate the bottom pillar
        bottomPillarTrans.eulerAngles += Vector3.up * 30f * Time.deltaTime;

        mainCharacterMeshRender.transform.position = mainCharacterMeshRender.transform.position + Vector3.up * (currentJumpVelocity * Time.deltaTime + fallingSpeed * Time.deltaTime * Time.deltaTime / 2);

        if (currentJumpVelocity < fallingSpeed)
            currentJumpVelocity = fallingSpeed;
        else
            currentJumpVelocity = currentJumpVelocity + fallingSpeed * Time.deltaTime;

        if (currentJumpVelocity < 0)
        {
            //Adjust scale
            Vector3 scale = mainCharacterMeshRender.transform.localScale;
            if (scale.x > minScale)
            {
                scale.x -= scalingFactor * Time.deltaTime;
            }
            else
                scale.x = minScale;
            mainCharacterMeshRender.transform.localScale = scale;

            //Calculate the distance
            float bottomY = (mainCharacterMeshRender.transform.position + Vector3.down * (mainCharacterMeshRender.bounds.size.y / 2f)).y;
            float distance = bottomY - bottomPillarTrans.position.y;
            if (distance <= 0.1f)
            {
                currentJumpVelocity = jumpVelocity;
            }
        }
        else
        {
            //Adjust scale
            Vector3 scale = mainCharacterMeshRender.transform.localScale;
            if (scale.x < maxScale)
            {
                scale.x += scalingFactor * Time.deltaTime;
            }
            else
                scale.x = maxScale;
            mainCharacterMeshRender.transform.localScale = scale;

        }


        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            ViewManager.Instance.PlayClickButtonSound();
            ViewManager.Instance.LoadScene("Ingame", 0.3f);
        }

    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
