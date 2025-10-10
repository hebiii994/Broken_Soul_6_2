using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class BossAI_Colloquio : MonoBehaviour
{
    private enum BossState { WaitingForGameData, Analyzing,Idle, Taunting, Attacking, AlreadyWon }
    private BossState _currentState;

    public enum BossFace { FaceOne,FaceTwo,FaceThree}
    private BossFace _currentFace;

    [Header("Behavior")]
    [SerializeField] private float _idleTime = 2f;
    [SerializeField] private float _attackCooldown = 3f;

    [Header("Arena")]
    [SerializeField] private Transform _arenaStart;
    [SerializeField] private Transform _arenaEnd;
    [SerializeField] private CinemachineCamera _playerCamera;

    [Header("Movement")]
    [SerializeField] private float _moveDuration = 1f;
    [SerializeField] private float _waitDuration = 1f;
    [SerializeField] private float _dodgeDistance = 2f;
    [SerializeField] private float _dodgeCooldown = 5f;
    private bool _canMove = false;

    [Header("Combat")]
    [SerializeField] private List<PoolScriptableObject> _projectilePoolSO;
    [SerializeField] private Transform _firePoint;
    private Transform _playerTransform;

    [Header("Graphics")]
    [SerializeField] private SpriteRenderer _renderBoss;
    [SerializeField] private List<Sprite> _spritesFaces;

    private BossDialogue _bossDialogue;
    private bool _aiStarted = false;
    private Coroutine _attackCoroutine;
    private float _originalLens;

    private void Awake()
    {
        _arenaStart = GameObject.FindWithTag("ColloquioArenaStart").transform;
        _arenaEnd = GameObject.FindWithTag("ColloquioArenaEnd").transform;
        _renderBoss = GetComponent<SpriteRenderer>();
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
        _originalLens = _playerCamera.Lens.OrthographicSize;
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
        StartCoroutine(MoveRoutine());
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
                    _playerCamera.Lens.OrthographicSize = 6.5f;
                    _canMove = false;
                    Debug.Log($"Current Boss STate {_currentState}");
                    break;

                case BossState.Idle:
                    yield return new WaitForSeconds(_idleTime);
                    var faceNumber = Random.Range(0, System.Enum.GetNames(typeof(BossFace)).Length);
                    _currentFace = (BossFace)faceNumber;
                    _renderBoss.sprite = _spritesFaces[faceNumber];
                    _currentState = BossState.Taunting;
                    _canMove = true;
                    Debug.Log($"Nuova faccia scelta: {faceNumber} ({_currentFace}). Prossimo stato: Taunting.");
                    break;

                case BossState.Taunting:
                    yield return _bossDialogue.ShowTaunt();
                    _currentState = BossState.Attacking;
                    _canMove = true;
                    Debug.Log($"Current Boss STate {_currentState}");
                    break;

                case BossState.Attacking:
                    Debug.Log("BOSS: Eseguo un attacco!");
                    Attack();
                    yield return new WaitForSeconds(_attackCooldown);
                    _currentState = BossState.Idle;
                    _canMove = false;
                    Debug.Log($"Current Boss STate {_currentState}");
                    break;

                case BossState.AlreadyWon:
                    _playerCamera.Lens.OrthographicSize = _originalLens;
                    _canMove = false;
                    break;

            }
        }
    }

    private void Attack()
    {
        if (_attackCoroutine != null) StopCoroutine(_attackCoroutine);
        _attackCoroutine = StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        PoolScriptableObject currentPool = _projectilePoolSO[(int)_currentFace];
        float fireRate = currentPool.fireRate;


        int numberOfShots = 1;
        if (_currentFace == BossFace.FaceTwo) numberOfShots = 20; 
        if (_currentFace == BossFace.FaceOne) numberOfShots = 10;
        if (_currentFace == BossFace.FaceThree) numberOfShots = 1; 

        for (int i = 0; i < numberOfShots; i++)
        {
            BossProjectile projectile = PoolManager.Instance.GetPooledObject(currentPool) as BossProjectile;
            if (projectile != null)
            {
                projectile.transform.position = _firePoint.position;
                Vector2 direction = (_playerTransform.position - _firePoint.position).normalized;
                projectile.Launch(direction);
            }
            yield return new WaitForSeconds(1f / fireRate); 
        }
    }

    private Vector2 MoveRandomically(Transform arenaStart, Transform arenaEnd)
    {
        float minX = Mathf.Min(arenaStart.position.x, arenaEnd.position.x);
        float maxX = Mathf.Max(arenaStart.position.x, arenaEnd.position.x);
        float randomX = Random.Range(minX, maxX);

        float minY = Mathf.Min(arenaStart.position.y, arenaEnd.position.y);
        float maxY = Mathf.Max(arenaStart.position.y, arenaEnd.position.y);
        float randomY = Random.Range(minY, maxY);

        Vector2 targetDestination = new Vector2(randomX, randomY);
        return targetDestination;
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            while (!_canMove) yield return null;

            Vector2 startingPosition = transform.position;
            Vector2 targetPosition = MoveRandomically(_arenaStart, _arenaEnd);
            float journeyLength = Vector2.Distance(startingPosition, targetPosition);
            float startTime = Time.time;

            while (Time.time < startTime + _moveDuration)
            {
                float t = (Time.time - startTime) / _moveDuration;
                transform.position = Vector2.Lerp(startingPosition, targetPosition, t);
                yield return null;
            }
            
            startingPosition = targetPosition;
            yield return new WaitForSeconds(_waitDuration);
        }
    }

    public IEnumerator DodgeRoutine()
    {
        if (_playerTransform == null)
        {
            Debug.LogError("BOSS AI: Riferimento al giocatore mancante. Impossibile schivare.", this);
            yield break;
        }
        yield return new WaitForSeconds(_dodgeCooldown);
        Vector3 startingPosition = transform.position;
        Vector3 dodgeDirection = (startingPosition - _playerTransform.position).normalized;
        if (Mathf.Abs(dodgeDirection.x) > Mathf.Abs(dodgeDirection.y))
            dodgeDirection.y = 0;
        else
            dodgeDirection.x = 0;
        dodgeDirection.Normalize();

        Vector3 dodgeTarget = startingPosition + dodgeDirection * _dodgeDistance;

        float minX = Mathf.Min(_arenaStart.position.x, _arenaEnd.position.x);
        float maxX = Mathf.Max(_arenaStart.position.x, _arenaEnd.position.x);
        float minY = Mathf.Min(_arenaStart.position.y, _arenaEnd.position.y);
        float maxY = Mathf.Max(_arenaStart.position.y, _arenaEnd.position.y);
        dodgeTarget.x = Mathf.Clamp(dodgeTarget.x, minX, maxX);
        dodgeTarget.y = Mathf.Clamp(dodgeTarget.y, minY, maxY);

        _renderBoss.color = Color.cyan;
        float startTime = Time.time;
        while (Time.time < startTime + _moveDuration)
        {
            float t = (Time.time - startTime) / _moveDuration;
            transform.position = Vector3.Lerp(startingPosition, dodgeTarget, t);
            yield return null;
        }
        _renderBoss.color = Color.white;
        Debug.Log($"BOSS AI: Schivata diretta verso {dodgeTarget}");
        Debug.DrawLine(startingPosition, dodgeTarget, Color.cyan, 2f);
        Debug.DrawLine(startingPosition, _playerTransform.position, Color.red, 2f);
    }
}