using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject player;

    public float speed;

    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void moveCamera(Vector2 point)
    {
        transform.position = new Vector3(point.x, point.y, transform.position.z);
    }
    public IEnumerator Shake(float dur, float mag)//camera shake
    {
        Vector3 ogPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < dur)
        {
            float x = Random.Range(-1f, 1f) * mag;
            float y = Random.Range(-1f, 1f) * mag;

            transform.localPosition = new Vector3(player.transform.localPosition.x + x, ogPos.y + y, ogPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = new Vector3(player.transform.localPosition.x, ogPos.y, ogPos.z);
    }
}
