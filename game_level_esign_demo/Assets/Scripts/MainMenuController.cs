using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
    [Tooltip("Name of the scene to load when pressing Start. Make sure this scene is added in Build Settings.")]
    public string levelToLoad = "Level1";

    public void OnStartButton()
    {
        if (string.IsNullOrEmpty(levelToLoad))
        {
            Debug.LogError("MainMenuController: levelToLoad is empty!");
            return;
        }
        SceneManager.LoadScene(levelToLoad);
    }

    public void OnQuitButton()
    {
        Debug.Log("Quit button pressed.");
#if UNITY_EDITOR
        // If in the editor, stop play mode
        EditorApplication.isPlaying = false;
#else
        // If in a built application, close the game
        Application.Quit();
#endif
    }
}
