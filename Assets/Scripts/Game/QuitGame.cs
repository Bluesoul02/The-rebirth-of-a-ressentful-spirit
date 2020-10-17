using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // fonction appelée quand le joueur appuie sur le bouton quit du menu principal
    public void QuitTheGame()
    {
        // fonction Unity qui ne fonctionne pas en mode Editeur, permet de fermer l'application
        Application.Quit();
    }
}
