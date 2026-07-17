using UnityEngine;
using System.Collections;

public class ControlGuardarropas : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int maxEntradasPermitidas = 3;
    [SerializeField] private float tiempoDeBloqueo = 5f;

    [Header("Referencias")]
    [SerializeField] private GameObject cartelAdvertencia;
    [SerializeField] private GameObject bloqueoEntrada;

    private int cantidadEntradasTotal = 0;
    private bool entradaBloqueada = false;

    void Start()
    {
        // El cartel del seguridad queda visible desde el principio
        if (cartelAdvertencia != null)
            cartelAdvertencia.SetActive(true);

        // El bloqueo físico empieza apagado
        if (bloqueoEntrada != null)
            bloqueoEntrada.SetActive(false);

        cantidadEntradasTotal = 0;
        entradaBloqueada = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (entradaBloqueada) return;

        cantidadEntradasTotal++;

        Debug.Log("Entradas al guardarropas: " + cantidadEntradasTotal);

        if (cantidadEntradasTotal > maxEntradasPermitidas)
        {
            StartCoroutine(BloquearEntrada());
        }
    }

    private IEnumerator BloquearEntrada()
    {
        entradaBloqueada = true;

        if (cartelAdvertencia != null)
            cartelAdvertencia.SetActive(true);

        if (bloqueoEntrada != null)
            bloqueoEntrada.SetActive(true);

        yield return new WaitForSeconds(tiempoDeBloqueo);

        if (bloqueoEntrada != null)
            bloqueoEntrada.SetActive(false);

        // Importante:
        // NO reiniciamos cantidadEntradasTotal.
        // Así, después de la cuarta entrada, cada nuevo intento vuelve a bloquear.

        entradaBloqueada = false;
    }
}
