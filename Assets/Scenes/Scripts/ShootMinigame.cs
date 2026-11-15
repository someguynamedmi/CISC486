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
    private float precision = 0.15f;
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
            mainGame();
        }
        if (!shot)
        {
            Debug.Log(timer);
        }
    }

    void mainGame()
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
            if (!shot)
            {
                result.gameObject.SetActive(true);
                result.text = "You Didn't Even Try?!";
            }
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
                Vector2 crosshairBounds = new Vector2(crosshair.transform.position.x - 50f, crosshair.transform.position.x + 50f);
                Vector2 targetBounds = new Vector2(target.transform.position.x - 50f, target.transform.position.x + 50f);
                float intersection = Mathf.Min(crosshairBounds.y, targetBounds.y) - Mathf.Max(crosshairBounds.x, targetBounds.x);  
                float union = Mathf.Max(crosshairBounds.y, targetBounds.y) - Mathf.Min(crosshairBounds.x, targetBounds.x);
                float score = Mathf.Round((intersection / union) * 10000f) / 100f;
                result.text = "WIN! \n score = " + score;
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
