using UnityEngine;

public class TitleBGMManager : MonoBehaviour
{
    public string titleBGMName = "TitleTheme"; // CSV에 적는 것과 똑같은 파일명
    private AudioSource audioSource;

    void Start()
    {
        // 1. AudioSource 설정
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;

        // 2. Resources에서 음악 로드 및 재생
        AudioClip titleClip = Resources.Load<AudioClip>("BGM/" + titleBGMName);
        
        if (titleClip != null)
        {
            audioSource.clip = titleClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"타이틀 BGM을 찾을 수 없습니다: Resources/BGM/{titleBGMName}");
        }
    }
}