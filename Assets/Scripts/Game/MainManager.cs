using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

/*
 * le système de sauvegarde fonctionne avec des fichiers texte
 * data.txt pour le joueur
 * Settings.txt pour les paramètres
 * LootToCheck_zonedujoueur.txt pour les états des coffres
 * tous ces fichiers sont dans le dossier StreamingAssets
*/
public class MainManager : MonoBehaviour
{
    public GameObject Music;
    public GameObject PauseMenu;
    public GameObject Player;
    private string DataSeparator = "%DATA%";
    private string ValueSeparator = "%VALUE%";
    public GameObject[] LootToCheck;
    string PlayerZone;

    private void Awake()
    {
        // pause menu est affecté depuis l'inspecteur mais lors d'un changement de scène il se peut qu'il ne soit plus affecté
        if(PauseMenu == null)
        {
            PauseMenu = GameObject.Find("PauseMenu");
        }

        // chargement du joueur, de la zone et des paramètres

        LoadSettings();
        Cursor.visible = false;
        if(Player == null)
            Player = GameObject.FindGameObjectWithTag("Player");

        // récupération de la zone dans laquelle le joueur se trouve où va se trouver
        PlayerZone = File.ReadAllText(Application.dataPath + "/StreamingAssets/data.txt");
        if (PlayerZone == "")
            PlayerZone = Player.GetComponent<InfosPlayer>().GetZone();
        else
            PlayerZone = PlayerZone.Split(new[] { "%DATA" }, System.StringSplitOptions.None)[0];


        LoadPlayer();
        LoadInventory();
        if (Player.GetComponent<InfosPlayer>().IsRespawning()){
            LoadZone();
        }
    }

