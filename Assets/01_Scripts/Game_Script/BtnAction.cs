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
}