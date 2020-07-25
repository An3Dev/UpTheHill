using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampManager : MonoBehaviour
{
    public Transform rampOne, rampTwo, rampThree, rampFour;
    public Transform leftWall, rightWall;

    int numberOfRecycles = 0;

    float offsetZDistance;
    float offsetYDistance;

    Transform player;

    bool movedRamp;
    float movedPlaneZPos;

    float lastMoveRampTime;

    public float rampWidth = 30;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        rampOne.localScale = new Vector3(rampWidth, rampOne.localScale.y, rampOne.localScale.z);
        rampTwo.localScale = new Vector3(rampWidth, rampOne.localScale.y, rampOne.localScale.z);

        rampThree.localScale = new Vector3(rampWidth, rampOne.localScale.y, rampOne.localScale.z);

        rampFour.localScale = new Vector3(rampWidth, rampOne.localScale.y, rampOne.localScale.z);

        offsetZDistance = rampOne.lossyScale.z * Mathf.Sin((90 - rampOne.rotation.eulerAngles.x) * Mathf.Deg2Rad);
        offsetYDistance = Mathf.Abs(rampOne.lossyScale.z * Mathf.Sin(rampOne.rotation.eulerAngles.x * Mathf.Deg2Rad));

        rampTwo.position = new Vector3(rampTwo.position.x, rampOne.position.y - offsetYDistance, rampOne.position.z - offsetZDistance);
        rampThree.position = new Vector3(rampThree.position.x, rampTwo.position.y -offsetYDistance, rampTwo.position.z -offsetZDistance);
        rampFour.position = new Vector3(rampFour.position.x, rampThree.position.y - offsetYDistance, rampThree.position.z - offsetZDistance);
        //PlacePlane();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(movedRamp);


        if (Time.time - lastMoveRampTime > 4 && (player.position.z % (offsetZDistance) > -2 && player.position.z % (offsetZDistance) < 2))
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
        if (numberOfRecycles % 4 == 0)
        {
            // position rampTwo
            ramp = rampFour;
        } else if (numberOfRecycles % 4 == 1)
        {
            // position rampOne
            ramp = rampThree;
        } else if (numberOfRecycles % 4 == 2)
        {
            ramp = rampTwo;
        } else
        {
            ramp = rampOne;
        }
        ramp.position = new Vector3(ramp.position.x, ramp.position.y + (offsetYDistance * 3), ramp.position.z + offsetZDistance * 3);
        // set hierarchy index
        ramp.SetSiblingIndex(0);
        numberOfRecycles++;
    }

    public void OnTriggerExit(Collider other)
    {
        //PlacePlane();
    }
}
