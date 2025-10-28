using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControllerTextInput : MonoBehaviour
{
    [Header("Riferimenti UI")]
    [Tooltip("I 10 campi di input (slot) del nome.")]
    [SerializeField] private TMP_InputField[] _charSlots = new TMP_InputField[10];
    [Tooltip("Il campo che mostra il carattere selezionabile (visore).")]
    [SerializeField] private TextMeshProUGUI _charSelectorDisplay;
    [SerializeField] private Button _confirmButton;

    [Header("Input Logico")]
    [Tooltip("Il set di caratteri scorribile.")]
    private readonly string _characterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._-";
    private const int COMMAND_COUNT = 2;
    private int _totalOptions;
    private PlayerControls _controls;
    private int _currentSlotIndex = 0;    
    private int _currentCharIndex = 0; 
    private bool _canNavigate = true;
    

    public TMP_InputField[] CharSlots => _charSlots;

    private void Awake()
    {
        _controls = new PlayerControls();
        _totalOptions = _characterSet.Length + COMMAND_COUNT;
    }

    private void Start()
    {
        foreach (var slot in _charSlots)
        {
            if (slot != null)
            {
                slot.text = ""; 
                slot.readOnly = true;
            }
        }
        UpdateCharSelectorDisplay();
    }

    private void OnEnable()
    {
        _controls.Player.Disable();
        _controls.UI.Enable();
        _controls.UI.Navigate.performed += HandleNavigation;
        _controls.UI.Submit.performed += HandleConfirmInput;

        SetSlotFocus(0);
    }

    private void OnDisable()
    {
        _controls.UI.Navigate.performed -= HandleNavigation;
        _controls.UI.Submit.performed -= HandleConfirmInput;
        _controls.UI.Disable();

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }


    private void HandleNavigation(InputAction.CallbackContext context)
    {
        if (!_canNavigate) return;
        Vector2 input = context.ReadValue<Vector2>();

        if (Mathf.Abs(input.x) > 0.5f)
        {
            int direction = (int)Mathf.Sign(input.x);
            int newIndex = Mathf.Clamp(_currentSlotIndex + direction, 0, _charSlots.Length - 1);

            if (newIndex != _currentSlotIndex)
            {
                SetSlotFocus(newIndex);
                StartCoroutine(DebounceRoutine());
            }
        }

        else if (Mathf.Abs(input.y) > 0.5f)
        {
            int direction = -(int)Mathf.Sign(input.y); 
            _currentCharIndex += direction;

            if (_currentCharIndex >= _totalOptions) _currentCharIndex = 0;
            if (_currentCharIndex < 0) _currentCharIndex = _totalOptions - 1;

            UpdateCharSelectorDisplay();
            StartCoroutine(DebounceRoutine());
        }
    }

    private void SetSlotFocus(int index)
    {
        _currentSlotIndex = index;
        if (_charSlots.Length > _currentSlotIndex && _charSlots[_currentSlotIndex] != null)
        {
            EventSystem.current.SetSelectedGameObject(_charSlots[_currentSlotIndex].gameObject);

            string currentSlotChar = _charSlots[_currentSlotIndex].text.Trim();
            if (!string.IsNullOrEmpty(currentSlotChar) && currentSlotChar.Length == 1)
            {
                _currentCharIndex = _characterSet.IndexOf(currentSlotChar[0]);
                if (_currentCharIndex == -1) 
                {
                    _currentCharIndex = 0; 
                    _charSlots[_currentSlotIndex].text = ""; 
                }
            }
            else
            {

                _currentCharIndex = 0;
                _charSlots[_currentSlotIndex].text = ""; 
            }
            UpdateCharSelectorDisplay();
        }
    }


    private void UpdateCharSelectorDisplay()
    {
        string textToDisplay = "";

        if (_currentCharIndex < _characterSet.Length)
        {
            textToDisplay = _characterSet.Substring(_currentCharIndex, 1);
        }
        else if (_currentCharIndex == _characterSet.Length)
        {
            textToDisplay = "DELETE";
        }
        else if (_currentCharIndex == _characterSet.Length + 1)
        {
            textToDisplay = "CONFIRM";
        }
        if (_charSelectorDisplay != null)
        {
            _charSelectorDisplay.text = textToDisplay;
            Debug.Log($"Updating Char Selector Display: Index={_currentCharIndex}, Text='{textToDisplay}'"); // Aggiungi log per debug
        }
    }

    private void HandleConfirmInput(InputAction.CallbackContext context)
    {
        if (_currentCharIndex == _characterSet.Length + 1) 
        {
            if (_confirmButton != null)
            {
                _confirmButton.onClick.Invoke();
            }
        }
        else if (_currentCharIndex == _characterSet.Length) 
        {
            if (_charSlots[_currentSlotIndex].text.Trim().Length > 0)
            {
                _charSlots[_currentSlotIndex].text = "";
                _currentCharIndex = 0; 
                UpdateCharSelectorDisplay();
            }
            else
            {
                SetSlotFocus(Mathf.Max(0, _currentSlotIndex - 1));

            }
        }
        else if (_currentCharIndex >= 0 && _currentCharIndex < _characterSet.Length) 
        {
            char selectedChar = _characterSet[_currentCharIndex];
            _charSlots[_currentSlotIndex].text = selectedChar.ToString();
            SetSlotFocus(Mathf.Min(_charSlots.Length - 1, _currentSlotIndex + 1));
        }
    }


    private System.Collections.IEnumerator DebounceRoutine()
    {
        _canNavigate = false;
        yield return new WaitForSeconds(0.2f);
        _canNavigate = true;
    }
}