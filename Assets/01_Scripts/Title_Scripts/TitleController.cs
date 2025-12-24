using UnityEngine;

public class TitleController : MonoBehaviour
{
    // 이동할 첫 번째 게임 씬의 정확한 이름 (예: "01_Subway")
    public string firstSceneName = "01_Subway"; 
    // 첫 번째 대사의 ID (CSV의 첫 줄 ID)
    public int startID = 100;

    public void StartGame()
    {
        if (GameManager.Instance != null)
        {
            // GameManager를 통해 씬 이동과 시작 ID 전달
            GameManager.Instance.ResetGameData();
            GameManager.Instance.GoToNextScene(firstSceneName, startID);
        }
        else
        {
            Debug.LogError("GameManager가 씬에 없습니다! Title 씬에 GameManager 프리팹을 배치했는지 확인하세요.");
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}