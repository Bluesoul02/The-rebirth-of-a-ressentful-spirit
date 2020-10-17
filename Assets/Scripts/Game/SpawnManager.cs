using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private GameObject Infos;
    public GameObject ToDestroyOnRespawn;
    public GameObject Player;
    private bool SpawnDone = false;

    // la méthode Awake est appelée quand on arrive ou revient dans la scène
    void Update()
    {
        if (!SpawnDone)
        {
            if (GameObject.FindGameObjectWithTag("Infos"))
            {
                if (Player == null)
                {
                    Player = GameObject.FindGameObjectWithTag("Player");
                }
                Infos = GameObject.Find("Infos");
                List<string> EnemiesKilled = Infos.GetComponent<InfosPlayer>().EnemiesKilled;

                if (Infos.GetComponent<InfosPlayer>().IsLoosingRespawning())
                {
                    // si le joueur a perdu un combat alors on le déplace sur SpawnManager
                    Infos.GetComponent<InfosPlayer>().SetLoosingRespawning(false);
                    Player.transform.position = transform.position;

                    // ici on détruit le deuxième inventaire qui s'est créé avec le changement de scène sinon ça amène des bugs
                    if (ToDestroyOnRespawn != null)
                        Destroy(ToDestroyOnRespawn);
                }
                else if (Infos.GetComponent<InfosPlayer>().IsWinningRespawning())
                {
                    // mais s'il a gagné, on peut le laisser à la position de l'ennemi qu'il a battu
                    Infos.GetComponent<InfosPlayer>().SetWinningRespawning(false);
                    Player.transform.position = GameObject.Find(EnemiesKilled[EnemiesKilled.Count - 1]).transform.position;

                    // ici on détruit le deuxième inventaire qui s'est créé avec le changement de scène sinon ça amène des bugs
                    if (ToDestroyOnRespawn != null)
                        Destroy(ToDestroyOnRespawn);
                }
                else if (Infos.GetComponent<InfosPlayer>().IsEscapeRespawning())
                {
                    // par contre s'il s'est enfui alors le joueur se retrouve à une position spécifique sans danger mais proche de l'ennemi qu'il vient de fuir
                    Debug.Log("le joueur s'est enfui");
                    Infos.GetComponent<InfosPlayer>().SetEscapeRespawning(false);
                    Player.transform.position = GameObject.Find("EscapeRespawn" + Infos.GetComponent<InfosPlayer>().GetNameEnemyEncountred()).transform.position;

                    // ici on détruit le deuxième inventaire qui s'est créé avec le changement de scène sinon ça amène des bugs
                    if (ToDestroyOnRespawn != null)
                        Destroy(ToDestroyOnRespawn);
                }

                // on enlève de la scène les ennemis que le joueur a battus 
                KillEnemies(EnemiesKilled.ToArray());

                // enfin on copie les infos contenues par Infos dans le joueur par souci de clarté
                Player.GetComponent<Stats>().CopyStatsFrom(Infos);
                Player.GetComponent<InfosPlayer>().CopyInfosFrom(Infos);
            }
        }
        SpawnDone = true;
    }

    public void KillEnemies(string[] EnemiesToKill)
    {
        for (int i = 0; i < EnemiesToKill.Length; i++)
        {
            // pour chaque ennemis battus, on détruit ses deux repères de déplacement, le point sûr de fuite qui lui est associé et sa présence dans la scène
            Destroy(GameObject.Find(EnemiesToKill[i]));
            Destroy(GameObject.Find("Waypoint1" + EnemiesToKill[i]));
            Destroy(GameObject.Find("Waypoint2" + EnemiesToKill[i]));
            Destroy(GameObject.Find("EscapeRespawn" + EnemiesToKill[i]));
        }
    }
}
