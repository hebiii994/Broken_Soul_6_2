using UnityEngine;
using DG.Tweening;
using System.Collections;

public class ScreenFader : SingletonGeneric<ScreenFader>
{
    [Header("Riferimenti")]
    [SerializeField] private CanvasGroup _fadeCanvasGroup;


    protected override bool ShouldBeDestroyOnLoad() => false;

    protected override void Awake()
    {
        base.Awake();
        _fadeCanvasGroup.alpha = 0;
    }

    public Coroutine FadeOut(float duration)
    {
        return StartCoroutine(FadeRoutine(1f, duration));
    }


    public Coroutine FadeIn(float duration)
    {
        return StartCoroutine(FadeRoutine(0f, duration));
    }

    private IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        _fadeCanvasGroup.blocksRaycasts = true;


        yield return _fadeCanvasGroup.DOFade(targetAlpha, duration).WaitForCompletion();

        if (targetAlpha == 0)
        {
            _fadeCanvasGroup.blocksRaycasts = false;
        }
    }
}