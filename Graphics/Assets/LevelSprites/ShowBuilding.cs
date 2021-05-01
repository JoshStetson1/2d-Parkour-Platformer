using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBuilding : MonoBehaviour
{
    public GameObject building;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerScript>() != null) building.SetActive(false);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerScript>() != null) building.SetActive(true);
    }
}
