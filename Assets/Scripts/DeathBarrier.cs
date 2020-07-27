using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBarrier : MonoBehaviour
{

    Rigidbody rb;

    float speed = 25;

    public Transform[] ramps;
    public RampManager rampManager;
    Transform player;
    PlayerHealth playerHealth;
    PlayerMovement playerMovement;
    float minimumSpeed = 3;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        playerMovement = player.GetComponent<PlayerMovement>();
        CheckSpeed();
    }

    void CheckSpeed()
    {
        speed = playerMovement.rb.velocity.magnitude - 0.25f;
        speed = Mathf.Clamp(speed, playerMovement.startingMaxSpeed, playerMovement.maximumMaxSpeed);
        Invoke("CheckSpeed", 3);
    }

    public void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player.gameObject)
        {
            playerHealth.WasHit(Vector3.zero, Vector3.zero);
        }
    }
}
