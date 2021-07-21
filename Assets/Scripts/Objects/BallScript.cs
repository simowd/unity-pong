using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public delegate void PointScored();
    public static event PointScored pointScored1;
    public static event PointScored pointScored2;


    const float ACC_MODIFIER = 0.2f;
    const float MIN_ANG = 0.3f;
    const float MAX_ANG = 0.7f;

    [SerializeField]
    const float SPEED = 2f;
    [SerializeField]
    float acceleration = 0f;

    Transform _transform;
    Rigidbody2D _rigidbody;
    BoxCollider2D boxCollider;
    AudioSource audioSource;

    [SerializeField]
    Vector2 direction;

    float upperBound;
    float bottonBound;
    float remainingSpace;

    void Start()
    {
        Random.InitState((int) System.DateTime.Now.Ticks);

        _transform = transform;
        boxCollider = GetComponent<BoxCollider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        remainingSpace = boxCollider.bounds.size.y / 2;
        upperBound = MainManager.Instance.screenLimit.y - remainingSpace;
        bottonBound = MainManager.Instance.screenLimit.y * -1 + remainingSpace;

        float firstNumber = Random.Range(-1f,1f);
        float secondNumber = Random.Range(-1f, 1f);

        int randomX = firstNumber > 0 ? Mathf.CeilToInt(firstNumber) : Mathf.FloorToInt(firstNumber);
        int randomY = secondNumber > 0 ? Mathf.CeilToInt(secondNumber) : Mathf.FloorToInt(secondNumber);

        direction = new Vector2(randomX, randomY);
    }

    void FixedUpdate()
    {
        Collision();
        Movement();
        Scored();
    }

    void Movement()
    {
        float modifierY = _rigidbody.position.y + direction.y * ((SPEED + acceleration) * Time.fixedDeltaTime);
        float currentX = _rigidbody.position.x + direction.x * ((SPEED + acceleration) * Time.fixedDeltaTime);
        float currentY = Mathf.Clamp(modifierY, bottonBound, upperBound);
        Vector2 clampedPosition = new Vector2(currentX, currentY);
        _rigidbody.MovePosition(clampedPosition);
    }

    void Collision()
    {
        if (_rigidbody.position.y >= upperBound || _rigidbody.position.y <= bottonBound)
        {
            acceleration += ACC_MODIFIER;
            direction.x = directionInverter(direction.x);
            direction.y *= -1;
            direction = direction.normalized;

            audioSource.Play();
        }
    }

    void Scored()
    {
        if (_rigidbody.position.x > MainManager.Instance.screenLimit.x || _rigidbody.position.x < -MainManager.Instance.screenLimit.x)
        {
            if (_rigidbody.position.x < - MainManager.Instance.screenLimit.x)
            {
                if (pointScored2 != null)
                    pointScored2();
            }
            else if(_rigidbody.position.x > MainManager.Instance.screenLimit.x)
            {
                if (pointScored1 != null)
                    pointScored1();
            }
            Destroy(gameObject);
        }
            
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Bar")
        {
            acceleration += ACC_MODIFIER;
            direction.y = directionInverter(direction.y);
            direction.x *= -1;
            direction = direction.normalized;

            audioSource.Play();
        }
    }

    float directionInverter(float direction)
    {
        float newDirection = 0;

        if(direction > 0)
            newDirection = Random.Range(MIN_ANG, MAX_ANG);
        else
            newDirection = Random.Range(-MIN_ANG, -MAX_ANG);

        return newDirection;
    }
}
