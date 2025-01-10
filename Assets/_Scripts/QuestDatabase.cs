using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestDatabase", menuName = "Quest Database")]
public class QuestDatabase : ScriptableObject
{
    public List<QuestData> quests = new List<QuestData>();
}
