using UnityEngine;
using UnityEngine.SceneManagement; // 🔥 Obligatorio para manejar el cambio de escenas

public class MenuManager : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [Tooltip("Escribí acá el nombre EXACTO de la escena del salón (respetá mayúsculas y acentos)")]
    [SerializeField] private string nombreEscenaSalon = "Salon";

    // Función pública para que la pueda detectar el botón del Canvas
    public void Jugar()
    {
        // Le dice a Unity que descargue el menú y cargue el salón
        SceneManager.LoadScene(nombreEscenaSalon);
    }

    // Por si tenés o querés poner un botón de "Salir"
    public void SalirDelJuego()
    {
        Debug.Log("Cerrando el juego...");
        Application.Quit(); // Cierra el ejecutable (.exe) cuando el juego esté compilado
    }
}