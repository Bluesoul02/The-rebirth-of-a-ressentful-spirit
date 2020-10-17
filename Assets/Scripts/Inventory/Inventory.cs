using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    public string inputInventory;

    private GameObject player;
    private Canvas inventory;
    private GameObject panel;
    private Text money;
    private Text weight;

    void Awake()
    {
        inventory = GetComponent<Canvas>();
        inventory.enabled = false;
        panel = transform.GetChild(0).gameObject;
        money = panel.GetComponentsInChildren<Text>()[0];
        weight = panel.GetComponentsInChildren<Text>()[1];
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update
    void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        // ouvre et ferme l'inventaire à la pression du bouton dédié
        if (Input.GetKeyDown(inputInventory))
        {
            inventory.enabled = !inventory.enabled;
        }

        // lorsque l'inventaire est actif on désactive les déplacements du joueur
        if (inventory.enabled)
        {
            if (player.GetComponent<PlayerController>())
            {
                player.GetComponent<PlayerController>().enabled = false;
            }
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        // et on les réactive à sa fermeture
        else
        {
            if (player.GetComponent<PlayerController>())
            {
                player.GetComponent<PlayerController>().enabled = true;
            }
        }
    }
}
