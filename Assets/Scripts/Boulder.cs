using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    Transform player;
    PlayerHealth health;
    //Color[] colors;
    //MeshRenderer renderer;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //renderer = GetComponent<MeshRenderer>();

        Invoke("CheckPos", 0.0001f);

        health = player.GetComponent<PlayerHealth>();

        //int colorIndex = Random.Range(0, colors.Length - 1);
        //renderer.materials[1].color = colors[colorIndex];

    }

    void CheckPos()
    {
        if (transform.position.z < player.position.z)
        {
            Destroy(gameObject, 5);
        }
        else
        {
            Invoke("CheckPos", 2);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (player)
        {
            if (collision.collider.gameObject == player.gameObject)
            {
                health.WasHit(transform.lossyScale, collision.GetContact(0).normal);
            }
        }
        
    }
}
