using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ShootMinigame : MonoBehaviour
{
    public GameObject key;
    public GameObject crosshair;
    public GameObject target;

    public TextMeshProUGUI result;

    public float timer = 4f;
    private float timer_length;
    private float precision = 0.1f;
    private bool isTimerActive = false;
    private bool isPlayed = false;
    private bool canShoot = false;
    private bool shot = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        key.transform.position = new Vector2(960,340);
        crosshair.transform.position = new Vector2(360,710);
        target.transform.position = new Vector2(1260,710);
        result.gameObject.SetActive(false);
        timer_length = timer;
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
            countdown();
        }
        if (!shot)
        {
            Debug.Log(timer);
        }
    }

    void countdown()
    {
        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            canShoot = true;
        }
        timer -= Time.deltaTime;

        crosshair.transform.position = new Vector2(360 + ((timer_length-timer) * 300), 710);
        if (timer <= 0)
        {
            isTimerActive = false;
            timer = timer_length;
            result.gameObject.SetActive(true);
            result.text = "Too Late!";
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && !shot && canShoot)
        {
            if (timer >= 1f + precision)
            {
                result.gameObject.SetActive(true);
                result.text = "Too Early!";
            }
            if (timer < 1f + precision && timer > 1f - precision)
            {
                Debug.Log(1f+precision);
                result.gameObject.SetActive(true);
                result.text = "WIN!";
            }
            if (timer <= 1f - precision)
            {
                result.gameObject.SetActive(true);
                result.text = "Too Late!";
            }
            shot = true;
        }
    }

}
