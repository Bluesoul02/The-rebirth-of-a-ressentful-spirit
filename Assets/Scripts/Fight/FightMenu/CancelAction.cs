using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script pour passer du panneau actuel au précédent
public class CancelAction : MonoBehaviour
{
    public string cancel;
    public GameObject previousPanel;
    public GameObject currentPanel;

    void Update()
    {
        // Input.GetKey sert à détecter un appui sur touche
        if (Input.GetKey(cancel))
        {
            // le panneau actuel se désactive tandis que le précédent se réactive
            previousPanel.SetActive(true);
            currentPanel.SetActive(false);
        }
    }
}
