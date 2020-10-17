using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// script qui affiche des infos supplémentaires sur les talents et les sorts que le joueur possède
public class InfosUI : MonoBehaviour
{

    private bool isMagic;
    private bool isSkill;
    public int id;
    void Start()
    {
        isSkill = transform.parent.parent.name == "SkillsPanel";
        isMagic = transform.parent.parent.name == "MagicPanel";
        gameObject.SetActive(false);
        SkillsPlayer skills = GameObject.FindGameObjectWithTag("Player").GetComponent<SkillsPlayer>();
        GameObject infos = transform.GetChild(0).gameObject;

        // on ajoute le texte correspondant à la description du skill
        if (isSkill)
        {
            infos.GetComponentsInChildren<Text>()[0].text += skills.getPlayerSkills()[id].getRange().ToString();
            infos.GetComponentsInChildren<Text>()[1].text += skills.getPlayerSkills()[id].getDamage();
            infos.GetComponentsInChildren<Text>()[2].text += skills.getPlayerSkills()[id].getCost();
        }
        else if (isMagic)
        {
            infos.GetComponentsInChildren<Text>()[0].text += skills.getPlayerSpells()[id].getRange().ToString();
            infos.GetComponentsInChildren<Text>()[1].text += skills.getPlayerSpells()[id].getDamage();
            infos.GetComponentsInChildren<Text>()[2].text += skills.getPlayerSpells()[id].getCost();
        }

        // permet d'afficher les descriptions seulement quand on passe le curseur dessus en jeu
        EventTrigger trigger = GetComponentInParent<EventTrigger>(); // event trigger permet de récupérer les évènements qui arrivent sur l'objet
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventData) => { OnPointerEnter(); }); // ici on ajoute une fonction (OnPointerEnter) qui sera un listener à l'objet
        trigger.triggers.Add(entry);
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((eventData) => { OnPointerExit(); });
        trigger.triggers.Add(entry);
    }
    public void OnPointerEnter()
    {
        gameObject.SetActive(true);
    }

    public void OnPointerExit()
    {
        gameObject.SetActive(false);
    }
}
