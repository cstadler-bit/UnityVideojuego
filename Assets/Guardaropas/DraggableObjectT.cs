using UnityEngine;
using System.Collections;

public class DraggableObject : MonoBehaviour
{
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;
    
    private bool isBeingCarried = false;
    private bool isBlinking = false; // Nos avisa si la campera está titilando en este momento

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
        // Si el personaje ya la agarró, se mueve pegada a él a donde sea que vayas con WASD
        if (isBeingCarried && playerTransform != null)
        {
            // Podés cambiar estos números (X, Y) para acomodar la campera sobre el cuerpo de tu personaje
            transform.position = playerTransform.position + new Vector3(0.2f, 0.1f, 0f);
        }
    }

    void OnMouseDown()
    {
        // Si el personaje ya la lleva puesta, no hace falta hacer más nada
        if (isBeingCarried) return;

        // SEGUNDO CLIC: Si la tocás MIENTRAS está titilando, se acopla al personaje
        if (isBlinking)
        {
            GrabCoat();
        }
        // PRIMER CLIC: Si está normal, empieza a titilar
        else
        {
            StartCoroutine(BlinkEffect());
        }
    }

    // Efecto de titileo por opacidad (cambia entre 100% y 40% de transparencia)
    IEnumerator BlinkEffect()
    {
        isBlinking = true;
        Debug.Log("Primer clic: Titilando " + gameObject.name);

        // Va a parpadear unas 6 veces (durante unos 2.4 segundos). 
        // Si en ese tiempo no la volvés a tocar, el efecto se corta y vuelve a la normalidad.
        for (int i = 0; i < 6; i++)
        {
            // Si la agarraste a mitad del parpadeo, frena la animación para que no quede invisible
            if (isBeingCarried) yield break;

            if (spriteRenderer != null)
            {
                // Baja la opacidad al 40% (transparente)
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.4f);
                yield return new WaitForSeconds(0.2f);
                
                // Vuelve al 100% (sólido)
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.2f);
            }
        }

        // Si se acabó el tiempo y no la tocaste de nuevo, se apaga el modo selección
        isBlinking = false;
        Debug.Log("Se terminó el tiempo para agarrar la campera.");
    }

    void GrabCoat()
    {
        isBeingCarried = true;
        isBlinking = false;
        
        // Frenamos cualquier parpadeo que haya quedado activo para que quede 100% visible
        StopAllCoroutines();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        // Se vuelve hija del Player en la jerarquía para que herede su movimiento automáticamente
        if (playerTransform != null)
        {
            transform.SetParent(playerTransform);
        }

        Debug.Log("¡Segundo clic! La campera se acopló al personaje.");
    }
}