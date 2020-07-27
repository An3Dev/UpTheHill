using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampManager : MonoBehaviour
{
    public Transform rampOne, rampTwo, rampThree, rampFour, rampFive;
    public Transform leftWall, rightWall;

    int numberOfRecycles = 0;

    float offsetZDistance;
    float offsetYDistance;

    Transform player;
    PlayerHealth health;

    bool movedRamp;
    float movedPlaneZPos;

    float lastMoveRampTime;

    public float rampWidth = 30;
    public BoulderSpawner boulderSpawner;

    public GameObject[] powerUpPrefabs;

    public GameObject powerUpParent;

    float powerUpProbability = 40;
    PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        health = player.GetComponent<PlayerHealth>();
        ResetPosition();
        playerMovement = player.GetComponent<PlayerMovement>();
        //PlacePlane();
        InvokeRepeating("CheckPlayerPosition", 5, 1);
    }

    void SpawnPowerUp()
    {
        int index = Random.Range(0, powerUpPrefabs.Length - 1);

        float randomXPosition = Random.Range(-rampWidth / 2 + 1, rampWidth / 2 - 1);
        Vector3 topRampPos = GetTopRampPosition();
        Vector3 location = new Vector3(randomXPosition, topRampPos.y + 3, topRampPos.z);

        GameObject powerUp = Instantiate(powerUpPrefabs[index], location, Quaternion.identity);
    }

    void CheckPlayerPosition()
    {
        Transform ramp = null;

        // recycle is on even number
        if (numberOfRecycles % 5 == 0)
        {
            // position rampTwo
            ramp = rampFive;
        }
        else if (numberOfRecycles % 5 == 1)
        {
            // position rampOne
            ramp = rampFour;
        }
        else if (numberOfRecycles % 5 == 2)
        {
            ramp = rampThree;
        }
        else if (numberOfRecycles % 5 == 3)
        {
            ramp = rampTwo;
        }
        else
        {
            ramp = rampOne;
        }

        if (player.position.y < ramp.position.y - offsetYDistance)
        {
            health.WasHit(Vector3.one * 10, Vector3.one);
        }
    }

    public void RecycleRamp(Transform ramp)
    {
        ramp.position = new Vector3(ramp.position.x, ramp.position.y + (offsetYDistance * 4), ramp.position.z + offsetZDistance * 4);
        // set hierarchy index
        ramp.SetSiblingIndex(0);
        numberOfRecycles++;
    }

    void ResetPosition()
    {
        rampOne.localScale = new Vector3(rampWidth, rampOne.localScale.y, rampOne.localScale.z);
        rampTwo.localScale = new Vector3(rampWidth, rampOne.localScale.y, rampOne.localScale.z);

        rampThree.localScale = new Vector3(rampWidth, rampOne.localScale.y, rampOne.localScale.z);

        rampFour.localScale = new Vector3(rampWidth, rampOne.localScale.y, rampOne.localScale.z);

        rampFour.localScale = new Vector3(rampWidth, rampOne.localScale.y, rampOne.localScale.z);


        offsetZDistance = rampOne.lossyScale.z * Mathf.Sin((90 - rampOne.rotation.eulerAngles.x) * Mathf.Deg2Rad);
        offsetYDistance = Mathf.Abs(rampOne.lossyScale.z * Mathf.Sin(rampOne.rotation.eulerAngles.x * Mathf.Deg2Rad));

        rampTwo.position = new Vector3(rampTwo.position.x, rampOne.position.y - offsetYDistance, rampOne.position.z - offsetZDistance);
        rampThree.position = new Vector3(rampThree.position.x, rampTwo.position.y - offsetYDistance, rampTwo.position.z - offsetZDistance);
        rampFour.position = new Vector3(rampFour.position.x, rampThree.position.y - offsetYDistance, rampThree.position.z - offsetZDistance);
        rampFive.position = new Vector3(rampFive.position.x, rampFour.position.y - offsetYDistance, rampFour.position.z - offsetZDistance);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(movedRamp);
        if (Time.time - lastMoveRampTime > 1 && (player.position.z % (offsetZDistance) > -5 && player.position.z % (offsetZDistance) < 5))
        {
            lastMoveRampTime = Time.time;
            PlacePlane();
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
           // PlacePlane();
        }
    }

    public Vector3 GetTopRampPosition()
    {
        // the top ramp is always at index 0
        // so this returns the top ramp's position
        return transform.GetChild(0).transform.position;
    }

    void PlacePlane()
    {
        Transform ramp = null;

        // recycle is on even number
        if (numberOfRecycles % 5 == 0)
        {
            // position rampTwo
            ramp = rampFive;
        } else if (numberOfRecycles % 5 == 1)
        {
            // position rampOne
            ramp = rampFour;
        } else if (numberOfRecycles % 5 == 2)
        {
            ramp = rampThree;
        } else if (numberOfRecycles % 5 == 3)
        {
            ramp = rampTwo;
        } else
        {
            ramp = rampOne;
        }

        ramp.position = new Vector3(ramp.position.x, ramp.position.y + (offsetYDistance * 5), ramp.position.z + offsetZDistance * 5);

        bool spawnPowerUp = Random.Range(0, 100) <= powerUpProbability;

        if (playerMovement.usingSpeedBoost)
        {
            spawnPowerUp = false;
        }

        if (spawnPowerUp)
        {
            SpawnPowerUp();
        }

        // set hierarchy index
        ramp.SetSiblingIndex(0);
        numberOfRecycles++;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player.gameObject)
        {
            player.GetComponent<PlayerHealth>().WasHit(Vector3.zero, Vector3.zero);
        }
    }
}
