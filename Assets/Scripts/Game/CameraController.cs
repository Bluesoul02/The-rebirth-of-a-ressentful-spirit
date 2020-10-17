using UnityEngine;

public class CameraController : MonoBehaviour
{

    private GameObject player;


    private Vector3 offset;

    // Initialisation
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        // Calcul et stocke le décalage entre le joueur et la caméra
        offset = transform.position - player.transform.position;
    }

    // La fonction LateUpdate() est appelée après la fonction Update() à chaque image
    void LateUpdate()
    {
        if (GameObject.Find("Infos"))
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Définit la position de la caméra avec celle du joueur tout en ajoutant un décalage.
        if (player != null)
        {
            transform.position = player.transform.position + offset;
        }
    }
}
