using UnityEngine;

public class RevealHiddenObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Header("Configuración de Visibilidad")]
    [Range(0f, 1f)] 
    [SerializeField] private float hiddenOpacity = 0f; // 0 es invisible total, podés ponerle 0.1f si querés que se intuya un poquito

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Al arrancar el juego, ocultamos la imagen aplicando la opacidad baja
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, hiddenOpacity);
        }
    }

    // Cuando el personaje PISA la imagen, se revela al 100%
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white; // Vuelve al color original con 100% de opacidad
            }
            Debug.Log("¡Imagen revelada por el personaje!");
        }
    }

    // OPCIONAL: Si querés que cuando el personaje se ALEJE se vuelva a ocultar,
    // descomentá las líneas de abajo (borrando las barras // )
    /*
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, hiddenOpacity);
            }
        }
    }
    */
}