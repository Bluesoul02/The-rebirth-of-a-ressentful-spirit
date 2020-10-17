using UnityEngine;

public class SkipTurn : MonoBehaviour
{  
    // fonction appelée quand le joueur clique sur le bouton de fin de tour 
    public void Skip()
    {
        GameObject.Find("FightManager").GetComponent<FightManager>().GoToNextTurn();
    }
}
