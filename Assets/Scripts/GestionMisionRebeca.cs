using UnityEngine;
using System.Collections;

public class GestionMisionNuevoPersonaje : MonoBehaviour
{
    [Header("Referencias de la Misión")]
    [SerializeField] private GameObject signoExclamacion;   
    [SerializeField] private GameObject panelPopUpDialogo;  
    
    [Header("Nueva Interfaz (Éxito)")]
    [SerializeField] private GameObject cartelMisionCumplida; 
    [SerializeField] private GameObject tildeCanvas; 

    private bool misionesCompletada = false;
    private bool cartelFinalVisible = false;

    void Start()
    {
        if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);
        if (cartelMisionCumplida != null) cartelMisionCumplida.SetActive(false);
        if (signoExclamacion != null) signoExclamacion.SetActive(true);
        if (tildeCanvas != null) tildeCanvas.SetActive(false); 
        
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
        
        // 1. 🔥 CORREGIDO: En vez de hacer SetActive(false), solo apagamos lo visual y el colisionador.
        // Esto evita que el script muera antes de que termine la corutina de 3 segundos.
        if (signoExclamacion != null) 
        {
            SpriteRenderer renderizador = signoExclamacion.GetComponent<SpriteRenderer>();
            Collider2D colisionador = signoExclamacion.GetComponent<Collider2D>();
            
            if (renderizador != null) renderizador.enabled = false;
            if (colisionador != null) colisionador.enabled = false;
        }

        if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);
        if (saco != null) saco.SetActive(false);

        DetectorCamperaEquivocada scriptError = GetComponent<DetectorCamperaEquivocada>();
        if (scriptError != null) scriptError.enabled = false;

        // Arrancamos el conteo de 3 segundos con el script totalmente vivo
        StartCoroutine(MostrarCartelPorTiempo());

        EvolucionGladys evo = GetComponent<EvolucionGladys>();
        if (evo != null) 
        {
            evo.ForzarCaraFeliz();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegistrarMisionCumplida();
        }
    }

    IEnumerator MostrarCartelPorTiempo()
    {
        if (cartelMisionCumplida != null) cartelMisionCumplida.SetActive(true);
        if (tildeCanvas != null) tildeCanvas.SetActive(true); 
        
        cartelFinalVisible = true; 

        yield return new WaitForSeconds(3f);

        CerrarCartelFinal();
    }

    private void CerrarCartelFinal()
    {
        StopAllCoroutines(); 
        
        cartelFinalVisible = false;
        if (cartelMisionCumplida != null)
        {
            cartelMisionCumplida.SetActive(false);
        }

        // 🔥 OPCIONAL: Ahora que todo terminó y el cartel se cerró, 
        // desactivamos por completo este GameObject para limpiar la escena.
        gameObject.SetActive(false);
    }
}