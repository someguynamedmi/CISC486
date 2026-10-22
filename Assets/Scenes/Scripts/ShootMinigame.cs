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
    private bool isTimerActive = false;
    private bool isPlayed = false;
    private bool canShoot = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        key.transform.position = new Vector2(960,340);
        crosshair.transform.position = new Vector2(360,710);
        target.transform.position = new Vector2(1260,710);
        result.gameObject.SetActive(false);
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
        //Debug.Log(timer);
    }

    void countdown()
    {
        bool shot = false;
        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            canShoot = true;
        }
        timer -= Time.deltaTime;

        crosshair.transform.position = new Vector2(360 + ((4f-timer) * 300), 710);
        if (timer <= 0)
        {
            isTimerActive = false;
            timer = 3f;
        }

        Debug.Log(!shot && canShoot);
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !shot && canShoot)
        {
            if (timer >= 1.5)
            {
                result.gameObject.SetActive(true);
                result.text = "Too Early!";
            }
            if (timer < 1.5 && timer > 0.5)
            {
                result.gameObject.SetActive(true);
            }
            if (timer < 0.5)
            {
                result.gameObject.SetActive(true);
                result.text = "Too Late!";
            }
        }
    }

}
