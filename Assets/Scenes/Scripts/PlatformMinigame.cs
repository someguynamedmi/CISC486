using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class PlatformMinigame : MonoBehaviour
{
    public GameObject player;
    public GameObject enemies_holder;

    public TextMeshProUGUI result;
    public TextMeshProUGUI timer_text;
    public float timer = 4f;
    private float timer_length;
    private bool isTimerActive = false;
    private bool isPlayed = false;

    public float amplitude = 200f;
    public float frequency = 2f;
    private List<Vector2> startPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        result.gameObject.SetActive(false);
        timer_length = timer;
        startPosition = new List<Vector2>();
        foreach (RectTransform child in enemies_holder.transform)
        {
            startPosition.Add(child.anchoredPosition);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !isPlayed)
        {
            isTimerActive = true;
            isPlayed = true;
        }
        if (isTimerActive)
        {
            mainGame();
        }
        timer_text.text = "Time: " + (Mathf.Round(timer * 10) / 10).ToString();
    }

    void mainGame()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            isTimerActive = false;
            result.gameObject.SetActive(true);
            result.text = "YOU LOSE!";
            return;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            RectTransform playerRect = player.GetComponent<RectTransform>();

            playerRect.anchoredPosition += new Vector2(256, 0);
        }
        else if (player.transform.position.x >= 1472)
        {
            Debug.Log(player.transform.position.x);
            isTimerActive = false;
            result.gameObject.SetActive(true);
            result.text = "YOU WIN!";
            return;
        }
        if (checkCollision(player, enemies_holder))
        {
            isTimerActive = false;
            result.gameObject.SetActive(true);
            result.text = "YOU LOSE!";
            return;
        }
        moveEnemies();
    }

    bool checkCollision(GameObject player, GameObject enemy)
    {
        RectTransform playerRect = player.GetComponent<RectTransform>();

        foreach (RectTransform child in enemy.transform)
        {
            if (RectsOverlap(playerRect, child))
            {
                return true;
            }
        }

        return false;
    }

    bool RectsOverlap(RectTransform rect1, RectTransform rect2)
    {
        Vector3[] corners1 = new Vector3[4];
        Vector3[] corners2 = new Vector3[4];
        rect1.GetWorldCorners(corners1);
        rect2.GetWorldCorners(corners2);
        return !(corners1[2].x < corners2[0].x || // rect1 is left of rect2
                 corners1[0].x > corners2[2].x || // rect1 is right of rect2
                 corners1[2].y < corners2[0].y || // rect1 is below rect2
                 corners1[0].y > corners2[2].y);   // rect1 is above rect2
    }

    void moveEnemies()
    {
        foreach (RectTransform child in enemies_holder.transform)
        {
            int index = child.GetSiblingIndex();
            float phaseOffset = index * Mathf.PI / 2f;

            float newY = startPosition[index].y + Mathf.Sin(Time.time * frequency + phaseOffset) * amplitude;

            child.anchoredPosition = new Vector2(
                startPosition[index].x,
                newY
            );
        }
    }
}
