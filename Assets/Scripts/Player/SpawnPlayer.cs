using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    // ce script permet simplement de définir l'endroit où le joueur apparaît quand il change de scène
    private void Start()
    {
        GameObject.FindGameObjectWithTag("Player").transform.position = transform.position;
    }
}
