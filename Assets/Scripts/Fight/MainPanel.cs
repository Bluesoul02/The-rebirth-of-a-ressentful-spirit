using UnityEngine;

public class MainPanel : MonoBehaviour
{
    private FightManager fightManager;

    void Awake()
    {
        fightManager = GameObject.Find("FightManager").GetComponent<FightManager>();
    }

    void Update()
    {
        if (fightManager.GetCurrentFighter() != fightManager.GetPlayer())
        {
            gameObject.SetActive(false);
        }
    }
}
