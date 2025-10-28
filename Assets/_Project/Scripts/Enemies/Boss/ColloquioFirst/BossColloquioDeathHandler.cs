using UnityEngine;

public class BossColloquioDeathHandler : MonoBehaviour, IDeathHandler
{
    private BossAI_Colloquio _bossAI;

    private void Awake()
    {
        _bossAI = GetComponent<BossAI_Colloquio>();
    }
    public void HandleDeath()
    {
        Debug.Log("Il Colloquio è stato sconfitto.");


        if (CameraUtility.Instance != null)
        {

            CameraUtility.Instance.StartCoroutine(
            CameraUtility.Instance.ZoomCameraRoutine(_bossAI.NormalCameraZoom, 1.5f) 
            );
        }

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.MarkFirstBossAsDefeated();
            SaveManager.Instance.SaveGame();
        }

        gameObject.SetActive(false);
    }
}