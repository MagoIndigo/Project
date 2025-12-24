using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BackgroundScaleEffect : MonoBehaviour
{
    private Image backgroundImage;
    private Vector3 originalScale;
    private Coroutine scaleCoroutine;

    [Header("Settings")]
    public string targetSpriteName = "GameScene01_0";
    public float scaleAmount = 0.025f; // 원래 크기(1.0)에서 더해질 양
    public float totalDuration = 5.0f;
    public int bounceCount = 2;

    void Awake()
    {
        backgroundImage = GetComponent<Image>();
        originalScale = transform.localScale; // 보통 (1, 1, 1)
    }

    void Update()
    {
        if (backgroundImage.sprite != null && backgroundImage.sprite.name == targetSpriteName)
        {
            if (scaleCoroutine == null)
            {
                scaleCoroutine = StartCoroutine(ScaleBounceRoutine());
            }
        }
        else
        {
            if (scaleCoroutine != null)
            {
                StopCoroutine(scaleCoroutine);
                scaleCoroutine = null;
                transform.localScale = originalScale;
            }
        }
    }

    IEnumerator ScaleBounceRoutine()
    {
        float elapsed = 0f;

        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;

            // 0 -> 1 -> 0 -> 1 -> 0 흐름 생성 (절대값 사인)
            float progress = elapsed / totalDuration;
            float pulse = Mathf.Abs(Mathf.Sin(progress * Mathf.PI * bounceCount));
            
            // 원래 크기(1.0)에 pulse(0~1) * 0.025를 더함
            float currentScale = 1.0f + (pulse * scaleAmount);
            transform.localScale = new Vector3(currentScale, currentScale, 1f);

            yield return null;
        }

        transform.localScale = originalScale;
        scaleCoroutine = null;
    }
}