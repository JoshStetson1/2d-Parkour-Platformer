using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject arm;
    public Weapon weapon;
    public float addAngle;
    GameObject player;

    public int health = 100;
    public float jumpForce = 5;
    [HideInInspector]
    public bool isDead;

    public GameObject destroyPart;

    Collider2D[] colliders;
    Rigidbody2D[] rigidbodies;

    // Start is called before the first frame update
    void Start()
    {
        colliders = GetComponentsInChildren<Collider2D>(true);
        rigidbodies = GetComponentsInChildren<Rigidbody2D>(true);

        player = FindObjectOfType<PlayerScript>().gameObject;
    }

    void Update()
    {
        if (!isDead)
        {
            //weapon point at target
            if (canShoot())
            {
                pointAtTarget();
                if (weapon != null) weapon.shoot();
            }
        }
    }
    void pointAtTarget()
    {
        GameObject playerBody = player.transform.root.Find("body").gameObject;//hooks to center of player body instead of parent object

        float addAng = addAngle;//adds the angle of players forearm
        if (transform.localScale.x == -1) addAng = -addAngle + 180;

        //weapon point at target
        Vector3 targetPos = playerBody.transform.position - transform.position;

        float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
        arm.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle + addAng));
    }
    public bool canShoot()
    {
        if (player.GetComponent<PlayerScript>().isDead) return false;

        bool canShoot = false;
        if (Vector3.Distance(transform.position, player.transform.position) < 15)//can see enemy on camera
        {
            RaycastHit2D[] objects = Physics2D.LinecastAll(transform.position, player.transform.position);
            List<RaycastHit2D> objToHit = new List<RaycastHit2D>();

            foreach (RaycastHit2D obj in objects)
            {
                if (obj.collider.transform.root.GetComponent<EnemyScript>() == null && (obj.collider.gameObject.layer != 9)) objToHit.Add(obj);
            }
            if (objToHit.Count == 1) canShoot = true;
        }

        return canShoot;
    }
    public void takeDamage(int damage, Vector3 force)
    {
        health -= damage;
        if (health <= 0) die(force);
    }
    public void die(Vector3 force)
    {
        if (isDead) return;

        isDead = true;

        //set all limbs to ragdoll
        foreach (var col in colliders)
        {
            col.isTrigger = false;
        }
        foreach (Rigidbody2D rig in rigidbodies)
        {
            rig.isKinematic = false;
            float randX = Random.Range(force.x + jumpForce, force.x - jumpForce);
            float randY = Random.Range(force.y + jumpForce, force.y - jumpForce);
            float randZ = Random.Range(force.z + jumpForce, force.z - jumpForce);
            Vector3 randForce = new Vector3(randX, randY, randZ);
            rig.AddForce(randForce, ForceMode2D.Impulse);
        }

        GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponent<Rigidbody2D>().isKinematic = true;
    }
}
