using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NostalgiaUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider _nostalgiaSlider;
    [SerializeField] private NostalgiaSystem _playerNostalgiaSystem;

    [Header("Visual Settings")]
    [SerializeField] private float _widthPerNostalgiaPoint = 10f;
    [SerializeField] private float _animationDuration = 1f;

    private RectTransform _sliderRectTransform;
    private bool _isInitialized = false;

    private void Awake()
    {
        _sliderRectTransform = _nostalgiaSlider.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (_playerNostalgiaSystem != null) _playerNostalgiaSystem.OnNostalgiaChanged += UpdateUI;
    }

    private void OnDisable()
    {
        if (_playerNostalgiaSystem != null) _playerNostalgiaSystem.OnNostalgiaChanged -= UpdateUI;
    }

    private void UpdateUI(int current, int max)
    {
        if (!_isInitialized)
        {
            _sliderRectTransform.sizeDelta = new Vector2(max * _widthPerNostalgiaPoint, _sliderRectTransform.sizeDelta.y);
            _nostalgiaSlider.maxValue = max;
            _nostalgiaSlider.value = current;
            _isInitialized = true;
            return;
        }

        if (!Mathf.Approximately(_nostalgiaSlider.maxValue, max))
        {
            float targetWidth = max * _widthPerNostalgiaPoint;
            _sliderRectTransform.DOSizeDelta(new Vector2(targetWidth, _sliderRectTransform.sizeDelta.y), _animationDuration);
            DOTween.To(() => _nostalgiaSlider.maxValue, x => _nostalgiaSlider.maxValue = x, max, _animationDuration);
        }

        _nostalgiaSlider.DOValue(current, _animationDuration);
    }
}