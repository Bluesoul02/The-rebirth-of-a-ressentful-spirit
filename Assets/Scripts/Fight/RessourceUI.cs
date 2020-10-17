using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RessourceUI : MonoBehaviour
{
    private float max;
    private float current;
    private Stats stats;
    private bool isMP;
    void Awake()
    {
        isMP = transform.parent.name == "MagicPanel";
        stats = GameObject.FindGameObjectWithTag("Player").GetComponent<Stats>();
        if (stats != null)
        {
            if (isMP)
            {
                current = stats.Mana.Value;
                max = stats.Mana.BaseValue;
                GetComponent<Text>().text = "MP : " + current.ToString() + "/" + max.ToString();
            }
            else
            {
                current = stats.TP.Value;
                max = stats.TP.BaseValue;
                GetComponent<Text>().text = "TP : " + current.ToString() + "/" + max.ToString();
            }
        }
    }

    void Update()
    {
        if (stats != null)
        {
            if (isMP && (current != stats.Mana.Value || max != stats.Mana.BaseValue))
            {
                current = stats.Mana.Value;
                max = stats.Mana.BaseValue;
                GetComponent<Text>().text = "MP : " + current.ToString() + "/" + max.ToString();
            }
            else if (!isMP && (current != stats.TP.Value || max != stats.TP.BaseValue))
            {
                current = stats.TP.Value;
                max = stats.TP.BaseValue;
                GetComponent<Text>().text = "TP : " + current.ToString() + "/" + max.ToString();
            }
        }
    }
}
