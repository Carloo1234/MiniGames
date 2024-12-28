using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinDatabase", menuName = "Skin Database")]
public class SkinDatabase : ScriptableObject
{
    public List<SkinData> skins = new List<SkinData>();
}