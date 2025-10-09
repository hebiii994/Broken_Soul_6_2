using UnityEngine;
using System.Collections;

public class BossAI_Colloquio : MonoBehaviour
{
    private enum BossState { Idle, Taunting, Attacking }
    private BossState _currentState;

    [SerializeField] private float _idleTime = 2f;
    [SerializeField] private float _attackCooldown = 3f;

    private BossDialogue _bossDialogue;

    private void Awake()
    {
        _bossDialogue = GetComponent<BossDialogue>();
    }

    private void OnEnable()
    {
        StartCoroutine(WaitForNarrativeManagerThenRun());
    }

    private IEnumerator WaitForNarrativeManagerThenRun()
    {
        while (NarrativeManager.Instance == null)
            yield return null;
        _currentState = BossState.Idle;
        StartCoroutine(StateMachineRoutine());
    }

    private IEnumerator StateMachineRoutine()
    {
        yield return _bossDialogue.ShowIntroAnalysis();

        while (true)
        {
            switch (_currentState)
            {
                case BossState.Idle:
                    yield return new WaitForSeconds(_idleTime);
                    _currentState = BossState.Taunting;
                    break;

                case BossState.Taunting:
                    yield return _bossDialogue.ShowTaunt();
                    _currentState = BossState.Attacking;
                    break;

                case BossState.Attacking:
                    Debug.Log("BOSS: Eseguo un attacco!");
                    // TODO: Aggiungere qui la logica di attacco
                    yield return new WaitForSeconds(_attackCooldown);
                    _currentState = BossState.Idle;
                    break;
            }
        }
    }
}