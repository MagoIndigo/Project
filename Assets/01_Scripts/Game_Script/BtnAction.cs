using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BtnAction : MonoBehaviour
{
    public string type;
    public string context;
    public GameObject child;

    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnClickButton);
        }
    }

    public void OnClickButton()
    {
        switch (type)
        {
            case "Dialog":
                child?.SetActive(true);
                break;
            case "Scene":
                SceneManager.LoadScene(context);
                break;
        }
    }
    // 이동 버튼을 관리하는 스크립트 내 함수
    public void BackToTitle()
    {
        // 1. 데이터 리셋
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGameData();
        }

        // 2. 타이틀 씬 로드
        SceneManager.LoadScene("TitleScene");
    }
}