using UnityEngine;
using System.Collections;

public class TioBorracho : MonoBehaviour
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
                StartCoroutine(AplicarEfecto(jugador));
            }
        }
    }

    IEnumerator AplicarEfecto(Movimiento jugador)
    {
        efectoActivo = true;

        float velocidadOriginal = jugador.velocidad;

        // CONGELADO
        jugador.congelado = true;
        jugador.AplicarCongeladoVisual();

        yield return new WaitForSeconds(5f);

        jugador.congelado = false;
        jugador.QuitarCongeladoVisual();

        // RALENTIZADO
        jugador.velocidad = velocidadOriginal * 0.5f;

        StartCoroutine(jugador.Parpadear(10f));

        yield return new WaitForSeconds(10f);

        // RESTAURAR
        jugador.velocidad = velocidadOriginal;

        efectoActivo = false;
    }
}