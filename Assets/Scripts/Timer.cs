using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float tiempoRestante = 60f;
    public TMP_Text timerText;

    void Update()
    {
        // Evita que siga restando si ya llegó a cero
        if (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            
            // Si el tiempo cae por debajo de cero, lo clava en cero
            if (tiempoRestante < 0) 
            {
                tiempoRestante = 0;
            }
        }

        // EL ESCUDO: Solo actualiza el texto si la variable NO está vacía
        if (timerText != null)
        {
            timerText.text = Mathf.Ceil(tiempoRestante).ToString();
        }
        else
        {
            // Te avisa en la consola sin romperte el flujo del juego
            Debug.LogWarning("Che, te olvidaste de arrastrar el componente de texto al script Timer.");
        }
    }
}