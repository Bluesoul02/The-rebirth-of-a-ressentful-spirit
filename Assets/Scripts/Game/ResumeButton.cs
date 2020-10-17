using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeButton : MonoBehaviour
{
    public GameObject PauseMenu;

    // fonction appelée quand le joueur appuie sur le bouton Resume du menu Pause, permet de désactiver le menu Pause
    public void Resume()
    {
        PauseMenu.SetActive(false);
    }
}
