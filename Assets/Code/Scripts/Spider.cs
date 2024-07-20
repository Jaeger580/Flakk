using System.Collections;
using System.Collections.Generic;
using GeneralUtility;
using UnityEngine;

[ExecuteInEditMode]
public class Spider : MonoBehaviour
{
    public static Spider instance;
    protected List<Transform> webs = new List<Transform>();
    private HashSet<WebSelection> usedSelection = new HashSet<WebSelection>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitWebs();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitWebs()
    {
        foreach(Transform child in transform)
        {
            if (!webs.Contains(child))
            {
                webs.Add(child);
            }
        }
    }

    public Waypoint_Web GetWeb(int _index)
    {
        if (webs.Count <= _index) { Editor_Utility.ThrowWarning("ERR: Web index out of range.", this);  return webs[0].GetComponent<Waypoint_Web>(); }
        return webs[_index].GetComponent<Waypoint_Web>();
    }

    public bool IsStateAvailable(WebSelection _selection)
    {
        return !usedSelection.Contains(_selection);
    }

    public bool UseState(WebSelection _selection)
    {
        if (IsStateAvailable(_selection))
        {
            usedSelection.Add(_selection);
            return true;
        }

        return false;
    }

    public void ReleaseState(WebSelection _selection)
    {
        if (usedSelection.Contains(_selection))
        {
            usedSelection.Remove(_selection);
        }
    }

    public HashSet<WebSelection> GetUsedStates()
    {
        return new HashSet<WebSelection>(usedSelection);
    }
}