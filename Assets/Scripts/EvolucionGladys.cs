using UnityEngine;
using UnityEngine.UI;

public class EvolucionGladys : MonoBehaviour
{
    [Header("Objetos de la UI (Jerarquía Canvas)")]
    [SerializeField] private Image filtroRojoUI;          
    [SerializeField] private GameObject fondoIncognito;    
    
    [Header("Objetos de las Caras en la UI")]
    [SerializeField] private GameObject caraFelizUI;      
    [SerializeField] private GameObject caraPreocupadaUI;  
    [SerializeField] private GameObject caraEnojadaUI;     

    private int faseActual = 0;
    private float cronometro = 0f;
    private bool temporizadorActivo = false;
    private bool estaTitilando = false;

    void Start()
    {
        // Estado inicial limpio
        if (fondoIncognito != null) fondoIncognito.SetActive(true);
        
        if (caraFelizUI != null) caraFelizUI.SetActive(false);
        if (caraPreocupadaUI != null) caraPreocupadaUI.SetActive(false);
        if (caraEnojadaUI != null) caraEnojadaUI.SetActive(false);
        
        if (filtroRojoUI != null) filtroRojoUI.fillAmount = 0f;
    }

    void Update()
    {
        if (!temporizadorActivo) return;

        cronometro += Time.deltaTime;

        // --- CONTROL DE TIEMPOS AJUSTADO ---

        // Fase 2: A los 20 segundos pasa a Preocupada
        if (faseActual == 1 && cronometro >= 20f)
        {
            faseActual = 2;
            if (caraFelizUI != null) caraFelizUI.SetActive(false);
            if (caraPreocupadaUI != null) caraPreocupadaUI.SetActive(true);
            if (filtroRojoUI != null) filtroRojoUI.fillAmount = 0.66f;
            Debug.Log("20s: Tía Preocupada.");
        }
        
        // Fase 3: Al minuto (40s desde el inicio) pasa a Enojada y TITILA
        if (faseActual == 2 && cronometro >= 40f)
        {
            faseActual = 3;
            if (caraPreocupadaUI != null) caraPreocupadaUI.SetActive(false);
            if (caraEnojadaUI != null) caraEnojadaUI.SetActive(true);
            if (filtroRojoUI != null) filtroRojoUI.fillAmount = 1f;
            estaTitilando = true;
            Debug.Log("40s: Tía Enojada y titilando.");
        }

        // CONTROL DEL TITILEO (Se ejecuta mientras esté activo y no llegue a 70s)
        if (estaTitilando && cronometro < 70f)
        {
            if (caraEnojadaUI != null)
            {
                // Parpadeo rápido usando el tiempo del juego
                bool mostrar = Mathf.PingPong(Time.time * 9f, 1f) > 0.5f;
                caraEnojadaUI.SetActive(mostrar);
            }
        }

        // 🔥 FINAL: A los 70 segundos, se congela y pasa a Blanco y Negro fotográfico
        if (cronometro >= 70f && faseActual == 3)
        {
            faseActual = 4; // Fin del tiempo
            estaTitilando = false;
            
            if (caraEnojadaUI != null) 
            {
                caraEnojadaUI.SetActive(true); // Forzamos que quede encendida y fija
                
                Image imgEnojada = caraEnojadaUI.GetComponent<Image>();
                if (imgEnojada != null)
                {
                    // Usa el shader de sprites de Unity para quitarle el color por completo
                    imgEnojada.material = new Material(Shader.Find("UI/Default"));
                    imgEnojada.color = new Color(0.25f, 0.25f, 0.25f, 1f); // Contraste ByN de diseño
                }
            }
            
            // Apagamos el filtro rojo para que se note el cambio radical a gris
            if (filtroRojoUI != null) filtroRojoUI.gameObject.SetActive(false); 
            
            Debug.Log("70s: Fin del juego. Tía fija en Blanco y Negro.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !temporizadorActivo)
        {
            RevelarTia();
        }
    }

    private void RevelarTia()
    {
        // Reseteamos el color por si reiniciás la partida
        if (caraEnojadaUI != null)
        {
            Image imgEnojada = caraEnojadaUI.GetComponent<Image>();
            if (imgEnojada != null) imgEnojada.color = Color.white;
        }

        temporizadorActivo = true;
        faseActual = 1;
        cronometro = 0f;

        if (fondoIncognito != null) fondoIncognito.SetActive(false);
        if (caraFelizUI != null) caraFelizUI.SetActive(true);
        if (filtroRojoUI != null) filtroRojoUI.fillAmount = 0.33f;
        Debug.Log("Inicio: Tía Feliz.");
    }
}