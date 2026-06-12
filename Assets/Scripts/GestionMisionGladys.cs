using UnityEngine;

public class GestionMisionGladys : MonoBehaviour
{
    [Header("Referencias de la Misión")]
    [SerializeField] private GameObject signoExclamacion;   // El signo flotante del pasillo
    [SerializeField] private GameObject panelPopUpDialogo;  // El cartel viejo ("Necesito mi saco...")
    
    [Header("Nueva Interfaz")]
    [SerializeField] private GameObject cartelNuevoMisionCumplida; // Tu objeto "PopUpMISIONcumplida 1_0"

    private bool misionCompletada = false;
    private bool cartelFinalVisible = false;

    void Start()
    {
        // Nos aseguramos de que arranque todo en su estado inicial correcto
        if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);
        if (cartelNuevoMisionCumplida != null) cartelNuevoMisionCumplida.SetActive(false);
        if (signoExclamacion != null) signoExclamacion.SetActive(true);
        
        misionCompletada = false;
        cartelFinalVisible = false;
    }

    void Update()
    {
        // 🚨 SI EL CARTEL ESTÁ VISIBLE: No importa nada más, el Enter O EL CLIC lo tienen que apagar
        if (cartelFinalVisible)
        {
            // Forzamos la lectura en crudo de Unity para Enter (tanto normal como teclado numérico), Espacio o Clic
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
        if (!misionCompletada && other.CompareTag("Player"))
        {
            if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (misionCompletada) return;

        if (other.gameObject.name.Contains("sacoamarillo"))
        {
            // Respaldo por si se suelta justo adentro
            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
            {
                FinalizarMision(other.gameObject);
            }
        }
    }

    private void FinalizarMision(GameObject saco)
    {
        // Activamos los estados ANTES de apagar objetos para que Unity no se maree
        misionCompletada = true;
        cartelFinalVisible = true; 

        Debug.Log("¡Misión completada! Activando PopUpMISIONcumplida 1_0");

        // 1. Borramos el signo de exclamación
        if (signoExclamacion != null) signoExclamacion.SetActive(false);

        // 2. Apagamos el pop-up viejo
        if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);
        
        // 3. Apagamos el saco amarillo de la escena
        if (saco != null) saco.SetActive(false);

        // 4. Encendemos el cartel nuevo
        if (cartelNuevoMisionCumplida != null)
        {
            cartelNuevoMisionCumplida.SetActive(true);
        }
    }

    private void CerrarCartelFinal()
    {
        cartelFinalVisible = false;
        
        if (cartelNuevoMisionCumplida != null)
        {
            cartelNuevoMisionCumplida.SetActive(false);
            Debug.Log("¡Cartel congelado desatascado y cerrado!");
        }
    }
}