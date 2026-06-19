using UnityEngine;
using System.Collections;

public class QuinceBoost : MonoBehaviour
{
    private bool efectoActivo = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (efectoActivo) return;

        if (other.CompareTag("Player"))
        {
            Movimiento jugador = other.GetComponent<Movimiento>();

            if (jugador != null)
            {
                StartCoroutine(DarBoost(jugador));
            }
        }
    }

    IEnumerator DarBoost(Movimiento jugador)
    {
        efectoActivo = true;

        float velocidadOriginal = jugador.velocidad;

        jugador.velocidad = velocidadOriginal * 1.5f;

        StartCoroutine(jugador.BrillarBoost(15f));

        yield return new WaitForSeconds(15f);

        jugador.velocidad = velocidadOriginal;

        efectoActivo = false;
    }
}