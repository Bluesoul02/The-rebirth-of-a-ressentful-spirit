using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackPanel : MonoBehaviour
{
    private int NbEnemies;
    private List<GameObject> Enemies;
    private FightManager FightManager;
    private void Awake()
    {
        FightManager = GameObject.Find("FightManager").GetComponent<FightManager>();
        Enemies = FightManager.GetEnemies();
        NbEnemies = FightManager.GetNbEnemies();

        // optimisation : on ne charge qu'une seule fois la ressource
        Button attackButton = Resources.Load<Button>("Buttons/AttackButton");

        for (int i = 0; i < NbEnemies; i++)
        {
            Button button = Instantiate(attackButton);
            // le grid layout group prend en charge la position
            button.GetComponent<AttackButton>().SetEnemy(Enemies[i]);
            button.GetComponent<AttackButton>().SetIdEnemy(i);
            button.transform.SetParent(transform, false);
        }
    }
}
