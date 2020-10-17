using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public GameObject SettingsPanel;

    private void Awake()
    {
        Cursor.visible = true;
    }

    // fonction appelée quand le joueur appuie sur Play du menu principal
    public void Play()
    {
        // on cherche la zone à charger dans le fichier de sauvegarde data.txt
        // s'il est vide alors la zone chargée sera la zone Intro
        string ZoneToLoad = File.ReadAllText(Application.dataPath + "/StreamingAssets/data.txt");
        if (ZoneToLoad == "")
            ZoneToLoad = "Intro";
        else
            ZoneToLoad = ZoneToLoad.Split(new[] { "%DATA%" }, System.StringSplitOptions.None)[0];

        // on sauvegarde le volume de la musique car le joueur l'a peut-être modifié en allant dans les paramètres
        string SettingsToSave = SettingsPanel.GetComponent<GameSettings>().GetInformations();
        File.WriteAllText(Application.dataPath + "/StreamingAssets/Settings.txt", SettingsToSave);

        // on charge la scène voulue
        SceneManager.LoadScene(ZoneToLoad);
    }
}
