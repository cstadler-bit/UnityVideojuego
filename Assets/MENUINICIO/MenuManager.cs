using UnityEngine;
using UnityEngine.SceneManagement; // 🔥 Obligatorio para manejar el cambio de escenas

public class MenuManager : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [Tooltip("Escribí acá el nombre EXACTO de la escena del salón (respetá mayúsculas y acentos)")]
    [SerializeField] private string nombreEscenaSalon = "Salon";

    [Header("Paneles del Menú (¡NUEVO!)")]
    [Tooltip("El panel principal que dice 'Jugar' (se apaga al mostrar el mapa)")]
    [SerializeField] private GameObject panelMenuPrincipal;

    [Tooltip("El panel con la imagen del mapa y el botón de Play que ya tenés hecho")]
    [SerializeField] private GameObject panelMapa;

    [Tooltip("Todas las pantallas de historia. NO se borran, pero quedan forzadas a apagadas: nunca se van a mostrar")]
    [SerializeField] private GameObject[] pantallasHistoria;

    void Start()
    {
        // 🔥 Forzamos que las pantallas de historia arranquen (y queden) apagadas,
        // sin borrarlas del proyecto ni de la jerarquía.
        if (pantallasHistoria != null)
        {
            foreach (GameObject pantalla in pantallasHistoria)
            {
                if (pantalla != null)
                    pantalla.SetActive(false);
            }
        }

        // El mapa arranca apagado; se muestra recién cuando se aprieta "Jugar"
        if (panelMapa != null)
            panelMapa.SetActive(false);
    }

    // Función pública para el botón "Jugar" del menú principal.
    // 🔧 CAMBIO: antes cargaba la escena del salón directo. Ahora, en vez de eso,
    // muestra el panel del mapa (saltando las pantallas de historia).
    public void Jugar()
    {
        // 🔧 DIAGNÓSTICO: esto nos va a decir en la consola si el problema es una referencia
        // sin asignar en el Inspector.
        Debug.Log($"[Diagnóstico menú] Jugar() fue llamado. panelMenuPrincipal={(panelMenuPrincipal != null ? panelMenuPrincipal.name : "NULL")} | panelMapa={(panelMapa != null ? panelMapa.name : "NULL")}");

        if (panelMenuPrincipal != null)
            panelMenuPrincipal.SetActive(false);

        if (panelMapa != null)
            panelMapa.SetActive(true);

        // 🔧 DIAGNÓSTICO: si panelMapa no es NULL pero igual no se ve en pantalla, revisá si
        // "panelMapa" es HIJO de "panelMenuPrincipal" en la Jerarquía. Si lo es, al apagar el
        // padre (panelMenuPrincipal) el mapa queda apagado también, aunque acá le pongamos
        // SetActive(true) a su propio GameObject (activeSelf=true no alcanza si el padre está
        // inactivo: lo que se ve en pantalla depende de activeInHierarchy).
        if (panelMapa != null)
            Debug.Log($"[Diagnóstico menú] panelMapa.activeInHierarchy = {panelMapa.activeInHierarchy} (si dice False, el mapa NO se está mostrando)");
    }

    // 🔥 NUEVA FUNCIÓN: esta es la que tenés que enganchar al botón "Play" que ya
    // armaste DENTRO del panel del mapa (OnClick del botón → MenuManager → IniciarPartidaDesdeMapa).
    // Es la que efectivamente carga la escena del salón.
    public void IniciarPartidaDesdeMapa()
    {
        SceneManager.LoadScene(nombreEscenaSalon);
    }

    // Por si tenés o querés poner un botón de "Salir"
    public void SalirDelJuego()
    {
        Debug.Log("Cerrando el juego...");
        Application.Quit(); // Cierra el ejecutable (.exe) cuando el juego esté compilado
    }
}