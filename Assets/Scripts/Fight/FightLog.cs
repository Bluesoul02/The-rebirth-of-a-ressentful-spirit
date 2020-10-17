using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

public class FightLog : MonoBehaviour
{
    private Text textLog;
    private Text[] logs;
    private int nbLog;
    private const int LOGLENGTH = 8;
    private int oldestLogIndex;

    public void Start()
    {
        textLog = Resources.Load<Text>("textLog");
        logs = new Text[LOGLENGTH];
        nbLog = 0;
        oldestLogIndex = 0;
    }

    public void deathLog(string name)
    {
        // Debug.Log(name + " died");
        log(name + " died");
    }

    public void defenseLog(string defender)
    {
        Debug.Log(defender + " is protecting himself");
        log(defender + " is protecting himself");
    }

    public void escapeLog(bool success)
    {
        Debug.Log("You try to escape...");
        log("You try to escape...");
        if (!success)
        {
            Debug.Log("... But you failed");
            log("... But you failed");
        }
    }

    public void attackLog(string attacker, string receiver, string attack, string dmg)
    {
        // Debug.Log(attacker + " inflict " + dmg + " to " + receiver + " with " + attack);
        log(attacker + " inflict " + dmg + " to " + receiver + " with " + attack);
    }

    public void log(string txt)
    {
        if (nbLog < LOGLENGTH)
        {
            logs[nbLog] = Instantiate(textLog, new Vector3(0, 0), Quaternion.identity, transform);
            logs[nbLog].text = txt;
            nbLog++;
        }
        else
        {
            Destroy(logs[oldestLogIndex]);
            logs[oldestLogIndex] = Instantiate(textLog, new Vector3(0, 0), Quaternion.identity, transform);
            logs[oldestLogIndex].text = txt;
            if (oldestLogIndex < (LOGLENGTH - 1))
            {
                oldestLogIndex++;
            }
            else
            {
                oldestLogIndex = 0;
            }
        }
    }
}