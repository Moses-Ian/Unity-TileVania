using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameSession : MonoBehaviour
{
    [SerializeField] int playerLives = 3;
    [SerializeField] float levelLoadDelay = 3f;
    [SerializeField] int score = 0;

    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI scoreText;

    void Awake()
    {
        int numberOfGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numberOfGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Start()
    {
        livesText.text = playerLives.ToString();
        scoreText.text = score.ToString();
    }

    public void ProcessPlayerDeath()
    {
        if (playerLives > 1)
        {
            TakeLife();
            livesText.text = playerLives.ToString();
            StartCoroutine(ReloadLevel());
        }
        else
        {
            StartCoroutine(ResetGameSession());
        }
    }

    public void AddToScore(int pointsToAdd)
    {
        score += pointsToAdd;
        scoreText.text = score.ToString();
    }

    public IEnumerator ReloadLevel()
    {
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }


    private void TakeLife()
    {
        playerLives--;
    }

    private IEnumerator ResetGameSession()
    {
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        FindObjectOfType<ScenePersist>().ResetScenePersist();
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    void Update()
    {
        
    }
}
