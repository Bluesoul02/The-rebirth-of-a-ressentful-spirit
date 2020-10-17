using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public string name;
    public string type;
    public int stat1;
    public string typeStat1;
    public int stat2;
    public string typeStat2;
    public int stat3;
    public string typeStat3;
    public double weight;
    public const int STACKLIMIT = 5;
    public int stack = 1;
    private int oldStack; // pour limiter l'usage de ressources

    public void Awake()
    {
        // permet de renommer l'item si on Instantiate et afin que l'on puisse le stacker
        if (name != "")
        {
            transform.name = name;
        }
        // si c'est item utilisable on met en place son nombre de stack sur l'UI
        if (type == "usable")
        {
            transform.GetChild(0).GetComponent<Text>().text = stack.ToString();
        }
        oldStack = stack;
    }

    public void Update()
    {
        // permet de renommer l'item si on Instantiate et afin que l'on puisse le stacker
        if (name != "")
        {
            transform.name = name;
        }
        // si le nombre de stack change on le change aussi sur l'UI
        if (oldStack != stack)
        {
            transform.GetChild(0).GetComponent<Text>().text = stack.ToString();
            oldStack = stack;
        }
    }
}
