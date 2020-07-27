using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BoulderSpawner : MonoBehaviour
{
    public GameObject[] boulderPrefabs;


    GameObject[] boulders;

    float spawnTime = 2;

    public float minTimeB4Spawn = 0.5f, maxTimeB4Spawn = 1;

    float timeBeforeSpawn;

    public RampManager rampManager;
    public ScoreManager scoreManager;

    public float minSize, maxSize;

    float timer;

    public float boulderForce = 10;
    public float maxSizePointAmount = 200;
    public float minBoulderForce = 40, maxBoulderForce = 100;

    public bool inMenu = false;

    public void Start()
    {
        //SpawnBoulder();
    }

    void SpawnMenuBoulders()
    {
        int boulderIndex = Random.Range(0, boulderPrefabs.Length - 1);

        //float pointPercentage =0f;
        //float scale = pointPercentage * maxSize;

        //scale = Mathf.Clamp(scale, minSize, maxSize);
        float scale = maxSize;

        //boulderForce = pointPercentage * maxBoulderForce;
        //boulderForce = Mathf.Clamp(boulderForce, minBoulderForce, maxBoulderForce);
        float boulderForce = 100;

        float randomXPosition = Random.Range(-rampManager.rampWidth / 2 + scale / 2, rampManager.rampWidth / 2 - scale / 2);
        Vector3 location = new Vector3(randomXPosition, rampManager.GetTopRampPosition().y + scale / 2, rampManager.GetTopRampPosition().z);


        GameObject boulder = Instantiate(boulderPrefabs[0], location, Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360))));
        boulder.transform.localScale = new Vector3(scale, scale, scale);
        Rigidbody rb = boulder.GetComponent<Rigidbody>();

        Destroy(boulder, 20);

        Vector3 dir = new Vector3(0, -0.6f, -0.4f);
        rb.AddForce(dir * boulderForce, ForceMode.VelocityChange);
        rb.mass = scale / minSize * 1000;
        Destroy(boulder.GetComponent<Boulder>());

        //timeBeforeSpawn = Random.Range(minTimeB4Spawn, maxTimeB4Spawn);
        timeBeforeSpawn = 1;
    }


    void SpawnBoulder()
    {
        Debug.Log("Spawn");
        int boulderIndex = Random.Range(0, boulderPrefabs.Length - 1);
        float pointPercentage;

        if (!inMenu)
        {
            pointPercentage = scoreManager.currentScore / maxSizePointAmount;
        } else
        {
            pointPercentage = 1;
        }

        float scale = pointPercentage * maxSize;
        scale = Mathf.Clamp(scale, minSize, maxSize);

        boulderForce = pointPercentage * maxBoulderForce;
        boulderForce = Mathf.Clamp(boulderForce, minBoulderForce, maxBoulderForce);

        float randomXPosition = Random.Range(-rampManager.rampWidth / 2 + scale / 2, rampManager.rampWidth / 2 - scale / 2);
        Vector3 location = new Vector3(randomXPosition, rampManager.GetTopRampPosition().y + scale / 2, rampManager.GetTopRampPosition().z);


        GameObject boulder = Instantiate(boulderPrefabs[boulderIndex], location, Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360))));
        Destroy(boulder, 20);
        boulder.transform.localScale = new Vector3(scale, scale, scale);
        Rigidbody rb = boulder.GetComponent<Rigidbody>();
        Vector3 dir = new Vector3(0, -0.6f, -0.4f);
        rb.AddForce(dir * boulderForce, ForceMode.VelocityChange);
        rb.mass = scale / minSize * 1000;

        //timeBeforeSpawn = Random.Range(minTimeB4Spawn, maxTimeB4Spawn);
        timeBeforeSpawn = (1 - pointPercentage) * maxTimeB4Spawn;
        timeBeforeSpawn = Mathf.Clamp(timeBeforeSpawn, minTimeB4Spawn, maxTimeB4Spawn);

        //boulder.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1) * boulderForce));


        // add a random force to the boulders;
        // add a particle system that creates dust behind the boulders

        // add an event where two boulders spawn next to each other or one on each side so use has to move more.
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeBeforeSpawn && !inMenu)
        {
            SpawnBoulder();
            timer = 0;
        }
        //else if (inMenu)
        //{
        //    SpawnMenuBoulders();
        //    timer = 0;
        //}r
    }
}
