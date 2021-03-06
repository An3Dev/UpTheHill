﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
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

    public bool inMenu = false;

    public TextMeshProUGUI allText, bgText;

    bool muteAll = false, muteBGMusic = false;

    string muteAllKey = "MuteAll", muteBGMusicKey = "MuteBGMusic";

    public AudioSource bgMusicSource;

    Camera main;
    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
        ResetPosition();
                    muteAll = bool.Parse(PlayerPrefs.GetString(muteAllKey, "false"));
            muteBGMusic = bool.Parse(PlayerPrefs.GetString(muteBGMusicKey, "false"));
        if (!inMenu)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            health = player.GetComponent<PlayerHealth>();
            playerMovement = player.GetComponent<PlayerMovement>();
            InvokeRepeating("CheckPlayerPosition", 5, 1);

            AudioSource[] sources = FindObjectsOfType<AudioSource>();
            for (int i = 0; i < sources.Length; i++)
            {
                sources[i].enabled = !muteAll;
            }
        }

        if (inMenu)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;


            bgMusicSource.mute = muteBGMusic;

            //main.GetComponent<AudioListener>().enabled = !muteAll;
            AudioSource[] sources = FindObjectsOfType<AudioSource>();

            for(int i = 0; i < sources.Length; i++)
            {
                sources[i].enabled = !muteAll;
            }

            if (muteAll)
            {
                allText.text = "Unmute ALL";
            }
            else
            {
                allText.text = "Mute ALL";
            }

            if (muteBGMusic)
            {
                bgText.text = "Unmute BG music";
            }
            else
            {
                bgText.text = "Mute BG music";
            }
        }
        //PlacePlane();
    }

    // toggle
    public void MuteAll()
    {
        muteAll = !muteAll;
        if (muteAll)
        {
            allText.text = "Unmute All";
        } else
        {
            allText.text = "Mute all";
        }

        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        for (int i = 0; i < sources.Length; i++)
        {
            sources[i].enabled = !muteAll;
        }

        PlayerPrefs.SetString(muteAllKey, muteAll.ToString());
        PlayerPrefs.Save();
    }

    // toggle
    public void MuteBGMusic()
    {
        muteBGMusic = !muteBGMusic;

        bgMusicSource.mute = muteBGMusic;

        if (muteBGMusic)
        {
            bgText.text = "Unmute BG music";
        }
        else
        {
            bgText.text = "Mute BG music";
            bgMusicSource.Play();
        }
        PlayerPrefs.SetString(muteBGMusicKey, muteBGMusic.ToString());
        PlayerPrefs.Save();
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
        if (!inMenu && Time.time - lastMoveRampTime > 1 && (player.position.z % (offsetZDistance) > -5 && player.position.z % (offsetZDistance) < 5))
        {
            lastMoveRampTime = Time.time;
            PlacePlane();
        }
    }

    public Vector3 GetTopRampPosition()
    {
        // the top ramp is always at index 0
        // so this returns the top ramp's position
        return transform.GetChild(0).transform.position;
    }

    public void GoToGame()
    {
        SceneManager.LoadScene(1);
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
        if (!inMenu && other.gameObject == player.gameObject)
        {
            player.GetComponent<PlayerHealth>().WasHit(Vector3.zero, Vector3.zero);
        }
    }
}
