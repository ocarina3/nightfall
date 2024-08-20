using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Define the different states of the game
    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver
    }

    // Store the current state of the game
    public GameState currentState;

    // Store the previous state of the game before it was paused
    public GameState previousState;

    [Header("UI")]
    public GameObject pauseScreen;

    void Awake()
    {
        DisableScreens();
    }

    void Update()
    {
        // Define the behavior for each state
        switch (currentState)
        {
            case GameState.Gameplay:
                // Code for the gameplay state
                CheckForPauseAndResume();
                break;
            case GameState.Paused:
                // Code for the paused state
                CheckForPauseAndResume();
                break;
            case GameState.GameOver:
                // Code for the game over state
                break;
            default:
                Debug.LogWarning("STATE DOES NOT EXIST");
                break;
        }
    }

    // Define the method to change the state of the game
    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }

    public void PauseGame()
    {
        if (currentState != GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f; // Stop the game
            pauseScreen.SetActive(true); // Enable the pause screen
            Debug.Log("Game is paused");
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
            Time.timeScale = 1f; // Resume the game
            pauseScreen.SetActive(false); //Disable the pause screen
            Debug.Log("Game is resumed");
        }
    }

    // Define the method to check for pause and resume input
    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void DisableScreens()
    {
        pauseScreen.SetActive(false);
    }
}