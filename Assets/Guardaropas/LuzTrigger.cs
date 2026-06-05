using UnityEngine;
using UnityEngine.Rendering.Universal; // Necesario para las luces 2D

public class LuzTrigger : MonoBehaviour
{
    [Header("Referencias de Luces")]
    [SerializeField] private Light2D luzGeneral;       // Componente de luz del salón
    [SerializeField] private Light2D linternaPlayer;    // Componente de luz de la linterna

    private bool lucesApagadas = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            lucesApagadas = !lucesApagadas;

            if (lucesApagadas)
            {
                // CAMINO DE IDA: Desactivamos el componente de la luz general y ACTIVAMOS la linterna
                if (luzGeneral != null) luzGeneral.enabled = false;
                if (linternaPlayer != null) linternaPlayer.enabled = true;
                Debug.Log("¡Corte de luz! Se habilitó la linterna.");
            }
            else
            {
                // CAMINO DE VUELTA: Activamos la luz general y DESACTIVAMOS la linterna
                if (luzGeneral != null) luzGeneral.enabled = true;
                if (linternaPlayer != null) linternaPlayer.enabled = false;
                Debug.Log("¡Volvió la luz! Se deshabilitó la linterna.");
            }
        }
    }
}