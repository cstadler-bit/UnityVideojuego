using UnityEngine;
using System.Collections; // 🔥 Agregado para poder usar la corutina de tiempo (IEnumerator)

public class GestionMisionGladys : MonoBehaviour
{
    [Header("Referencias de la Misión")]
    [SerializeField] private GameObject signoExclamacion;   // El signo flotante del pasillo
    [SerializeField] private GameObject panelPopUpDialogo;  // El cartel viejo ("Necesito mi saco...")
    
    [Header("Nueva Interfaz")]
    [SerializeField] private GameObject cartelNuevoMisionCumplida; // Tu objeto "PopUpMISIONcumplida 1_0"
    [SerializeField] private GameObject tildeCanvas;               // 🔥 NUEVO: Arrastrá acá el tilde verde de tu Canvas

    private bool misionCompletada = false;
    private bool cartelFinalVisible = false;

    void Start()
    {
        // Nos aseguramos de que arranque todo en su estado inicial correcto
        if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);
        if (cartelNuevoMisionCumplida != null) cartelNuevoMisionCumplida.SetActive(false);
        if (signoExclamacion != null) signoExclamacion.SetActive(true);
        if (tildeCanvas != null) tildeCanvas.SetActive(false); // 🔥 El tilde arranca oculto
        
        misionCompletada = false;
        cartelFinalVisible = false;
    }

    void Update()
    {
        // 🚨 SI EL CARTEL ESTÁ VISIBLE: El Enter O EL CLIC lo pueden apagar antes de tiempo
        if (cartelFinalVisible)
        {
            if (Input.GetKeyDown(KeyCode.Return) || 
                Input.GetKeyDown(KeyCode.KeypadEnter) || 
                Input.GetKeyDown(KeyCode.Space) || 
                Input.GetMouseButtonDown(0))
            {
                CerrarCartelFinal();
            }
        }
    }

    // 1. DETECTOR DE PASO (Abre y cierra el diálogo viejo al caminar)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (misionCompletada) return;

        if (other.CompareTag("Player"))
        {
            if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(true);
        }
        
        // Si lo que entra es el saco amarillo, completamos de una
        if (other.gameObject.name.Contains("sacoamarillo"))
        {
            FinalizarMision(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (misionCompletada) return;

        if (other.CompareTag("Player"))
        {
            if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (misionCompletada) return;

        if (other.gameObject.name.Contains("sacoamarillo"))
        {
            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
            {
                FinalizarMision(other.gameObject);
            }
        }
    }

    private void FinalizarMision(GameObject saco)
    {
        misionCompletada = true;

        Debug.Log("¡Misión completada! Activando PopUpMISIONcumplida y Tilde de Gladys");

        // 1. Borramos el signo de exclamación
        if (signoExclamacion != null) signoExclamacion.SetActive(false);

        // 2. Apagamos el pop-up viejo
        if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);
        
        // 3. Apagamos el saco amarillo de la escena
        if (saco != null) saco.SetActive(false);

        // 4. 🔥 MODIFICADO: Lanzamos la corutina para manejar el cartel por tiempo y prender el tilde
        StartCoroutine(MostrarCartelPorTiempo());

        // 5. 😊 EVOLUCIÓN: Buscamos el script único en Gladys y lo ponemos feliz
        EvolucionGladys evo = GetComponent<EvolucionGladys>();
        if (evo != null) 
        {
            evo.ForzarCaraFeliz();
        }
        else
        {
            Debug.LogWarning("No se encontró el componente de Evolución (EvolucionGladys) en Gladys.");
        }

        // 📢 GameManager central: Suma la misión cumplida
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegistrarMisionCumplida();
        }
        else
        {
            Debug.LogWarning("¡Ojo! No se encontró el GameManager en la escena.");
        }
    }

    // ✨ NUEVA CORUTINA: Controla de forma asíncrona los 3 segundos de exposición del cartel
    IEnumerator MostrarCartelPorTiempo()
    {
        if (cartelNuevoMisionCumplida != null) cartelNuevoMisionCumplida.SetActive(true);
        if (tildeCanvas != null) tildeCanvas.SetActive(true); // 🔥 Se prende tu tilde en el Canvas
        
        cartelFinalVisible = true;

        // Espera exactamente 3 segundos reales de juego
        yield return new WaitForSeconds(3f);

        CerrarCartelFinal();
    }

    private void CerrarCartelFinal()
    {
        StopAllCoroutines(); // Frena el contador por si el jugador presionó una tecla antes de los 3 segundos
        cartelFinalVisible = false;
        
        if (cartelNuevoMisionCumplida != null)
        {
            cartelNuevoMisionCumplida.SetActive(false);
            Debug.Log("¡Cartel de Gladys cerrado de forma limpia!");
        }
        // Nota: El tildeCanvas NO se apaga para que quede marcado en el HUD de la UI.
    }
}