using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Globalization;

public class InvDragAndDrop : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    Vector3 startPosition;
    Transform startParent;
    private Text weight;
    private Text maxWeight;
    public NumberFormatInfo nfi;
    private CanvasGroup canvasGroup;
    private GameObject player;
    private bool inInventory;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        canvasGroup = GetComponent<CanvasGroup>();
        startParent = transform.parent;
        startPosition = transform.position;
        transform.position = startPosition;
        Transform inventory = GameObject.FindGameObjectWithTag("Inventaire").transform.GetChild(0);
        inInventory = startParent.name.Contains("Inventory") || startParent.parent.name.Contains("Equipment");
        nfi = new NumberFormatInfo();
        nfi.NumberDecimalSeparator = ".";
        if (inventory != null)
        {
            weight = inventory.Find("Weight").GetComponent<Text>();
            maxWeight = inventory.Find("MaxWeight").GetComponent<Text>();
            if (inInventory)
            {
                Item item = gameObject.GetComponent<Item>();
                weight.text = (double.Parse(weight.text, CultureInfo.InvariantCulture) + item.weight * item.stack).ToString(nfi);
            }
        }
    }
    public void Update()
    {
        // si le joueur est detruit
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public Text getWeight()
    {
        return weight;
    }

    public Text getMaxWeight()
    {
        return maxWeight;
    }

    public void setWeight(Text weight)
    {
        this.weight = weight;
    }

    public bool isInInventory()
    {
        return inInventory;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // si l'item qu'on prend est dans l'inventaire on retire son poids
        if (inInventory)
        {
            Item item = eventData.pointerDrag.GetComponent<Item>();
            weight.text = (double.Parse(weight.text, CultureInfo.InvariantCulture) - item.weight * item.stack).ToString(nfi);
        }
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // si l'item est drop à un endroit non valide il retourne à sa position de départ
        if (startParent == transform.parent)
        {
            transform.position = startPosition;
        }
        // si le poids depasse la limite on ne peut pas ajouter d'items
        if (double.Parse(weight.text, CultureInfo.InvariantCulture) >= double.Parse(maxWeight.text, CultureInfo.InvariantCulture)
            &&
            (eventData.pointerDrag.transform.parent.name.Contains("Inventory") || eventData.pointerDrag.transform.parent.parent.name.Contains("Equipment")) && !inInventory)
        {
            transform.position = startPosition;
            transform.SetParent(startParent);
        }
        // si l'item est mis dans l'inventaire on ajoute son poids
        else if (eventData.pointerDrag.transform.parent.name.Contains("Inventory") || eventData.pointerDrag.transform.parent.parent.name.Contains("Equipment"))
        {
            Item item = eventData.pointerDrag.GetComponent<Item>();
            weight.text = (double.Parse(weight.text, CultureInfo.InvariantCulture) + item.weight * item.stack).ToString(nfi);
        }
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        // on met à jour la position à laquelle l'item ira si il est drop à un endroit non valide
        startParent = transform.parent;
        startPosition = startParent.position;
        // on vérifie si il a été drop dans l'inventaire
        inInventory = startParent.name.Contains("Inventory") || startParent.parent.name.Contains("Equipment");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // si on double click on utilise l'item
        if (eventData.clickCount == 2)
        {
            Debug.Log("double click");
            Item itemUsed = transform.GetComponent<Item>();
            if (itemUsed.type == "usable")
            {
                // application des effets de l'utilisation de l'item
                int stat;
                string type;
                Stats stats = player.GetComponent<Stats>();
                for (int i = 1; i <= 3; i++)
                {
                    if (i == 1)
                    {
                        stat = itemUsed.stat1;
                        type = itemUsed.typeStat1;
                    }
                    else if (i == 2)
                    {
                        stat = itemUsed.stat2;
                        type = itemUsed.typeStat2;
                    }
                    else
                    {
                        stat = itemUsed.stat3;
                        type = itemUsed.typeStat3;
                    }
                    if (stat != 0)
                    {
                        if (type == "Health")
                        {
                            if (stats.Health.Value < stats.Health.BaseValue)
                            {
                                stats.Health.AddModifier(new StatModifier(stat, StatModType.Flat));
                            }
                        }
                        if (type == "Mana")
                        {
                            if (stats.Mana.Value < stats.Mana.BaseValue)
                            {
                                stats.Mana.AddModifier(new StatModifier(stat, StatModType.Flat));
                            }
                        }
                        Debug.Log("Vous regagnez " + stat + " " + type);
                    }
                }
                // destruction de l'item si il n'y a qu'un stack
                if (itemUsed.stack == 1)
                {
                    Destroy(transform.gameObject);
                }
                // on retire un stack si il y en a plus qu'un
                else
                {
                    itemUsed.stack--;
                }
                // suppression du poids
                if (inInventory)
                {
                    weight.text = (double.Parse(weight.text, CultureInfo.InvariantCulture) - transform.gameObject.GetComponent<Item>().weight).ToString(nfi);
                }
            }
        }
    }
}
