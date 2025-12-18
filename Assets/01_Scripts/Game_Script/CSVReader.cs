using UnityEngine;
using System.Collections.Generic;

public class CSVReader : MonoBehaviour
{
    // 대화 데이터를 담을 클래스
    [System.Serializable]
    public class DialogueData
    {
        public int id;
        public string speaker;
        public string context;
        public string nextID; // 선택지일 경우 "201,202" 처럼 쉼표로 구분
        public string type;   // "Dialog" 또는 "Choice"
    }

    public List<DialogueData> dialogueList = new List<DialogueData>();

    void Start()
    {
        LoadCSV("StoryData"); // 확장자 없이 파일 이름만 입력
    }

    public void LoadCSV(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(fileName);
        if (csvFile == null) return;

        // 줄바꿈으로 나누기
        string[] lines = csvFile.text.Split('\n');

        // 첫 번째 줄(헤더)은 건너뛰고 인덱스 1부터 시작
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] row = lines[i].Split(','); // 쉼표로 데이터 구분

            DialogueData data = new DialogueData();
            int.TryParse(row[0], out data.id);
            data.speaker = row[1];
            data.context = row[2].Replace("\"", ""); // 따옴표 제거
            data.nextID = row[3];
            data.type = row[4].Trim();

            dialogueList.Add(data);
        }
    }
}