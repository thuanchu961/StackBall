using System.Collections.Generic;
using UnityEngine;
using CBGames;
using System.Linq;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { private set; get; }

    [SerializeField] private StackController[] stackPrefabs = null;


    private List<StackController> listStackControl = new List<StackController>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(Instance.gameObject);
            Instance = this;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }


    /// <summary>
    /// Get an inactive StackControl by given StackType.
    /// </summary>
    /// <param name="stackType"></param>
    /// <returns></returns>
    public StackController GetStackController(StackType stackType)
    {
        StackController stackControl = listStackControl.Where(a => (!a.gameObject.activeInHierarchy && a.StackType == stackType)).FirstOrDefault();

        if (stackControl == null)
        {
            //Did not find one -> create new one
            GameObject stackPrefab = stackPrefabs.Where(a => a.StackType == stackType).FirstOrDefault().gameObject;
            stackControl = Instantiate(stackPrefab, Vector3.zero, Quaternion.identity).GetComponent<StackController>();
            stackControl.gameObject.SetActive(false);
            listStackControl.Add(stackControl);
        }

        return stackControl;
    }

}
