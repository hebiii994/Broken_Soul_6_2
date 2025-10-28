using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class MainMenuController : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject _startScreenPanel; 
    [SerializeField] private GameObject _slotSelectionPanel; 
    [SerializeField] private GameObject _resetConfirmationPanel; 
    [SerializeField] private GameObject _nameInputPanel;      

    [Header("Slot UI Elements")]
    [SerializeField] private Button _startButton;         
    [SerializeField] private Button _mainResetButton;
    [SerializeField] private Button _cancelResetButton;
    [SerializeField] private Button[] _slotButtons = new Button[SaveManager.NUM_SLOTS];
    [SerializeField] private Button[] _resetSlotButtons = new Button[SaveManager.NUM_SLOTS];

    [Header("Name Input Specifics")]
    [SerializeField] private ControllerTextInput _controllerTextInputScript;

    private int _selectedSlotID = -1;
    private const string GameSceneName = "MainScene"; 

    private void Start()
    {
        if (_startButton != null)
        {
            EventSystem.current.SetSelectedGameObject(_startButton.gameObject);
        }

        SetupListeners();

        _startScreenPanel.SetActive(true);
        _slotSelectionPanel.SetActive(false);
        _resetConfirmationPanel.SetActive(false);
        _nameInputPanel.SetActive(false);

        if (_controllerTextInputScript != null)
        {
            _controllerTextInputScript.enabled = false;
        }

        if (_startButton != null)
        {
            EventSystem.current.SetSelectedGameObject(_startButton.gameObject);
        }

        PlayerInputController playerInput = FindAnyObjectByType<PlayerInputController>();
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }


    }

    private void SetupListeners()
    {

        _startButton.onClick.AddListener(OnStartPressed);


        _mainResetButton.onClick.AddListener(ShowResetConfirmation);


        for (int i = 0; i < SaveManager.NUM_SLOTS; i++)
        {
            int slotID = i + 1;
            _slotButtons[i].onClick.AddListener(() => OnSlotSelected(slotID));
            _resetSlotButtons[i].onClick.AddListener(() => OnResetSlot(slotID));
        }
    }



    public void OnStartPressed()
    {
        _startScreenPanel.SetActive(false);
        _slotSelectionPanel.SetActive(true);
        RefreshSlotDisplay();
        if (_slotButtons.Length > 0 && _slotButtons[0] != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_slotButtons[0].gameObject);
        }
    }

    private void ShowResetConfirmation()
    {
        _slotSelectionPanel.SetActive(false);
        _resetConfirmationPanel.SetActive(true);
        if (_resetSlotButtons.Length > 0 && _resetSlotButtons[0] != null)
        {
            EventSystem.current.SetSelectedGameObject(_resetSlotButtons[0].gameObject);
        }
    }

    public void CancelReset()
    {
        _resetConfirmationPanel.SetActive(false);
        _slotSelectionPanel.SetActive(true);

        if (_slotButtons.Length > 0 && _slotButtons[0] != null)
        {
            EventSystem.current.SetSelectedGameObject(_slotButtons[0].gameObject);
        }
    }


    private void RefreshSlotDisplay()
    {
        for (int i = 0; i < SaveManager.NUM_SLOTS; i++)
        {
            int slotID = i + 1;
            GameData data = SaveManager.Instance.PeekSaveData(slotID);
            UpdateSlotButtonDisplay(slotID, data);
        }
    }

    private void UpdateSlotButtonDisplay(int slotID, GameData data)
    {
        Button slotButton = _slotButtons[slotID - 1];
        TextMeshProUGUI buttonText = slotButton.GetComponentInChildren<TextMeshProUGUI>();

        _resetSlotButtons[slotID - 1].interactable = (data != null);


        if (data == null)
        {
            buttonText.text = $"SLOT {slotID}\n[NUOVO GIOCO]";
        }
        else
        {
            string progress = data.hasCompletedIntro ? "Corridoio" : "Inizio";
            if (data.hasDefeatedFirstBoss) progress = "Post-Colloquio";

            buttonText.text = $"SLOT {slotID}\nNome: {data.playerName}\nProgresso: {progress}";
        }
    }

    private void OnSlotSelected(int slotID)
    {
        _selectedSlotID = slotID;
        GameData data = SaveManager.Instance.PeekSaveData(slotID);

        if (data == null)
        {
            ShowNameInputPanel();
        }
        else
        {
            StartGame(slotID, data.playerName, false);
        }
    }

    private void OnResetSlot(int slotID)
    {
        SaveManager.Instance.ResetSave(slotID);
        RefreshSlotDisplay();
        CancelReset();
    }


    private void ShowNameInputPanel()
    {
        _slotSelectionPanel.SetActive(false);
        _resetConfirmationPanel.SetActive(false);
        _startScreenPanel.SetActive(false);
        _nameInputPanel.SetActive(true);
        if (_controllerTextInputScript != null)
        {
            _controllerTextInputScript.enabled = true;
        }
        else
        {
            Debug.LogError("ControllerTextInput script reference is missing in MainMenuController!");
        }
    }

    public void OnNameConfirmed()
    {
        ControllerTextInput inputManager = _controllerTextInputScript;

        string playerName = "";

        if (inputManager != null)
        {
            Debug.Log("--- Assembling Name ---"); 
            for (int i = 0; i < inputManager.CharSlots.Length; i++)
            {
                TMP_InputField slot = inputManager.CharSlots[i];
                if (slot != null)
                {
                    string slotText = slot.text;
                    string trimmedText = slotText.Trim();
                    Debug.Log($"Slot {i}: Text='{slotText}', Trimmed='{trimmedText}'");
                    playerName += trimmedText;
                }
                else
                {
                    Debug.LogWarning($"Slot {i} is null!");
                }
            }
            Debug.Log($"--- Assembled Name: '{playerName}' ---"); 
        }
        else
        {
            Debug.LogError("inputManager (ControllerTextInput) is NULL!"); 
        }

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("PlayerName is empty, defaulting to 'Senza Nome'."); 
            playerName = "Senza Nome";
        }


        StartGame(_selectedSlotID, playerName, true);
    }
    public void CancelNameInput() 
    {
        if (_controllerTextInputScript != null)
        {
            _controllerTextInputScript.enabled = false;
        }
        _nameInputPanel.SetActive(false);
        _slotSelectionPanel.SetActive(true); 
                                             
        if (_slotButtons.Length > 0 && _slotButtons[0] != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_slotButtons[0].gameObject);
        }
    }

    private void StartGame(int slotID, string playerName, bool isNewGame)
    {
        SaveManager.Instance.LoadGame(slotID, isNewGame);

        if (isNewGame)
        {
            SaveManager.Instance.SetPlayerName(playerName);
        }

        PlayerInputController playerInput = FindAnyObjectByType<PlayerInputController>();
        if (playerInput != null)
        {
            playerInput.enabled = true;
        }

        SceneManager.LoadScene(GameSceneName);
    }
}