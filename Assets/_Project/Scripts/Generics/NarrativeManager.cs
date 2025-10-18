using DG.Tweening;
using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Cinemachine;

public class NarrativeManager : SingletonGeneric<NarrativeManager>, ISaveable
{
    protected override bool ShouldBeDestroyOnLoad() => true;


    [Header("Ink")]
    [SerializeField] private TextAsset _inkJSON;
    private Story _story;
    private bool _introCompleted = false;
    public bool IntroCompleted => _introCompleted;

    [Header("UI")]
    [SerializeField] private GameObject _dialogPanel;
    [SerializeField] private TextMeshProUGUI _voceOniricaText;
    [SerializeField] private TextMeshProUGUI _pensieroPrecarioText;
    [SerializeField] private float _pensieroDisplayTime = 4f;

    [Header("Scene Objects")]
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _playerStartPosition;
    [SerializeField] private Transform _bossArenaStartPosition;
    [SerializeField] private GameObject _bossObject;
    [SerializeField] private GameObject _doorPrefab;
    [SerializeField] private Transform[] _doorSpawnPoints;
    [SerializeField] private CinemachineCamera _playerCamera;

    [Header("Boss Fight Shortcut")]
    [SerializeField] private string _bossShortcutLabel = "Torna al colloquio";
    [SerializeField] private Transform _bossShortcutDoorSpawnPoint;

    [Header("Effects")]
    [SerializeField] private Image _flashImage;
    [SerializeField] private float _flashDuration = 1f;

    [Header("Flow")] 
    [SerializeField] private CheckpointSO _playerSpawnOnBoss;

    [Header("Typewriter Effect")]
    [SerializeField] private float _typingSpeed = 0.04f;
    //[SerializeField] private float _fastTypingSpeed = 0.01f;

    private Coroutine _writingCoroutine;
    //private string _fullTextForSkip;
    //private TextMeshProUGUI _activeTextComponentForSkip;

    private List<GameObject> _currentDoors = new List<GameObject>();
    private int _choicesIgnored = 0;
    private bool _isChoosing = false;
    private bool _isStoryLoading = true;

    protected override void Awake()
    {
        base.Awake();
        if (_bossObject != null) _bossObject.SetActive(false);
        if (_dialogPanel != null) _dialogPanel.SetActive(false);
        if (_flashImage != null) _flashImage.gameObject.SetActive(false);
    }

    private void Start()
    {
        // StartStory(); // La storia viene ora avviata solo dal SaveManager tramite LoadData
    }

    private void Update()
    {

        //if (_writingCoroutine != null && Keyboard.current.eKey.wasPressedThisFrame)
        //{
        //    StopCoroutine(_writingCoroutine);

        //    if (_activeTextComponentForSkip != null)
        //    {
        //        _activeTextComponentForSkip.text = _fullTextForSkip;
        //    }

        //    _writingCoroutine = null;
        //    _fullTextForSkip = null;
        //    _activeTextComponentForSkip = null;
        //}
    }

    private void StartStory(string inkStoryState = null)
    {
        _story = new Story(_inkJSON.text);
        if (!string.IsNullOrEmpty(inkStoryState))
        {
            _story.state.LoadJson(inkStoryState);
        }
        _isStoryLoading = false;
        RefreshView();
    }

    public void LoadData(GameData data)
    {
        _introCompleted = data.hasCompletedIntro;

        if (!_introCompleted)
        {
            StartStory(data.inkStoryState);
        }
        else
        {
            _isStoryLoading = false;
            RefreshView();
        }
    }

    public void SaveData(ref GameData data)
    {
        if (_story != null && !_introCompleted)
        {
            data.inkStoryState = _story.state.ToJson();
        }
    }

    private void RefreshView()
    {
        ClearScene();

        if (_introCompleted)
        {
            CleanupIntroObjects();
            DisplayBossShortcutDoor();
            return;
        }

        while (_story.canContinue)
        {
            string currentLine = _story.Continue();
            Debug.Log("POST-CONTINUE || canContinue=" + _story.canContinue + " || tag correnti: " + string.Join(",", _story.currentTags));
            DisplayLine(currentLine, _story.currentTags);
        }

        if (HandleSpecialTags(_story.currentTags))
        {
            return;
        }

        DisplayChoices();
    }

