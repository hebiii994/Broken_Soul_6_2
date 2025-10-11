using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraUtility : SingletonGeneric<CameraUtility>
{
    protected override bool ShouldBeDestroyOnLoad() => true;

    [Header("Riferimenti")]
    [SerializeField] private CinemachineCamera _virtualCamera;

    public IEnumerator ZoomCameraRoutine(float targetSize, float duration)
    {
        float startSize = _virtualCamera.Lens.OrthographicSize;
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            float normalizedTime = (Time.time - startTime) / duration;
            float easedT = EaseInOutCubic(normalizedTime);

            _virtualCamera.Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, easedT);

            yield return null;
        }

        _virtualCamera.Lens.OrthographicSize = targetSize;
    }


    private float EaseInOutCubic(float x)
    {
        return x < 0.5f ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }
}

