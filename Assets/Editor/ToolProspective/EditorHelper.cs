using UnityEngine;

public static class EditorHelper
{

    public static T[] FindAllObjectsByType<T>(
        FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude,
        FindObjectsSortMode sortMode = FindObjectsSortMode.None) where T : Component
    {
        return UnityEngine.Object.FindObjectsByType<T>((UnityEngine.FindObjectsInactive)findObjectsInactive, (UnityEngine.FindObjectsSortMode)sortMode);
    }
}