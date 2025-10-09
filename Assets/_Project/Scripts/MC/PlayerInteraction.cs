using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private DoorInteractor _nearbyDoor;
    private NarrativeManager _narrativeManager;
    private PlayerInputController _inputController;

    private void Awake()
    {
        _narrativeManager = FindFirstObjectByType<NarrativeManager>();
        _inputController = GetComponent<PlayerInputController>();
    }

    private void Update()
    {
        if (_inputController.InteractInput)
        {

            if (_nearbyDoor != null)
            {
                _narrativeManager.MakeChoice(_nearbyDoor.choiceIndex);
            }
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
        if (other.GetComponent<DoorInteractor>() == _nearbyDoor)
        {
            Debug.Log("Uscito dal trigger della porta: " + other.name);
            _nearbyDoor = null;
        }
    }
}