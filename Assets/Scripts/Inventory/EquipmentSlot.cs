using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Globalization;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    public string type;
    private GameObject player;
    private bool used;
    private StatModifier statInUse;
    private Item itemInUse;

    // vérifie si le slot est vide ou non
    public GameObject empty
    {
        get
        {
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;
            }
            return null;
        }
    }

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Update()
    {
        // si le joueur est detruit
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        // si on équipe l'item
        if (empty != null && !used)
        {
            itemInUse = empty.GetComponent<Item>();
            statManager(used);
            used = true;
        }
        // si on déséquipe l'item
        else if (empty == null && used)
        {
            statManager(used);
            used = false;
        }
        // permet de régler un bug de duplication
        if (transform.childCount > 1)
        {
            for (int i = 1; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                Item item = child.GetComponent<Item>();
                InvDragAndDrop inv = child.GetComponent<InvDragAndDrop>();
                Text weight = inv.getWeight();
                weight.text = (double.Parse(inv.getWeight().text, CultureInfo.InvariantCulture) - item.weight * item.stack).ToString(inv.nfi);
                inv.setWeight(weight);
                Destroy(child);
            }
        }
    }

    // attribue les différentes stats de l'objet au joueur
    private void statManager(bool use)
    {
        int stat;
        string type;

        // pour chaque stat que l'item offre l'applique au joueur
        for (int i = 1; i <= 3; i++)
        {
            if (i == 1)
            {
                stat = itemInUse.stat1;
                type = itemInUse.typeStat1;
            }
            else if (i == 2)
            {
                stat = itemInUse.stat2;
                type = itemInUse.typeStat2;
            }
            else
            {
                stat = itemInUse.stat3;
                type = itemInUse.typeStat3;
            }
            if (stat != 0)
            {
                // si l'item améliore la défense
                if (type == "Defense")
                {
                    if (use == true)
                    {
                        player.GetComponent<Stats>().Defense.RemoveModifier(new StatModifier(stat, StatModType.Flat));
                    }
                    else
                    {
                        player.GetComponent<Stats>().Defense.AddModifier(new StatModifier(stat, StatModType.Flat));
                    }

                }
                // si l'item améliore la vie
                else if (type == "Health")
                {
                    if (use == true)
                    {
                        player.GetComponent<Stats>().Health.BaseValue -= (float)stat;
                    }
                    else
                    {
                        player.GetComponent<Stats>().Health.BaseValue += (float)stat;
                    }
                }
                // si l'item améliore l'attaque
                else if (type == "Attack")
                {
                    if (use == true)
                    {
                        player.GetComponent<Stats>().Atk.RemoveModifier(new StatModifier(stat, StatModType.Flat));
                    }
                    else
                    {
                        player.GetComponent<Stats>().Atk.AddModifier(new StatModifier(stat, StatModType.Flat));
                    }
                }
                // si l'item améliore la vitesse
                else if (type == "Speed")
                {
                    if (use == true)
                    {
                        player.GetComponent<Stats>().Speed.RemoveModifier(new StatModifier(stat, StatModType.Flat));
                    }
                    else
                    {
                        player.GetComponent<Stats>().Speed.AddModifier(new StatModifier(stat, StatModType.Flat));
                    }
                }
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // si le slot est vide et que l'item est du bon type alors on l'équipe
        if (!empty && eventData.pointerDrag.GetComponent<Item>().type == this.type)
        {
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.transform.position = transform.position;

        }
    }
}
