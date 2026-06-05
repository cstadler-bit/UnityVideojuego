using UnityEngine;

public class DarknessTrigger : MonoBehaviour
{
    [Header("Referencias de Luces")]
    [SerializeField] private UnityEngine.Rendering.Universal.Light2D globalLight;   // La luz general (foco de la escena)
    [SerializeField] private UnityEngine.Rendering.Universal.Light2D playerFlashlight; // La linterna (hija del Player)

    [Header("Configuración de Intensidad")]
    [SerializeField] private float darkIntensity = 0.05f; // Qué tan oscuro se pone adentro (0 es negro total)
    [SerializeField] private float normalIntensity = 1f;   // La luz normal de afuera (suele ser 1)

    // 1. Cuando el personaje ENTRA a la imagen: se oscurece todo y se prende la linterna
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (globalLight != null) globalLight.intensity = darkIntensity;
            if (playerFlashlight != null) playerFlashlight.gameObject.SetActive(true);
            
            Debug.Log("Entró a la zona oscura. Linterna ON.");
        }
    }

    // 2. Cuando el personaje SALE de la imagen: todo vuelve a la normalidad
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (globalLight != null) globalLight.intensity = normalIntensity;
            if (playerFlashlight != null) playerFlashlight.gameObject.SetActive(false);
            
            Debug.Log("Salió de la zona oscura. Luz normal ON.");
        }
    }
}