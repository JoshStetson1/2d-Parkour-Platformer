using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Laser : MonoBehaviour
{
    public GameObject laserBeam, broken;
    public Transform bodyPos;
    public float timeToFix, damage;

    [HideInInspector] public bool isBroken;

    void Start()
    {
        laserBeam.SetActive(true);
        broken.SetActive(false);

        Sound s = Array.Find(FindObjectOfType<AudioManager>().sounds, sound => sound.soundName == "laser");
        float soundLength = s.source.clip.length;
        InvokeRepeating("PlaySound", 0, soundLength);
    }
    void PlaySound()
    {
        if (isBroken)
            return;

        //float distToPlayer = Vector2.Distance(transform.position, player.transform.position);
        //float volume = (maxNoiseDist - distToPlayer) / maxNoiseDist * droneNoise;
        //FindObjectOfType<AudioManager>().Play("laser");
    }
    public IEnumerator startBeam()
    {
        yield return new WaitForSeconds(timeToFix);

        laserBeam.SetActive(true);
        broken.SetActive(false);
        isBroken = false;

        FindObjectOfType<AudioManager>().Play("laser");
    }
    public void turnOff()
    {
        laserBeam.SetActive(false);
        broken.SetActive(true);
        isBroken = true;

        StartCoroutine(startBeam());
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerScript>() != null)
        {
            if (isBroken) return;

            Vector2 direction = (transform.position - collision.transform.position).normalized;
            RaycastHit2D rayHit = Physics2D.Raycast(transform.position, direction, 20, LayerMask.NameToLayer("ground"));

            if (rayHit.point != null)
            {
                float distToPlayer = Vector2.Distance(transform.position, collision.transform.position);
                float distToHit = Vector2.Distance(transform.position, rayHit.point);

                if (distToPlayer < distToHit) collision.GetComponent<PlayerScript>().takeDamage((int)(damage * Time.timeScale), Vector2.zero);
            }
            else collision.GetComponent<PlayerScript>().takeDamage((int)(damage * Time.timeScale), Vector2.zero);
        }
        else if (collision.GetComponent<Bullet>() != null)
        {
            Bullet b = collision.GetComponent<Bullet>();
            if (b.type == "player")//player shot the bullet
            {
                if (Vector2.Distance(bodyPos.position, collision.transform.position) < 0.65f)
                {
                    if (!isBroken) turnOff();

                    Instantiate(b.bulletPart, transform.position, transform.rotation);
                    Destroy(b.gameObject);
                }
            }
        }
        else if (collision.GetComponent<Explosion>() != null)
        {
            if(collision.GetComponent<EnemyScript>() == null)
            {
                collision.GetComponent<Explosion>().Explode();
            }
        }
    }
}
