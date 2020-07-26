using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderSpawner : MonoBehaviour
{
    public GameObject[] boulderPrefabs;


    GameObject[] boulders;

    float spawnTime = 2;

    public float minTimeB4Spawn = 2, maxTimeB4Spawn = 7;

    float timeBeforeSpawn;

    public RampManager rampManager;
    public ScoreManager scoreManager;

    public float minSize, maxSize;

    float timer;

    public float boulderForce = 10;
    public float maxSizePointAmount = 150;
    public float minBoulderForce = 40, maxBoulderForce = 100;

    public void Start()
    {
        SpawnBoulder();
    }

    void SpawnBoulder()
    {
        int boulderIndex = Random.Range(0, boulderPrefabs.Length - 1);

        // TODO: make the scale change according to the score


        float pointPercentage = scoreManager.currentScore / maxSizePointAmount;
        float scale = pointPercentage * maxSize;
        scale = Mathf.Clamp(scale, minSize, maxSize);

        boulderForce = pointPercentage * maxBoulderForce;
        boulderForce = Mathf.Clamp(boulderForce, minBoulderForce, maxBoulderForce);

        float randomXPosition = Random.Range(-rampManager.rampWidth / 2 + scale / 2, rampManager.rampWidth / 2 - scale / 2);
        Vector3 location = new Vector3(randomXPosition, rampManager.GetTopRampPosition().y + scale / 2, rampManager.GetTopRampPosition().z);


        GameObject boulder = Instantiate(boulderPrefabs[boulderIndex], location, Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360))));
        boulder.transform.localScale = new Vector3(scale, scale, scale);
        Rigidbody rb = boulder.GetComponent<Rigidbody>();
        rb.AddForce(Vector3.back * boulderForce, ForceMode.VelocityChange);
        rb.mass = scale / minSize * 1000;

        //timeBeforeSpawn = Random.Range(minTimeB4Spawn, maxTimeB4Spawn);
        timeBeforeSpawn = 1;

        //boulder.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1) * boulderForce));


        // add a random force to the boulders;
        // add a particle system that creates dust behind the boulders

        // add an event where two boulders spawn next to each other or one on each side so use has to move more.
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeBeforeSpawn)
        {
            SpawnBoulder();
            timer = 0;
        }
    }
}
