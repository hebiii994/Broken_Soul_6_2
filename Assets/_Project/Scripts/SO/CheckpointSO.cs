using UnityEngine;

[CreateAssetMenu(fileName = "NewCheckpoint", menuName = "Broken Soul/Data/Checkpoint")]
public class CheckpointSO : ScriptableObject
{
    [Header("Checkpoint Info")]
    public string checkpointId;
}