using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TemporizadorFlechasConInterruptor : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject flechasGuia; // Objeto Padre (BotonPasillo)
    [SerializeField] private GameObject interruptor; // Objeto Interruptor

    [Header("Configuración de Tiempo")]
    [SerializeField] private float tiempoParaAparecer = 20f; 

    [Header("Efecto de Bombeo Personalizado (¡NUEVO!)")]
    [SerializeField] private float velocidadBombeo = 5f;  

    [Tooltip("Cuánto es lo MÍNIMO que se pueden encoger. (1.0 = nunca se encogen de su tamaño original / 0.95 = se encogen solo un 5%)")]
    [SerializeField] private float escalaMinima = 0.95f; 

    [Tooltip("Cuánto es lo MÁXIMO que se pueden agrandar. (1.15 = se agrandan un 15% más de su tamaño original)")]
    [SerializeField] private float escalaMaxima = 1.15f;

    private bool jugadorPasoPorInterruptor = false;
    private bool flechasActivas = false;
    private Coroutine corutinaReloj;

    private List<Transform> hijosFlechas = new List<Transform>();
    private List<Vector3> escalasOriginalesHijos = new List<Vector3>();

    void Start()
    {
        if (flechasGuia != null)
        {
            foreach (Transform hijo in flechasGuia.transform)
            {
                hijosFlechas.Add(hijo);
                escalasOriginalesHijos.Add(hijo.localScale);
            }
            flechasGuia.SetActive(false); 
        }

        if (interruptor != null)
        {
            Collider2D colisionador = interruptor.GetComponentInChildren<Collider2D>();
            if (colisionador != null)
            {
                PuenteDetectorFlechas puente = colisionador.gameObject.AddComponent<PuenteDetectorFlechas>();
                puente.ConfigurarPuente(this);
            }
        }

        corutinaReloj = StartCoroutine(ContadorTiempo());
    }

    void Update()
    {
        if (flechasActivas && hijosFlechas.Count > 0)
        {
            // 🔥 TRUCO: Pasamos el vaivén del seno (que va de -1 a 1) a un rango limpio de 0 a 1
            float senoNormalizado = (Mathf.Sin(Time.time * velocidadBombeo) + 1f) / 2f;
            
            // Interpolamos exactamente entre el mínimo y el máximo que vos elijas en el Inspector
            float factorEscala = Mathf.Lerp(escalaMinima, escalaMaxima, senoNormalizado);
            
            for (int i = 0; i < hijosFlechas.Count; i++)
            {
                if (hijosFlechas[i] != null)
                {
                    // Multiplicamos su tamaño de fábrica por el factor controlado
                    hijosFlechas[i].localScale = escalasOriginalesHijos[i] * factorEscala;
                }
            }
        }
    }

    IEnumerator ContadorTiempo()
    {
        yield return new WaitForSeconds(tiempoParaAparecer);

        if (!jugadorPasoPorInterruptor)
        {
            if (flechasGuia != null)
            {
                flechasGuia.SetActive(true);
                flechasActivas = true; 
            }
        }
    }

    public void RegistrarPasoDelJugador()
    {
        if (jugadorPasoPorInterruptor) return; 

        jugadorPasoPorInterruptor = true;

        if (flechasActivas)
        {
            flechasActivas = false;
            if (flechasGuia != null)
            {
                flechasGuia.SetActive(false);
                for (int i = 0; i < hijosFlechas.Count; i++)
                {
                    if (hijosFlechas[i] != null)
                    {
                        hijosFlechas[i].localScale = escalasOriginalesHijos[i];
                    }
                }
            }
        }
        else
        {
            if (corutinaReloj != null) StopCoroutine(corutinaReloj);
        }
    }
}

// ============================================================================
// SCRIPT PUENTE AUTOMÁTICO
// ============================================================================
public class PuenteDetectorFlechas : MonoBehaviour
{
    private TemporizadorFlechasConInterruptor scriptPrincipal;

    public void ConfigurarPuente(TemporizadorFlechasConInterruptor principal)
    {
        scriptPrincipal = principal;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (scriptPrincipal != null)
            {
                scriptPrincipal.RegistrarPasoDelJugador();
            }
        }
    }
}