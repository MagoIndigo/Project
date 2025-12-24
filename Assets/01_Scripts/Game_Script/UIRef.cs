using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRef : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject dialogPanel;
    public GameObject choicePanel;

    [Header("Dialog UI (Auto-Filled)")]
    public GameObject nameGroup;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contextText;

    [Header("Choice UI (Auto-Filled)")]
    public TextMeshProUGUI choiceText1;
    public TextMeshProUGUI choiceText2;
    public Button choiceBtn1;
    public Button choiceBtn2;

    // 인스펙터에서 값이 변경될 때마다 실행되는 함수
    private void OnValidate()
    {
        AutoAssign();
    }

    // 컴포넌트를 처음 붙이거나 인스펙터 메뉴에서 Reset을 누를 때 실행
    private void Reset()
    {
        AutoAssign();
    }

    public void AutoAssign()
    {
        // 1. 대화창 패널이 할당되어 있다면 자식에서 찾음
        if (dialogPanel != null)
        {
            if (nameText == null) nameText = FindInChild<TMPro.TextMeshProUGUI>(dialogPanel, "Name");
            
            // NameText의 바로 위 부모를 nameGroup으로 자동 설정
            if (nameText != null && nameGroup == null) 
                nameGroup = nameText.transform.parent.gameObject;

            if (contextText == null) contextText = FindInChild<TMPro.TextMeshProUGUI>(dialogPanel, "Context");
        }

        // 2. 선택지 패널이 할당되어 있다면 자식에서 찾음
        if (choicePanel != null)
        {
            // 버튼들 찾기 (첫 번째 자식과 두 번째 자식 등 순서나 이름으로 찾기)
            Button[] buttons = choicePanel.GetComponentsInChildren<Button>(true);
            if (buttons.Length >= 1) choiceBtn1 = buttons[0];
            if (buttons.Length >= 2) choiceBtn2 = buttons[1];

            // 텍스트들 찾기
            if (choiceBtn1 != null && choiceText1 == null) 
                choiceText1 = choiceBtn1.GetComponentInChildren<TextMeshProUGUI>();
            if (choiceBtn2 != null && choiceText2 == null) 
                choiceText2 = choiceBtn2.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    // 이름 기반으로 자식을 찾는 편의용 함수
    private T FindInChild<T>(GameObject parent, string nameContains) where T : Component
    {
        T[] components = parent.GetComponentsInChildren<T>(true);
        foreach (var comp in components)
        {
            if (comp.gameObject.name.Contains(nameContains))
                return comp;
        }
        return null;
    }
}