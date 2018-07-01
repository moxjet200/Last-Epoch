using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateRandom : MonoBehaviour
{
    public List<GameObject> candidates = new List<GameObject>();
    public int numberToActivate = 1;

    public void Start()
    {
        int j;
        for (int i = 0; i < numberToActivate; i++)
        {
            if (candidates.Count > 0)
            {
                j = Random.Range(0, candidates.Count - 1);
                candidates[j].SetActive(true);
                candidates.RemoveAt(j);
            }
        }


    }

}
