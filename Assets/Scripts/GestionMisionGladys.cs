using UnityEngine;
using System.Collections;

public class GestionMisionGladys : MonoBehaviour
{
    [Header("Referencias de la Misión")]
    [SerializeField] private GameObject signoExclamacion;   
    [SerializeField] private GameObject panelPopUpDialogo;  // El cartel viejo ("Necesito mi saco...")
    
    [Header("Nueva Interfaz (Éxito)")]
    [SerializeField] private GameObject cartelNuevoMisionCumplida; 
    [SerializeField] private GameObject tildeCanvas;               

    [Header("Nueva Interfaz (Error)")]
    // 🔥 NUEVO: Arrastrá acá tu pop-up de "Ese no es mi saco"
    [SerializeField] private GameObject cartelSacoEquivocado; 

    [Header("Animación y Movimiento Final")]
    [SerializeField] private Animator animatorTia;           
    [SerializeField] private float velocidadCaminata = 3f;    
    [SerializeField] private Vector3 direccionSalida = new Vector3(1f, 0f, 0f); 

    private bool misionCompletada = false;
    private bool cartelFinalVisible = false;
    private bool tiaCaminando = false; 

    void Start()
    {
        if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);
        if (cartelNuevoMisionCumplida != null) cartelNuevoMisionCumplida.SetActive(false);
        if (cartelSacoEquivocado != null) cartelSacoEquivocado.SetActive(false); // 🔥 Arranca apagado
        if (signoExclamacion != null) signoExclamacion.SetActive(true); 
        if (tildeCanvas != null) tildeCanvas.SetActive(false); 
        
        misionCompletada = false;
        cartelFinalVisible = false;
        tiaCaminando = false;
    }

    void Update()
    {
        if (tiaCaminando)
        {
            transform.Translate(direccionSalida * velocidadCaminata * Time.deltaTime);
        }

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
        if (misionCompletada) return;

        if (other.CompareTag("Player"))
        {
            // Solo abrimos el cartel de pedido normal si NO está el cartel de error activo
            if (panelPopUpDialogo != null && !cartelSacoEquivocado.activeSelf) 
                panelPopUpDialogo.SetActive(true);
        }
        
        // 🟡 CHEQUEO DE ENTREGAS:
        if (other.gameObject.name.Contains("sacoamarillo"))
        {
            FinalizarMision(other.gameObject);
        }
        // 🔥 Si es una campera (DraggableObject) pero NO es el saco amarillo
        else if (other.GetComponent<DraggableObject>() != null)
        {
            MostrarErrorSacoEquivocado();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (misionCompletada) return;

        if (other.CompareTag("Player"))
        {
            if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);
            // Si el jugador se aleja, también hacemos desaparecer el cartel de error
            if (cartelSacoEquivocado != null) cartelSacoEquivocado.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (misionCompletada) return;

        // Si suelta el saco correcto adentro
        if (other.gameObject.name.Contains("sacoamarillo"))
        {
            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
            {
                FinalizarMision(other.gameObject);
            }
        }
        // 🔥 Si suelta un saco EQUIVOCADO adentro
        else if (other.GetComponent<DraggableObject>() != null)
        {
            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
            {
                MostrarErrorSacoEquivocado();
            }
        }
    }

    // ✨ FUNCIÓN DE ERROR: Controla el cruce de carteles
    private void MostrarErrorSacoEquivocado()
    {
        if (misionCompletada) return;

        // Apagamos el texto flotante normal para que no se pisen
        if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);

        // Reiniciamos la corutina por si el jugador tira dos camperas mal seguidas
        StopCoroutine("OcultarCartelError");
        StartCoroutine(OcultarCartelError());
    }

    // ✨ CORUTINA DE TIEMPO PARA EL ERROR
    IEnumerator OcultarCartelError()
    {
        if (cartelSacoEquivocado != null) cartelSacoEquivocado.SetActive(true);

        // Espera 2.5 segundos en pantalla y se apaga solo
        yield return new WaitForSeconds(2.5f);

        if (cartelSacoEquivocado != null) cartelSacoEquivocado.SetActive(false);
    }

    private void FinalizarMision(GameObject saco)
    {
        misionCompletada = true;

        // Por seguridad, si el cartel de error estaba prendido, lo fulminamos
        StopCoroutine("OcultarCartelError");
        if (cartelSacoEquivocado != null) cartelSacoEquivocado.SetActive(false);

        if (signoExclamacion != null)
        {
            SpriteRenderer renderSigno = signoExclamacion.GetComponent<SpriteRenderer>();
            Collider2D colSigno = signoExclamacion.GetComponent<Collider2D>();
            if (renderSigno != null) renderSigno.enabled = false;
            if (colSigno != null) colSigno.enabled = false;
        }

        if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);
        if (saco != null) saco.SetActive(false);

        if (animatorTia != null) animatorTia.SetBool("estaCaminando", true);
        tiaCaminando = true; 

        StartCoroutine(MostrarCartelPorTiempo());

        EvolucionGladys evo = GetComponent<EvolucionGladys>();
        if (evo != null) evo.ForzarCaraFeliz();

        if (GameManager.Instance != null) GameManager.Instance.RegistrarMisionCumplida();
    }

    IEnumerator MostrarCartelPorTiempo()
    {
        if (cartelNuevoMisionCumplida != null) cartelNuevoMisionCumplida.SetActive(true);
        if (tildeCanvas != null) tildeCanvas.SetActive(true); 
        
        cartelFinalVisible = true;
        yield return new WaitForSeconds(3f);
        CerrarCartelFinal();
    }

    private void CerrarCartelFinal()
    {
        StopAllCoroutines(); 
        cartelFinalVisible = false;
        tiaCaminando = false; 
        
        if (cartelNuevoMisionCumplida != null) cartelNuevoMisionCumplida.SetActive(false);
        gameObject.SetActive(false); 
    }
}