using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;


public class BossAI_Colloquio : MonoBehaviour
{
    private enum BossState { WaitingForGameData, Analyzing,Idle, Taunting, Attacking }
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
    [SerializeField] private float _dodgeTriggerRange = 5f;
    private float _lastDodgeTime = -999f;
    private bool _canMove = false;
    private bool _isDodging = false;    
    public float DodgeTriggerRange => _dodgeTriggerRange;

    [Header("Combat")]
    [SerializeField] private List<PoolScriptableObject> _projectilePoolSO;
    [SerializeField] private Transform _firePoint;
    private int _attackCycleCount = 0;
    private Transform _playerTransform;
    private bool _fightIsOver = false;

    [Header("Graphics")]
    [SerializeField] private SpriteRenderer _renderBoss;
    [SerializeField] private List<Sprite> _spritesFaces;

    [Header("Flow")]
    [SerializeField] private CheckpointSO _playerSpawnCheckpointAfterDefeat;

    [SerializeField] private Transform _playerSpawnPointAfterDefeat;
    private BossDialogue _bossDialogue;
    private bool _aiStarted = false;
    private Coroutine _attackCoroutine;
    private Coroutine _stateMachineRoutine;
    private Coroutine _moveRoutine;


    // --- NUOVE VARIABILI DI PROVA PER I GIZMOS ---
    private Vector3 _dodgeStartPosition;
    private Vector3 _dodgeEndPosition;
    private Vector3 _playerPosOnDodge;
    private bool _drawDodgeGizmos = false;

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
        PlayerDeathHandler.OnPlayerDied += OnPlayerDefeated;
        if (GameManager.Instance != null && GameManager.Instance.IsGameReady)
        {
            StartAI();
        }
    }

    private void OnDisable()
    {
        GameManager.OnGameReady -= StartAI;
        PlayerDeathHandler.OnPlayerDied -= OnPlayerDefeated;
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
        _stateMachineRoutine = StartCoroutine(StateMachineRoutine());
        _moveRoutine = StartCoroutine(MoveRoutine());
    }

    private IEnumerator StateMachineRoutine()
    {

        while (NarrativeManager.Instance == null || NarrativeManager.Instance.IsStoryLoading())
        {
            yield return null;
        }
        _currentState = BossState.Analyzing;
        Debug.Log($"Nuovo stato del boss: {_currentState}");

        while (!_fightIsOver)
        {
            switch (_currentState)
            {
                case BossState.Analyzing:
                    yield return _bossDialogue.ShowIntroAnalysis();

                    ChangeState(BossState.Idle);

                    CameraUtility.Instance.StartCoroutine(CameraUtility.Instance.ZoomCameraRoutine(6.5f, 1.5f));
                    _canMove = false;
                    break;

                case BossState.Idle:
                    yield return new WaitForSeconds(_idleTime);
                    if (_fightIsOver) yield break;

                    var faceNumber = Random.Range(0, System.Enum.GetNames(typeof(BossFace)).Length);
                    _currentFace = (BossFace)faceNumber;
                    _renderBoss.sprite = _spritesFaces[faceNumber];
                    _canMove = true;

                    ChangeState(BossState.Taunting);
                    Debug.Log($"Nuova faccia scelta: {faceNumber} ({_currentFace}).");
                    break;

                case BossState.Taunting:
                    yield return _bossDialogue.ShowTaunt();
                    if (_fightIsOver) yield break;

                    _canMove = true;

                    ChangeState(BossState.Attacking);
                    break;

                case BossState.Attacking:
                    Debug.Log("BOSS: Eseguo un attacco!");
                    Attack();
                    yield return new WaitForSeconds(_attackCooldown);
                    if (_fightIsOver) yield break;

                    _attackCycleCount++;
                    _canMove = false;

                    ChangeState(BossState.Idle);
                    break;
            }
        }
        Debug.Log("Ciclo FSM del boss terminato.");
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


        int baseShots = 1;
        if (_currentFace == BossFace.FaceTwo) baseShots = 5; 
        if (_currentFace == BossFace.FaceOne) baseShots = 3;
        if (_currentFace == BossFace.FaceThree) baseShots = 1;

        int bonusShots = _attackCycleCount;
        int totalShots = Mathf.Min(baseShots + bonusShots, 15);

        for (int i = 0; i < totalShots; i++)
        {
            if (_fightIsOver) yield break;
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
            while (!_canMove || _isDodging) yield return null;

            Vector2 startingPosition = transform.position;
            Vector2 targetPosition = MoveRandomically(_arenaStart, _arenaEnd);
            float journeyLength = Vector2.Distance(startingPosition, targetPosition);
            float startTime = Time.time;

            while (Time.time < startTime + _moveDuration)
            {
                float normalizedTime = (Time.time - startTime) / _moveDuration;
                float easedT = EaseInOutCubic(normalizedTime);
                transform.position = Vector2.Lerp(startingPosition, targetPosition, easedT);
                yield return null;
            }
            
            startingPosition = targetPosition;
            yield return new WaitForSeconds(_waitDuration);
        }
    }
    public void TryToDodge()
    {
        if (!_isDodging && Time.time >= _lastDodgeTime + _dodgeCooldown)
        {
            StartCoroutine(DodgeRoutine());
        }
    }

    public IEnumerator DodgeRoutine()
    {
        _isDodging = true;
        _lastDodgeTime = Time.time;
        if (_playerTransform == null)
        {
            Debug.LogError("BOSS AI: Riferimento al giocatore mancante. Impossibile schivare.", this);
            yield break;
        }
        Vector3 startingPosition = transform.position;
        Vector3 dodgeDirection = (startingPosition - _playerTransform.position).normalized;
        if (Mathf.Abs(dodgeDirection.x) > Mathf.Abs(dodgeDirection.y))
            dodgeDirection.y = 0;
        else
            dodgeDirection.x = 0;
        dodgeDirection.Normalize();

        Vector3 dodgeTarget = startingPosition + dodgeDirection * _dodgeDistance;

        Vector2 bossHalfSize = _renderBoss.bounds.size / 2;

        float minX_world = Mathf.Min(_arenaStart.position.x, _arenaEnd.position.x);
        float maxX_world = Mathf.Max(_arenaStart.position.x, _arenaEnd.position.x);
        float minY_world = Mathf.Min(_arenaStart.position.y, _arenaEnd.position.y);
        float maxY_world = Mathf.Max(_arenaStart.position.y, _arenaEnd.position.y);

        float minX_pivot = minX_world + bossHalfSize.x;
        float maxX_pivot = maxX_world - bossHalfSize.x;
        float minY_pivot = minY_world + bossHalfSize.y;
        float maxY_pivot = maxY_world - bossHalfSize.y;

        dodgeTarget.x = Mathf.Clamp(dodgeTarget.x, minX_pivot, maxX_pivot);
        dodgeTarget.y = Mathf.Clamp(dodgeTarget.y, minY_pivot, maxY_pivot);

        _renderBoss.color = Color.cyan;
        float startTime = Time.time;
        while (Time.time < startTime + _moveDuration)
        {
            float timeElapsed = Time.time - startTime;
            float normalizedTime = timeElapsed / _moveDuration;
            float easedT = EaseOutCubic(normalizedTime);
            transform.position = Vector3.Lerp(startingPosition, dodgeTarget, easedT);
            yield return null;
        }
        _renderBoss.color = Color.white;
        Debug.Log($"BOSS AI: Schivata diretta verso {dodgeTarget}");
        //Debug schivata
        _dodgeStartPosition = startingPosition;
        _dodgeEndPosition = dodgeTarget;
        _playerPosOnDodge = _playerTransform.position;
        _drawDodgeGizmos = true;
        //fine DEBUG schivata
        _isDodging = false;
    }

    public void OnPlayerDefeated()
    {
        if (_fightIsOver) return;
        CameraUtility.Instance.StartCoroutine(CameraUtility.Instance.ZoomCameraRoutine(4.5f, 1.5f));
        _fightIsOver = true;
        if (_attackCoroutine != null) StopCoroutine(_attackCoroutine);
        if (_moveRoutine != null) StopCoroutine(_moveRoutine);
        if (_stateMachineRoutine != null) StopCoroutine(_stateMachineRoutine);
        StartCoroutine(DefeatSequenceRoutine());
    }
    private IEnumerator DefeatSequenceRoutine()
    {
        Debug.Log("BOSS AI: Avvio sequenza di sconfitta del giocatore.");
        _canMove = false;
        yield return _bossDialogue.ShowDefeatMonologue();  
        yield return StartCoroutine(TeleportPlayerAndStartGame());
    }

    private IEnumerator TeleportPlayerAndStartGame()
    {

         yield return ScreenFader.Instance.FadeOut(1.5f); 


        SaveManager.Instance.MarkFirstBossAsDefeated(); 
        PlayerStateMachine player = FindFirstObjectByType<PlayerStateMachine>(FindObjectsInactive.Include);
        if (player != null)
        {

            player.transform.position = _playerSpawnPointAfterDefeat.position;
            SaveManager.Instance.SetCurrentCheckpoint(_playerSpawnCheckpointAfterDefeat);
            player.gameObject.SetActive(true);
            SaveManager.Instance.SaveGame();
        }

        yield return ScreenFader.Instance.FadeIn(1.5f);
    }

    private void ChangeState(BossState newState)
    {
        if (_fightIsOver)
        {
            return;
        }

        _currentState = newState;
        Debug.Log($"Nuovo stato del boss: {_currentState}");
    }

    private void OnDrawGizmos()
    {
        if (_drawDodgeGizmos)
        {
            // Linea della schivata (Ciano)
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(_dodgeStartPosition, _dodgeEndPosition);
            Gizmos.DrawSphere(_dodgeEndPosition, 0.25f); 

            // Linea verso il giocatore (Rossa)
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_dodgeStartPosition, _playerPosOnDodge);
        }
    }

    private float EaseOutCubic(float x)
    {
        return 1 - Mathf.Pow(1 - x, 3);
    }
    private float EaseInOutCubic(float x)
    {
        return x < 0.5f ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }
}