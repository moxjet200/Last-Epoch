using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateRandomChild : MonoBehaviour
{
    public int numberToActivate = 1;
    public bool deactivateOthers = true;
    public bool ignoreActive = false;

    public void Start()
    {
        // get go list
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (!(ignoreActive && child.gameObject.activeSelf))
            {
                children.Add(child.gameObject);
            }
        }

        // deactivate any active ones
        if (deactivateOthers)
        {
            foreach (GameObject child in children)
            {
                child.SetActive(false);
            }
        }

        // activate
        int j;
        for (int i = 0; i < numberToActivate; i++)
        {
            if (children.Count > 0)
            {
                j = Random.Range(0, children.Count - 1);
                children[j].SetActive(true);
                children.RemoveAt(j);
            }
        }

    }

}

