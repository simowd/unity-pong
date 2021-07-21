using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class BarScript : MonoBehaviour
{
    public enum Bar
    {
        LEFT,
        RIGHT
    }

    public enum Player
    {
        PLAYER,
        CPU
    }

    [SerializeField]
    const float SPEED = 3.5f;
    const float BALL_THR = 0.5f;
    float initialXPosition;

    Transform _transform;
    Gamepad gamepad;
    Mouse mouse;
    Keyboard keyboard;

    bool up = false;
    bool down = false;

    float upperBound;
    float bottonBound;

    [SerializeField]
    public Bar state;
    [SerializeField]
    public Player mode;

    BoxCollider2D boxCollider;
    Rigidbody2D _rigidbody;

    float remainingSpace;

    BallScript currentBall;

    // Start is called before the first frame update
    void Start()
    {
        if(mode != Player.CPU)
        {
            if (state == Bar.LEFT && Gamepad.all.Count > 0)
                gamepad = Gamepad.all[0];
            if (state == Bar.RIGHT && Gamepad.all.Count > 1)
                gamepad = Gamepad.all[1];

            if (Mouse.current != null)
                mouse = Mouse.current;
            if (Keyboard.current != null)
                keyboard = Keyboard.current;
        }

        _transform = transform;
        boxCollider = GetComponent<BoxCollider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();

        remainingSpace = boxCollider.bounds.size.y / 2;
        upperBound = MainManager.Instance.screenLimit.y - remainingSpace;
        bottonBound = MainManager.Instance.screenLimit.y * -1 + remainingSpace;

        initialXPosition = MainManager.Instance.screenLimit.x - 0.5f;

        if (state == Bar.LEFT)
            _transform.position = new Vector2(- initialXPosition, 0);
        if (state == Bar.RIGHT)
            _transform.position = new Vector2(initialXPosition, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(mode == Player.PLAYER)
            CheckInput();
        if (mode == Player.CPU)
            BarAI();

        MoveBar();
    }

    void BarAI()
    {
        if (currentBall == null)
            CheckBall();
        else
            FollowBall();
    }

    void FollowBall()
    {
        Vector2 positionBall = currentBall.transform.position;
        Vector2 positionBar = _transform.position;
        if(down == false)
            up = positionBar.y < (positionBall.y - BALL_THR) ? true : false;
        if(up == false)
            down = positionBar.y > (positionBall.y + BALL_THR) ? true : false;
    }

    void CheckBall()
    {
            GameObject gameObject = GameObject.FindWithTag("Ball");
            if (gameObject != null)
                currentBall = gameObject.GetComponent<BallScript>();   
    }

    void CheckInput()
    {
        if(state == Bar.LEFT)
        {            
            up = keyboard.wKey.isPressed ? true : false;
            down = keyboard.sKey.isPressed ? true : false;
        }
        else if(state == Bar.RIGHT)
        {
            up = keyboard.upArrowKey.isPressed ? true : false;
            down = keyboard.downArrowKey.isPressed ? true : false;
        }

        if(gamepad != null)
        {
            up = (gamepad.dpad.up.isPressed || gamepad.leftStick.up.ReadValue() > 0) ? true : false;
            down = (gamepad.dpad.down.isPressed || gamepad.leftStick.down.ReadValue() > 0) ? true : false;
        }
    }

    void MoveBar()
    {
        if (up || down)
        {
            short direction = 1;
            if (down)
                direction = -1;

            float currentY = Mathf.Clamp(_rigidbody.position.y + (SPEED * Time.fixedDeltaTime * direction), bottonBound, upperBound);
            float currentX = Mathf.Clamp(_rigidbody.position.x, MainManager.Instance.screenLimit.x * -1, MainManager.Instance.screenLimit.x);
            Vector2 clampedPosition = new Vector2(currentX, currentY);
            _rigidbody.MovePosition(clampedPosition);
        }
    }
}
