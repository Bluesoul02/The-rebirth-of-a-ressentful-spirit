using UnityEngine;

public class SelectAction : MonoBehaviour
{
    public GameObject panel;
    public GameObject inv;

    public void Awake()
    {
        if (transform.name == "ItemsButton")
        {
            inv = GameObject.FindGameObjectWithTag("Inventaire").transform.GetChild(0).gameObject;
            CancelAction cancel = inv.AddComponent<CancelAction>();
            cancel.previousPanel = transform.parent.gameObject;
            cancel.currentPanel = inv;
            cancel.cancel = "c";
        }
    }

    void deactivatePanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    void activatenextPanel(GameObject nextPanel)
    {
        if (nextPanel != null)
        {
            nextPanel.SetActive(true);
        }
    }

    public void Attack(GameObject attackPanel)
    {
        deactivatePanel();
        for (int i = 0; i < attackPanel.transform.childCount; i++)
        {
            attackPanel.GetComponentsInChildren<AttackButton>()[i].setSkill(new Skill("auto-attack", GameObject.Find("FightManager").GetComponent<FightManager>().GetPlayer().GetComponent<Stats>().Atk.Value, 0, Range.SINGLE));
        }
        activatenextPanel(attackPanel);
    }

    public void Defend()
    {
        // double la stat de defense pour le moment
        Debug.Log("click defend");
        GameObject.Find("FightManager").GetComponent<FightManager>().DefendAction();
    }

    public void SkillsOrMagic(GameObject nextPanel)
    {
        deactivatePanel();
        activatenextPanel(nextPanel);
    }

    public void Items()
    {
        deactivatePanel();
        activatenextPanel(inv);
    }

    public void Escape(GameObject escapePanel)
    {
        FightManager fightManager = GameObject.Find("FightManager").GetComponent<FightManager>();
        if (Random.Range(0, 10) <= GameObject.FindGameObjectWithTag("Player").GetComponent<Stats>().Speed.Value)
        {
            // Fuite
            fightManager.quit();
        }
        else
        {
            deactivatePanel();
            fightManager.escapeFailed();
            activatenextPanel(escapePanel);
        }
    }
}
