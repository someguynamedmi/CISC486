using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class TypingMinigame : MonoBehaviour
{
    public GameObject button_holder;

    public TextMeshProUGUI result;
    public TextMeshProUGUI timer_text;

    private List<GameObject> buttons;
    private List<char> letters = new List<char>();
    public float timer = 4f;
    private float timer_length;
    private bool isTimerActive = false;
    private bool isPlayed = false;
    private int currentButtonIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        result.gameObject.SetActive(false);
        buttons = new List<GameObject>();
        foreach (Transform child in button_holder.transform)
        {
            buttons.Add(child.gameObject);
        }
        timer_length = timer; 
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !isPlayed)
        {
            isTimerActive = true;
            isPlayed = true;
            setupGame();
        }
        if (isTimerActive)
        {
            mainGame();
        }
        timer_text.text = "Time: " + (Mathf.Round(timer * 10) / 10).ToString();
    }

    void setupGame()
    {
        foreach (GameObject button in buttons)
        {
            TextMeshProUGUI childText = button.GetComponentInChildren<TextMeshProUGUI>();
            char randomChar = (char)('a' + Random.Range(0, 26));
            letters.Add(randomChar);
            childText.text = (randomChar.ToString().ToUpper());
        }
    }

    void mainGame()
    {
        if (currentButtonIndex >= letters.Count)
        {
            isTimerActive = false;
            result.gameObject.SetActive(true);
            result.text = "WIN!";
            return;
        }
        var keyboard = Keyboard.current;
        var keyControl = keyboard.FindKeyOnCurrentKeyboardLayout(letters[currentButtonIndex].ToString());
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            isTimerActive = false;
            result.gameObject.SetActive(true);
            result.text = "You failed!";
            return;
        }
        else if (keyControl != null && keyControl.wasPressedThisFrame)
        {
            Image buttonImage = buttons[currentButtonIndex].GetComponent<Image>();
            buttonImage.color = Color.green;
            Debug.Log("Key pressed: " + keyControl);
            currentButtonIndex++;
        }
    }
}
