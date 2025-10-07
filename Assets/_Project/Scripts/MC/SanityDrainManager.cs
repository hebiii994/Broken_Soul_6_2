using UnityEngine;

public class SanityDrainManager : MonoBehaviour
{
    [SerializeField] private CharacterStats _characterStats;

    private HealthSystem _healthSystem;
    private NostalgiaSystem _nostalgiaSystem;
    private float _timer;

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();
        _nostalgiaSystem = GetComponent<NostalgiaSystem>();
    }

    private void Update()
    {
        if (_nostalgiaSystem.CurrentNostalgia <= 0)
        {
            _timer += Time.deltaTime;
            if (_timer >= _characterStats.drainTickRate)
            {
                _timer = 0f;
                _healthSystem.TakeDamage(_characterStats.drainAmount);
            }
        }
        else
        {
            _timer = 0f; 
        }
    }
}