using System.IO;
using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    public GameObject SettingsPanel;
    public GameObject Music;

    private void Awake()
    {
        // ici on ajuste le volume de la musique si une sauvegarde du volume a été faite dans le fichier Settings.txt
        if (Music.name == "Music Menu" & gameObject.name != "ReturnButton")
        {
            string Content = File.ReadAllText(Application.dataPath + "/StreamingAssets/Settings.txt");
            if(Content != "")
                Music.GetComponent<AudioSource>().volume = float.Parse(Content);
        }
    }

    // fonction appelée lorsque le joueur appuie sur le bouton Settings de menu principal
    public void EnableSettingsPanel()
    {
        SettingsPanel.SetActive(true);
    }

    // fonction appelée lorsque le joueur appuie sur le bouton Back du panneau Settings
    public void DisableSettingsPanel()
    {
        SettingsPanel.SetActive(false);
    }
}
