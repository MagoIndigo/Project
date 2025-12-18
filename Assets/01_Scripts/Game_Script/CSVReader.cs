using UnityEngine;
using System.Collections.Generic;

public class CSVReader : MonoBehaviour
{
    [System.Serializable]
    public class DialogueData
    {
        public int id;
        public string speaker;
        public string context;
        public string nextID;
        public string type;
    }

    public List<DialogueData> dialogueList = new List<DialogueData>();

    void Start()
    {
        LoadCSV("StoryData");
    }

    public void LoadCSV(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(fileName);
        if (csvFile == null) return;

        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] row = lines[i].Split(',');

            DialogueData data = new DialogueData();
            int.TryParse(row[0], out data.id);
            data.speaker = row[1];
            data.context = row[2].Replace("\"", "");
            data.nextID = row[3];
            data.type = row[4].Trim();

            dialogueList.Add(data);
        }
    }
}