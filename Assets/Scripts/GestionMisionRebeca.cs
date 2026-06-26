using UnityEngine;
using System.Collections;

public class GestionMisionNuevoPersonaje : MonoBehaviour
{
    [Header("Referencias de la Misión")]
    [SerializeField] private GameObject signoExclamacion;   
    [SerializeField] private GameObject panelPopUpDialogo;  // El cartel viejo ("Necesito mi saco...")
    
    [Header("Nueva Interfaz (Éxito)")]
    [SerializeField] private GameObject cartelMisionCumplida; 
    [SerializeField] private GameObject tildeCanvas; 

    [Header("Nueva Interfaz (Error) (¡NUEVO!)")]
    // 🔥 Arrastrá acá tu pop-up de "Ese no es mi saco" para este personaje
    [SerializeField] private GameObject cartelSacoEquivocado; 

    [Header("Animación y Movimiento Final (¡NUEVO!)")]
    [SerializeField] private Animator animatorPersonaje;     // El Animator del personaje (Rebeca/Tía)
    [SerializeField] private float velocidadCaminata = 3f;    // Qué tan rápido se va a ir caminando
    [SerializeField] private Vector3 direccionSalida = new Vector3(1f, 0f, 0f); // (1,0,0) camina hacia la derecha

    private bool misionesCompletada = false;
    private bool cartelFinalVisible = false;
    private bool estaCaminando = false; // Nos avisa si el personaje debe estarse moviendo ahora mismo

    void Start()
    {
        // Nos aseguramos de que arranque todo en su estado inicial correcto
        if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);
        if (cartelMisionCumplida != null) cartelMisionCumplida.SetActive(false);
        if (cartelSacoEquivocado != null) cartelSacoEquivocado.SetActive(false); // 🔥 Arranca apagado
        if (signoExclamacion != null) signoExclamacion.SetActive(true);
        if (tildeCanvas != null) tildeCanvas.SetActive(false); 
        
        misionesCompletada = false;
        cartelFinalVisible = false;
        estaCaminando = false;
    }

    void Update()
    {
        // 🏃‍♂️ MOVIMIENTO FISICO: Si debe caminar, movemos el transform físicamente
        if (estaCaminando)
        {
            transform.Translate(direccionSalida * velocidadCaminata * Time.deltaTime);
        }

        // Cierre manual del cartel de éxito
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
            // Solo abrimos el cartel de pedido normal si NO está el cartel de error activo
            if (panelPopUpDialogo != null && !cartelSacoEquivocado.activeSelf) 
                panelPopUpDialogo.SetActive(true);
        }
        
        // 🟡 CHEQUEO DE ENTREGAS:
        if (other.gameObject.name.ToLower().Contains("sacopeludo"))
        {
            FinalizarMision(other.gameObject);
        }
        // 🔥 Si es una campera (DraggableObject) pero NO es el "sacopeludo"
        else if (other.GetComponent<DraggableObject>() != null)
        {
            MostrarErrorSacoEquivocado();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (misionesCompletada) return;

        if (other.CompareTag("Player"))
        {
            if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);
            // Si el jugador se aleja, también hacemos desaparecer el cartel de error si estaba visible
            if (cartelSacoEquivocado != null) cartelSacoEquivocado.SetActive(false);
        }
    }

    // ✨ ¡NUEVO!: Crucial para cuando suelta un ítem adentro del trigger
    private void OnTriggerStay2D(Collider2D other)
    {
        if (misionesCompletada) return;

        // Si suelta el saco correcto adentro
        if (other.gameObject.name.ToLower().Contains("sacopeludo"))
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

    // ✨ FUNCIÓN DE ERROR (¡NUEVO!): Controla el cruce de carteles
    private void MostrarErrorSacoEquivocado()
    {
        if (misionesCompletada) return;

        // Apagamos el texto flotante normal para que no se pisen
        if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);

        // Reiniciamos la corutina por si el jugador tira dos camperas mal seguidas
        StopCoroutine("OcultarCartelError");
        StartCoroutine(OcultarCartelError());
    }

    // ✨ CORUTINA DE TIEMPO PARA EL ERROR (¡NUEVO!)
    IEnumerator OcultarCartelError()
    {
        if (cartelSacoEquivocado != null) cartelSacoEquivocado.SetActive(true);

        // Espera 2.5 segundos en pantalla y se apaga solo
        yield return new WaitForSeconds(2.5f);

        if (cartelSacoEquivocado != null) cartelSacoEquivocado.SetActive(false);
    }

    private void FinalizarMision(GameObject saco)
    {
        misionesCompletada = true;

        // 🔥 Por seguridad, si el cartel de error estaba prendido, lo fulminamos
        StopCoroutine("OcultarCartelError");
        if (cartelSacoEquivocado != null) cartelSacoEquivocado.SetActive(false);
        
        // 1. 🔥 CORREGIDO (Como antes): Solo apagamos lo visual y el colisionador.
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

        // 🔥 2. ACTIVAMOS LA ANIMACIÓN Y EL DESPLAZAMIENTO FÍSICO
        if (animatorPersonaje != null)
        {
            // Activamos tu parámetro del Animator (ej: un Bool "estaCaminando")
            animatorPersonaje.SetBool("estaCaminando", true); 
        }
        estaCaminando = true; // Habilita la traslación física en el Update

        // Arrancamos el conteo de 3 segundos del cartel de éxito con el script totalmente vivo
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

        // Espera los 3 segundos reales de exposición
        yield return new WaitForSeconds(3f);

        CerrarCartelFinal();
    }

    private void CerrarCartelFinal()
    {
        StopAllCoroutines(); 
        
        cartelFinalVisible = false;
        estaCaminando = false; // Frenamos el movimiento por seguridad

        if (cartelMisionCumplida != null)
        {
            cartelMisionCumplida.SetActive(false);
        }

        // 🔥 Ahora que todo terminó (cartel cerrado tras 3 segundos de caminata), 
        // desactivamos por completo este GameObject para simular que ya se fue.
        gameObject.SetActive(false);
    }
}