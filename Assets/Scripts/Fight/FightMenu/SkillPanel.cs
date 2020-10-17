using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
    public GameObject nextPanel;
    private SkillsPlayer skills;
    private bool isMagic;


    void Awake()
    {
        isMagic = transform.name == "MagicPanel";
        Button skillButton = Resources.Load<Button>("Buttons/SkillButton");
        skills = GameObject.FindGameObjectWithTag("Player").GetComponent<SkillsPlayer>();
        int max = 0;
        if (isMagic)
        {
            max = skills.getNbSpells();
        }
        else
        {
            max = skills.getNbSkills();
        }
        for (int i = 0; i < max; i++)
        {
            Button button = Instantiate(skillButton, new Vector3(0, 0, 0), Quaternion.identity, transform);
            Skill skill;
            if (isMagic)
            {
                skill = skills.getPlayerSpells()[i];
            }
            else
            {
                skill = skills.getPlayerSkills()[i];
            }
            button.GetComponentInChildren<Text>().text = skill.getName();
            button.GetComponent<SkillButton>().id = i;
            button.transform.Find("InfosUI").GetComponent<InfosUI>().id = i;
            if (skill.getRange() == Range.ALL)
            {
                button.onClick.AddListener(delegate { attackAll(skill); });
            }
            else
            {
                button.onClick.AddListener(delegate { skillUse(skill); });
            }
        }
    }

    void skillUse(Skill skill)
    {
        bool attacking = nextPanel.name == "AttackPanel";
        if (attacking)
        {
            for (int i = 0; i < nextPanel.transform.childCount; i++)
            {
                nextPanel.GetComponentsInChildren<AttackButton>()[i].setSkill(skill);
            }
        }
        transform.parent.gameObject.SetActive(false);
        nextPanel.SetActive(true);
    }

    public void attackAll(Skill skill)
    {
        GameObject.FindGameObjectWithTag("FightManager").GetComponent<FightManager>().PlayerAttacksAll(skill);
    }
}
