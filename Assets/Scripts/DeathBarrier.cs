using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBarrier : MonoBehaviour
{

    float speed = 5;

    public void Update()
    {
        Vector3 dir = new Vector3(0, 0.34202014333f, 0.93969262079f);
        transform.Translate(dir * speed * Time.deltaTime);
        Debug.Log(transform.forward);
    }
}
