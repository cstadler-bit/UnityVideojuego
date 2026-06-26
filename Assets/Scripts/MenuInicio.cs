using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuInicio : MonoBehaviour
{
    public void CambiarAEscenaJuego()
    {
        SceneManager.LoadScene("0");
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliste del juego");
        Application.Quit(); 
    }
}