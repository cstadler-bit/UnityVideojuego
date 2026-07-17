using UnityEngine;
using System.Collections;

public class ControlGuardarropas : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int maxEntradasAntesDeBloqueo = 3;
    [SerializeField] private float tiempoDeBloqueo = 5f;

    [Header("Referencias")]
    [SerializeField] private GameObject cartelAdvertencia;
    [SerializeField] private GameObject bloqueoEntrada;

    private int cantidadEntradas = 0;
    private bool entradaBloqueada = false;

    void Start()
    {
        // El cartel del seguridad queda visible desde el principio
        if (cartelAdvertencia != null)
            cartelAdvertencia.SetActive(true);

        // El bloqueo físico empieza apagado
        if (bloqueoEntrada != null)
            bloqueoEntrada.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (entradaBloqueada)
            return;

        cantidadEntradas++;

        if (cantidadEntradas > maxEntradasAntesDeBloqueo)
        {
            StartCoroutine(BloquearEntrada());
        }
    }

    private IEnumerator BloquearEntrada()
    {
        entradaBloqueada = true;

        // El cartel ya está visible, pero lo dejamos por seguridad
        if (cartelAdvertencia != null)
            cartelAdvertencia.SetActive(true);

        if (bloqueoEntrada != null)
            bloqueoEntrada.SetActive(true);

        yield return new WaitForSeconds(tiempoDeBloqueo);

        if (bloqueoEntrada != null)
            bloqueoEntrada.SetActive(false);

        // No apagamos el cartel porque debe quedar visible siempre

        cantidadEntradas = 0;
        entradaBloqueada = false;
    }
}