using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int totalCount = 0;           // 누적 카운트
    public TMP_Text globalCountText;     // UI 텍스트 참조

    // ★ 이 변수가 없어서 에러가 발생한 것입니다! ★
    public int nextStartID = 100;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            Debug.Log(totalCount);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (globalCountText != null)
        {
            globalCountText.text = "Count: " + totalCount;
        }
    }

    public void AddCount(int amount)
    {
        totalCount += amount;
    }

    // ★ 이 함수도 VisualNovelManager에서 사용하므로 반드시 있어야 합니다! ★
    public void GoToNextScene(string sceneName, int startID)
    {
        nextStartID = startID; 
        SceneManager.LoadScene(sceneName);
    }
    public void ResetGameData()
    {
        totalCount = 0;      // 카운트 초기화
        nextStartID = 100;     // 시작 ID 초기화 (필요 시 100 등 기본값으로 설정)
        
        // UI가 연결되어 있다면 UI도 즉시 갱신
        if (globalCountText != null)
        {
            globalCountText.text = "Count: 0";
        }
        
        Debug.Log("게임 데이터가 초기화되었습니다.");
    }
}
