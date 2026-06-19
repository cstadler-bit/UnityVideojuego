using UnityEngine;
using UnityEngine.SceneManagement; // 🚨 IMPORTANTE: Necesario para cambiar de escena

public class DetectorCamperaEquivocada : MonoBehaviour
{
    [Header("Referencias de la Misión")]
    [SerializeField] private string nombreSacoCorrecto = "sacopeludo"; // O "sacoamarillo" según el personaje
    [SerializeField] private GameObject cartelMisionFallida;

    [Header("Configuración de Reinicio")]
    [SerializeField] private string nombreEscenaMenu = "campera menu"; // 🚨 Poné acá el nombre exacto de tu escena de menú

    private bool juegoFallido = false;

    void Start()
    {
        if (cartelMisionFallida != null)
        {
            cartelMisionFallida.SetActive(false);
        }
        juegoFallido = false;
    }

    void Update()
    {
        // 🚨 SI YA PERDIÓ: Nos quedamos escuchando el Enter, Espacio o Clic para reiniciar e ir al menú
        if (juegoFallido)
        {
            if (Input.GetKeyDown(KeyCode.Return) || 
                Input.GetKeyDown(KeyCode.KeypadEnter) || 
                Input.GetKeyDown(KeyCode.Space) || 
                Input.GetMouseButtonDown(0))
            {
                ReiniciarAlMenu();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si ya perdió o si lo que entró no es un objeto arrastrable (saco), no hacemos nada
        if (juegoFallido || other.CompareTag("Player")) return;

        string nombreObjeto = other.gameObject.name.ToLower();

        // Si entra un saco, pero NO es el correcto de este personaje... ¡PUM, error!
        if (nombreObjeto.Contains("saco") && !nombreObjeto.Contains(nombreSacoCorrecto.ToLower()))
        {
            DispararMisionFallida(other.gameObject);
        }
    }

    private void DispararMisionFallida(GameObject sacoEquivocado)
    {
        juegoFallido = true;
        Debug.Log($"🚨 ¡Campera equivocada! Se entregó: {sacoEquivocado.name}. Se esperaba: {nombreSacoCorrecto}");

        // 1. Apagamos el saco equivocado para que no quede tirado
        if (sacoEquivocado != null) sacoEquivocado.SetActive(false);

        // 2. Encendemos el cartel de Misión Fallida
        if (cartelMisionFallida != null)
        {
            cartelMisionFallida.SetActive(true);
        }

        // 3. 🚨 OPCIONAL: Le avisamos al script de evolución de este objeto que se ponga en Blanco y Negro de una
        EvolucionGladys evo = GetComponent<EvolucionGladys>();
        if (evo != null)
        {
            // Forzamos el estado de pérdida en el script de evolución si fuera necesario
            evo.enabled = false; // Frenamos su Update para que no se pise la lógica
        }
    }

    private void ReiniciarAlMenu()
    {
        Debug.Log($"🎬 Cargando la escena de menú: {nombreEscenaMenu}");
        
        // Despausamos el juego por las dudas si usaste Time.timeScale = 0
        Time.timeScale = 1f; 
        
        // Cargamos la escena del menú de camperas
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}