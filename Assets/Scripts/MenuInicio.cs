using UnityEngine;
using UnityEngine.SceneManagement; // Esta l�nea permite el control y cambio de escenas

public class MenuInicio : MonoBehaviour
{
    // Esta funci�n se va a activar cuando hagas click en Button-Jugar
    public void CambiarAEscenaJuego()
    {
        // Cambia autom�ticamente de la escena Menu a la escena Clase
        SceneManager.LoadScene("0");
    }

    // Esta funci�n la pod�s usar si ten�s un bot�n para cerrar el juego
    public void SalirDelJuego()
    {
        Debug.Log("Saliste del juego");
        Application.Quit(); // Cierra la aplicaci�n (funciona en el juego ya exportado)
    }
}