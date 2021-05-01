﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    Explosion explosion;
    Bullet bullet;

    void Start()
    {
        explosion = GetComponent<Explosion>();
        bullet = GetComponent<Bullet>();
    }
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.root.gameObject != bullet.origin)//doesn't hit itself
        {
            if (explosion.explodeOnBullet) explosion.Explode();
            else if (collision.GetComponent<Bullet>() == null) explosion.Explode();
        }
    }
}
