using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    PlayerController controller;
    public InputMaster input;
    [HideInInspector]
    public Animator anim;

    [HideInInspector]
    public bool facingRight, isDead;

    public SliderScript healthBar;
    int health = 100;
    public GameObject deathScreen;

    //for ragdoll
    Collider2D[] colliders;
    Rigidbody2D[] rigidbodies;

    private void Awake()
    {
        input = new InputMaster();//for input keys
        controller = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();

        Physics2D.IgnoreLayerCollision(9, 9, true);
        Physics2D.IgnoreLayerCollision(10, 9, true);
    }
    private void OnEnable()
    {
        input.Enable();
    }
    private void OnDisable()
    {
        input.Disable();
    }
    void Start()
    {
        colliders = GetComponentsInChildren<Collider2D>(true);
        rigidbodies = GetComponentsInChildren<Rigidbody2D>(true);

        input.Player.Jump.performed += _ => restart();
    }
    void Update()
    {
        if (transform.localScale.x == 1) facingRight = true;
        if (transform.localScale.x == -1) facingRight = false;

        if (transform.position.y < -10 && !isDead) takeDamage(health, Vector3.zero);

        aimCamera();
    }
    void aimCamera()
    {
        CameraScript cam = Camera.main.GetComponent<CameraScript>();

        if (!isDead)
        {
            cam.moveCamera(new Vector2(transform.position.x, cam.transform.position.y));
        }
        else
        {
            float velX = 0, velY = 0, velS = 0;

            float camX = Mathf.SmoothDamp(cam.transform.position.x, transform.Find("body").position.x, ref velX, cam.speed);
            float camY = Mathf.SmoothDamp(cam.transform.position.y, transform.Find("body").position.y, ref velY, cam.speed);
            cam.moveCamera(new Vector2(camX, camY));

            float size = Mathf.SmoothDamp(cam.GetComponent<Camera>().orthographicSize, 4, ref velS, cam.speed);
            cam.GetComponent<Camera>().orthographicSize = size;
        }
    }
    public void FootStep()
    {
        FindObjectOfType<AudioManager>().Play("step");
    }
    public int getHealth()
    {
        return health;
    }
    public void takeDamage(int damage, Vector3 force)
    {
        health -= damage;
        healthBar.setValue(health);

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
            float randX = Random.Range(force.x + 5, force.x - 5);
            float randY = Random.Range(force.y + 5, force.y - 5);
            float randZ = Random.Range(force.z + 5, force.z - 5);
            Vector3 randForce = new Vector3(randX, randY, randZ);
            rig.AddForce(randForce, ForceMode2D.Impulse);
        }
        //turn off player colliders so dead body can be ragdoll
        GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        //cancel all things that make character move
        GetComponent<Animator>().enabled = false;
        GetComponent<PlayerController>().enabled = false;
        GetComponent<WeaponHandler>().enabled = false;
        GetComponent<SloMo>().enabled = false;

        //set camera to follow dead body
        //Camera.main.GetComponent<CameraScript>().player = transform.Find("body").gameObject;

        deathScreen.SetActive(true);
    }
    void restart()
    {
        if (!isDead) return;

        FindObjectOfType<SceneScript>().Restart();
    }
}
