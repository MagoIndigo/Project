using System;
using System.Collections.Generic;

[Serializable]
public class DialogData
{
    public int count_add;
    public int id;
    public string type;
    public string name;
    public string context;
    public int nextID;
    public string choice1_context;
    public int choice1_next;
    public string choice2_context;
    public int choice2_next;
}