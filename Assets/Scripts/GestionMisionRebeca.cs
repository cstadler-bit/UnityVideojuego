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

        if (other.gameObject.name.ToLower().Contains("sacopeludo"))
        {
            FinalizarMision(other.gameObject);
        }
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
            if (panelPopUpDialogo != null)
                panelPopUpDialogo.SetActive(false);

            if (cartelSacoEquivocado != null)
                cartelSacoEquivocado.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (misionesCompletada) return;

        if (other.gameObject.name.ToLower().Contains("sacopeludo"))
        {
            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
            {
                FinalizarMision(other.gameObject);
            }
        }
        else if (other.GetComponent<DraggableObject>() != null)
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