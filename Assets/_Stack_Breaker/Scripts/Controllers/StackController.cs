using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CBGames;

public class StackController : MonoBehaviour
{
    [SerializeField] private StackType stackType = StackType.STACK_6_PARTS;
    [SerializeField] private MeshRenderer stackPartMeshRender = null;
    [SerializeField] private StackPartController[] stackPartControls = null;


    public StackType StackType { get { return stackType; } }
    public bool IsShattered { private set; get; }

    public void InitValues(float yAngle, List<int> listDeadlyPartIndex, Material normalPart, Material deadlyPart)
    {
        //Set deadly parts and normal parts 
        for (int i = 0; i < stackPartControls.Length; i++)
        {
            if (listDeadlyPartIndex.Contains(i))
            {
                stackPartControls[i].SetDeadlyPart(true);
                stackPartControls[i].SetMaterial(deadlyPart);
            }
            else
            {
                stackPartControls[i].SetDeadlyPart(false);
                stackPartControls[i].SetMaterial(normalPart);
            }
        }

        //Set angle for this stack
        transform.eulerAngles = new Vector3(0, yAngle, 0);
    }


    /// <summary>
    /// shatter all parts of this stack.
    /// </summary>
    public void ShatterAllParts()
    {
        //Set paramaters
        IsShattered = true;
        transform.SetParent(null);

        IngameManager.Instance.UpdateCurrentBrokenStack();

        //Shatter all parts
        foreach(StackPartController o in stackPartControls)
        {
            o.Shatter();
        }
        StartCoroutine(CRDisableObject());
    }

    private IEnumerator CRDisableObject()
    {
        yield return new WaitForSeconds(1f);
        foreach (StackPartController o in stackPartControls)
        {
            o.RemoveAllChilds();
        }
        IsShattered = false;
        gameObject.SetActive(false);
    }



    public StackPartController GetClosestStackPartController()
    {
        StackPartController result = null;
        float magnitude = 1000f;
        foreach(StackPartController o in stackPartControls)
        {
            float currentMagnitude = Vector3.Distance(o.GetCenterMeshPosition(), PlayerController.Instance.transform.position);
            if (currentMagnitude < magnitude)
            {
                magnitude = currentMagnitude;
                result = o;
            }
        }

        return result;
    }



    /// <summary>
    /// Get the top y axis of this stack.
    /// </summary>
    /// <returns></returns>
    public float GetTopYAxis()
    {
        return transform.position.y + (stackPartMeshRender.bounds.size.y / 2f);
    }



    /// <summary>
    /// Get the size of y axis of this stack.
    /// </summary>
    /// <returns></returns>
    public float GetYSize()
    {
        return stackPartMeshRender.bounds.size.y;
    }
}
