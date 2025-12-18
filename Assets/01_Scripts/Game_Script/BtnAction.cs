using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // 버튼 컴포넌트 사용을 위해 필요

public class BtnAction : MonoBehaviour
{
    public string type;
    public string context;
    public GameObject child;

    // Start는 게임 시작 시 한 번 실행됩니다.
    void Start()
    {
        // 이 오브젝트에 붙어있는 Button 컴포넌트를 가져와서 클릭 시 실행할 함수를 연결합니다.
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnClickButton);
        }
    }

    // Update 함수는 아예 삭제하거나 비워두셔도 됩니다.

    // 버튼이 클릭되었을 때만 실행될 함수
    public void OnClickButton()
    {
        switch (type)
        {
            case "Dialog":
                child?.SetActive(true);
                Debug.Log("Dialog 활성화: " + context);
                break;
            case "Scene":
                SceneManager.LoadScene(context);
                Debug.Log("씬 이동: " + context);
                break;
        }
    }
}