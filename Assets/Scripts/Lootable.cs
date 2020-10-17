using UnityEngine;

public class Lootable : MonoBehaviour
{
    public GameObject Loot;
    public GameObject LootInstance;
    public GameObject Inventaire;
    private Vector3 pos;

    private void Awake()
    {
        // les deux if permettent de corriger un certain bug

        // on réaffecte l'inventaire s'il n'est plus là
        Inventaire = GameObject.Find("Drag&Drop");

        // création de l'instance des objets présents dans le coffre
        pos = Inventaire.transform.position;
        LootInstance = Instantiate(Loot, new Vector3(pos.x, pos.y * 1.5f, pos.z), Quaternion.identity, Inventaire.transform);
        LootInstance.SetActive(false);
    }

    private void Update()
    {

        if (Inventaire == null)
        {
            // on réaffecte l'inventaire s'il n'est plus là
            Inventaire = GameObject.Find("Drag&Drop");
        }

        if (LootInstance == null)
        {
            // création de l'instance des objets présents dans le coffre
            pos = Inventaire.transform.position;
            LootInstance = Instantiate(Loot, new Vector3(pos.x, pos.y * 1.5f, pos.z), Quaternion.identity, Inventaire.transform);
            LootInstance.SetActive(false);
        }
    }
}
