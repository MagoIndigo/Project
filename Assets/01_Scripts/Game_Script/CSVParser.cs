using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class CSVParser : MonoBehaviour
{
    public static CSVParser Instance;
    public Dictionary<int, DialogData> dialogDictionary = new Dictionary<int, DialogData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    public void ParseCSV()
    {
        dialogDictionary.Clear();
        TextAsset data = Resources.Load<TextAsset>("Dialog"); 
        string[] lines = data.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++)
        {
            string pattern = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";
            string[] row = Regex.Split(lines[i], pattern);

            if (row.Length < 1 || string.IsNullOrEmpty(row[0].Trim())) continue;

            DialogData dialog = new DialogData();
            int.TryParse(row[0], out dialog.id);
            if (row.Length > 1) dialog.type = row[1].Trim();
            if (row.Length > 2) dialog.name = row[2].Trim();
            if (row.Length > 3) dialog.context = row[3].Trim('\"');
            if (row.Length > 4) int.TryParse(row[4], out dialog.nextID);
            if (row.Length > 5) dialog.choice1_context = row[5].Trim('\"', ' ');
            if (row.Length > 6) int.TryParse(row[6], out dialog.choice1_next);
            if (row.Length > 7) dialog.choice2_context = row[7].Trim('\"', ' ');
            if (row.Length > 8) int.TryParse(row[8], out dialog.choice2_next);
            if (row.Length > 9) int.TryParse(row[9], out dialog.count_add);

            if (!dialogDictionary.ContainsKey(dialog.id)) dialogDictionary.Add(dialog.id, dialog);
        }
    }
}