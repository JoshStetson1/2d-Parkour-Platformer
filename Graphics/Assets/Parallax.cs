using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    GameObject cam;

    public float parallaxEffect;
    public bool repeat;
    float startPos, length;

    void Start()
    {
        cam = Camera.main.gameObject;
        startPos = transform.position.x;
        
        if(repeat) length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float dist = cam.transform.position.x * (1 - parallaxEffect);
        float tempPos = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector2(startPos + tempPos, transform.position.y);

        if (repeat)
        {
            if (dist > startPos + length) startPos += length;
            if (dist < startPos - length) startPos -= length;
        }
    }
}
