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

    [Header("Nueva Interfaz (Error)")]
    [SerializeField] private GameObject cartelSacoEquivocado;

    [Header("Personaje que se retira")]
    [SerializeField] private PersonajeCampera personajeCampera;

    private bool misionesCompletada = false;
    private bool cartelFinalVisible = false;

    void Start()
    {
        if (panelPopUpDialogo != null)
            panelPopUpDialogo.SetActive(false);

        if (cartelMisionCumplida != null)
            cartelMisionCumplida.SetActive(false);

        if (cartelSacoEquivocado != null)
            cartelSacoEquivocado.SetActive(false);

        if (signoExclamacion != null)
            signoExclamacion.SetActive(true);

        if (tildeCanvas != null)
            tildeCanvas.SetActive(false);

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
            if (panelPopUpDialogo != null && 
                (cartelSacoEquivocado == null || !cartelSacoEquivocado.activeSelf))
            {
                panelPopUpDialogo.SetActive(true);
            }
        }

        // 🔧 FIX: antes solo se chequeaba el nombre ("sacopeludo"), así que CUALQUIER objeto con
        // ese texto en el nombre y un Collider2D disparaba la misión como cumplida, aunque no fuera
        // un saco real (por eso aparecía el cartel de "misión cumplida" antes de entregar nada).
        // Ahora también exigimos que tenga el componente DraggableObject, que es lo que identifica
        // a un saco de verdad dentro del juego.
        bool esSacoReal = other.GetComponent<DraggableObject>() != null;

        if (esSacoReal && other.gameObject.name.ToLower().Contains("sacopeludo"))
        {
            FinalizarMision(other.gameObject);
        }
        else if (esSacoReal)
        {
            MostrarErrorSacoEquivocado();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (misionesCompletada) return;

        if (other.CompareTag("Player"))
        {
            if (panelPopUpDialogo != null)
                panelPopUpDialogo.SetActive(false);

            if (cartelSacoEquivocado != null)
                cartelSacoEquivocado.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (misionesCompletada) return;

        bool esSacoReal = other.GetComponent<DraggableObject>() != null;

        if (esSacoReal && other.gameObject.name.ToLower().Contains("sacopeludo"))
        {
            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
            {
                FinalizarMision(other.gameObject);
            }
        }
        else if (esSacoReal)
        {
            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
            {
                MostrarErrorSacoEquivocado();
            }
        }
    }

    private void MostrarErrorSacoEquivocado()
    {
        if (misionesCompletada) return;

        if (panelPopUpDialogo != null)
            panelPopUpDialogo.SetActive(false);

        StopCoroutine("OcultarCartelError");
        StartCoroutine(OcultarCartelError());
    }

    IEnumerator OcultarCartelError()
    {
        if (cartelSacoEquivocado != null)
            cartelSacoEquivocado.SetActive(true);

        yield return new WaitForSeconds(2.5f);

        if (cartelSacoEquivocado != null)
            cartelSacoEquivocado.SetActive(false);
    }

    private void FinalizarMision(GameObject saco)
{
    misionesCompletada = true;

    StopCoroutine("OcultarCartelError");

    if (cartelSacoEquivocado != null)
        cartelSacoEquivocado.SetActive(false);

    ApagarSignoExclamacion();

    if (panelPopUpDialogo != null)
        panelPopUpDialogo.SetActive(false);

    if (saco != null)
        saco.SetActive(false);

    DetectorCamperaEquivocada scriptError = GetComponent<DetectorCamperaEquivocada>();

    if (scriptError != null)
        scriptError.enabled = false;

    if (personajeCampera != null)
    {
        personajeCampera.RecibirCamperaYRetirarse();
    }
    else
    {
        // 🔧 DIAGNÓSTICO Bug 2: si ves este warning, la referencia "Personaje Campera" quedó
        // sin asignar en el Inspector de este GameObject. Asignala y listo.
        Debug.LogWarning($"⚠️ {gameObject.name}: 'personajeCampera' no está asignado en el Inspector. " +
                          "La tía no se puede retirar.");
    }

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
        if (cartelMisionCumplida != null)
            cartelMisionCumplida.SetActive(true);

        if (tildeCanvas != null)
            tildeCanvas.SetActive(true);

        cartelFinalVisible = true;

        yield return new WaitForSeconds(3f);

        CerrarCartelFinal();
    }

   private void CerrarCartelFinal()
{
    StopAllCoroutines();

    cartelFinalVisible = false;

    if (cartelMisionCumplida != null)
        cartelMisionCumplida.SetActive(false);

    // NO apagamos tildeCanvas porque es el tick del menú superior
    // y tiene que quedar marcado cuando la misión ya fue completada.
}    private void ApagarSignoExclamacion()
{
    if (signoExclamacion == null) return;

    // Si el signo es un SpriteRenderer normal
    SpriteRenderer[] renderizadores = signoExclamacion.GetComponentsInChildren<SpriteRenderer>();

    foreach (SpriteRenderer renderizador in renderizadores)
    {
        renderizador.enabled = false;
    }

    // Si el signo es una imagen UI dentro de un Canvas
    UnityEngine.UI.Image[] imagenes = signoExclamacion.GetComponentsInChildren<UnityEngine.UI.Image>();

    foreach (UnityEngine.UI.Image imagen in imagenes)
    {
        imagen.enabled = false;
    }

    // NO hacemos signoExclamacion.SetActive(false)
    // porque este objeto también tiene el script de la misión.
}
}