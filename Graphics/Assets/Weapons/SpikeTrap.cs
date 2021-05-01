using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public GameObject spikes;
    public float pushForce = 5;
    public float speed = 0.5f;

    public float upPos = 1.3f;
    public float downPos = -1.3f;

    public GameObject bloodPart;
    public float bloodAmount = 5;

    bool opening = false;

    void Start()
    {
        spikes.transform.localPosition = transform.up * downPos;
    }
    private void FixedUpdate()
    {
        if (!opening) return;

        float yPos = spikes.transform.localPosition.y;
        float tempPosY = Mathf.Lerp(yPos, upPos, speed);

        spikes.transform.localPosition = new Vector2(spikes.transform.localPosition.x, tempPosY);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerScript>() != null)
        {
            PlayerScript player = collision.GetComponent<PlayerScript>();
            player.takeDamage(player.getHealth(), Vector2.up * pushForce);

            FindObjectOfType<AudioManager>().Play("spikes");
            for (int i = 0; i < bloodAmount; i++)
            {
                Instantiate(bloodPart, collision.transform.position, collision.transform.rotation);
            }

            opening = true;
        }
    }
}
