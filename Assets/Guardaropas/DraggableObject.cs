using UnityEngine;
using System.Collections;

public class DraggableObject : MonoBehaviour
{
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb; 
    
    private bool isBeingCarried = false;
    private bool isBlinking = false; 

    [Header("Configuración de Cercanía")]
    [SerializeField] private float distanciaParaInteractuar = 1.5f; 

    [Header("Configuración de Posición al Cargar")]
    [SerializeField] private Vector3 posicionRelativaAlJugador = new Vector3(0.2f, 0.1f, 0f); 

    [Header("Ajuste Anti-Trabas")]
    [SerializeField] private Vector3 offsetAlSoltar = new Vector3(0f, -0.4f, 0f); 

    [Header("Sprites de Estado (¡NUEVO!)")]
    [SerializeField] private Sprite spriteAbollado; // 🔥 Arrastrá acá la imagen del saco hecho un bollo
    private Sprite spriteNormal; // Se guarda solo al iniciar el juego

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>(); 

        // 🔥 NUEVO: Guardamos el sprite original para poder recuperarlo al soltarlo
        if (spriteRenderer != null)
        {
            spriteNormal = spriteRenderer.sprite;
        }

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
        if (isBeingCarried && playerTransform != null)
        {
            transform.position = playerTransform.position + posicionRelativaAlJugador;
        }

        if (Input.GetKeyDown(KeyCode.Space) && playerTransform != null)
        {
            if (isBeingCarried)
            {
                DropCoat();
                return;
            }

            float distanciaActual = Vector2.Distance(transform.position, playerTransform.position);

            if (distanciaActual <= distanciaParaInteractuar)
            {
                DraggableObject camperaEquipada = playerTransform.GetComponentInChildren<DraggableObject>();

                if (camperaEquipada == null)
                {
                    GrabCoat();
                }
                else
                {
                    Debug.Log("¡Ya estás cargando una campera! Tenés que soltarla antes de agarrar otra.");
                }
            }
        }
    }

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

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
            spriteRenderer.sortingOrder = 12; 

            // 🔥 NUEVO: Cambiamos la imagen al modelo abollado/hecho un bollo
            if (spriteAbollado != null)
            {
                spriteRenderer.sprite = spriteAbollado;
            }
        }

        if (playerTransform != null)
        {
            transform.SetParent(playerTransform);
        }

        Debug.Log("¡Campera agarrada y abollada con éxito!");
    }

    public void DropCoat()
    {
        isBeingCarried = false;
        isBlinking = false;

        transform.SetParent(null);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        transform.position += offsetAlSoltar;

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 2; 

            // 🔥 NUEVO: Cuando la dejás en el piso, vuelve a recuperar su forma estirada
            if (spriteNormal != null)
            {
                spriteRenderer.sprite = spriteNormal;
            }
        }

        Debug.Log("¡Campera soltada y estirada en el piso!");
    }
}