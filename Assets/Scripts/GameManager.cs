using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
  public static GameManager instance;


  // Define the different states of the game
  public enum GameState
  {
    Gameplay,
    Paused,
    GameOver,
    LevelUp
  }

  // Store the current state of the game
  public GameState currentState;

  // Store the previous state of the game before it was paused
  public GameState previousState;

  [Header("Damage Text Settings")]
    public Canvas damageTextCanvas;
    public float textFontSize = 20;
    public TMP_FontAsset textFont;
    public Camera referenceCamera;

  [Header("Screens")]
  public GameObject pauseScreen;
  public GameObject resultsScreen;
  public GameObject levelUpScreen;

  [Header("Current Stat Displays")]
  //Current stat displays
  public TextMeshProUGUI currentHealthDisplay;
  public TextMeshProUGUI currentRecoveryDisplay;
  public TextMeshProUGUI currentMoveSpeedDisplay;
  public TextMeshProUGUI currentMightDisplay;
  public TextMeshProUGUI currentProjectileSpeedDisplay;
  public TextMeshProUGUI currentMagnetDisplay;

  [Header("Results Screen Displays")]
  public Image chosenCharacterImage;
  public TextMeshProUGUI chosenCharacterName;
  public TextMeshProUGUI levelReachedDisplay;
  public TextMeshProUGUI timeSurvivedDisplay;
  public List<Image> chosenWeaponsUI = new List<Image>(6);
  public List<Image> chosenPassiveItemsUI = new List<Image>(6);

  [Header("Stopwatch")]
  public float timeLimit;
  float stopwatchTime;
  public Text stopwatchDisplay;

  public bool isGameOver = false;

  public bool choosingUpgrade;
  public GameObject playerObject;

  void Awake()
  {
    //Warning check to see if there is another singleton of this kind already in the game
    if (instance == null)
    {
      instance = this;
    }
    else
    {
      Debug.LogWarning("EXTRA " + this + " DELETED");
      Destroy(gameObject);
    }

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
        UpdateStopwatch();
        break;
      case GameState.Paused:
        // Code for the paused state
        CheckForPauseAndResume();
        break;
      case GameState.GameOver:
        // Code for the game over state
        if (!isGameOver)
        {
          isGameOver = true;
          Time.timeScale = 0f;
          Debug.Log("THE GAME IS OVER");
          DisplayResults();
        }
        break;
      case GameState.LevelUp:
        if (!choosingUpgrade)
        {
          choosingUpgrade = true;
          Time.timeScale = 0f;
          Debug.Log("Upgrades shown");
          levelUpScreen.SetActive(true);
        }
        break;
      default:
        Debug.LogWarning("STATE DOES NOT EXIST");
        break;
    }
  }

  IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 1f, float speed = 50f)
    {
        // Start generating the floating text.
        GameObject textObj = new GameObject("Damage Floating Text");
        RectTransform rect = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI tmPro = textObj.AddComponent<TextMeshProUGUI>();
        tmPro.text = text;
        tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmPro.fontSize = textFontSize;
        if (textFont) tmPro.font = textFont;
        rect.position = referenceCamera.WorldToScreenPoint(target.position);

        // Makes sure this is destroyed after the duration finishes.
        Destroy(textObj, duration);

        // Parent the generated text object to the canvas.
        textObj.transform.SetParent(instance.damageTextCanvas.transform);

        // Pan the text upwards and fade it away over time.
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0;
        float yOffset = 0;
        while(t < duration)
        {
            // Wait for a frame and update the time.
            yield return w;
            t += Time.deltaTime;

            // Fade the text to the right alpha value.
            tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, 1 - t / duration);

            // Pan the text upwards.
            yOffset += speed * Time.deltaTime;
            rect.position = referenceCamera.WorldToScreenPoint(target.position + new Vector3(0,yOffset));
        }
    }

    public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        // If the canvas is not set, end the function so we don't
        // generate any floating text.
        if (!instance.damageTextCanvas) return;

        // Find a relevant camera that we can use to convert the world
        // position to a screen position.
        if (!instance.referenceCamera) instance.referenceCamera = Camera.main;

        instance.StartCoroutine(instance.GenerateFloatingTextCoroutine(
            text, target, duration, speed
        ));
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
    resultsScreen.SetActive(false);
    levelUpScreen.SetActive(false);
  }

  public void GameOver()
  {
    timeSurvivedDisplay.text = stopwatchDisplay.text;
    ChangeState(GameState.GameOver);
  }

  void DisplayResults()
  {
    resultsScreen.SetActive(true);
  }

  public void AssignChosenCharacterUI(CharacterData chosenCharacterData)
  {
    chosenCharacterImage.sprite = chosenCharacterData.Icon;
    chosenCharacterName.text = chosenCharacterData.name;
  }

  public void AssignLevelReachedUI(int levelReachedData)
  {
    levelReachedDisplay.text = levelReachedData.ToString();
  }

  public void AssignChosenWeaponsAndPassiveItemsUI(List<Image> chosenWeaponsData, List<Image> chosenPassiveItemsData)
  {
    if (chosenWeaponsData.Count != chosenWeaponsUI.Count || chosenPassiveItemsData.Count != chosenPassiveItemsUI.Count)
    {
      Debug.Log("Chosen Weapons and passive items data lists have different lengths");
      return;
    }

    for (int i = 0; i < chosenWeaponsUI.Count; i++)
    {
      if (chosenWeaponsData[i].sprite)
      {
        chosenWeaponsUI[i].enabled = true;
        chosenWeaponsUI[i].sprite = chosenWeaponsData[i].sprite;
      }
      else
      {
        chosenWeaponsUI[i].enabled = false;
      }
    }

    for (int i = 0; i < chosenPassiveItemsUI.Count; i++)
    {
      if (chosenPassiveItemsData[i].sprite)
      {
        chosenPassiveItemsUI[i].enabled = true;
        chosenPassiveItemsUI[i].sprite = chosenPassiveItemsData[i].sprite;
      }
      else
      {
        chosenPassiveItemsUI[i].enabled = false;
      }
    }
  }

  void UpdateStopwatch()
  {
    stopwatchTime += Time.deltaTime;

    UpdateStopwatchDisplay();

    if (stopwatchTime >= timeLimit)
    {
      playerObject.SendMessage("Kill");
    }
  }

  void UpdateStopwatchDisplay()
  {
    int minutes = Mathf.FloorToInt(stopwatchTime / 60);
    int seconds = Mathf.FloorToInt(stopwatchTime % 60);
    stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
  }

  public void StartLevelUp()
  {
    ChangeState(GameState.LevelUp);
    playerObject.SendMessage("RemoveAndApplyUpgrades");
  }
  public void EndLevelUp()
  {
    choosingUpgrade = false;
    Time.timeScale = 1f;
    levelUpScreen.SetActive(false);
    ChangeState(GameState.Gameplay);
  }
}