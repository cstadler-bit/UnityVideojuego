using UnityEngine;
using UnityEngine.SceneManagement; // 🔥 ¡Obligatorio para poder manejar pantallas y niveles!

public class ReiniciarNivel : MonoBehaviour
{
    public void Reiniciar()
    {
        // 1. Buscamos el nombre de la escena que está abierta ahora mismo
        string nombreEscenaActual = SceneManager.GetActiveScene().name;

        // 2. Le decimos a Unity que la vuelva a cargar desde cero
        SceneManager.LoadScene(nombreEscenaActual);

        // ⚠️ TIP PRO CRUCIAL: 
        // Si usás pantallas de pausa o Game Over donde congelás el juego con Time.timeScale = 0,
        // Al reiniciar tenés que devolver el tiempo a la normalidad (1) o el juego arrancará congelado.
        Time.timeScale = 1f; 
        
        Debug.Log("¡Nivel reiniciado con éxito!");
    }
}