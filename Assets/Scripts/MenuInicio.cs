using UnityEngine;
using UnityEngine.SceneManagement; // Esta línea permite el control y cambio de escenas

public class MenuInicio : MonoBehaviour
{
    // Esta función se va a activar cuando hagas click en Button-Jugar
    public void CambiarAEscenaJuego()
    {
        // Cambia automáticamente de la escena Menu a la escena Clase
        SceneManager.LoadScene("Clase");
    }

    // Esta función la podés usar si tenés un botón para cerrar el juego
    public void SalirDelJuego()
    {
        Debug.Log("Saliste del juego");
        Application.Quit(); // Cierra la aplicación (funciona en el juego ya exportado)
    }
}