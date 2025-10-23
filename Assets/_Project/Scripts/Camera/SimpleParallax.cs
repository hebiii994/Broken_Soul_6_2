using UnityEngine;

[ExecuteInEditMode]
public class StableParallax : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Range(0f, 2f)]
    public float parallaxEffectMultiplier = 0.5f;

    public bool applyToYAxis = false;
    public bool applyToZAxis = false;


    private Camera mainCamera;
    private Vector3 cameraStartPosition;
    private Vector3 layerStartPosition;

    //public Vector3 DebugcameraStartPosition;
    //public Vector3 DebuglayerStartPosition;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("StableParallax: Main Camera non trovata!", this);
            enabled = false;
            return;
        }
        cameraStartPosition = mainCamera.transform.position;
        layerStartPosition = transform.localPosition;
        //DebugcameraStartPosition = mainCamera.transform.position;
        //DebuglayerStartPosition = transform.localPosition;
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        Vector3 distanceMoved = mainCamera.transform.position - cameraStartPosition;

        float parallaxX = distanceMoved.x * parallaxEffectMultiplier;
        float parallaxY = applyToYAxis ? (distanceMoved.y * parallaxEffectMultiplier) : 0f;
        float parallaxZ = applyToZAxis ? (distanceMoved.z * parallaxEffectMultiplier) : 0f;


            transform.localPosition = new Vector3(
            layerStartPosition.x + parallaxX,
            layerStartPosition.y + parallaxY,
            layerStartPosition.z + parallaxZ
        );
    }

    //void EditorLateUpdate()
    //{
    //    if (mainCamera == null) return;

    //    Vector3 distanceMoved = mainCamera.transform.position - DebugcameraStartPosition;

    //    float parallaxX = distanceMoved.x * parallaxEffectMultiplier;
    //    float parallaxY = applyToYAxis ? (distanceMoved.y * parallaxEffectMultiplier) : 0f;
    //    float parallaxZ = applyToZAxis ? (distanceMoved.z * parallaxEffectMultiplier) : 0f;


    //    transform.localPosition = new Vector3(
    //    DebuglayerStartPosition.x + parallaxX,
    //    DebuglayerStartPosition.y + parallaxY,
    //    DebuglayerStartPosition.z + parallaxZ
    //);
    //}

    //private void LateUpdate()
    //{
    //    if (Application.IsPlaying(this))
    //    {
    //        GameLateUpdate();
    //    }
    //    else
    //    {
    //        EditorLateUpdate();
    //    }
    //}
}
