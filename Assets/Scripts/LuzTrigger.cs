using UnityEngine;
using UnityEngine.Rendering.Universal; // Necesario para que reconozca las luces 2D

public class LuzTrigger : MonoBehaviour
{
    [Header("Referencias de Luces")]
    [SerializeField] private Light2D luzGeneral;       // La luz global o del salón
    [SerializeField] private Light2D linternaPlayer;    // La linterna (Spot Light 2D) adentro del personaje

    private bool lucesApagadas = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificamos si el que pisó el cuadrado sensor es el Player
        if (other.CompareTag("Player"))
        {
            // Invertimos el estado de las luces
            lucesApagadas = !lucesApagadas;

            if (lucesApagadas)
            {
                // CAMINO DE IDA: Apagamos el componente de luz general y prendemos la linterna
                if (luzGeneral != null) luzGeneral.enabled = false;
                if (linternaPlayer != null) linternaPlayer.enabled = true;
                Debug.Log("¡Corte de luz! Se habilitó la linterna del personaje.");
            }
            else
            {
                // CAMINO DE VUELTA: Prendemos la luz general y apagamos la linterna
                if (luzGeneral != null) luzGeneral.enabled = true;
                if (linternaPlayer != null) linternaPlayer.enabled = false;
                Debug.Log("¡Volvió la luz! Se deshabilitó la linterna.");
            }
        }
    }
}