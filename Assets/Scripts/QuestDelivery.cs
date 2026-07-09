using UnityEngine;

public class QuestDeliveryZone : MonoBehaviour
{
    [Header("Referencias de UI")]
    public GameObject popUpAviso;        // El globo de "necesito dulces"
    public GameObject contenedorGlobos;  // El objeto padre de los globos de sueño
    
    [Header("Cambio de Imagen")]
    public GameObject imagenVieja;
    public GameObject imagenNueva;

    private bool misionCompletada = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Solo nos importa si entra el Jugador
        if (collision.CompareTag("Player") && !misionCompletada)
        {
            // 2. Buscamos si el jugador tiene el objeto agarrado (es hijo de él)
            DraggableObject objetoAgarrado = collision.GetComponentInChildren<DraggableObject>();

            if (objetoAgarrado != null)
            {
                // ¡TIENE LOS DULCES! (o el objeto)
                CompletarMision(objetoAgarrado.gameObject);
            }
            else
            {
                // NO TIENE NADA: Mostramos el popup de aviso
                if (popUpAviso != null) popUpAviso.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !misionCompletada)
        {
            // Al salir, ocultamos el aviso
            if (popUpAviso != null) popUpAviso.SetActive(false);
        }
    }

    void CompletarMision(GameObject objetoEntregado)
    {
        misionCompletada = true;

        // 1. Ocultamos las UI
        if (popUpAviso != null) popUpAviso.SetActive(false);
        if (contenedorGlobos != null) contenedorGlobos.SetActive(false);

        // 2. Cambiamos las imágenes
        if (imagenVieja != null) imagenVieja.SetActive(false);
        if (imagenNueva != null) imagenNueva.SetActive(true);

        // 3. Eliminamos el objeto entregado
        Destroy(objetoEntregado);

        Debug.Log("¡Misión completada!");
    }
}