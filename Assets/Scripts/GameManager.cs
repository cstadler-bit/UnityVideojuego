using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Interfaz Final")]
    [SerializeField] private GameObject cartelJuegoFinalizado;
    [SerializeField] private string nombreEscenaMenu = "Menu";

    [Header("Sistema de Estrellas")]
    [SerializeField] private GameObject[] starIcons;

    [Header("Configuración de Nivel")]
    [SerializeField] private int totalMisiones = 3;
    [SerializeField] private float limiteTiempoSegundos = 240f; // 4 minutos

    private int misionesCompletadasCount = 0;
    private int personajesEnojadosCount = 0;
    private int personajesEnojoTotalCount = 0;

    private float startTime;
    private bool juegoGanado = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (cartelJuegoFinalizado != null)
            cartelJuegoFinalizado.SetActive(false);

        startTime = Time.time;

        personajesEnojadosCount = 0;
        personajesEnojoTotalCount = 0;
        misionesCompletadasCount = 0;
        juegoGanado = false;

        ApagarEstrellasUI();
    }

    void Update()
    {
        if (cartelJuegoFinalizado != null && cartelJuegoFinalizado.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SceneManager.LoadScene(nombreEscenaMenu);
            }
        }
    }

    public void RegistrarPersonajeEnojado()
    {
        personajesEnojadosCount++;

        if (personajesEnojadosCount > totalMisiones)
            personajesEnojadosCount = totalMisiones;

        Debug.Log("Personajes enojados por error de campera: " + personajesEnojadosCount);
    }

    public void RegistrarTiaEnojoTotal()
    {
        personajesEnojoTotalCount++;

        if (personajesEnojoTotalCount > totalMisiones)
            personajesEnojoTotalCount = totalMisiones;

        Debug.Log("Personajes que llegaron a enojo total: " + personajesEnojoTotalCount);
    }

    public void RegistrarMisionCumplida()
    {
        if (juegoGanado) return;

        misionesCompletadasCount++;

        Debug.Log("Misiones completadas: " + misionesCompletadasCount + " / " + totalMisiones);

        if (misionesCompletadasCount >= totalMisiones)
        {
            juegoGanado = true;
            StartCoroutine(GanarJuegoConDelay(3f));
        }
    }

    private IEnumerator GanarJuegoConDelay(float segundosDeEspera)
    {
        yield return new WaitForSeconds(segundosDeEspera);
        GanarJuego();
    }

    private void GanarJuego()
    {
        if (cartelJuegoFinalizado != null)
        {
            cartelJuegoFinalizado.SetActive(true);

            float tiempoTotalNivel = Time.time - startTime;
            int estrellasConseguidas = CalcularEstrellasFinales(tiempoTotalNivel);

            ActualizarEstrellasUI(estrellasConseguidas);

            Debug.Log("Juego finalizado. Tiempo total: " + tiempoTotalNivel);
            Debug.Log("Estrellas conseguidas: " + estrellasConseguidas);
        }
    }

    private int CalcularEstrellasFinales(float tiempoTranscurrido)
    {
        // Si tarda más del límite, queda en 1 estrella.
        if (tiempoTranscurrido > limiteTiempoSegundos)
        {
            return 1;
        }

        int estrellas = 5;

        // Resta 1 estrella por cada personaje que se enojó por recibir una campera incorrecta.
        estrellas -= personajesEnojadosCount;

        // Resta 1 estrella extra por cada personaje que llegó al enojo total por tiempo.
        estrellas -= personajesEnojoTotalCount;

        if (estrellas < 1)
            estrellas = 1;

        return estrellas;
    }

    private void ApagarEstrellasUI()
    {
        if (starIcons == null) return;

        for (int i = 0; i < starIcons.Length; i++)
        {
            if (starIcons[i] != null)
                starIcons[i].SetActive(false);
        }
    }

    private void ActualizarEstrellasUI(int cantidad)
    {
        if (starIcons == null) return;

        ApagarEstrellasUI();

        if (cantidad > starIcons.Length)
            cantidad = starIcons.Length;

        for (int i = 0; i < cantidad; i++)
        {
            if (starIcons[i] != null)
                starIcons[i].SetActive(true);
        }
    }
}

