using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class VisualNovelManager : MonoBehaviour
{
    public static VisualNovelManager Instance;

    [HideInInspector] 
    public CSVParser parser; 

    [Header("UI References (Auto-Linked via UIRef)")]
    private GameObject nameGroup;
    private TMP_Text nameText;
    private TMP_Text contextText;
    private GameObject choicePanel;
    private Button[] choiceButtons = new Button[2];
    private TMP_Text[] choiceTexts = new TMP_Text[2];
    private GameObject dialogPanel;

    private AudioSource bgmSource;

    [Header("Settings")]
    public int currentID = 100;
    public float typingSpeed = 0.03f;
    public int endingThreshold = 2;

    private bool isTyping = false;
    private bool isInputBlocked = true;
    private string fullText = "";
    private System.Action onTypingComplete;

    void Awake()
    {
        // 싱글톤 설정: 씬이 넘어가도 파괴되지 않음
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 오디오 설정
        bgmSource = GetComponent<AudioSource>();
        if (bgmSource == null) bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    // 씬 로드 시 UIRef를 찾아 UI 컴포넌트들을 자동으로 다시 연결
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "00_Title") 
        {
            if (bgmSource != null)
            {
                bgmSource.Stop();
                bgmSource.clip = null; // 클립 초기화
            }
        }

        UIRef ui = Object.FindFirstObjectByType<UIRef>(); 
        
        if (ui != null)
        {
            this.dialogPanel = ui.dialogPanel;
            this.nameGroup = ui.nameGroup;
            this.nameText = ui.nameText;
            this.contextText = ui.contextText;
            this.choicePanel = ui.choicePanel;
            
            this.choiceButtons[0] = ui.choiceBtn1;
            this.choiceButtons[1] = ui.choiceBtn2;
            this.choiceTexts[0] = ui.choiceText1;
            this.choiceTexts[1] = ui.choiceText2;

            // 초기 상태: 패널들 숨기기
            if (dialogPanel != null) dialogPanel.SetActive(false);
            if (choicePanel != null) choicePanel.SetActive(false);

            StopAllCoroutines();
            StartCoroutine(InitializeRoutine());
        }
    }

    IEnumerator InitializeRoutine()
    {
        isInputBlocked = true;

        while (GameManager.Instance == null || CSVParser.Instance == null)
            yield return null;

        // 배경음 중복 방지: 다른 씬의 오디오 소스 정지
        AudioSource[] allSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allSources)
        {
            if (source != bgmSource) source.Stop();
        }

        parser = CSVParser.Instance;
        parser.ParseCSV();

        if (GameManager.Instance.nextStartID != 0)
        {
            currentID = GameManager.Instance.nextStartID;
            GameManager.Instance.nextStartID = 0;
        }

        yield return new WaitForSeconds(0.5f);
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

        // 카운트 추가 로직
        if (data.count_add != 0) GameManager.Instance.AddCount(data.count_add);

        // 타입별 특수 처리
        switch (data.type)
        {
            case "Scene":
                GameManager.Instance.GoToNextScene(data.context, data.nextID);
                return;
            case "BGM":
                PlayBGM(data.context);
                ShowDialog(data.nextID);
                return;
            case "Ending":
                CheckEndingCondition();
                return;
        }

        // 이름 처리 및 이름표 그룹(배경) On/Off
        bool hasName = !string.IsNullOrEmpty(data.name) && data.name.Trim().Length > 0;
        if (nameText != null) nameText.text = data.name;
        if (nameGroup != null) nameGroup.SetActive(hasName);

        fullText = data.context;
        currentID = id;

        // 선택지 존재 여부 확인 (onTypingComplete 예약)
        bool hasChoice = !string.IsNullOrEmpty(data.choice1_context) && data.choice1_context.Trim().Length > 0;
        onTypingComplete = hasChoice ? (System.Action)(() => ShowChoices(data)) : null;
        
        if (dialogPanel != null && !dialogPanel.activeSelf) dialogPanel.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(TypeText(fullText));
    }

    void PlayBGM(string fileName)
    {
        if (string.IsNullOrEmpty(fileName) || fileName.ToLower() == "none")
        {
            bgmSource.Stop();
            bgmSource.clip = null;
            return;
        }

        AudioClip clip = Resources.Load<AudioClip>("BGM/" + fileName);
        if (clip != null && bgmSource.clip != clip)
        {
            bgmSource.Stop();
            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        if (contextText != null) contextText.text = "";
        
        foreach (char letter in text.ToCharArray())
        {
            if (contextText != null) contextText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
        
        // 타이핑 종료 후 선택지가 있다면 실행
        onTypingComplete?.Invoke();
        onTypingComplete = null;
    }

    void SkipTyping()
    {
        StopAllCoroutines();
        if (contextText != null) contextText.text = fullText;
        isTyping = false;
        
        // 스킵 시에도 선택지가 있다면 즉시 실행
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
        if (choicePanel == null) return;
        choicePanel.SetActive(true);
        
        SetBtn(0, data.choice1_context, data.choice1_next);
        
        if (!string.IsNullOrEmpty(data.choice2_context) && choiceButtons.Length > 1)
        {
            choiceButtons[1].gameObject.SetActive(true);
            SetBtn(1, data.choice2_context, data.choice2_next);
        }
        else if (choiceButtons.Length > 1) 
        {
            choiceButtons[1].gameObject.SetActive(false); 
        }
    }

    void SetBtn(int i, string txt, int next)
    {
        if (choiceTexts[i] != null) choiceTexts[i].text = txt;
        if (choiceButtons[i] != null)
        {
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => { 
                choicePanel.SetActive(false); 
                ShowDialog(next); 
            });
        }
    }

    void CheckEndingCondition()
    {
        if (GameManager.Instance.totalCount >= endingThreshold)
            GameManager.Instance.GoToNextScene("06_BadEnding", 601);
        else
            GameManager.Instance.GoToNextScene("05_Lobby2", 505);
    }
}