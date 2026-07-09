using UnityEngine;

public class QuestAndPopUpManager : MonoBehaviour
{
    [Header("Referencias de UI")]
    public GameObject popUpAviso;        
    public GameObject contenedorGlobos;  
    public GameObject imagenVieja;
    public GameObject imagenNueva;

    // 🔥 MODIFICADO: Ahora es una lista de objetos para soportar al nene y a la nena juntos
    [Header("Los Niños Ayudantes")]
    [Tooltip("Cambiá el tamaño a 2 y arrastrá acá al nene y a la nena")]
    public GameObject[] ninosFollowers; 

    private bool misionCompletada = false;

    void Start()
    {
        // 🔥 Apaga a todos los niños que hayas puesto en la lista al empezar el juego
        if (ninosFollowers != null)
        {
            foreach (GameObject nino in ninosFollowers)
            {
                if (nino != null) nino.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (misionCompletada) return;

            DraggableObject objetoAgarrado = collision.GetComponentInChildren<DraggableObject>();

            if (objetoAgarrado != null)
            {
                CompletarMision(objetoAgarrado.gameObject);
            }
            else
            {
                if (popUpAviso != null) popUpAviso.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (misionCompletada) return;
            if (popUpAviso != null) popUpAviso.SetActive(false);
        }
    }

    void CompletarMision(GameObject objetoEntregado)
    {
        misionCompletada = true;

        if (popUpAviso != null) popUpAviso.SetActive(false);
        if (contenedorGlobos != null) contenedorGlobos.SetActive(false);

        if (imagenVieja != null) imagenVieja.SetActive(false);
        if (imagenNueva != null) imagenNueva.SetActive(true);

        // 🔥 ¡APARECEN LOS DOS NIÑOS JUNTOS!
        if (ninosFollowers != null)
        {
            foreach (GameObject nino in ninosFollowers)
            {
                if (nino != null) nino.SetActive(true);
            }
        }

        Destroy(objetoEntregado);
    }
}