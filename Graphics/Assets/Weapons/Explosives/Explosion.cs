using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explodeDistance, blastForce, damage, rangeRatio;
    public bool destroyOnExplode, explodeOnBullet;
    [HideInInspector]
    public bool exploded;

    public GameObject boom;

    public void Explode()
    {
        if (exploded) return;

        exploded = true;

        FindObjectOfType<AudioManager>().Play("explode");
        Instantiate(boom, transform.position, transform.rotation);

        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, explodeDistance);

        foreach (Collider2D obj in objects)
        {
            if (obj.transform.name != obj.transform.root.name) continue;//body parts dont get effected

            Vector2 direction = (obj.transform.position - transform.position).normalized;
            float fDistance = Vector2.Distance(obj.transform.position, transform.position);
            float fForce = (blastForce / (explodeDistance * rangeRatio)) * (explodeDistance - fDistance);

            RaycastHit2D sightLine = Physics2D.Raycast(transform.position, direction, fDistance, 8);
            if (sightLine.collider == null)//there is nothing in between the barrel and the thing it hit
            {
                Vector2 force = direction * fForce;
                //blow back
                if (obj.GetComponent<Rigidbody2D>() != null && obj.GetComponent<Bullet>() == null) obj.GetComponent<Rigidbody2D>().AddForce(direction * fForce, ForceMode2D.Impulse);
                //damage
                float Fdamage = (damage / (explodeDistance * rangeRatio)) * (explodeDistance - fDistance);

                if (obj.GetComponent<PlayerScript>() != null)//hit player
                {
                    StartCoroutine(Camera.main.GetComponent<CameraScript>().Shake(.25f, Fdamage / 100));//cameraShake
                    obj.GetComponent<PlayerScript>().takeDamage((int)Fdamage/2, force/2);
                }
                if (obj.GetComponent<EnemyScript>() != null) obj.GetComponent<EnemyScript>().takeDamage((int)Fdamage*2, force/5);//hit enemy
            }
        }
        if(destroyOnExplode) Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (explodeOnBullet && collision.GetComponent<Bullet>() != null)
        {
            Explode();
            Destroy(collision.gameObject);
        }
    }
}
