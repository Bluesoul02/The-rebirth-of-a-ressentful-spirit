using UnityEngine;

// script qui gère le déplacement (uniquement horizontal) d'un ennemi
public class PatrouilleSnek : MonoBehaviour
{
    public float speed;
    public Transform[] waypoints;

    public SpriteRenderer graphics;
    private Transform target;
    private int destPoint;

    void Start()
    {
        // on initialise la première destination de l'ennemi, il se dirigera vers elle
        target = waypoints[0];
    }

    void Update()
    {
        // déplace l'ennemi
        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        if(Vector3.Distance(transform.position, target.position) < 0.3f)
        {
            // change la destination si la dernière a été atteinte
            destPoint = (destPoint + 1) % waypoints.Length;
            target = waypoints[destPoint];

            // renversement horizontal du sprite de l'ennemi pour un cohérence visuelle
            graphics.flipX = !graphics.flipX;
        }
    }
}
