using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WeaponHandler : MonoBehaviour
{
    private InputMaster input;
    PlayerScript playerScript;

    public GameObject arm, weaponContainer, gunContainer;
    public Weapon weapon;
    public float pickupDist = 0.5f;

    Transform gunsHeld;
    float armAngle;//for controller
    bool shooting;
    int selectedWeapon = 0;
    bool aiming;

    private void Awake()
    {
        playerScript = GetComponent<PlayerScript>();
        setWeapon(0);
    }
    void Start()
    {
        input = playerScript.input;

        input.Player.SwapWeapons.performed += _ => setWeapon(selectedWeapon + (int)input.Player.SwapWeapons.ReadValue<float>());
        input.Player.Lock.performed += _ => aiming = true;
        input.Player.Lock.canceled += _ => aiming = false;
    }
    void Update()
    {
        //pointAtTarget();
        if (aiming) lockOnEnemy();
        else pointAtTarget(false);

        if (weapon == null) return;

        if (weapon.isAuto)
        {
            input.Player.Shoot.performed += _ => shooting = true;
            input.Player.Shoot.canceled += _ => shooting = false;
        }
        else input.Player.Shoot.performed += _ => weapon.shoot();

        if (shooting && weapon.isAuto) weapon.shoot();
    }
    void pointAtTarget(bool isXbox)
    {
        Vector2 mousePos = input.Player.MousePosition.ReadValue<Vector2>();
        float angle, addAngle;
        if (playerScript.facingRight) addAngle = 70;
        else addAngle = -70 + 180;

        if (isXbox)
        {
            //for xbox controller
            angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            if (angle == 0) angle = armAngle;
            else armAngle = angle;
        }
        else
        {
            //weapon point at target
            Vector3 mouseToWorld = Camera.main.ScreenToWorldPoint(mousePos);
            Vector3 targetPos = mouseToWorld - arm.transform.position;

            angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
        }

        arm.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle + addAngle));
    }
    void lockOnEnemy()
    {
        //finding enemy
        EnemyScript[] enemys = GameObject.FindObjectsOfType<EnemyScript>();
        EnemyScript enemyToAim = null;
        float shortestDist = Mathf.Infinity;

        foreach(EnemyScript enemy in enemys)
        {
            if (!enemy.isDead)
            {
                float distToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
                if (distToEnemy < shortestDist)
                {
                    shortestDist = distToEnemy;
                    enemyToAim = enemy;
                }
            }
        }
        if (enemyToAim != null)
        {
            //rotating arm
            float addAngle;//adds the angle of players forearm
            if (playerScript.facingRight) addAngle = 70;
            else addAngle = -70 + 180;

            Vector2 mousePos = input.Player.MousePosition.ReadValue<Vector2>();
            Vector3 targetPos = enemyToAim.transform.position - arm.transform.position;

            float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
            if (angle == 0) angle = armAngle;
            else armAngle = angle;

            arm.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle + addAngle));
        }
    }
    public void setWeapon(int select)
    {
        selectedWeapon = select;
        //loop
        if (selectedWeapon > weaponContainer.transform.childCount-1) selectedWeapon = 0;
        if (selectedWeapon < 0) selectedWeapon = weaponContainer.transform.childCount-1;

        for (int i = 0; i < weaponContainer.transform.childCount; i++)
        {
            Transform gun = weaponContainer.transform.GetChild(i);

            if (i == selectedWeapon)
            {
                gun.gameObject.SetActive(true);
                weapon = gun.GetComponent<Weapon>();

                weapon.readyToShoot = false;
                StartCoroutine(weapon.waitToShoot());

                showContainer();
            }
            else
            {
                gun.gameObject.SetActive(false);
            }
        }
    }
    void showContainer()
    {
        //activate weapon container
        GameObject container = gunContainer.transform.Find("Container").gameObject;
        container.SetActive(true);

        //find position of gun to left and right of active gun
        int leftGun = selectedWeapon - 1;
        if (leftGun < 0) leftGun = weaponContainer.transform.childCount - 1;

        int rightGun = selectedWeapon + 1;
        if (rightGun > weaponContainer.transform.childCount - 1) rightGun = 0;

        //find gun images and change them
        GameObject gunLeft = container.transform.Find("gunLeft").gameObject;
        GameObject gunMiddle = container.transform.Find("gunMiddle").gameObject;
        GameObject gunRight = container.transform.Find("gunRight").gameObject;

        //set default to visible
        gunLeft.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.6f);
        gunRight.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.6f);

        //middle gun is changed to active weapon
        gunMiddle.GetComponent<Image>().sprite = weapon.GetComponent<SpriteRenderer>().sprite;
        gunMiddle.GetComponent<Image>().SetNativeSize();

        if (weaponContainer.transform.childCount < 3)//container is not full, only draw certain guns
        {
            if (selectedWeapon - 1 >= 0)
            {
                gunLeft.GetComponent<Image>().sprite = weaponContainer.transform.GetChild(leftGun).GetComponent<SpriteRenderer>().sprite;
                gunLeft.GetComponent<Image>().SetNativeSize();
            }
            else gunLeft.GetComponent<Image>().color = new Vector4(1, 1, 1, 0);

            if (selectedWeapon + 1 < weaponContainer.transform.childCount)
            {
                gunRight.GetComponent<Image>().sprite = weaponContainer.transform.GetChild(rightGun).GetComponent<SpriteRenderer>().sprite;
                gunRight.GetComponent<Image>().SetNativeSize();
            }
            else gunRight.GetComponent<Image>().color = new Vector4(1, 1, 1, 0);
        }
        else//container is full, draw as normal
        {
            gunLeft.GetComponent<Image>().sprite = weaponContainer.transform.GetChild(leftGun).GetComponent<SpriteRenderer>().sprite;
            gunLeft.GetComponent<Image>().SetNativeSize();

            gunRight.GetComponent<Image>().sprite = weaponContainer.transform.GetChild(rightGun).GetComponent<SpriteRenderer>().sprite;
            gunRight.GetComponent<Image>().SetNativeSize();
        }

        gunContainer.GetComponent<GunContainer>().waitToClose();
    }
    void checkForPickup()
    {
        Transform feet = transform.Find("feet");

        Collider2D[] objects = Physics2D.OverlapCircleAll(feet.position, pickupDist);

        foreach (Collider2D obj in objects)
        {
            if (obj.GetComponent<Weapon>() != null)
            {
                //print("yes");
                Weapon gunOnGround = obj.GetComponent<Weapon>();
                Weapon match = null;
                for (int i = 0; i < weaponContainer.transform.childCount; i++)
                {
                    Transform gun = weaponContainer.transform.GetChild(i);
                    if (gun.name == gunOnGround.name && gunOnGround.transform.root.GetComponent<PlayerScript>() == null) match = gunOnGround;//player doesn't pickup own gun
                }
                if(match == null)
                {
                    gunOnGround.transform.parent = weaponContainer.transform;
                    gunOnGround.transform.position = weaponContainer.transform.position;
                    setWeapon(weaponContainer.transform.childCount-1);//set weapon to newest one picked up, aka gunOnGround
                }
            }
        }
    }
}
