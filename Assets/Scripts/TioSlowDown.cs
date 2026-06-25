using UnityEngine;
using System.Collections;

public class TioBorracho : MonoBehaviour
{
    private bool efectoActivo = false;

    private TioMovimiento tioMovimiento;
    private AudioTioBorracho audioTio;

    void Start()
    {
        tioMovimiento = GetComponent<TioMovimiento>();
        audioTio = GetComponent<AudioTioBorracho>();
    }

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

        // PRIMERA PARTE: Pepe y el tío quedan frenados 5 segundos.
        if (tioMovimiento != null)
        {
            tioMovimiento.DetenerTio();
        }

        if (audioTio != null)
        {
            audioTio.ReproducirBalbuceo();
        }

        jugador.congelado = true;
        jugador.AplicarCongeladoVisual();

        yield return new WaitForSeconds(5f);

        // Después de 5 segundos, Pepe y el tío vuelven a moverse.
        jugador.congelado = false;
        jugador.QuitarCongeladoVisual();

        if (audioTio != null)
        {
            audioTio.DetenerBalbuceo();
        }

        if (tioMovimiento != null)
        {
            tioMovimiento.ReanudarTio();
        }

        // SEGUNDA PARTE: Pepe queda ralentizado 10 segundos.
        jugador.velocidad = velocidadOriginal * 0.5f;

        StartCoroutine(jugador.Parpadear(10f));

        yield return new WaitForSeconds(10f);

        // Restaurar velocidad de Pepe.
        jugador.velocidad = velocidadOriginal;

        efectoActivo = false;
    }
}