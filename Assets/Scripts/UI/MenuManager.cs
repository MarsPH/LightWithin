using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Method to start the game
    public void StartGame()
    {
        // Replace "GameScene" with the name of your game scene
        SceneManager.LoadScene("MainScene");
    }

    // Method to quit the game
    public void QuitGame()
    {
        // Quits the application (this won't work in the editor)
        Debug.Log("Quit Game");
        Application.Quit();
    }

    // Method to load a specific scene by name
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}