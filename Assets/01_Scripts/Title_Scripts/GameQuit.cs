using UnityEngine;

public class GameQuit : MonoBehaviour
{
    // Update is called once per frame
    public void QuitGame()
    {
        Debug.Log("게임 종료!");
        Application.Quit();
    }
}
