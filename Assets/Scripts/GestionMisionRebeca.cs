using UnityEngine;

public class GestionMisionNuevoPersonaje : MonoBehaviour
{
    [Header("Referencias de la Misión")]
    [SerializeField] private GameObject signoExclamacion;   
    [SerializeField] private GameObject panelPopUpDialogo;  
    
    [Header("Nueva Interfaz (Éxito)")]
    [SerializeField] private GameObject cartelMisionCumplida; 

    private bool misionesCompletada = false;
    private bool cartelFinalVisible = false;

    void Start()
    {
        if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);
        if (cartelMisionCumplida != null) cartelMisionCumplida.SetActive(false);
        if (signoExclamacion != null) signoExclamacion.SetActive(true);
        
        misionesCompletada = false;
        cartelFinalVisible = false;
    }

    void Update()
    {
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (misionesCompletada) return;

        if (other.CompareTag("Player"))
        {
            if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(true);
        }
        
        // 🟢 DETECCIÓN DEL SACO CORRECTO
        if (other.gameObject.name.ToLower().Contains("sacopeludo"))
        {
            FinalizarMision(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (misionesCompletada) return;

        if (other.CompareTag("Player"))
        {
            if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);
        }
    }

    private void FinalizarMision(GameObject saco)
    {
        misionesCompletada = true;
        
        // 1. Apagamos el signo y el diálogo base
        if (signoExclamacion != null) signoExclamacion.SetActive(false);
        if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);
        
        // 2. Destruimos o apagamos el saco del mapa
        if (saco != null) saco.SetActive(false);

        // 3. 🔥 TRUCO DE SEGURIDAD: Apagamos el script de error para que no moleste
        DetectorCamperaEquivocada scriptError = GetComponent<DetectorCamperaEquivocada>();
        if (scriptError != null) scriptError.enabled = false;

        // 4. 🔥 PRENDEMOS EL CARTEL DE ÉXITO
        if (cartelMisionCumplida != null) 
        {
            // Nos aseguramos de que su padre (si tuviera) esté prendido, o lo independizamos
            cartelMisionCumplida.SetActive(true);
            cartelFinalVisible = true; 
            Debug.Log("¡Cartel de éxito encendido con éxito!");
        }
    }

    private void CerrarCartelFinal()
    {
        cartelFinalVisible = false;
        if (cartelMisionCumplida != null)
        {
            cartelMisionCumplida.SetActive(false);
        }
    }
}