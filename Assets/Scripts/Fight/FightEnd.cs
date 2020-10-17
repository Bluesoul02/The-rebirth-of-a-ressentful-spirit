using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightEnd : MonoBehaviour
{
    private FightManager fightManager;

    void Awake()
    {
        fightManager = GameObject.FindGameObjectWithTag("FightManager").GetComponent<FightManager>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && transform.GetChild(0).gameObject.activeInHierarchy)
        {
            if (transform.name == "Defeat")
            {
                Debug.Log("defeat");
                fightManager.GetPlayer().GetComponent<Stats>().ExitOfLoosedFight();
                fightManager.GetPlayer().GetComponent<Stats>().Health.AddModifier(new StatModifier(-(fightManager.GetPlayer().GetComponent<Stats>().Health.Value - 1), StatModType.Flat, "lose"));
            }
            fightManager.enabled = false;
            fightManager.quit();
        }
    }
}
