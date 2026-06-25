using UnityEngine;
using System.Collections;

public class DraggableObject : MonoBehaviour
{
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;
    
    private bool isBeingCarried = false;
    private bool isBlinking = false; // Nos avisa si la campera está titilando

    [Header("Configuración de Cercanía")]
    [SerializeField] private float distanciaParaInteractuar = 1.5f; // Qué tan cerca tiene que estar el Player

    [Header("Configuración de Posición al Cargar")]
    [SerializeField] private Vector3 posicionRelativaAlJugador = new Vector3(0.2f, 0.1f, 0f); // Posición fija al llevarla

    void Start()
    {
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
        // Si el personaje ya la agarró, se mueve en una posición fija pegada a él
        if (isBeingCarried && playerTransform != null)
        {
            transform.position = playerTransform.position + posicionRelativaAlJugador;
        }

        // DETECCIÓN DE LA BARRA ESPACIADORA
        if (Input.GetKeyDown(KeyCode.Space) && playerTransform != null)
        {
            // Si ESTA campera en específico ya la llevás puesta, la barra la deja en el piso
            if (isBeingCarried)
            {
                DropCoat();
                return;
            }

            // Si NO la llevás puesta, calculamos la distancia para ver si estás cerca de ella
            float distanciaActual = Vector2.Distance(transform.position, playerTransform.position);

            if (distanciaActual <= distanciaParaInteractuar)
            {
                // 🔥 LA SOLUCIÓN: Buscamos si el jugador ya está cargando OTRA campera como objeto hijo
                DraggableObject camperaEquipada = playerTransform.GetComponentInChildren<DraggableObject>();

                if (camperaEquipada == null)
                {
                    // Si no tiene ninguna campera encima, lo dejamos agarrar esta
                    GrabCoat();
                }
                else
                {
                    // Si ya tiene una, cancelamos la acción
                    Debug.Log("¡Ya estás cargando una campera! Tenés que soltarla antes de agarrar otra.");
                }
            }
        }
    }

    // Efecto de titileo por opacidad 
    IEnumerator BlinkEffect()
    {
        isBlinking = true;

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
            spriteRenderer.sortingOrder = 12; // Un orden alto fijo para que se renderice siempre frente al personaje
        }

        if (playerTransform != null)
        {
            transform.SetParent(playerTransform);
        }

        Debug.Log("¡Campera agarrada con éxito!");
    }

    void DropCoat()
    {
        isBeingCarried = false;
        isBlinking = false;

        // Le quitamos el padre para dejarla en el piso
        transform.SetParent(null);

        if (spriteRenderer != null)
        {
            // Volvemos el sorting order a un número base en el piso
            spriteRenderer.sortingOrder = 2; 
        }

        Debug.Log("¡Campera soltada en el piso!");
    }
}