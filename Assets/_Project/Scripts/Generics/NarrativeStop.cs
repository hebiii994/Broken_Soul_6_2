using UnityEngine;
using UnityEngine.Tilemaps;

public class NarrativeStop : MonoBehaviour
{
    [SerializeField] private NarrativeManager _narrativeManager;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider2D;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _spriteRenderer.enabled = false;
        _boxCollider2D.enabled = false;
    }
    public void Update()
    {
        if (_narrativeManager.IntroCompleted)
        {
            _spriteRenderer.enabled = true;
            _boxCollider2D.enabled = true;

        }
    }
}
