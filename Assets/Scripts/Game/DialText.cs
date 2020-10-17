using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialText : MonoBehaviour
{
    public GameObject eventAfterDial;
    public string[] firstText;
    public string[] secondText;
    public bool twoDial;
    private bool talked;
    // si il y plusieurs évènement dans une scène on le change depuis 
    public int indexEvent = 0;

    void Start()
    {
        DialEvents dialEvents = GameObject.Find("Dialogue").GetComponent<DialEvents>();
        // s'il s'agit du deuxième ou plus awake d'un dialogue possèdant un évènement
        if (eventAfterDial && dialEvents.done[indexEvent])
        {
            eventAfterDial.SetActive(dialEvents.values[indexEvent]);
            talked = dialEvents.values[indexEvent];
        }
        // s'il s'agit du premier awake d'un dialogue possèdant un évènement
        else if (eventAfterDial && !dialEvents.done[indexEvent])
        {
            // on indique que le premier awake a été effectué
            dialEvents.done[indexEvent] = true;
            dialEvents.values[indexEvent] = eventAfterDial.activeInHierarchy;
        }
    }

    public bool hasTalked()
    {
        return talked;
    }

    public void Talked()
    {
        talked = true;
        // met à jour la liste d'évènements des dialogues si évènement
        if (eventAfterDial)
        {
            GameObject.Find("Dialogue").GetComponent<DialEvents>().values[indexEvent] = eventAfterDial.activeInHierarchy;
            Debug.Log(eventAfterDial.activeInHierarchy);
        }
    }
}
