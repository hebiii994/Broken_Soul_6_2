using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private DoorInteractor _nearbyDoor;
    private NarrativeManager _narrativeManager;
    private PlayerInputController _inputController;

    private void Awake()
    {
        _narrativeManager = FindAnyObjectByType<NarrativeManager>();
        _inputController = GetComponent<PlayerInputController>();
    }

    private void Update()
    {
        if (_inputController.InteractInput && _nearbyDoor != null)
        {
            _narrativeManager.MakeChoice(_nearbyDoor.choiceIndex);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<DoorInteractor>(out DoorInteractor door))
        {
            Debug.Log("Entrato nel trigger della porta: " + other.name);
            _nearbyDoor = door;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {

        if (_nearbyDoor != null && other.gameObject == _nearbyDoor.gameObject)
        {
            Debug.Log("Uscito dal trigger della porta: " + other.name);
            _nearbyDoor = null;
        }
    }
}