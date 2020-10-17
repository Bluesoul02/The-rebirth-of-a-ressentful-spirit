using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStat : MonoBehaviour
{
    private float maxHP;
    private float currentHP;
    private Text uiHP;
    private Stats stats;

    void Start()
    {
        // setup
        GameObject canvas = GameObject.Find("HealthDisplay");
        GameObject child = new GameObject("Health");
        // child.name = "Health";
        child.transform.SetParent(canvas.transform);

        // position et taille
        Vector3 pos = transform.position;
        float x = canvas.transform.position.x;
        float y = canvas.transform.position.y;
        if (transform.tag == "Player")
        {
            y += pos.y;
            x += (pos.x + pos.x/3);
        }
        else
        {
            if (transform.GetComponent<Infos>().MobName.Contains("3"))
            {
                y -= pos.y + pos.y / 10;
            }
            else if (transform.GetComponent<Infos>().MobName.Contains("2"))
            {
                y += pos.y - pos.y;
            }
            else
            {
                y += pos.y + pos.y/6;
            }
            x -= (pos.x + pos.x / 2 - pos.x/3);
        }
        Vector2 v = new Vector2(x, y);
        canvas.GetComponent<CanvasScaler>().referenceResolution.Scale(v);
        RectTransform rectTransform = child.gameObject.AddComponent<RectTransform>();
        rectTransform.position = new Vector3(v.x, v.y);
        rectTransform.sizeDelta = new Vector2(500, 120);

        // texte
        uiHP = child.gameObject.AddComponent<Text>();
        uiHP.fontSize = 80;
        uiHP.font = Resources.Load<Font>("Thaleah_PixelFont/Materials/ThaleahFat_TTF");
        uiHP.alignment = TextAnchor.MiddleCenter;

        // stats
        stats = transform.gameObject.GetComponent<Stats>();
        if (stats != null)
        {
            currentHP = stats.Health.Value;
            maxHP = stats.Health.BaseValue;
            uiHP.text = "HP: " + currentHP.ToString() + "/" + maxHP.ToString();
        }
    }

    void Update()
    {
        if (stats != null && (currentHP != stats.Health.Value || maxHP != stats.Health.BaseValue))
        {
            currentHP = stats.Health.Value;
            maxHP = stats.Health.BaseValue;
            uiHP.text = "HP : " + currentHP.ToString() + "/" + maxHP.ToString();
        }
    }

    public void dead()
    {
        Destroy(uiHP);
    }
}
