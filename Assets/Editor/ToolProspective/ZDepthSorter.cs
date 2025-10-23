using UnityEngine;
using UnityEditor; 
using System.Collections.Generic;
using System.Linq;

public class ZDepthSorter
{
    [MenuItem("Tools/I MIEI TOOL/1. Ordina Sprite per Z-Depth")]
    private static void SortSprites()
    {

        SpriteRenderer[] renderers = EditorHelper.FindAllObjectsByType<SpriteRenderer>();
        if (renderers.Length == 0)
        {
            Debug.Log("Nessun SpriteRenderer trovato nella scena.");
            return;
        }

        List<SpriteRenderer> rendererList = renderers.ToList();


        rendererList.Sort((a, b) => b.transform.position.z.CompareTo(a.transform.position.z));


        for (int i = 0; i < rendererList.Count; i++)
        {
            rendererList[i].sortingOrder = i;


            EditorUtility.SetDirty(rendererList[i]);
        }

        Debug.Log($"[ZDepthSorter] Ordinati {rendererList.Count} sprite in base alla Z-Depth.");
    }
}