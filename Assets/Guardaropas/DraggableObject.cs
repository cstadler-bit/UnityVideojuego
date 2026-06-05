using UnityEngine;
using System.Collections;

public class DraggableObject : MonoBehaviour
{
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;
    
    private bool isBeingCarried = false;
    private bool isBlinking = false; // Nos avisa si la campera está titilando en este momento

    [Header("Configuración de Cercanía")]
    [SerializeField] private float distanciaParaInteractuar = 1.5f; // Qué tan cerca tiene que estar el Player

    [Header("Efecto de Apilado Multi-Campera")]
    [SerializeField] private float separacionPorCampera = 0.25f; // Cuánto se desfasa hacia arriba cada campera nueva

    void Start()
    {
        Debug.Log("start", gameObject);
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Buscamos al objeto "Player" en la escena automáticamente usando su etiqueta
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("¡Cuidado! No encontré ningún objeto con el Tag 'Player' en la escena.");
        }
    }

    void Update()
    {
        // Si el personaje ya la agarró, se mueve pegada a él
        if (isBeingCarried && playerTransform != null)
        {
            // Contamos cuántas camperas ya tiene el jugador para calcular la altura de esta
            int indiceApilado = ContarCamperasEnPlayer();

            // Modificamos el eje Y multiplicándolo por su lugar en la pila
            // Ej: la primera va a Y: 0.1, la segunda a Y: 0.35, la tercera a Y: 0.6...
            float desplazamientoY = 0.1f + (indiceApilado * separacionPorCampera);

            transform.position = playerTransform.position + new Vector3(0.2f, desplazamientoY, 0f);
        }

        // DETECCIÓN DE LA BARRA ESPACIADORA
        if (Input.GetKeyDown(KeyCode.Space) && playerTransform != null)
        {
            // Si ESTA campera en específico ya la llevás puesta, la barra espaciadora la deja en el piso
            if (isBeingCarried)
            {
                DropCoat();
                return;
            }

            // Si NO la llevás puesta, calculamos la distancia para ver si estás cerca de ella
            float distanciaActual = Vector2.Distance(transform.position, playerTransform.position);

            if (distanciaActual <= distanciaParaInteractuar)
            {
                // SEGUNDO PASO: Si la tocás MIENTRAS está titilando, se acopla
                if (isBlinking)
                {
                    GrabCoat();
                }
                // PRIMER PASO: Si está normal, empieza a titilar
                else
                {
                    StartCoroutine(BlinkEffect());
                }
            }
        }
    }

    // Función auxiliar para saber cuántas camperas se están cargando ahora mismo
    int ContarCamperasEnPlayer()
    {
        int contador = 0;
        // Buscamos todos los scripts "DraggableObject" que tengan al Player como padre
        DraggableObject[] todasLasCamperas = playerTransform.GetComponentsInChildren<DraggableObject>();
        
        for (int i = 0; i < todasLasCamperas.Length; i++)
        {
            // Si la campera de la lista es ESTA misma, frenamos el conteo acá
            // Esto determina el orden de llegada en la "pila"
            if (todasLasCamperas[i] == this)
            {
                return contador;
            }
            contador++;
        }
        return contador;
    }

    // Efecto de titileo por opacidad
    IEnumerator BlinkEffect()
    {
        isBlinking = true;
        Debug.Log("Primer paso: Titilando " + gameObject.name);

        for (int i = 0; i < 6; i++)
        {
            if (isBeingCarried) yield break;

            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.4f);
                yield return new WaitForSeconds(0.2f);
                
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.2f);
            }
        }

        isBlinking = false;
    }

    void GrabCoat()
    {
        isBeingCarried = true;
        isBlinking = false;
        
        StopAllCoroutines();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
            
            // Le aumentamos dinámicamente el Sorting Order según cuántas lleve
            // Para que las camperas nuevas se rendericen siempre ADELANTE de las anteriores
            spriteRenderer.sortingOrder = 10 + ContarCamperasEnPlayer();
        }

        if (playerTransform != null)
        {
            transform.SetParent(playerTransform);
        }

        Debug.Log("¡Campera agregada a la pila!");
    }

    void DropCoat()
    {
        isBeingCarried = false;
        isBlinking = false;

        // Le quitamos el padre para dejarla en el piso
        transform.SetParent(null);

        if (spriteRenderer != null)
        {
            // Volvemos el sorting order a un número base en el piso (ej: 2)
            spriteRenderer.sortingOrder = 2; 
        }

        Debug.Log("¡Campera soltada de la pila!");
    }
}