using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private Vector2 direction;
    private List<Transform> segments;
    public Transform SegmentPrefab;
    public int initialSize = 4;
    public float hungry = 1.1f;
    public static float maxFixedDeltaTime;
    public static float minFixedDeltaTime;
    public bool gameActive;

    public static Action OnSnakeGrows;
    public static Action OnRestartGame;

    private void OnEnable()
    {
        OnSnakeGrows += Grow;
        OnSnakeGrows += CalmDesperation;
        OnRestartGame += InitGame;
        OnRestartGame += CalmDesperation;
    }

    private void Awake()
    {
        maxFixedDeltaTime = .12f;
        minFixedDeltaTime = .016f;
    }

    private void Start()
    {
        segments = new List<Transform>();
        InitGame();
        StartCoroutine(HandleHungry());
    }

    private IEnumerator HandleHungry()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);
            Time.fixedDeltaTime = Mathf.Clamp(Time.fixedDeltaTime /= hungry,
                                              minFixedDeltaTime,
                                              maxFixedDeltaTime);
        }
    }

    private void Update()
    {
        if (gameActive)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        if (Input.GetKeyDown(KeyCode.W) && direction != Vector2.down)
        {
            direction = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.A) && direction != Vector2.right)
        {
            direction = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.S) && direction != Vector2.up)
        {
            direction = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.D) && direction != Vector2.left)
        {
            direction = Vector2.right;
        }
    }

    private void FixedUpdate()
    {
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;
        }

        this.transform.position = new Vector3(
            Mathf.Round(transform.position.x) + direction.x,
            Mathf.Round(transform.position.y) + direction.y,
            0.0f
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag is "Food")
        {
            OnSnakeGrows();
        }
        else if (other.tag is "Obstacle")
        {
            OnRestartGame();
        }
    }

    private void Grow()
    {
        var segment = Instantiate(SegmentPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }

    private void CalmDesperation()
    {
        Time.fixedDeltaTime = 0.12f;
    }

    private void InitGame()
    {
        for (int i = 1; i < segments.Count; i++)
        {
            Destroy(segments[i].gameObject);
        }
        segments.Clear();
        segments.Add(this.transform);

        for (int i = 1; i < initialSize; i++)
        {
            segments.Add(Instantiate(SegmentPrefab));
        }
        direction = Vector2.right;
        transform.position = new Vector3(-15.0f, 0.0f, 0.0f);
    }

    private void OnDisable()
    {
        OnSnakeGrows -= Grow;
        OnSnakeGrows -= CalmDesperation;
        OnRestartGame -= InitGame;
        OnRestartGame -= CalmDesperation;
    }
}
