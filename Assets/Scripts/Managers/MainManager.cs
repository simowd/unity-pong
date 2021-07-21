using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    // Start is called before the first frame update

    public enum SceneState
    {
        MENU,
        ONEPLAYER,
        MULTIPLAYER
    }

    public static MainManager Instance;
    public SceneState state = SceneState.MENU;
    public Vector2 screenLimit;
    public int screenHeight;
    public int screenWidth;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        screenHeight = Screen.height;
        screenWidth = Screen.width;

        screenLimit = Camera.main.ScreenToWorldPoint(new Vector3(screenWidth, screenHeight, Camera.main.transform.position.z));

        DontDestroyOnLoad(gameObject);
    }
}
