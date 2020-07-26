using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    public enum PowerUpType { Speed, Shield, Time }

    public PowerUpType type;

    GameObject player;
    PlayerMovement movement;
    bool collected = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        movement = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        // float up and down slowly
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        Debug.Log(other);
        if (other.gameObject == player && ! collected)
        {
            StartCoroutine(Shrink(transform.localScale.x));
            collected = true;
            movement.CollectPowerUp(type);
        }
    }

    IEnumerator Shrink(float startScale)
    {
        yield return new WaitForEndOfFrame();
        float scale = transform.localScale.x - 0.05f;
        transform.localScale = new Vector3(scale, scale, 1);
        if (transform.localScale.x > 0)
        {
            StartCoroutine(Shrink(startScale));
        } else
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }
}
