using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandStill : MonoBehaviour
{
    EnemyScript enemyScript;
    GameObject player;

    public bool killOnSlide = true;

    void Start()
    {
        enemyScript = GetComponent<EnemyScript>();
        player = FindObjectOfType<PlayerScript>().gameObject;
    }
    void Update()
    {
        if (enemyScript.isDead) return;

        if (killOnSlide)
        {
            //check if player hit enemy while sliding
            if (player.GetComponent<PlayerController>().sliding)
            {
                Vector2 feetSlide = new Vector2(player.transform.position.x + (0.8f * player.transform.localScale.x), player.transform.position.y - 1.2f);
                if (Mathf.Abs(feetSlide.x - transform.position.x) < 0.6 && Mathf.Abs(feetSlide.y - transform.position.y) < 1.5)//player feet intersect enemy
                {
                    float force = player.GetComponent<Rigidbody2D>().velocity.x / 3;
                    enemyScript.die(new Vector3(0, 1.25f, 1) * force);
                    if (enemyScript.weapon != null) enemyScript.weapon.pickUp(player);
                }
            }
        }
        //flipping enemy
        Vector3 scale = transform.localScale;
        if (player.transform.position.x < transform.position.x) transform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
        if (player.transform.position.x > transform.position.x) transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
    }
}
