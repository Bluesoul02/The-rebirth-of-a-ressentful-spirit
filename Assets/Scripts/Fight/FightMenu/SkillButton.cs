using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    private bool isMagic;
    private GameObject player;
    private SkillsPlayer skills;
    public int id;
    void Awake()
    {
        isMagic = transform.parent.name == "MagicPanel";
        player = GameObject.FindGameObjectWithTag("Player");
        skills = player.GetComponent<SkillsPlayer>();
    }

    void Update()
    {
        if (isMagic && player.GetComponent<Stats>().Mana.Value < skills.getPlayerSpells()[id].getCost())
        {
            gameObject.GetComponent<Button>().interactable = false;
        }
        else if (!isMagic && player.GetComponent<Stats>().TP.Value < skills.getPlayerSkills()[id].getCost())
        {
            gameObject.GetComponent<Button>().interactable = false;
        }
        else
        {
            gameObject.GetComponent<Button>().interactable = true;
        }
    }
}
