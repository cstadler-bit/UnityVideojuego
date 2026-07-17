using UnityEngine;
using System.Collections;

public class GestionMisionAbuelo : MonoBehaviour
{
    [Header("Configuración de la Misión")]
    [SerializeField] private string nombreSacoCorrecto = "sacomarron";

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

    private bool misionCompletada = false;
    private bool cartelFinalVisible = false;
    private bool mensajeYaVisto = false;

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

        misionCompletada = false;
        cartelFinalVisible = false;
        mensajeYaVisto = false;
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
        if (misionCompletada) return;

        if (other.CompareTag("Player"))
        {
            if (panelPopUpDialogo != null &&
                (cartelSacoEquivocado == null || !cartelSacoEquivocado.activeSelf))
            {
                panelPopUpDialogo.SetActive(true);
            }

            if (!mensajeYaVisto)
            {
                mensajeYaVisto = true;
                ApagarSignoExclamacion();
            }
        }

        bool esSacoReal = other.GetComponent<DraggableObject>() != null;

        if (esSacoReal && other.gameObject.name.ToLower().Contains(nombreSacoCorrecto.ToLower()))
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
        if (misionCompletada) return;

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
        if (misionCompletada) return;

        bool esSacoReal = other.GetComponent<DraggableObject>() != null;

        if (esSacoReal && other.gameObject.name.ToLower().Contains(nombreSacoCorrecto.ToLower()))
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
        if (misionCompletada) return;

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
        misionCompletada = true;

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
            Debug.LogWarning($"{gameObject.name}: falta asignar PersonajeCampera en el Inspector.");
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

        // No apagamos tildeCanvas porque el tick del menú superior debe quedar marcado.
    }

    private void ApagarSignoExclamacion()
    {
        if (signoExclamacion == null) return;

        SpriteRenderer[] renderizadores = signoExclamacion.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer renderizador in renderizadores)
        {
            renderizador.enabled = false;
        }

        UnityEngine.UI.Image[] imagenes = signoExclamacion.GetComponentsInChildren<UnityEngine.UI.Image>();

        foreach (UnityEngine.UI.Image imagen in imagenes)
        {
            imagen.enabled = false;
        }

        Collider2D[] colisionadores = signoExclamacion.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D colisionador in colisionadores)
        {
            colisionador.enabled = false;
        }

        // No usamos signoExclamacion.SetActive(false), porque puede apagar el objeto que contiene el script.
    }
}