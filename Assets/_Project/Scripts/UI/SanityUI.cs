using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SanityUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider _sanitySlider;
    [SerializeField] private HealthSystem _playerHealthSystem;

    [Header("Visual Settings")]
    [SerializeField] private float _widthPerHealthPoint = 10f;
    [SerializeField] private float _animationDuration = 1f;

    private RectTransform _sliderRectTransform;
    private bool _isInitialized = false;

    private void Awake()
    {
        _sliderRectTransform = _sanitySlider.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (_playerHealthSystem != null) _playerHealthSystem.OnHealthChanged += UpdateUI;
    }

    private void OnDisable()
    {
        if (_playerHealthSystem != null) _playerHealthSystem.OnHealthChanged -= UpdateUI;
    }

    private void UpdateUI(int current, int max)
    {
        if (!_isInitialized)
        {
            _sliderRectTransform.sizeDelta = new Vector2(max * _widthPerHealthPoint, _sliderRectTransform.sizeDelta.y);
            _sanitySlider.maxValue = max;
            _sanitySlider.value = current;
            _isInitialized = true;
            return;
        }

        if (!Mathf.Approximately(_sanitySlider.maxValue, max))
        {
            float targetWidth = max * _widthPerHealthPoint;
            _sliderRectTransform.DOSizeDelta(new Vector2(targetWidth, _sliderRectTransform.sizeDelta.y), _animationDuration);
            DOTween.To(() => _sanitySlider.maxValue, x => _sanitySlider.maxValue = x, max, _animationDuration);
        }

        _sanitySlider.DOValue(current, _animationDuration);
    }
}