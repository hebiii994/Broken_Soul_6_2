using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AdvancedZDepthSorter
{
    // --- IMPOSTAZIONI ---
    private const string bgFarLayer = "BackgroundFar";
    private const string bgMidLayer = "BackgroundMid";
    private const string bgNearLayer = "BackgroundNear";
    private const string gameplayLayer = "Gameplay";
    private const string fgNearLayer = "ForegroundNear";
    private const string fgFarLayer = "ForegroundFar";


    private const float bgFarThreshold = 30.0f;     // Tutto ciò con Z > 30
    private const float bgMidThreshold = 15.0f;     // Tra Z = 30 e 15
    private const float bgNearThreshold = 5.0f;     // Tra Z = 15 e 5
    private const float gameplayThreshold = -5.0f;  // Tra Z = 5 e -5 (il piano di gioco)
    private const float fgNearThreshold = -15.0f;   // Tra Z = -5 e -15
    // Tutto ciò con Z < -15 sarà 'ForegroundFar'
    // --- FINE IMPOSTAZIONI ---


    [MenuItem("Tools/I MIEI TOOL/2. Ordina Sprite per Layer e Z-Depth")]
    private static void SortSpritesAdvanced()
    {
        SpriteRenderer[] allRenderers = EditorHelper.FindAllObjectsByType<SpriteRenderer>();
        if (allRenderers.Length == 0) return;


        foreach (SpriteRenderer renderer in allRenderers)
        {
            float z = renderer.transform.position.z;

            if (z > bgFarThreshold)
            {
                renderer.sortingLayerName = bgFarLayer;
            }
            else if (z > bgMidThreshold)
            {
                renderer.sortingLayerName = bgMidLayer;
            }
            else if (z > bgNearThreshold)
            {
                renderer.sortingLayerName = bgNearLayer;
            }
            else if (z > gameplayThreshold)
            {
                renderer.sortingLayerName = gameplayLayer;
            }
            else if (z > fgNearThreshold)
            {
                renderer.sortingLayerName = fgNearLayer;
            }
            else
            {
                renderer.sortingLayerName = fgFarLayer;
            }

            EditorUtility.SetDirty(renderer);
        }

        var groupedRenderers = allRenderers.GroupBy(r => r.sortingLayerName);


        foreach (var group in groupedRenderers)
        {
            List<SpriteRenderer> rendererList = group.ToList();


            rendererList.Sort((a, b) => b.transform.position.z.CompareTo(a.transform.position.z));


            for (int i = 0; i < rendererList.Count; i++)
            {
                rendererList[i].sortingOrder = i;
                EditorUtility.SetDirty(rendererList[i]);
            }
        }

        UnityEngine.Debug.Log($"[AdvancedZDepthSorter] Ordinati {allRenderers.Length} sprite in Layer e per Z-Depth.");
    }
}