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

    private void Awake()
    {
        _sliderRectTransform = _nostalgiaSlider.GetComponent<RectTransform>();
        _nostalgiaSlider.value = 0;
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
        if (!Mathf.Approximately(_nostalgiaSlider.maxValue, max)) 
        {
            float targetWidth = max * _widthPerNostalgiaPoint; 
            _sliderRectTransform.DOSizeDelta(new Vector2(targetWidth, _sliderRectTransform.sizeDelta.y), _animationDuration);


            _nostalgiaSlider.maxValue = max; 
        }


        _nostalgiaSlider.DOValue(current, _animationDuration);
    }
}