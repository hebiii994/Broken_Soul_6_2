using UnityEngine;

public class PortalController : MonoBehaviour
{
    public GameObject playerObject;

    [Tooltip("Controlla l'intensità dell'effetto di profondità. Valori consigliati: tra -0.05 e -0.2")]
    public float parallaxIntensity = -0.1f;

    private Material portalMaterial;
    private NostalgiaSystem nostalgiaSystem;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {

            portalMaterial = renderer.material;
        }
        else
        {
            Debug.LogError("Nessun Renderer trovato sull'oggetto Portale!", this);
            return;
        }

        if (playerObject != null)
        {
            nostalgiaSystem = playerObject.GetComponent<NostalgiaSystem>();
        }

        if (nostalgiaSystem == null)
        {
            Debug.LogError("Nessun componente NostalgiaSystem trovato sull'oggetto giocatore!", this);
        }
    }

    void Update()
    {
        if (playerObject == null || portalMaterial == null || nostalgiaSystem == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, playerObject.transform.position);
        float normalizedNostalgia = (float)nostalgiaSystem.CurrentNostalgia / (float)nostalgiaSystem.MaxNostalgia;

        portalMaterial.SetFloat("_PlayerDistance", distance);
        portalMaterial.SetFloat("_NostalgiaLevel", normalizedNostalgia);

        Vector3 playerDirection = playerObject.transform.position - transform.position;

        Vector2 parallaxOffset = new Vector2(playerDirection.x, playerDirection.y) * parallaxIntensity;

        portalMaterial.SetVector("_ParallaxOffset", parallaxOffset);
    }
}