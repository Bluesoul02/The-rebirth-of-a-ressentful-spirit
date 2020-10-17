using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueInv : MonoBehaviour
{
    // permet de garder uniquement un seul inventaire et le plus ancien de tous
    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Inventaire").Length > 1)
        {
            Destroy(GameObject.FindGameObjectsWithTag("Inventaire")[1]);
        }
    }
}
