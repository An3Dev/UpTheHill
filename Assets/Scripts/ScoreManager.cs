using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScoreManager : MonoBehaviour
{

    float speedIncreasePerStride = 0.1f;

    public PlayerMovement movement;

    public TextMeshProUGUI scoreText, highScoreText;

    public int currentScore = 0;
    string highScoreKey = "HighScore";

    public AudioSource bgMusicSource;

    int highScore = 0;

    float maxFogDensity;

    // Start is called before the first frame update
    void Start()
    {
        // gets high score
        highScore = PlayerPrefs.GetInt(highScoreKey, 0);
        highScoreText.text = "Best: " + highScore.ToString("00000");


    }

    // Update is called once per frame
    void Update()
    {
        //RenderSettings.fogDensity   
    }

    public void Stride()
    {
        AddToScore(1);

        if (!movement.usingSpeedBoost)
        {
            movement.maxSpeed += speedIncreasePerStride;
        }

        movement.PlayStepSound();

        if (movement.maxSpeed > movement.maximumMaxSpeed && !movement.usingSpeedBoost)
        {
            movement.maxSpeed = movement.maximumMaxSpeed;
        }
    }

    public void AddToScore(int amount)
    {
        currentScore += amount;
        Invoke("UpdateText", 0.04f);
        if (!bgMusicSource.isPlaying)
        {
            bgMusicSource.Play();
        }
    }

    public void SaveScore()
    {

    }

    void UpdateText()
    {
        scoreText.text = currentScore.ToString("00000");
    }

    public void OnDisable()
    {
        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt(highScoreKey, currentScore);
        }
    }

    public void OnApplicationQuit()
    {
        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt(highScoreKey, currentScore);
        }
    }
}
