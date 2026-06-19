using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Instancia única (Singleton) para que cualquier script lo encuentre fácil
    public static GameManager Instance { get; private set; }

    [Header("Interfaz Final")]
    [SerializeField] private GameObject cartelJuegoFinalizado; // El pop-up de fin de juego

    private int misionesCompletadasCount = 0;
    private const int TOTAL_MISIONES = 2; // Gladys + Rebeca

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
    }

    // Esta función la van a llamar los personajes al dar la campera correcta
    public void RegistrarMisionCumplida()
    {
        misionesCompletadasCount++;
        Debug.Log("Misiones completadas: " + misionesCompletadasCount + " / " + TOTAL_MISIONES);

        // 🏆 CONDICIÓN DE VICTORIA TOTAL
        if (misionesCompletadasCount >= TOTAL_MISIONES)
        {
            GanarJuego();
        }
    }

    private void GanarJuego()
    {
        if (cartelJuegoFinalizado != null)
        {
            cartelJuegoFinalizado.SetActive(true);
            Debug.Log("¡Felicidades! Completaste todas las misiones. Juego Finalizado.");
            
            // Opcional: Podés pausar el juego acá si querés que no se muevan más
            // Time.timeScale = 0f; 
        }
    }
}