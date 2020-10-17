using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class TriggerFight : MonoBehaviour
{
    // on récupére le gamemanager et l'inventaire depuis l'inspecteur
    public GameObject GameManager;
    public GameObject DragAndDrop;

    // fonction d'Unity qui se déclenche quand l'ennemi rentre en contact avec quelque chose
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject Infos;
        if (collision.CompareTag("Player"))
        {
            // si l'objet rencontré est le joueur alors on fait les préparatifs pour le combat
            if (!GameObject.Find("Infos"))
            {
                // l'objet infos contiendra les infos sur le joueur
                Infos = new GameObject("Infos");
                Infos.AddComponent<Stats>();
                Infos.AddComponent<InfosPlayer>();
                Infos.tag = "Infos";
            }
            else
            {
                // s'il existe déjà, on le récupère
                Infos = GameObject.Find("Infos");
            }

            // méthode pour garder un objet quand on change de scène
            DontDestroyOnLoad(Infos);
            DontDestroyOnLoad(GameObject.Find("Dialogue"));

            // if pour éviter certains bugs
            if (DragAndDrop == null)
            {
                DragAndDrop = GameObject.Find("Drag&Drop");
            }
            DragAndDrop.GetComponent<Inventory>().enabled = false;
            DragAndDrop.transform.GetChild(0).gameObject.SetActive(false);
            DragAndDrop.GetComponent<Canvas>().enabled = true;
            DontDestroyOnLoad(DragAndDrop);

            // save de la zone pour après le combat et des settings pour après et pendant le combat
            GameManager.GetComponent<MainManager>().SaveZone();
            GameManager.GetComponent<MainManager>().SaveSettings();

            // copie des infos du joueur dans Infos
            Infos.GetComponent<Stats>().CopyStatsFrom(collision.gameObject);
            Infos.GetComponent<InfosPlayer>().CopyInfosFrom(collision.gameObject);
            Infos.GetComponent<InfosPlayer>().SetZone(collision.gameObject.GetComponent<InfosPlayer>().GetZone());

            // sauvegarde du nom de l'ennemi rencontré (utilisé pour le chargement des zones) et initialisation des TP pour le combat
            Infos.GetComponent<InfosPlayer>().SetNameEnemyEncountred(transform.name);
            Infos.GetComponent<Stats>().TP.RemoveAllModifiers();
            Infos.GetComponent<Stats>().TP.AddModifier(new StatModifier(-Infos.GetComponent<Stats>().TP.Value, StatModType.Flat));

            // destruction du joueur pour éviter des doublons
            Destroy(GameObject.FindGameObjectWithTag("Player"));

            // chargement de la scène de combat
            SceneManager.LoadScene("fight");
        }
    }
}
