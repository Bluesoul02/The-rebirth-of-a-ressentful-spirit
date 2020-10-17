using UnityEngine;

// ce script contient les stats qu'on tous les personnages du jeu
// mais chacun a des valeurs différentes pour ces stats évidemment
public class Stats : MonoBehaviour
{
    public CharacterStat Speed;
    public CharacterStat Health;
    public CharacterStat Mana;
    public CharacterStat AP;
    public CharacterStat Defense;
    public CharacterStat Atk;
    public CharacterStat TP;
    public CharacterStat MR;

    private void Awake()
    {
        GameObject Infos = GameObject.FindGameObjectWithTag("Infos");
        if (Infos & gameObject.name != "Infos" & gameObject.name == "Player")
        {
            CopyStatsFrom(Infos);
        }
    }

    // permet de copier les stats depuis un autre GameObject, utile pour Infos et pour une potentielle capacité d'imitation
    public void CopyStatsFrom(GameObject g)
    {
        Speed = g.GetComponent<Stats>().Speed;
        // Speed.SetValue(g.GetComponent<Stats>().Speed.Value);
        Health = g.GetComponent<Stats>().Health;
        // Health.SetValue(g.GetComponent<Stats>().Health.Value);
        Mana = g.GetComponent<Stats>().Mana;
        // Mana.SetValue(g.GetComponent<Stats>().Mana.Value);
        Defense = g.GetComponent<Stats>().Defense;
        // Defense.SetValue(g.GetComponent<Stats>().Defense.Value);
        Atk = g.GetComponent<Stats>().Atk;
        // Atk.SetValue(g.GetComponent<Stats>().Atk.Value);
        AP = g.GetComponent<Stats>().AP;
        // AP.SetValue(g.GetComponent<Stats>().AP.Value);
        TP = g.GetComponent<Stats>().TP;
        // TP.SetValue(g.GetComponent<Stats>().TP.Value);
        MR = g.GetComponent<Stats>().MR;
        // MR.SetValue(g.GetComponent<Stats>().MR.Value);
    }

    // permet d'enlever tous les modificateurs qu'un combat a mis sur le joueur
    public void ExitOfLoosedFight()
    {
        Speed.RemoveAllModifiersFromSource("fight");
        Health.RemoveAllModifiersFromSource("fight");
        Mana.RemoveAllModifiersFromSource("fight");
        Defense.RemoveAllModifiersFromSource("fight");
        Atk.RemoveAllModifiersFromSource("fight");
        AP.RemoveAllModifiersFromSource("fight");
        TP.RemoveAllModifiersFromSource("fight");
        MR.RemoveAllModifiersFromSource("fight");
    }
}