    void Update()
    {
        // quand le joueur appuie sur echap, le menu de pause s'ouvre ou se ferme
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = !Cursor.visible;
            PauseMenu.SetActive(!PauseMenu.activeInHierarchy);
            Player.GetComponent<PlayerController>().enabled = !PauseMenu.activeInHierarchy;
        }
    }

    public void Quit()
    {
        /*
         * la méthode quit est appelée quand le bouton "save and quit" du menu pause est appuyé
         * on sauvegegarde toutes les infos sur le joueur (position, inventaire, ennemis tués et stats)
         * puis les infos sur la zone (états des coffres)
         * et enfin les paramètres (seulement le volume de la musique pour l'instant)
        */
        SavePlayer();
        SaveZone();
        SaveSettings();
        SaveInventory();
        Destroy(GameObject.Find("Drag&Drop"));

        // on charge la scène de menu
        SceneManager.LoadScene("Menu");
    }

    // fonction de sauvegarde des infos sur le joueur
    public void SavePlayer()
    {
        Stats PlayerStats = Player.GetComponent<Stats>();

        // sauvegarde de la position du joueur. Format : x%VALUE%y%VALUE%z
        string PlayerPosition = Player.transform.position.x.ToString() + ValueSeparator + Player.transform.position.y.ToString() + ValueSeparator + Player.transform.position.z.ToString();

        // sauvegarde des noms des ennemis battus par le joueur. Format : ennemi1%VALUE%ennem2%VALUE% etc
        string saveEnemiesKilled = "";
        List<string> EnemiesKilled = Player.GetComponent<InfosPlayer>().EnemiesKilled;

        for (int i = 0; i < EnemiesKilled.Count; i++)
        {
            if (i == EnemiesKilled.Count - 1)
                saveEnemiesKilled += EnemiesKilled[i];
            else
                saveEnemiesKilled += EnemiesKilled[i] + ValueSeparator;
        }

        // sauvegarde des stats du joueur. Format : speed%VALUE%health%VALUE%Mana%VALUE% etc
        string SavePlayerStats = PlayerStats.Speed.Value + ValueSeparator + PlayerStats.Health.Value + ValueSeparator + PlayerStats.Mana.Value + ValueSeparator + PlayerStats.AP.Value + ValueSeparator + PlayerStats.Defense.Value + ValueSeparator + PlayerStats.Atk.Value + ValueSeparator + PlayerStats.TP.Value + ValueSeparator + PlayerStats.MR.Value;

        // on met toutes les chaînes de caractères contenant les données dans un tableau
        string[] saveData = new string[]
        {
            PlayerZone,
            saveEnemiesKilled,
            PlayerPosition,
            SavePlayerStats
        };

        // la nouvelle chaîne de caractères saveString est un cumul de tous les éléments du tableau saveData séparés par %DATA%
        string saveString = string.Join(DataSeparator, saveData);

        // cette ligne permet d'écrire tout le contenu de saveString dans le fichier data.txt
        File.WriteAllText(Application.dataPath + "/StreamingAssets/data.txt", saveString);
        
        Debug.Log("Sauvegarde du joueur faite");
    }

    public void SaveInventory()
    {
        GameObject InventorySlots = GameObject.FindGameObjectWithTag("Inventaire").transform.GetChild(0).GetChild(0).gameObject;
        int NbSlots = InventorySlots.transform.childCount;
        string Items = "";

        // pour chaque item, on sauvegarde son nom et sa quantité. Format : item1%VALUE%stack1%VALUE%item2%VALUE%stack2%VALUE% etc
        for (int i = 0; i < NbSlots; i++)
        {
            if (InventorySlots.transform.GetChild(i).childCount > 0)
            {
                Items += InventorySlots.transform.GetChild(i).GetChild(0).gameObject.name + ValueSeparator + InventorySlots.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Item>().stack + ValueSeparator;
            }
            else
            {
                Items += "NULL" + ValueSeparator + 0 + ValueSeparator;
            }
        }

        Items += DataSeparator;

        GameObject PlayerEquipment = GameObject.FindGameObjectWithTag("Inventaire").transform.GetChild(0).GetChild(8).gameObject;
        int NbSlotsEquipment = PlayerEquipment.transform.childCount;

        for (int i = 0; i < NbSlotsEquipment; i++)
        {
            if (PlayerEquipment.transform.GetChild(i).childCount > 0)
            {
                Items += PlayerEquipment.transform.GetChild(i).GetChild(0).gameObject.name + ValueSeparator;
            }
            else
            {
                Items += "NULL" + ValueSeparator;
            }
        }

        File.WriteAllText(Application.dataPath + "/StreamingAssets/Inventory.txt", Items);

        Debug.Log("Sauvegarde de l'inventaire faite");
    }

    public void SaveZone()
    {
        string saveStringLoot = "";
        GameObject LootSlots;

        // pour chaque conteneur que l'on doit vérifier, on regarde le contenu de chacun de ses slots
        // Format : NomConteneur1%VALUE%iteml%VALUE%item2....%DATA%NomConteneur2 etc
        for (int i = 0; i < LootToCheck.Length; i++)
        {
            LootSlots = LootToCheck[i].GetComponent<Lootable>().LootInstance.transform.GetChild(0).gameObject;
            for(int j = 0; j < LootSlots.transform.childCount; j++)
            {
                if(LootSlots.transform.GetChild(j).childCount > 0)
                {
                    // si le slot est plein (a un fils) alors on rajoute le nom du fils à saveStringLoot
                    saveStringLoot += LootSlots.transform.GetChild(j).GetChild(0).name;
                }
                else
                {
                    // s'il est vide on rajoute NULL à saveStringLoot
                    saveStringLoot += "NULL";
                }
                saveStringLoot += ValueSeparator;
            }
            saveStringLoot += DataSeparator;
        }

        // cette ligne permet d'écrire tout le contenu de saveString dans le fichier LootToCheck_zonedujoueur.txt où zonedujoueur = nom de la scène où le joueur est
        File.WriteAllText(Application.dataPath + "/StreamingAssets/LootToCheck_" + PlayerZone + ".txt", saveStringLoot);

        Debug.Log("Sauvegarde de la zone faite");
    }

    public void SaveSettings()
    {
        // pour le moment seul le volume de la musique est sauvegardé
        string SettingsToSave = Music.GetComponent<AudioSource>().volume.ToString();
        File.WriteAllText(Application.dataPath + "/StreamingAssets/Settings.txt", SettingsToSave);
    }

    private void LoadPlayer()
    {
        string contentPlayer = File.ReadAllText(Application.dataPath + "/StreamingAssets/data.txt");

        if (contentPlayer != "")
        {
            // le contenu du fichier est séparé selon le délimiteur %DATA%
            string[] splited = contentPlayer.Split(new[] { DataSeparator }, System.StringSplitOptions.None);

            /* 
             * première ligne de splited = zone à charger
             * seconde ligne = noms ennemis battus par le joueur (séparés par %VALUE%)
             * troisième ligne = position du joueur (x, y, z) (séparés par %VALUE%)
             * quatrième ligne = valeurs des stats du joueur (séparés par %VALUE%)
             * cinquième ligne = items présents dans l'inventaire et leur quantité
             * 
             * all lines are separated with %DATA%
            */
            int IdToUse = 1;

            // pour chaque ligne du tableau, on split la chaîne de caractères avec comme délimiteur %VALUE%
            if (splited[IdToUse] != "")
            {
                string[] EnemiesToKill = splited[IdToUse].Split(new[] { ValueSeparator }, System.StringSplitOptions.None);

                // on utilise les noms des ennemis tués sauvegardés afin de les retirer de la scène grâce à la méthode KillEnemies de SpawnManager
                GameObject.Find("SpawnManager").GetComponent<SpawnManager>().KillEnemies(EnemiesToKill);
            }
            IdToUse++;

            // on récupère les coordonnées du joueur sauvegardées et on place le joueur à ces dernières
            string[] CoordsPlayer = splited[IdToUse].Split(new[] { ValueSeparator }, System.StringSplitOptions.None);
            Player.transform.position = new Vector2(float.Parse(CoordsPlayer[0]), float.Parse(CoordsPlayer[1]));
            IdToUse++;

            // puis on change les stats du joueur pour qu'elles soient égales à celles sauvegardées
            string[] SaveStatsPlayer = splited[IdToUse].Split(new[] { ValueSeparator }, System.StringSplitOptions.None);

            Player.GetComponent<Stats>().Speed.SetValue(float.Parse(SaveStatsPlayer[0]));
            Player.GetComponent<Stats>().Health.SetValue(float.Parse(SaveStatsPlayer[1]));
            Player.GetComponent<Stats>().Mana.SetValue(float.Parse(SaveStatsPlayer[2]));
            Player.GetComponent<Stats>().AP.SetValue(float.Parse(SaveStatsPlayer[3]));
            Player.GetComponent<Stats>().Defense.SetValue(float.Parse(SaveStatsPlayer[4]));
            Player.GetComponent<Stats>().Atk.SetValue(float.Parse(SaveStatsPlayer[5]));
            Player.GetComponent<Stats>().TP.SetValue(float.Parse(SaveStatsPlayer[6]));
            Player.GetComponent<Stats>().MR.SetValue(float.Parse(SaveStatsPlayer[7]));
        }
    }

    public void LoadInventory()
    {
        // enfin on vérifie son inventaire, par défaut l'inventaire est vide donc pas besoin de faire de vérification si la chaîne de caractères est vide 
        GameObject InventorySlots = GameObject.FindGameObjectWithTag("Inventaire").transform.GetChild(0).GetChild(0).gameObject;
        GameObject PlayerEquipment = GameObject.FindGameObjectWithTag("Inventaire").transform.GetChild(0).GetChild(8).gameObject;

        string Inventory = File.ReadAllText(Application.dataPath + "/StreamingAssets/Inventory.txt");
        if(Inventory != "")
        {
            string[] Equipment = Inventory.Split(new[] { DataSeparator }, System.StringSplitOptions.None)[1].Split(new[] { ValueSeparator }, System.StringSplitOptions.None);
            string[] Items = Inventory.Split(new[] { DataSeparator }, System.StringSplitOptions.None)[0].Split(new[] { ValueSeparator }, System.StringSplitOptions.None);
            int SlotId = 0;
            for (int i = 0; i < Items.Length; i += 2)
            {
                if (Items[i] != "" & Items[i] != "NULL")
                {
                    // si on a sauvegardé un item, alors on en instancie un semblable depuis le dossier Resources
                    string ItemName = Items[i];
                    // ce if permet de résoudre un problème lié à la méthode Instantiate

                    if (ItemName.IndexOf("(Clone)") > -1)
                    {
                        ItemName = ItemName.Split(new[] { "(Clone)" }, System.StringSplitOptions.None)[0];
                    }
                    GameObject Item = Instantiate((GameObject)Resources.Load("Items/" + ItemName), InventorySlots.transform.GetChild(SlotId));
                    Item.GetComponent<Item>().stack = int.Parse(Items[i + 1]);
                }
                SlotId++;
            }

            for (int i = 0; i < Equipment.Length; i += 1)
            {
                if (Equipment[i] != "" & Equipment[i] != "NULL")
                {
                    // si on a sauvegardé un item, alors on en instancie un semblable depuis le dossier Resources
                    string ItemName = Equipment[i];
                    // ce if permet de résoudre un problème lié à la méthode Instantiate

                    if (ItemName.IndexOf("(Clone)") > -1)
                    {
                        ItemName = ItemName.Split(new[] { "(Clone)" }, System.StringSplitOptions.None)[0];
                    }
                    Instantiate((GameObject)Resources.Load("Items/" + ItemName), PlayerEquipment.transform.GetChild(i));
                }
            }
            Debug.Log("Chargement de l'inventaire effectué et de l'équipement.");
        }
    }
    public void LoadZone()
    {
        // le fichier à charger varie en fonction de la zone dans laquelle le joueur apparaît
        string ContentZone = File.ReadAllText(Application.dataPath + "/StreamingAssets/LootToCheck_" + PlayerZone + ".txt");
        GameObject LootSlots;

        // pour chaque conteneur, on compare la donnée sauvegardée au contenu originel du conteneur
        if (ContentZone != "")
        {
            string[] Splited = ContentZone.Split(new[] { DataSeparator }, System.StringSplitOptions.None);

            for (int i = 0; i < LootToCheck.Length; i++)
            {
                LootSlots = LootToCheck[i].GetComponent<Lootable>().LootInstance.transform.GetChild(0).gameObject;
                string[] Items = Splited[i].Split(new[] { ValueSeparator }, System.StringSplitOptions.None);

                for (int j = 0; j < Items.Length - 1; j++)
                {
                    
                    Transform SlotToCheck = LootSlots.transform.GetChild(j);
                    if (Items[j] == "NULL" & SlotToCheck.childCount > 0)
                    {
                        // ici le slot du conteneur a été vidé, donc on détruit l'item qui est censé y être
                        Destroy(SlotToCheck.GetComponent<ItemSlot>().empty);
                    }
                    else if (Items[j] != "NULL" & Items[j] != "")
                    {
                        if (SlotToCheck.childCount > 0)
                        {
                            if (Items[j] != SlotToCheck.GetChild(0).name)
                            {
                                string ItemName = Items[j];
                                // ce if permet de résoudre un problème lié à la méthode Instantiate
                                if (ItemName.IndexOf("(Clone)") > -1)
                                {
                                    ItemName = ItemName.Split(new[] { "(Clone)" }, System.StringSplitOptions.None)[0];
                                }
                                // ici l'item est différent de celui de base, on détruit celui présent et on met le nouveau à la place
                                Destroy(SlotToCheck.GetComponent<ItemSlot>().empty);
                                Instantiate((GameObject)Resources.Load("Items/" + ItemName), SlotToCheck);
                            }
                        }
                    }
                    
                }
            }
        }
    }

    private void LoadSettings()
    {
        // volume de la musique changé en fonction des données du fichier Settings.txt
        string Content = File.ReadAllText(Application.dataPath + "/StreamingAssets/Settings.txt");
        if (Content != "")
            Music.GetComponent<AudioSource>().volume = float.Parse(File.ReadAllText(Application.dataPath + "/StreamingAssets/Settings.txt"));
    }
}