    public void MakeChoice(int choiceIndex)
    {
        if (_isChoosing) return;

        if (choiceIndex == -1) 
        {
            StartCoroutine(BossFightTransitionRoutine());
            return;
        }

        StartCoroutine(ChoiceTransitionRoutine(choiceIndex));
    }


    private IEnumerator ChoiceTransitionRoutine(int choiceIndex)
    {
        _isChoosing = true;
        PlayerInputController inputController = _player.GetComponent<PlayerInputController>();
        if (inputController != null) inputController.enabled = false;
        _player.GetComponent<PlayerMovement>().StopMovement();

        if (_flashImage != null)
        {
            _flashImage.gameObject.SetActive(true);
            yield return _flashImage.DOFade(1, _flashDuration / 2).WaitForCompletion();
        }

        if (_player != null && _playerStartPosition != null)
        {
            _player.transform.position = _playerStartPosition.position;
            Debug.Log($"[NarrativeManager] TELEPORTO Player a _playerStartPosition: {_player.transform.position}");

        }

        _story.ChooseChoiceIndex(choiceIndex);
        string pensieroText = "";
        bool hasPensiero = false;
        if (_story.canContinue)
        {
            pensieroText = _story.Continue();
            if (_story.currentTags.Contains("pensiero"))
            {
                hasPensiero = true;
            }
        }

        if (_flashImage != null)
        {
            yield return _flashImage.DOFade(0, _flashDuration / 2).WaitForCompletion();
            _flashImage.gameObject.SetActive(false);
        }

        if (hasPensiero)
        {
            if (_voceOniricaText != null) _voceOniricaText.gameObject.SetActive(false);
            if (_pensieroPrecarioText != null)
            {
                _writingCoroutine = StartCoroutine(WriteTextCoroutine(_pensieroPrecarioText, pensieroText));
                yield return _writingCoroutine;

                yield return new WaitForSeconds(_pensieroDisplayTime);
            }
        }

        if (_dialogPanel != null) _dialogPanel.SetActive(false);

        if (HandleSpecialTags(_story.currentTags))
        {
            _isChoosing = false;
            yield break;
        }

        if (inputController != null) inputController.enabled = true;
        PlayerStateMachine psm = _player.GetComponent<PlayerStateMachine>();
        if (psm != null)
        {
            psm.ChangeState(new PlayerIdleState(psm));
        }
        _isChoosing = false;
        RefreshView();
    }

    public void OnPlayerIgnoredChoice()
    {
        if (_isChoosing || _story == null || _story.currentChoices.Count == 0)
        {
            return;
        }
        if (_isChoosing) return;
        _choicesIgnored++;
        string knotToJumpTo = "";
        switch (_choicesIgnored)
        {
            case 1: knotToJumpTo = "Ignored_Once"; break;
            case 2: knotToJumpTo = "Ignored_Twice"; break;
            case 3: knotToJumpTo = "Ignored_Thrice"; break;
            case 4: knotToJumpTo = "Ignored_Fourth"; break;
            case 5: knotToJumpTo = "Ignored_Fifth"; break;
            default:
                if (_choicesIgnored >= 6) knotToJumpTo = "Finale_Segreto";
                break;
        }
        if (!string.IsNullOrEmpty(knotToJumpTo))
        {
            StartCoroutine(ShowSpecialDialogue(knotToJumpTo));
        }
    }

