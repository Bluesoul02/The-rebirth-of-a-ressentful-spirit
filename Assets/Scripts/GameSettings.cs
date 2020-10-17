using System.IO;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    
    private readonly float ChangeValue = 0.01f; // valeur utilisée pour modifier le volume de la musique
    public GameObject Music;

    // méthode qui augmente le volume de la musique
    public void RaiseVolume()
    {
        if((Music.GetComponent<AudioSource>().volume += ChangeValue) <= 1f)
            Music.GetComponent<AudioSource>().volume += ChangeValue;
    }

    // méthode qui réduit le volume de la musique
    public void LowVolume()
    {
        if ((Music.GetComponent<AudioSource>().volume -= ChangeValue) < 0f & Music.GetComponent<AudioSource>().volume > 0f)
            Music.GetComponent<AudioSource>().volume = 0f;
        else
            Music.GetComponent<AudioSource>().volume -= ChangeValue;
    }

    public string GetInformations()
    {
        return Music.GetComponent<AudioSource>().volume.ToString();
    }
}

