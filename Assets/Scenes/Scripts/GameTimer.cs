using TMPro;
using UnityEngine;

// Put this on a TextMeshPro UI text object.
// The timer shows 0:00.00 until the player's first hop, then starts counting.
public class GameTimer : MonoBehaviour
{
    [Tooltip("The text to write the time into. Leave empty to use the text on this object.")]
    public TMP_Text timerText;

    [Tooltip("The player. Leave empty to find it automatically.")]
    public PlayerHopper player;

    [Tooltip("Show hundredths of a second (0:12.34) or just whole seconds (0:12).")]
    public bool showHundredths = true;

    float elapsed;
    bool running;
    bool hasStarted;

    public float Elapsed => elapsed;
    public bool IsRunning => running;

    void Start()
    {
        if (timerText == null) timerText = GetComponent<TMP_Text>();
        if (player == null) player = FindAnyObjectByType<PlayerHopper>();

        if (player != null) player.Hopped += OnPlayerHopped;
        else Debug.LogWarning("GameTimer: no PlayerHopper found in the scene.");

        UpdateText();
    }

    void OnDestroy()
    {
        if (player != null) player.Hopped -= OnPlayerHopped;
    }

    // Only the very first hop starts the timer.
    void OnPlayerHopped()
    {
        if (hasStarted) return;
        hasStarted = true;
        running = true;
    }

    void Update()
    {
        if (!running) return;
        elapsed += Time.deltaTime;
        UpdateText();
    }

    // Call this when the level is finished, e.g. from a goal platform.
    public void StopTimer()
    {
        running = false;
    }

    // Puts the timer back to 0 and waits for the next first hop.
    public void ResetTimer()
    {
        elapsed = 0f;
        running = false;
        hasStarted = false;
        UpdateText();
    }

    void UpdateText()
    {
        if (timerText == null) return;

        int minutes = (int)(elapsed / 60f);
        float seconds = elapsed % 60f;

        timerText.text = showHundredths
            ? $"{minutes}:{seconds:00.00}"
            : $"{minutes}:{(int)seconds:00}";
    }
}