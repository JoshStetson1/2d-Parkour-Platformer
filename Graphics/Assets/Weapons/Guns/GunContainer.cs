using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunContainer : MonoBehaviour
{
    public float timeToClose = 2;
    float startTime;

    public void Start()
    {
        transform.Find("Container").gameObject.SetActive(false);
    }
    public void Update()
    {
        if (startTime + timeToClose < Time.time) transform.Find("Container").gameObject.SetActive(false);
    }
    public void waitToClose()
    {
        startTime = Time.time;
    }
}
