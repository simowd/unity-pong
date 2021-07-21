using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuManager : MonoBehaviour
{

    Mouse mouse;
    Gamepad gamepad;
    Keyboard keyboard;
    
    [SerializeField]
    GameObject eventSystem;
    [SerializeField]
    GameObject firstElement;
    [SerializeField]
    Text uiHelper;

    UnityEngine.EventSystems.EventSystem selection;

    void Start()
    {
        Time.timeScale = 1;
        MainManager.Instance.state = MainManager.SceneState.MENU;
        mouse = Mouse.current;
        gamepad = Gamepad.current;
        keyboard = Keyboard.current;

        if(eventSystem == null)
        {
            Debug.LogError("No event System assigned");
        }

        selection = eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>();
    }

    private void Update()
    {
        CheckInputMethods();
    }   

    public void OnExit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player
#endif
    }

    public void OnePlayerLoad()
    {
        MainManager.Instance.state = MainManager.SceneState.ONEPLAYER;
        SceneManager.LoadScene("Game");
    }

    public void TwoPlayerLoad()
    {
        MainManager.Instance.state = MainManager.SceneState.MULTIPLAYER;
        SceneManager.LoadScene("Game");
    }

    void CheckInputMethods()
    {
        //Verify mouse connection
        if (mouse != null)
        {
            if (mouse.delta.x.ReadValue() > 0 && mouse.delta.y.ReadValue() > 0 && selection.currentSelectedGameObject != null)
            {
                selection.SetSelectedGameObject(null);
                uiHelper.text = "";
            }
        }

        //Verify gamepad connection
        if (gamepad != null)
        {
            if (gamepad.wasUpdatedThisFrame)
            {
                if (selection.currentSelectedGameObject == null)
                {
                    selection.SetSelectedGameObject(firstElement);
                }
                uiHelper.text = "Use sticks to move    |    A to confirm";
            }
        }

        //Verify keyboard connection
        if (keyboard != null)
        {
            if (keyboard.wasUpdatedThisFrame)
            {
                if (selection.currentSelectedGameObject == null)
                {
                    selection.SetSelectedGameObject(firstElement);
                }
                uiHelper.text = "Use w/s or the arrows to move    |    Enter to confirm";
            }
        }
    }
}
