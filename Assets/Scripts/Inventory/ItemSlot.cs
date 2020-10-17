using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Globalization;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    // on vérifie si le slot est vide
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

    public void OnDrop(PointerEventData eventData)
    {
        // si le slot n'est pas vide et que l'item dragged n'est pas l'item appartenant au slot
        if (empty && empty != eventData.pointerDrag)
        {
            // si l'item est utilisable, il est donc stackable
            Item item = eventData.pointerDrag.GetComponent<Item>();
            if (item.type == "usable")
            {
                // on vérifie qu'on ne dépasse pas la limite de stack et qu'il s'agit du même type d'item
                Item childItem = empty.transform.GetComponent<Item>();
                InvDragAndDrop inv = eventData.pointerDrag.GetComponent<InvDragAndDrop>();
                if ((childItem.stack + item.stack) <= Item.STACKLIMIT && childItem.name == item.name)
                {
                    // si on stack dans l'inventaire on ajoute le poids des items
                    bool checkWeight = double.Parse(inv.getWeight().text, CultureInfo.InvariantCulture) < double.Parse(inv.getMaxWeight().text, CultureInfo.InvariantCulture);
                    Debug.Log(childItem.name == item.name);
                    if ((inv.isInInventory() || !inv.isInInventory()) && empty.GetComponent<InvDragAndDrop>().isInInventory() && checkWeight)
                    {
                        Debug.Log("Stack");
                        childItem.stack += item.stack;
                        Text weight = inv.getWeight();
                        weight.text = (double.Parse(inv.getWeight().text, CultureInfo.InvariantCulture) + item.weight * item.stack).ToString(inv.nfi);
                        inv.setWeight(weight);
                        Destroy(eventData.pointerDrag);
                    }
                    // si on stack en dehors de l'inventaire, on met juste à jour le nombre de stack
                    else if ((!inv.isInInventory() || inv.isInInventory()) && !empty.GetComponent<InvDragAndDrop>().isInInventory())
                    {
                        Debug.Log("Stack");
                        childItem.stack += item.stack;
                        Destroy(eventData.pointerDrag);
                    }
                }
            }
        }
        // sinon on met l'item dans le slot
        else if (!empty)
        {
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.transform.position = transform.position;
        }
    }

    public void Update()
    {
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
}
