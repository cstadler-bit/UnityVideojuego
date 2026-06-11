using UnityEngine;

public class LuzTrigger : MonoBehaviour
{
    [Header("Referencias de Luces")]
    [SerializeField] private GameObject luzGeneral;     // El objeto que ilumina el salón/escenario
    [SerializeField] private GameObject linternaPlayer;   // El objeto de la linterna que tiene el Player adentro

    private bool lucesApagadas = false; // Nos sirve para saber en qué estado estamos

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificamos si el que pisó el cuadrado es el Player
        if (other.CompareTag("Player"))
        {
            // Invertimos el estado de las luces
            lucesApagadas = !lucesApagadas;

            if (lucesApagadas)
            {
                // CAMINO DE IDA: Apagamos la luz del mapa y prendemos la linterna
                if (luzGeneral != null) luzGeneral.SetActive(false);
                if (linternaPlayer != null) linternaPlayer.SetActive(true);
                Debug.Log("¡Corte de luz! Se encendió la linterna del personaje.");
            }
            else
            {
                // CAMINO DE VUELTA: Prendemos la luz del mapa y apagamos la linterna
                if (luzGeneral != null) luzGeneral.SetActive(true);
                if (linternaPlayer != null) linternaPlayer.SetActive(false);
                Debug.Log("¡Volvió la luz! Se apagó la linterna.");
            }
        }
    }
}