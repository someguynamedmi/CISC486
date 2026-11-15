using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    public Button nextLevelButton;
    public Button menuButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        nextLevelButton.onClick.AddListener(NextLevel);
        menuButton.onClick.AddListener(ShowMenu);
    }

    void NextLevel()
    {
        // Load the next level scene
    }

    void ShowMenu()
    {
        // Load the main menu scene
    }
}
