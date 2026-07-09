using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Instancia única (Singleton) para que cualquier script lo encuentre fácil
    public static GameManager Instance { get; private set; }

    [Header("Interfaz Final")]
    [SerializeField] private GameObject cartelJuegoFinalizado; // El pop-up de fin de juego
    [SerializeField] private string nombreEscenaMenu = "Menu"; // Nombre exacto de la escena menú

    [Header("Sistema de Estrellas (¡NUEVO!)")]
    // Arrastrá acá tus 5 objetos de estrellas amarillas/rellenas del Canvas
    [SerializeField] private GameObject[] starIcons; 
    [SerializeField] private float limiteTiempoSegundos = 180f; // 3 minutos en segundos

    private int misionesCompletadasCount = 0;
    private const int TOTAL_MISIONES = 2; // Gladys + Rebeca

    // Variables internas para el cálculo
    private int personajesEnojadosCount = 0;
    private int tiasEnojoTotalCount = 0; // 🔥 NUEVO: tías que llegaron a los 70s sin recibir su saco
    private float startTime;

    void Awake()
    {
        // Configuramos el Singleton
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
        // El cartel arranca apagado sí o sí
        if (cartelJuegoFinalizado != null) cartelJuegoFinalizado.SetActive(false);

        // 🔥 Inicializamos los contadores de tiempo y errores
        startTime = Time.time;
        personajesEnojadosCount = 0;
        tiasEnojoTotalCount = 0; // 🔥 NUEVO
        misionesCompletadasCount = 0;

        // Limpiamos las estrellas de la pantalla al iniciar
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

    // 🔥 NUEVA FUNCIÓN: Llamala desde el script de error cuando un personaje se enoje
    public void RegistrarPersonajeEnojado()
    {
        personajesEnojadosCount++;
        if (personajesEnojadosCount > 2) personajesEnojadosCount = 2; // Tope por seguridad (son 2 tías)
        Debug.Log("Paciencia rota. Total personajes enojados: " + personajesEnojadosCount);
    }

    // 🔥 NUEVA FUNCIÓN: Llamala desde EvolucionGladys cuando la tía llega al enojo total (70s, B&N)
    public void RegistrarTiaEnojoTotal()
    {
        tiasEnojoTotalCount++;
        if (tiasEnojoTotalCount > 2) tiasEnojoTotalCount = 2; // Tope por seguridad (son 2 tías)
        Debug.Log("Enojo total alcanzado. Total tías en enojo completo: " + tiasEnojoTotalCount);
    }

    // Esta función la van a llamar los personajes al dar la campera correcta
    public void RegistrarMisionCumplida()
    {
        misionesCompletadasCount++;
        Debug.Log("Misiones completadas: " + misionesCompletadasCount + " / " + TOTAL_MISIONES);

        // 🏆 CONDICIÓN DE VICTORIA TOTAL
        if (misionesCompletadasCount >= TOTAL_MISIONES)
        {
            // 🔥 NUEVO: esperamos 3 segundos antes de mostrar la pantalla final,
            // para que se alcance a ver la animación/reacción de la última tía
            // recibiendo su campera, antes de que tape todo el cartel de victoria.
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
            Debug.Log("¡Felicidades! Completaste todas las misiones. Juego Finalizado.");
            
            // 📈 CALCULAMOS LAS ESTRELLAS EN BASE A TUS REGLAS
            float tiempoTotalNivel = Time.time - startTime;
            int estrellasConseguidas = CalcularEstrellasFinales(tiempoTotalNivel);

            // Activamos las estrellas calculadas en la interfaz
            ActualizarEstrellasUI(estrellasConseguidas);
        }
    }

    // 🧠 El motor de tus reglas de puntuación
    private int CalcularEstrellasFinales(float tiempoTranscurrido)
    {
        // 🚨 REGLA 1: "si encima tardas más de 3 minutos, tenes 1"
        if (tiempoTranscurrido > limiteTiempoSegundos)
        {
            return 1; 
        }

        // 🚨 REGLA 2: Si lo hizo en menos de 3 minutos, evalúa según el enojo
        int estrellas;
        if (personajesEnojadosCount == 0)
        {
            estrellas = 5; // "sin que se enoje alguno, tenes 5"
        }
        else if (personajesEnojadosCount == 1)
        {
            estrellas = 4; // "si una se enoja tenes 4"
        }
        else
        {
            estrellas = 3; // "si las dos se enojan 3"
        }

        // 🔥 REGLA 3 (NUEVA): -1 estrella extra por cada tía que llegó al enojo total (70s, B&N).
        // Se suma a la penalización de arriba (aunque sea la misma tía la que se confundió Y llegó
        // al enojo total, las dos restas se aplican).
        estrellas -= tiasEnojoTotalCount;

        // Piso de seguridad: nunca menos de 1 estrella si al menos llegaste a terminar el juego
        if (estrellas < 1) estrellas = 1;

        return estrellas;
    }

    // ---- MÉTODOS DE INTERFAZ ----

    private void ApagarEstrellasUI()
    {
        if (starIcons == null) return;
        for (int i = 0; i < starIcons.Length; i++)
        {
            if (starIcons[i] != null) starIcons[i].SetActive(false);
        }
    }

    private void ActualizarEstrellasUI(int cantidad)
    {
        if (starIcons == null) return;
        
        ApagarEstrellasUI(); // Limpieza previa por las dudas

        // Limite de seguridad
        if (cantidad > starIcons.Length) cantidad = starIcons.Length;

        // Encendemos tantas estrellas como correspondan de izquierda a derecha
        for (int i = 0; i < cantidad; i++)
        {
            if (starIcons[i] != null)
            {
                starIcons[i].SetActive(true);
            }
        }
        Debug.Log("Estrellas mostradas en el Canvas: " + cantidad);
    }
}