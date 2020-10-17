using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialEvents : MonoBehaviour
{
    public bool[] values;

    public bool[] done;

    void Awake()
    {
        values = new bool[10];
        done = new bool[10];

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // garde un seul canvas Dialogue
        if (GameObject.FindGameObjectsWithTag("Dialogue").Length > 1)
        {
            Destroy(GameObject.FindGameObjectsWithTag("Dialogue")[1]);
        }
    }
}