    private IEnumerator ShowSpecialDialogue(string knot)
    {
        _isChoosing = true;
        PlayerInputController inputController = _player.GetComponent<PlayerInputController>();
        if (inputController != null) inputController.enabled = false;
        string originalStoryState = _story.state.ToJson();
        ClearScene();

        if (_flashImage != null)
        {
            _flashImage.gameObject.SetActive(true);
            yield return _flashImage.DOFade(1, _flashDuration / 2).WaitForCompletion();
        }

        if (_player != null && _playerStartPosition != null)
        {
            _player.transform.position = _playerStartPosition.position;
            Debug.Log($"[NarrativeManager] TELEPORTO Player a _playerStartPosition: {_player.transform.position}");

        }

        _story.ChoosePathString(knot);
        if (_story.canContinue)
        {
            string specialLine = _story.Continue();
            Debug.Log("POST-CONTINUE || canContinue=" + _story.canContinue + " || tag correnti: " + string.Join(",", _story.currentTags));
            if (_story.currentTags.Contains("voce") && _voceOniricaText != null)
            {
                if (_pensieroPrecarioText != null) _pensieroPrecarioText.gameObject.SetActive(false);
                _writingCoroutine = StartCoroutine(WriteTextCoroutine(_voceOniricaText, specialLine));
                yield return _writingCoroutine;
            }
            else
            {
                Debug.LogWarning("Il dialogo speciale non ha un tag 'voce' o il campo di testo non è assegnato.");
                _voceOniricaText.text = specialLine;
            }
        }

        if (_flashImage != null)
        {
            yield return _flashImage.DOFade(0, _flashDuration / 2).WaitForCompletion();
            _flashImage.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(4f);
        _dialogPanel.SetActive(false);

        _story.state.LoadJson(originalStoryState);
        if (inputController != null) inputController.enabled = true;

        PlayerStateMachine psm = _player.GetComponent<PlayerStateMachine>();
        if (psm != null) psm.ChangeState(new PlayerIdleState(psm));

        _isChoosing = false;
        RefreshView();
    }

    private bool HandleSpecialTags(List<string> tags)
    {
        foreach (string tag in tags)
        {
            Debug.Log("CHECK TAG: " + tag);
            if (tag.Trim() == "evento:StartGame")
            {
                Debug.Log("[BossFight] TRANSIZIONE richiamata!");
                StartCoroutine(BossFightTransitionRoutine());
                return true;
            }
        }
        return false;
    }
    public bool IsStoryLoading()
    {
        return _isStoryLoading;
    }
    private IEnumerator BossFightTransitionRoutine()
    {
        _isChoosing = true;
        ClearScene();
        CleanupIntroObjects();
        PlayerInputController inputController = _player.GetComponent<PlayerInputController>();
        if (inputController != null) inputController.enabled = false;

        if (_flashImage != null)
        {
            _flashImage.gameObject.SetActive(true);
            yield return _flashImage.DOFade(1, _flashDuration / 2).WaitForCompletion();
        }

        SaveManager.Instance.SetCurrentCheckpoint(_playerSpawnOnBoss);
        SaveManager.Instance.MarkIntroAsCompleted();
        SaveManager.Instance.SaveGame();

        Checkpoint targetCheckpoint = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None)
                                  .FirstOrDefault(c => c.checkpointData == _playerSpawnOnBoss);

        if (_player != null && targetCheckpoint != null)
        {
            _player.transform.position = targetCheckpoint.transform.position;
            if (_playerCamera != null)
            {
                _playerCamera.OnTargetObjectWarped(_player, _player.transform.position - _playerCamera.transform.position);
            }
        }


        if (_bossObject != null)
        {
            _bossObject.SetActive(true);
        }

        if (_flashImage != null)
        {
            yield return _flashImage.DOFade(0, _flashDuration / 2).WaitForCompletion();
            _flashImage.gameObject.SetActive(false);
        }

        BossDialogue bossDialogue = _bossObject.GetComponent<BossDialogue>();
        if (bossDialogue != null)
        {
            yield return bossDialogue.ShowIntroAnalysis();
        }

        if (inputController != null) inputController.enabled = true;
        PlayerStateMachine psm = _player.GetComponent<PlayerStateMachine>();
        if (psm != null) psm.ChangeState(new PlayerIdleState(psm));

        _isChoosing = false;
    }

    private void CleanupIntroObjects()
    {
        IgnoreChoiceTrigger[] leftoverTriggers = FindObjectsByType<IgnoreChoiceTrigger>(FindObjectsSortMode.None);
        foreach (IgnoreChoiceTrigger trigger in leftoverTriggers)
        {
            Destroy(trigger.gameObject);
        }

        SecretEndingTrigger[] secretTriggers = FindObjectsByType<SecretEndingTrigger>(FindObjectsSortMode.None);
         foreach (SecretEndingTrigger trigger in secretTriggers)
         {
             Destroy(trigger.gameObject);
         }

        Debug.Log("Pulizia degli oggetti dell'introduzione completata.");
    }
    private void DisplayBossShortcutDoor()
    {
        if (_doorPrefab == null)
        {
            Debug.LogError("ERRORE: Il 'Door Prefab' non è stato assegnato!", this);
            return;
        }
        if (_bossShortcutDoorSpawnPoint == null)
        {
            Debug.LogError("ERRORE: Il 'Boss Shortcut Door Spawn Point' non è stato assegnato!", this);
            return;
        }

        GameObject doorObj = Instantiate(_doorPrefab, _bossShortcutDoorSpawnPoint.position, _bossShortcutDoorSpawnPoint.rotation);
        DoorInteractor doorInteractor = doorObj.GetComponent<DoorInteractor>();
        if (doorInteractor != null)
        {
            doorInteractor.choiceIndex = -1;
        }
        doorObj.GetComponentInChildren<TextMeshPro>().text = _bossShortcutLabel;
        _currentDoors.Add(doorObj);
    }

