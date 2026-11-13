using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public Button playButton;
    public Button optionsButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playButton.onClick.AddListener(PlayGame);
        optionsButton.onClick.AddListener(ShowOptions);
    }

    void PlayGame()
    {
        // Load the game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

    void ShowOptions()
    {
        return;
    }
}
