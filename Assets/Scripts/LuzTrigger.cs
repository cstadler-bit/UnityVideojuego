using UnityEngine;
using System.Collections; // Necesario para usar Corrutinas (IEnumerator)
using UnityEngine.Rendering.Universal; 

public class LuzTrigger : MonoBehaviour
{
    [Header("Referencias de Luces")]
    [SerializeField] private Light2D luzGeneral;       
    [SerializeField] private Light2D linternaPlayer;    

    [Header("Configuración del Degradé")]
    [SerializeField] private float tiempoTransicion = 1.5f; // Cuánto tarda en apagarse/prenderse (en segundos)
    [SerializeField] private float intensidadMinima = 0.1f; // 🔥 Para que no quede 100% oscuro (el toque sutil)
    [SerializeField] private float intensidadMaxima = 1f;   // La intensidad normal del salón

    private bool lucesApagadas = false;
    private Coroutine corrutinaLuz; // Guarda la transición actual para que no se superpongan

    private void Start()
    {
        // Nos aseguramos de que arranquen con los valores máximos/mínimos correctos
        if (luzGeneral != null) luzGeneral.intensity = intensidadMaxima;
        if (linternaPlayer != null) linternaPlayer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            lucesApagadas = !lucesApagadas;

            // Si ya había un degradé corriendo, lo frenamos para que no haga cosas raras
            if (corrutinaLuz != null) StopCoroutine(corrutinaLuz);

            if (lucesApagadas)
            {
                // CAMINO DE IDA: Bajamos la luz general con degradé y prendemos la linterna
                corrutinaLuz = StartCoroutine(TransicionDeLuz(intensidadMaxima, intensidadMinima));
                if (linternaPlayer != null) linternaPlayer.enabled = true;
                Debug.Log("Iniciando degradé sutil hacia la oscuridad...");
            }
            else
            {
                // CAMINO DE VUELTA: Subimos la luz general con degradé y apagamos la linterna
                corrutinaLuz = StartCoroutine(TransicionDeLuz(intensidadMinima, intensidadMaxima));
                if (linternaPlayer != null) linternaPlayer.enabled = false;
                Debug.Log("Iniciando degradé sutil hacia la claridad...");
            }
        }
    }

    // 🔥 LA MAGIA DEL DEGRADÉ: Cambia la intensidad frame a frame suavemente
    private IEnumerator TransicionDeLuz(float inicio, float fin)
    {
        float tiempoPasado = 0f;

        while (tiempoPasado < tiempoTransicion)
        {
            tiempoPasado += Time.deltaTime;
            // Evaluamos en qué parte del camino va el tiempo (entre 0 y 1)
            float porcentaje = tiempoPasado / tiempoTransicion; 

            if (luzGeneral != null)
            {
                // Lerp hace la interpolación matemática suave entre el inicio y el fin
                luzGeneral.intensity = Mathf.Lerp(inicio, fin, porcentaje);
            }

            yield return null; // Espera al siguiente frame
        }

        // Al terminar, nos aseguramos de clavar el valor exacto del final
        if (luzGeneral != null) luzGeneral.intensity = fin;
    }
}