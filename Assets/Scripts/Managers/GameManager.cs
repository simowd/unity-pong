using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public GameObject barPrefab;
    public GameObject ballPrefab;

    GameObject barLeft;
    GameObject barRight;
    GameObject currentBall;

    BarScript barScriptLeft;
    BarScript barScriptRight;

    Gamepad gamepad;
    Keyboard keyboard;
    Mouse mouse;

    AudioSource audioSource;
    [SerializeField]
    GameObject eventSystem;
    [SerializeField]
    GameObject menuScreen;
    [SerializeField]
    GameObject scoreText;
    [SerializeField]
    GameObject firstElement;
    Text gameOverText;
    UnityEngine.EventSystems.EventSystem selection;


    [SerializeField]
    int score1 = 0;
    Text textScore1;
    [SerializeField]
    int score2 = 0;
    Text textScore2;

    bool isPaused = false;
    bool gameOver = false;

    void Start()
    {
        Time.timeScale = 1;

        gamepad = Gamepad.current;
        keyboard = Keyboard.current;
        mouse = Mouse.current;
        audioSource = GetComponent<AudioSource>();

        //Check if game manager is not instanced
        if (MainManager.Instance.state == MainManager.SceneState.MENU)
        {
            MainManager.Instance.state = MainManager.SceneState.ONEPLAYER;
        }

        //Create both game bars
        if(barPrefab != null)
        {
            barLeft = Instantiate(barPrefab);
            barRight = Instantiate(barPrefab);

            barScriptLeft = barLeft.GetComponent<BarScript>();
            barScriptRight = barRight.GetComponent<BarScript>();

            barScriptLeft.state = BarScript.Bar.LEFT;
            barScriptRight.state = BarScript.Bar.RIGHT;

            if(MainManager.Instance.state == MainManager.SceneState.ONEPLAYER)
            {
                barScriptLeft.mode = BarScript.Player.PLAYER;
                barScriptRight.mode = BarScript.Player.CPU;
            }
            if (MainManager.Instance.state == MainManager.SceneState.MULTIPLAYER)
            {
                barScriptLeft.mode = BarScript.Player.PLAYER;
                barScriptRight.mode = BarScript.Player.PLAYER;
            }
        }
        else
            Debug.LogError("Prefab not referenced in Editor");

        if (menuScreen != null)
            gameOverText = GameObject.Find("GameOverText").GetComponent<Text>();
        else
            Debug.LogError("UI not attached in Editor");

        BallScript.pointScored1 += updateScoreOne;
        BallScript.pointScored2 += updateScoreTwo;

        textScore1 = GameObject.Find("Score1").GetComponent<Text>();
        textScore2 = GameObject.Find("Score2").GetComponent<Text>();

        menuScreen.SetActive(false);
        selection = eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>();
    }

    void Update()
    {
        //Spawn new ball when the old one is destroyed
        if(currentBall == null)
        {
            StartCoroutine(ballSpawner());
        }

        CheckInputMethods();
        WinningCondition();
    }

    void WinningCondition()
    {
        if (score1 > 4 || score2 > 4)
        {
            PauseGame();
            if (score1 > 4)
                gameOverText.text = "PLAYER 1 WINS";
            else if (score2 > 4)
                gameOverText.text = "PLAYER 2 WINS";
            gameOver = true;
        }
    }

    void CheckInputMethods()
    {
        //Verify mouse connection
        if (mouse != null)
        {
            if (isPaused)
            {
                if (mouse.delta.x.ReadValue() > 0 && mouse.delta.y.ReadValue() > 0 && selection.currentSelectedGameObject != null)
                {
                    selection.SetSelectedGameObject(null);
                }
            }
        }

        if (gamepad != null)
        {
            if (gamepad.startButton.wasReleasedThisFrame)
            {
                if (!isPaused)
                    PauseGame();
                else
                    UnpauseGame();
            }
            if (isPaused)
            {
                if (gamepad.wasUpdatedThisFrame)
                {
                    if (selection.currentSelectedGameObject == null)
                    {
                        selection.SetSelectedGameObject(firstElement);
                    }
                }
            }
        }

        if (keyboard != null)
        {
            if (isPaused)
            {
                if (keyboard.wasUpdatedThisFrame)
                {
                    if (selection.currentSelectedGameObject == null)
                    {
                        selection.SetSelectedGameObject(firstElement);
                    }
                }
            }
            if (keyboard.escapeKey.wasReleasedThisFrame)
            {
                if (!isPaused)
                    PauseGame();
                else
                    UnpauseGame();
            }
        }

    }

    void PauseGame()
    {
        gameOverText.text = "THE GAME IS PAUSED";
        isPaused = true;
        Time.timeScale = 0;
        menuScreen.SetActive(true);
        scoreText.SetActive(false);
    }

    void UnpauseGame()
    {
        if (!gameOver)
        {
            isPaused = false;
            Time.timeScale = 1;
            menuScreen.SetActive(false);
            scoreText.SetActive(true);
        }
    }

    void OnDisable()
    {
        BallScript.pointScored1 -= updateScoreOne;
        BallScript.pointScored2 -= updateScoreTwo;
    }

    void updateScoreOne()
    {
        audioSource.Play();
        score1++;
        if(textScore1 != null)
            textScore1.text = score1.ToString();
    }

    void updateScoreTwo()
    {
        audioSource.Play();
        score2++;
        if (textScore2 != null)
            textScore2.text = score2.ToString();
    }


    IEnumerator ballSpawner()
    {
        currentBall = Instantiate(ballPrefab);
        currentBall.SetActive(false);
        yield return new WaitForSeconds(2);
        currentBall.SetActive(true);        
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
