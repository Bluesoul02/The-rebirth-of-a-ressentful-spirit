using UnityEngine;
using System.Collections.Generic;

// ce script permet de contenir des informations utiles sur le joueur, certaines sont même sauvegardées

public class InfosPlayer : MonoBehaviour
{
    private bool LoosingRespawning;
    private bool WinningRespawning;
    private bool EscapeRespawning;
    public string Zone;
    private string NameEnemyEncountred;
    public List<string> EnemiesKilled;

    private void Awake()
    {
        GameObject Infos = GameObject.FindGameObjectWithTag("Infos");
        if (Infos & gameObject.name != "Infos" & gameObject.name == "Player")
        {
            CopyInfosFrom(Infos);
        }
        else
        {
            LoosingRespawning = false;
            WinningRespawning = false;
            EscapeRespawning = false;
            NameEnemyEncountred = "";
            EnemiesKilled = new List<string>();
        }
    }
    public bool IsLoosingRespawning()
    {
        return LoosingRespawning;
    }

    public void SetLoosingRespawning(bool r)
    {
        LoosingRespawning = r;
    }

    public bool IsWinningRespawning()
    {
        return WinningRespawning;
    }

    public void SetWinningRespawning(bool r)
    {
        WinningRespawning = r;
    }

    public bool IsEscapeRespawning()
    {
        return EscapeRespawning;
    }

    public void SetEscapeRespawning(bool r)
    {
        EscapeRespawning = r;
    }

    public string GetZone()
    {
        return Zone;
    }

    public void SetZone(string z)
    {
        Zone = z;
    }
    public string GetNameEnemyEncountred()
    {
        return NameEnemyEncountred;
    }

    public void SetNameEnemyEncountred(string n)
    {
        NameEnemyEncountred = n;
    }

    public bool IsRespawning()
    {
        return EscapeRespawning | WinningRespawning | LoosingRespawning;
    }

    public void CopyInfosFrom(GameObject g)
    {
        LoosingRespawning = g.GetComponent<InfosPlayer>().IsLoosingRespawning();
        WinningRespawning = g.GetComponent<InfosPlayer>().IsWinningRespawning();
        EscapeRespawning = g.GetComponent<InfosPlayer>().IsEscapeRespawning();
        NameEnemyEncountred = g.GetComponent<InfosPlayer>().GetNameEnemyEncountred();
        EnemiesKilled = g.GetComponent<InfosPlayer>().EnemiesKilled;
    }
}
