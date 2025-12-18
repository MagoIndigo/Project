using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isPaused = false;

    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void PauseGame() {
        isPaused = true;
        Time.timeScale = 0;
    }

    public void ResumeGame() {
        isPaused = false;
        Time.timeScale = 1;
    }

    public void LoadNewScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
