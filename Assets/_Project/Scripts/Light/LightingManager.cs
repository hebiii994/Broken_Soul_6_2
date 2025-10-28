using UnityEngine;
public class LightingManager : MonoBehaviour
{

    private void OnEnable()
    {
        GameManager.OnGameReady += ApplyCorridorLighting;
    }

    private void OnDisable()
    {
        GameManager.OnGameReady -= ApplyCorridorLighting;
    }

    private void ApplyCorridorLighting()
    {
        Debug.Log("LightingManager: Applico le luci del corridoio.");
    }
}