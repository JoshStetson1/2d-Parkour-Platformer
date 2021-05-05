using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Drone : MonoBehaviour
{
    public Transform target;

    public float speed = 200;
    public float nextWaypointDist = 3;

    Path path;
    int currentWaypoint = 0;

    public GameObject booster1, booster2;
    public float angleSpeed, maxAngle;

    Seeker seeker;
    Rigidbody2D rb;
    EnemyScript enemyScript;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        enemyScript = GetComponent<EnemyScript>();

        InvokeRepeating("UpdatePath", 0, 0.5f);
    }
    void UpdatePath()
    {
        if (seeker.IsDone()) seeker.StartPath(rb.position, target.position, OnPathComplete);
    }
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    private void Update()
    {
        if (enemyScript.isDead)
        {
            GetComponent<Explosion>().Explode();
        }
        else
        {
            //rotate boosters
            float angle = -rb.velocity.x * angleSpeed;
            if (angle > maxAngle) angle = maxAngle;
            if (angle < -maxAngle) angle = -maxAngle;

            booster1.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            booster2.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        }
    }
    void FixedUpdate()
    {
        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count) return;

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        if (rb.mass > 1) force *= rb.mass;
        if (!enemyScript.canShoot()) rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance <= nextWaypointDist) currentWaypoint++;
    }
}
