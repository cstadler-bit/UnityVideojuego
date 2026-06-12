using UnityEngine;

public class TriggerPopUpDirecto : MonoBehaviour
{
    [Header("UI Elemento")]
    [SerializeField] private GameObject panelPopUp;     // El Pop-up grande con el diálogo de la Tía Gladys
    [SerializeField] private GameObject signoExclamacion; // El gráfico flotante del signo (opcional)

    void Start()
    {
        // El juego arranca con el diálogo oculto
        if (panelPopUp != null) panelPopUp.SetActive(false);
        
        // El signo de exclamación arranca visible en el mapa esperando al jugador
        if (signoExclamacion != null) signoExclamacion.SetActive(true);
    }

    // Al pisar el collider del signo
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (panelPopUp != null) panelPopUp.SetActive(true); // Se abre el cartel de golpe
            
            // Si querés que el signo de exclamación desaparezca al abrir el diálogo, descomentá la línea de abajo:
            // if (signoExclamacion != null) signoExclamacion.SetActive(false);
            
            Debug.Log("El Player pisó el signo. Pop-up de Gladys activado.");
        }
    }

    // Al alejarse del collider (si querés que se cierre solo)
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (panelPopUp != null) panelPopUp.SetActive(false); // Se apaga al irse
            
            // Si habías apagado el signo y querés que vuelva a aparecer:
            // if (signoExclamacion != null) signoExclamacion.SetActive(true);
        }
    }
}