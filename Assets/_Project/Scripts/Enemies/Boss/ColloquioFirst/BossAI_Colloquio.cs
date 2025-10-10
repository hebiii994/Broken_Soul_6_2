using UnityEngine;
using System.Collections;

public class BossAI_Colloquio : MonoBehaviour
{
    private enum BossState { WaitingForGameData, Analyzing,Idle, Taunting, Attacking }
    private BossState _currentState;

    [Header("Behavior")]
    [SerializeField] private float _idleTime = 2f;
    [SerializeField] private float _attackCooldown = 3f;

    [Header("Combat")]
    [SerializeField] private PoolScriptableObject _projectilePoolSO;
    [SerializeField] private Transform _firePoint;
    private Transform _playerTransform;

    private BossDialogue _bossDialogue;
    private bool _aiStarted = false;

    private void Awake()
    {
        _bossDialogue = GetComponent<BossDialogue>();
        _playerTransform = FindFirstObjectByType<PlayerStateMachine>().transform;
        _currentState = BossState.WaitingForGameData;
    }

    private void OnEnable()
    {
        GameManager.OnGameReady += StartAI;
        if (GameManager.Instance != null && GameManager.Instance.IsGameReady)
        {
            StartAI();
        }
    }

    private void OnDisable()
    {
        GameManager.OnGameReady -= StartAI;
    }

    //private IEnumerator WaitForNarrativeManagerThenRun()
    //{
    //    while (NarrativeManager.Instance == null)
    //        yield return null;
    //    _currentState = BossState.Idle;
    //    StartCoroutine(StateMachineRoutine());
    //}

    private void StartAI()
    {
        if (_aiStarted) return;
        _aiStarted = true;

        Debug.Log("BOSS AI: Ricevuto segnale 'Game Ready'. Avvio IA.");
        StartCoroutine(StateMachineRoutine());
    }

    private IEnumerator StateMachineRoutine()
    {
        while (NarrativeManager.Instance == null || NarrativeManager.Instance.IsStoryLoading())
        {
            yield return null;
        }
        _currentState = BossState.Analyzing;
        while (true)
        {
            switch (_currentState)
            {
                case BossState.Analyzing:
                    yield return _bossDialogue.ShowIntroAnalysis();
                    _currentState = BossState.Idle;
                    Debug.Log($"Current Boss STate {_currentState}");
                    break;

                case BossState.Idle:
                    yield return new WaitForSeconds(_idleTime);
                    _currentState = BossState.Taunting;
                    Debug.Log($"Current Boss STate {_currentState}");
                    break;

                case BossState.Taunting:
                    yield return _bossDialogue.ShowTaunt();
                    _currentState = BossState.Attacking;
                    Debug.Log($"Current Boss STate {_currentState}");
                    break;

                case BossState.Attacking:
                    Debug.Log("BOSS: Eseguo un attacco!");
                    Attack();
                    yield return new WaitForSeconds(_attackCooldown);
                    _currentState = BossState.Idle;
                    Debug.Log($"Current Boss STate {_currentState}");
                    break;
            }
        }
    }

    private void Attack()
    {
        if (_playerTransform == null)
        {
            Debug.LogError("BOSS AI: Riferimento al giocatore mancante. Impossibile attaccare.", this);
            return;
        }
        if (_projectilePoolSO == null)
        {
            Debug.LogError("BOSS AI: Pool dei proiettili non assegnato nell'Inspector! Impossibile attaccare.", this);
            return;
        }


        BossProjectile projectile = PoolManager.Instance.GetPooledObject(_projectilePoolSO) as BossProjectile;
        if (projectile != null)
        {
            projectile.transform.position = _firePoint.position;

            Vector2 direction = (_playerTransform.position - _firePoint.position).normalized;
            projectile.Launch(direction);
        }
    }
}