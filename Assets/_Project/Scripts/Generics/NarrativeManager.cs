using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Ink.Runtime;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class NarrativeManager : MonoBehaviour
{
    public static NarrativeManager Instance { get; private set; }

    [Header("Ink")]
    [SerializeField] private TextAsset _inkJSON;
    private Story _story;

    [Header("UI")]
    [SerializeField] private GameObject _dialogPanel;
    [SerializeField] private TextMeshProUGUI _voceOniricaText;
    [SerializeField] private TextMeshProUGUI _pensieroPrecarioText;
    [SerializeField] private float _pensieroDisplayTime = 4f;

    [Header("Scene Objects")]
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _playerStartPosition;
    [SerializeField] private GameObject _doorPrefab;
    [SerializeField] private Transform[] _doorSpawnPoints;

    [Header("Effects")]
    [SerializeField] private Image _flashImage;
    [SerializeField] private float _flashDuration = 1f;
    [SerializeField] private float _textFadeDuration = 0.5f;

    private List<GameObject> _currentDoors = new List<GameObject>();
    private int _choicesIgnored = 0;
    private bool _isChoosing = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        if (_dialogPanel != null) _dialogPanel.SetActive(false);
        if (_flashImage != null) _flashImage.gameObject.SetActive(false);
        StartStory();
    }

    private void StartStory()
    {
        _story = new Story(_inkJSON.text);
        RefreshView();
    }

    private void RefreshView()
    {
        ClearScene();
        if (HandleSpecialTags(_story.currentTags)) return;
        if (_story.canContinue) DisplayLine();
        DisplayChoices();
    }

    public void MakeChoice(int choiceIndex)
    {
        if (_isChoosing) return;
        StartCoroutine(ChoiceTransitionRoutine(choiceIndex));
    }

    private IEnumerator ChoiceTransitionRoutine(int choiceIndex)
    {
        _isChoosing = true;

        PlayerInputController inputController = _player.GetComponent<PlayerInputController>();
        PlayerMovement playerMovement = _player.GetComponent<PlayerMovement>();
        if (inputController != null) inputController.enabled = false;
        if (playerMovement != null) playerMovement.enabled = false;
        playerMovement.StopMovement();

        if (_flashImage != null)
        {
            _flashImage.gameObject.SetActive(true);
            yield return _flashImage.DOFade(1, _flashDuration / 2).WaitForCompletion();
        }

        if (_player != null && _playerStartPosition != null)
        {
            _player.transform.position = _playerStartPosition.position;
        }

        _story.ChooseChoiceIndex(choiceIndex);
        string pensieroText = "";
        if (_story.canContinue)
        {
            pensieroText = _story.Continue();
            if (_story.currentTags.Contains("pensiero"))
            {
                if (_voceOniricaText != null) _voceOniricaText.gameObject.SetActive(false);
                if (_pensieroPrecarioText != null)
                {
                    _pensieroPrecarioText.text = pensieroText;
                    _pensieroPrecarioText.gameObject.SetActive(true);
                }
                if (_dialogPanel != null) _dialogPanel.SetActive(true);
            }
        }

        if (_flashImage != null)
        {
            yield return _flashImage.DOFade(0, _flashDuration / 2).WaitForCompletion();
            _flashImage.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(_pensieroDisplayTime);
        if (_dialogPanel != null) _dialogPanel.SetActive(false);


        if (playerMovement != null) playerMovement.enabled = true;
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
        PlayerMovement playerMovement = _player.GetComponent<PlayerMovement>();
        if (inputController != null) inputController.enabled = false;
        if (playerMovement != null) playerMovement.enabled = false;
        playerMovement.StopMovement();

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
        }

        _story.ChoosePathString(knot);
        DisplayLine();


        if (_flashImage != null)
        {
            yield return _flashImage.DOFade(0, _flashDuration / 2).WaitForCompletion();
            _flashImage.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(4f);
        _dialogPanel.SetActive(false);

        _story.state.LoadJson(originalStoryState);

        if (playerMovement != null) playerMovement.enabled = true;
        if (inputController != null) inputController.enabled = true;

        PlayerStateMachine psm = _player.GetComponent<PlayerStateMachine>();
        if (psm != null) psm.ChangeState(new PlayerIdleState(psm));

        _isChoosing = false;
        RefreshView();
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
        _pensieroPrecarioText.text = "...la scelta di non scegliere.";
        _pensieroPrecarioText.gameObject.SetActive(true);
        _dialogPanel.SetActive(true);
        yield return new WaitForSeconds(4f);

        _pensieroPrecarioText.text = "...di non essere.";
        yield return new WaitForSeconds(4f);
     
        Debug.Log("GAME OVER - Ritorno al menu principale...");
        // Esempio: SceneManager.LoadScene("MainMenu");
    }

    private void DisplayLine()
    {
        string currentLine = _story.Continue();
        if (string.IsNullOrWhiteSpace(currentLine) || !_story.currentTags.Contains("voce")) return;

        if (_voceOniricaText != null)
        {
            _voceOniricaText.text = currentLine;
            _voceOniricaText.gameObject.SetActive(true);
            _voceOniricaText.alpha = 0;
            _voceOniricaText.DOFade(1, _textFadeDuration);
        }
        if (_pensieroPrecarioText != null) _pensieroPrecarioText.gameObject.SetActive(false);
        if (_dialogPanel != null) _dialogPanel.SetActive(true);
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

    private bool HandleSpecialTags(List<string> tags)
    {
        foreach (string tag in tags)
        {
            if (tag.Contains("evento:StartGame"))
            {
                Debug.Log("TRANSIZIONE AL GIOCO PRINCIPALE!");
                // Qui andrà la logica per caricare la scena successiva
                return true;
            }
        }
        return false;
    }


    private void ClearScene()
    {
        foreach (GameObject door in _currentDoors) Destroy(door);
        _currentDoors.Clear();
        if (_dialogPanel != null) _dialogPanel.SetActive(false);
    }
}