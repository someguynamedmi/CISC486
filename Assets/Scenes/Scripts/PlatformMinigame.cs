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

    public float amplitude = 50f;
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
            player.transform.position = new Vector2(player.transform.position.x + 256, player.transform.position.y);
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
        foreach (Transform child in enemy.transform)
        {
            if (player.GetComponent<Collider2D>().bounds.Intersects(child.GetComponent<Collider2D>().bounds))
            {
                return true;
            }
        }
        return false;
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
