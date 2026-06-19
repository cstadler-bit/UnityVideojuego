using UnityEngine;
using UnityEngine.SceneManagement; 

public class DetectorCamperaEquivocada : MonoBehaviour
{
    [Header("Nueva Interfaz (Error)")]
    [SerializeField] private GameObject cartelCamperaEquivocada; 

    [Header("Configuración de Filtros de Ropa")]
    [Tooltip("La palabra clave que SÍ es la correcta para este personaje (ej: sacoamarillo o sacopeludo)")]
    [SerializeField] private string nombreSacoCorrecto = "sacoamarillo";

    private bool cartelErrorVisible = false;

    void Start()
    {
        if (cartelCamperaEquivocada != null) cartelCamperaEquivocada.SetActive(false);
        cartelErrorVisible = false;
    }

    void Update()
    {
        if (cartelErrorVisible)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                ReiniciarJuego();
            }
            else if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                CerrarCartelError();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string nombreObjeto = other.gameObject.name.ToLower();

        // 🚨 LA REGLA GENERAL: 
        // Si entra algo que se llama "saco" o "campera", pero NO es el saco correcto seteado en el Inspector... ¡EXPLOTA!
        if ((nombreObjeto.Contains("saco") || nombreObjeto.Contains("campera")) && !nombreObjeto.Contains(nombreSacoCorrecto.ToLower()))
        {
            MostrarErrorCampera();
        }
    }

    private void MostrarErrorCampera()
    {
        if (cartelErrorVisible) return;

        cartelErrorVisible = true;

        if (cartelCamperaEquivocada != null)
        {
            cartelCamperaEquivocada.SetActive(true);
            Debug.Log("¡Ropa incorrecta detectada! Activando cartel de Game Over.");
        }
    }

    public void CerrarCartelError()
    {
        cartelErrorVisible = false;
        if (cartelCamperaEquivocada != null)
        {
            cartelCamperaEquivocada.SetActive(false);
        }
    }

    private void ReiniciarJuego()
    {
        string escenaActual = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(escenaActual);
    }
}