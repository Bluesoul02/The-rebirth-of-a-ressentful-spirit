using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script qui se déclenche lors d'une fuite ratée
public class EscapeFailed : MonoBehaviour
{
    void Update()
    {
        // le joueur est informé de son échec et doit appuyer sur espace pour que le jeu reprenne
        if (Input.GetKeyDown(KeyCode.Space) && transform.gameObject.activeInHierarchy)
        {
            transform.gameObject.SetActive(false);
            GameObject.Find("FightManager").GetComponent<FightManager>().GoToNextTurn();
        }
    }
}
