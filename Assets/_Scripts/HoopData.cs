using UnityEngine;

[CreateAssetMenu(fileName ="Hoop Data")]
public class HoopData : ScriptableObject
{
    [Header("Spawn Data")]
    public string hoopType;
    public float xMinLimit = 1.5f, xMaxLimit = 3.0f;
    public float yMinLimit = -1.65f, yMaxLimit = 1.65f;
    public bool hasRotation;
    public float minRotation, maxRotation = 30.0f;
    public bool hasScale;
    public float minScale = 0.95f, maxScale = 1.1f;
}
