using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraUtility : SingletonGeneric<CameraUtility>
{
    protected override bool ShouldBeDestroyOnLoad() => true;

    [Header("Riferimenti")]
    [SerializeField] private CinemachineCamera _virtualCamera;

    public IEnumerator ZoomCameraRoutine(float targetFOV, float duration)
    {

        float startFOV = _virtualCamera.Lens.FieldOfView;
        Debug.Log($"[CameraUtility] ZOOM RICHIESTO: da {startFOV} a {targetFOV} in {duration} secondi.");
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            float normalizedTime = (Time.time - startTime) / duration;
            float easedT = EaseInOutCubic(normalizedTime);


            _virtualCamera.Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, easedT);


            yield return null;
        }


        _virtualCamera.Lens.FieldOfView = targetFOV;

    }


    private float EaseInOutCubic(float x)
    {
        return x < 0.5f ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }
}

