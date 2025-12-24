using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class VisualNovelManager : MonoBehaviour
{
    [HideInInspector] 
    public CSVParser parser; 

    [Header("UI References")]
    public TMP_Text nameText;
    public TMP_Text contextText;
    public TMP_Text sceneCountText; 
    public GameObject choicePanel;
    public Button[] choiceButtons;
    public TMP_Text[] choiceTexts;
    public GameObject dialogPanel;

    // [자동 할당] 인스펙터에서 연결하지 않아도 Start에서 스스로 찾습니다.
    private AudioSource bgmSource;

    [Header("Settings")]
    public int currentID = 100;
    public float typingSpeed = 0.03f;
    public int endingThreshold = 2;

    private bool isTyping = false;
    private bool isInputBlocked = true;
    private string fullText = "";
    private System.Action onTypingComplete;

    void Start()
    {
        if (choicePanel != null) choicePanel.SetActive(false);

        
        // 오디오 소스 자동 설정 및 초기화
        bgmSource = GetComponent<AudioSource>();
        if (bgmSource == null) bgmSource = gameObject.AddComponent<AudioSource>();
        
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;

        StartCoroutine(InitializeRoutine());
    }

    IEnumerator InitializeRoutine()
    {
        while (GameManager.Instance == null || CSVParser.Instance == null)
        {
            yield return null;
        }

        // [추가] 타이틀 씬 등 이전 씬에서 넘어온 모든 배경음을 정지시킵니다.
        // 씬에 존재하는 모든 AudioSource를 찾아 정지 (필요 시)
        AudioSource[] allSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allSources)
        {
            // 현재 내 오브젝트에 붙은 bgmSource가 아닌 다른 소스들은 정지
            if (source != bgmSource) source.Stop();
        }

        parser = CSVParser.Instance;
        parser.ParseCSV();

        // GameManager로부터 예약된 ID가 있는지 확인
        if (GameManager.Instance.nextStartID != 0)
        {
            currentID = GameManager.Instance.nextStartID;
        }

        if (sceneCountText != null)
            GameManager.Instance.globalCountText = sceneCountText;

        yield return new WaitForSeconds(1.0f);
        isInputBlocked = false;
        ShowDialog(currentID);
    }

    void Update()
    {
        if (isInputBlocked || (choicePanel != null && choicePanel.activeSelf)) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping) SkipTyping();
            else MoveToNext();
        }
    }

    public void ShowDialog(int id)
    {
        

        if (parser == null || !parser.dialogDictionary.ContainsKey(id)) return;
        DialogData data = parser.dialogDictionary[id];

        if (data.count_add != 0) 
        {
            GameManager.Instance.AddCount(data.count_add);
            Debug.Log($"카운트 추가됨: {data.count_add}, 현재 총합: {GameManager.Instance.totalCount}");
        }

        switch (data.type)
        {
            case "Scene":
                GameManager.Instance.GoToNextScene(data.context, data.nextID);
                return;

            case "BGM":
                PlayBGM(data.context);   // BGM 재생/교체 함수 호출
                ShowDialog(data.nextID); // 바로 다음 대사로 진행
                return;

            case "Ending":
                CheckEndingCondition();
                return;
        }

        nameText.text = data.name;
        fullText = data.context;
        currentID = id;

        bool hasChoice = !string.IsNullOrEmpty(data.choice1_context) && data.choice1_context.Trim().Length > 0;
        onTypingComplete = hasChoice ? (System.Action)(() => ShowChoices(data)) : null;
        
        if (dialogPanel != null && !dialogPanel.activeSelf)
        {
            dialogPanel.SetActive(true);
        }

        StartCoroutine(TypeText(fullText));
    }

    // [BGM 교체 로직]
    void PlayBGM(string fileName)
    {
        if (string.IsNullOrEmpty(fileName) || fileName.ToLower() == "none")
        {
            bgmSource.Stop();
            bgmSource.clip = null;
            return;
        }

        AudioClip clip = Resources.Load<AudioClip>("BGM/" + fileName);

        if (clip != null)
        {
            // 현재 재생 중인 곡과 다른 곡일 때만 교체 실행
            if (bgmSource.clip != clip)
            {
                bgmSource.Stop();      // 기존 곡 중단
                bgmSource.clip = clip; // 새 곡 할당
                bgmSource.Play();      // 재생 시작
                Debug.Log($"BGM 교체됨: {fileName}");
            }
        }
        else
        {
            Debug.LogWarning($"[BGM 에러] Resources/BGM/{fileName} 파일을 찾을 수 없습니다.");
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        contextText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            contextText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
        onTypingComplete?.Invoke();
        onTypingComplete = null;
    }

    void SkipTyping()
    {
        StopAllCoroutines();
        contextText.text = fullText;
        isTyping = false;
        onTypingComplete?.Invoke();
        onTypingComplete = null;
    }

    void MoveToNext()
    {
        int next = parser.dialogDictionary[currentID].nextID;
        if (next != 0) ShowDialog(next);
    }

    void ShowChoices(DialogData data)
    {
        choicePanel.SetActive(true);
        SetBtn(0, data.choice1_context, data.choice1_next);
        if (!string.IsNullOrEmpty(data.choice2_context))
        {
            choiceButtons[1].gameObject.SetActive(true);
            SetBtn(1, data.choice2_context, data.choice2_next);
        }
        else { choiceButtons[1].gameObject.SetActive(false); }
    }

    void SetBtn(int i, string txt, int next)
    {
        choiceTexts[i].text = txt;
        choiceButtons[i].onClick.RemoveAllListeners();
        choiceButtons[i].onClick.AddListener(() => { 
            choicePanel.SetActive(false); 
            ShowDialog(next); 
        });
    }

    void CheckEndingCondition()
    {
        if (GameManager.Instance.totalCount >= endingThreshold)
        {
            SceneManager.LoadScene("06_BadEnding");
            GameManager.Instance.GoToNextScene("06_BadEnding", 403);
        }    
        else
        {
            SceneManager.LoadScene("05_GoodEnding");
            GameManager.Instance.GoToNextScene("05_GoodEnding", 500);
        }
    }
}