    private void DisplayLine(string line, List<string> tags)
    {
        if (string.IsNullOrWhiteSpace(line)) return;

        if (tags.Contains("voce"))
        {
            if (_voceOniricaText != null)
            {
                _writingCoroutine = StartCoroutine(WriteTextCoroutine(_voceOniricaText, line));
            }
            if (_pensieroPrecarioText != null) _pensieroPrecarioText.gameObject.SetActive(false);
            if (_dialogPanel != null) _dialogPanel.SetActive(true);
        }
    }

    private void DisplayChoices()
    {
        if (_story.currentChoices.Count > 0)
        {
            for (int i = 0; i < _story.currentChoices.Count; i++)
            {
                if (i < _doorSpawnPoints.Length)
                {
                    GameObject doorObj = Instantiate(_doorPrefab, _doorSpawnPoints[i].position, _doorSpawnPoints[i].rotation);
                    DoorInteractor doorInteractor = doorObj.GetComponent<DoorInteractor>();
                    if (doorInteractor == null)
                    {
                        Debug.LogError("ERRORE: Il prefab della porta non ha lo script 'DoorInteractor'!", doorObj);
                        continue;
                    }
                    doorObj.GetComponentInChildren<TextMeshPro>().text = _story.currentChoices[i].text;
                    doorInteractor.choiceIndex = i;
                    _currentDoors.Add(doorObj);
                }
            }
        }
    }

    private void ClearScene()
    {
        foreach (GameObject door in _currentDoors) Destroy(door);
        _currentDoors.Clear();
        if (_dialogPanel != null) _dialogPanel.SetActive(false);
    }

    public void TriggerSecretEnding()
    {
        StartCoroutine(ShowSecretEnding());
    }

    private IEnumerator ShowSecretEnding()
    {
        ClearScene();
        _player.GetComponent<PlayerStateMachine>().enabled = false;
        _player.GetComponent<PlayerMovement>().StopMovement();

        _voceOniricaText.gameObject.SetActive(false);
        _writingCoroutine = StartCoroutine(WriteTextCoroutine(_pensieroPrecarioText, "...la scelta di non scegliere."));
        _pensieroPrecarioText.gameObject.SetActive(true);
        _dialogPanel.SetActive(true);
        yield return _writingCoroutine;


        _writingCoroutine = StartCoroutine(WriteTextCoroutine(_pensieroPrecarioText, "...di non essere."));
        yield return _writingCoroutine;

        Debug.Log("GAME OVER - Ritorno al menu principale...");
        // Esempio: SceneManager.LoadScene("MainMenu");
    }
    private IEnumerator WriteTextCoroutine(TextMeshProUGUI textComponent, string textToWrite)
    {
        //_fullTextForSkip = textToWrite;
        //_activeTextComponentForSkip = textComponent;

        textComponent.text = "";
        textComponent.gameObject.SetActive(true);
        _dialogPanel.SetActive(true);

        foreach (char letter in textToWrite.ToCharArray())
        {
            textComponent.text += letter;
            if (letter != ' ' && letter != '\n')
            {

                // AudioManager.Instance.PlayTypingSound(); 
            }
            yield return new WaitForSeconds(_typingSpeed);
        }

        _writingCoroutine = null;
        //_fullTextForSkip = null;
        //_activeTextComponentForSkip = null;
    }
    public object GetInkVariable(string variableName)
    {
        if (_story != null)
        {
            try
            {
                return _story.variablesState[variableName];
            }
            catch (System.Exception)
            {
                Debug.LogWarning("La variabile Ink '" + variableName + "' non è stata trovata.");
            }
        }
        return null;
    }

}