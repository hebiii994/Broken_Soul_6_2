using UnityEngine;

public class BossColloquioDeathHandler : MonoBehaviour, IDeathHandler
{
    public void HandleDeath()
    {
        Debug.Log("Il Colloquio è stato sconfitto.");


        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.MarkFirstBossAsDefeated();

            SaveManager.Instance.SaveGame();
        }



        gameObject.SetActive(false);
    }
}