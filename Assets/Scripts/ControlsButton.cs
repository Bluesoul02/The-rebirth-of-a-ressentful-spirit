using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsButton : MonoBehaviour
{
    public GameObject ControlsPanel;

    // méthodes pour activer et désactiver le panneau d'affichage des contrôles
    public void EnableControlsPanel()
    {
        ControlsPanel.SetActive(true);
    }

    public void DisableControlsPanel()
    {
        ControlsPanel.SetActive(false);
    }
}
