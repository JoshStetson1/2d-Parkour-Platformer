using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletPoint;
    public bool sizeByDamage = true;

    public float spread, amount, bulletSpeed, fireRate, recoil, damage;

    [HideInInspector]
    public bool readyToShoot = true;

    public bool isAuto;

    private void Update()
    {
        checkForPickup();
    }
    public void shoot()
    {
        if (!readyToShoot) return;//so it doesnt keep firing after release

        if(transform.root.GetComponent<PlayerScript>() != null)//don't shoot if player is dead
        {
            if (transform.root.GetComponent<PlayerScript>().isDead) return;
        }

        for (int i = 0; i < amount; i++)
        {
            makeBullet();
        }
        if (transform.root.name == "Player")//recoil
        {
            if (transform.root.GetComponent<PlayerController>().sliding == false)
            {
                Rigidbody2D tempRB = transform.root.GetComponent<Rigidbody2D>();
                Vector3 bulletVel = -((bulletPoint.right * transform.root.localScale.x) * recoil);
                Vector3 knockBack = (Vector3)tempRB.velocity + bulletVel;
                tempRB.velocity = knockBack;
            }

            float shake = (recoil / 50);
            if (shake < 0.05f) shake = 0.05f;
            StartCoroutine(Camera.main.GetComponent<CameraScript>().Shake(.15f, shake));//cameraShake
        }

        readyToShoot = false;
        StartCoroutine(waitToShoot());
    }
    void makeBullet()
    {
        Vector3 spreadVector = new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread));//setting spread/ how accurate the bullet is

        GameObject newBullet = Instantiate(bullet, bulletPoint.position, bulletPoint.rotation);
        Bullet bulletScript = newBullet.GetComponent<Bullet>();

        //changing settings of bullet to match the gun it was shot out of
        bulletScript.damage = damage;

        bulletScript.origin = transform.root.gameObject;
        if (transform.root.GetComponent<PlayerScript>() != null) bulletScript.type = "player";
        else if (transform.root.GetComponent<EnemyScript>() != null) bulletScript.type = "enemy";

        newBullet.GetComponent<Rigidbody2D>().velocity = ((bulletPoint.right * transform.root.localScale.x) * bulletSpeed) + spreadVector;
        if(sizeByDamage) newBullet.transform.localScale *= (1 + (damage / 200));
        newBullet.transform.localScale = new Vector3(newBullet.transform.localScale.x * transform.root.localScale.x, newBullet.transform.localScale.y, newBullet.transform.localScale.z);//flip bullet relative to player

        FindObjectOfType<AudioManager>().PlayAtPitch("shot", (2 - (damage/100)) + ((Time.timeScale / 5) - 0.25f));
    }
    public IEnumerator waitToShoot()
    {
        float timeToWait = fireRate;
        if (transform.root.GetComponent<EnemyScript>() != null) timeToWait = fireRate * 2f;

        yield return new WaitForSeconds(timeToWait);
        readyToShoot = true;
    }
    void checkForPickup()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Transform playerFeet = player.transform.Find("feet");

        if(Vector2.Distance(transform.position, playerFeet.position) < 1)
        {
            if (transform.root.GetComponent<EnemyScript>() != null)//enemy is holding weapon
            {
                if (transform.root.GetComponent<EnemyScript>().isDead)//enemy is dead
                {
                    pickUp(player);
                }
            }
            else if(transform.root.GetComponent<PlayerScript>() == null)
            {
                pickUp(player);
            }
        }
    }
    public void pickUp(GameObject parent)
    {
        WeaponHandler weaponHandler = parent.GetComponent<WeaponHandler>();
        GameObject weaponContainer = weaponHandler.weaponContainer;
        Weapon match = null;
        for (int i = 0; i < weaponContainer.transform.childCount; i++)
        {
            Transform gun = weaponContainer.transform.GetChild(i);
            if (gun.name == transform.name && transform.transform.root.GetComponent<PlayerScript>() == null) match = this;//player doesn't pickup own gun
        }
        if (match == null)
        {
            gameObject.SetActive(false);

            transform.parent = weaponContainer.transform;
            transform.position = weaponContainer.transform.position;
            transform.localScale = weaponContainer.transform.localScale;
            transform.rotation = weaponContainer.transform.rotation;

            weaponHandler.setWeapon(weaponContainer.transform.childCount - 1);//set weapon to newest one picked up, aka gunOnGround

            FindObjectOfType<AudioManager>().Play("gunPickup");
        }
    }
}
