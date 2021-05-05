using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public string type;
    [HideInInspector] public GameObject origin;

    float health;

    public GameObject bloodPart, bulletPart;

    void Start()
    {
        health = damage;
        StartCoroutine(DestroyBulletAfterTime());
    }
    IEnumerator DestroyBulletAfterTime()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.GetComponent<PlayerScript>() != null && type == "enemy")
        {
            if (!other.transform.root.GetComponent<PlayerScript>().isDead)
            {
                float blowForce = damage;
                if (blowForce > 25) blowForce = 25;
                Vector2 force = GetComponent<Rigidbody2D>().velocity * (blowForce / 200);
                other.transform.root.GetComponent<PlayerScript>().takeDamage((int)damage / 5, force);

                GameObject blood = Instantiate(bloodPart, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
        else if (other.transform.GetComponent<EnemyScript>() != null && type == "player")
        {
            if (!other.transform.root.GetComponent<EnemyScript>().isDead)
            {
                float blowForce = damage;
                if (blowForce > 50) blowForce = 50;
                Vector2 force = GetComponent<Rigidbody2D>().velocity * (blowForce / 200);
                other.transform.root.GetComponent<EnemyScript>().takeDamage((int)damage, force);

                Instantiate(other.transform.root.GetComponent<EnemyScript>().destroyPart, transform.position, transform.rotation);

                Destroy(gameObject);
            }
        }
        else if (other.GetComponent<Bullet>() != null)//if it has a bullet script
        {
            if (other.GetComponent<Bullet>().type != type)
            {
                health -= other.GetComponent<Bullet>().damage;
                if (health <= 0)
                {
                    Instantiate(bulletPart, transform.position, transform.rotation);

                    Destroy(gameObject);
                }
            }
        }
        else if (other.gameObject.layer.Equals(8))//bullet hits a wall or something
        {
            Instantiate(bulletPart, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
}
