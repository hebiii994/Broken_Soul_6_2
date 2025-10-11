using UnityEngine;
using UnityEngine.Tilemaps;

public class NarrativeStop : MonoBehaviour
{
    [SerializeField] private NarrativeManager _narrativeManager;
    private TilemapRenderer _tilemapRenderer;
    private TilemapCollider2D _tilemapCollider2D;

    private void Awake()
    {
        _tilemapRenderer = GetComponent<TilemapRenderer>();
        _tilemapCollider2D = GetComponent<TilemapCollider2D>();
        _tilemapRenderer.enabled = false;
        _tilemapCollider2D.enabled = false;
    }
    public void Update()
    {
        if (_narrativeManager.IntroCompleted)
        {
            _tilemapRenderer.enabled = true;
            _tilemapCollider2D.enabled = true;

        }
    }
}
