using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    int health = 1;

    public Animator canvasAnimator;
    PlayerMovement playerMovement;

    public float deathForce = 10;
    float maxDeathForce = 20;
    float minDeathForce = 5;


    public void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        if (Time.time < 0.0000001f)
        {
            Restart();
        }
        playerMovement = GetComponent<PlayerMovement>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WasHit(Vector3 scale, Vector3 direction)
    {
        health--;

        if (health <= 0)
        {
            // 
            if (scale != Vector3.zero)
            {
                playerMovement.PlayBoulderHitSound();
            }
            // disable movement
            playerMovement.Die(scale.x * deathForce, direction);

            Invoke("ShowAnimation", 2);
        }
    }

    void ShowAnimation()
    {
        canvasAnimator.SetTrigger("GameOver");
        Invoke("Restart", 0.7f);

    }

    void Restart()
    {
        SceneManager.LoadScene("MainScene");
    }


}
