using UnityEngine;

public class VHSCollectible : MonoBehaviour, ISaveable
{
    [Header("Collectible Settings")]
    [SerializeField] private int _maxHealthIncreaseAmount = 1;
    [SerializeField] private string _id;

    private bool _isCollected = false;

    public void LoadData(GameData data)
    {
        data.cassetteCollected.TryGetValue(_id, out _isCollected);
        if (_isCollected)
        {
            gameObject.SetActive(false);
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.cassetteCollected.ContainsKey(_id))
        {
            data.cassetteCollected[_id] = _isCollected;
        }
        else
        {
            data.cassetteCollected.Add(_id, _isCollected);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isCollected) return;
        if (other.CompareTag("Player") && other.TryGetComponent<HealthSystem>(out HealthSystem healthSystem))
        {
            healthSystem.IncreaseMaxHealth(_maxHealthIncreaseAmount);
            _isCollected = true;
            SaveManager.Instance.SaveGame();
            gameObject.SetActive(false);
        }
    }

    [ContextMenu("Generate GUID for ID")]
    private void GenerateGuid()
    {
        _id = System.Guid.NewGuid().ToString();
    }
}