using UnityEngine;
using System.Collections;

public class DraggableObject : MonoBehaviour
{
    [Header("Configuración de Escala")]
    [SerializeField] private float scaleMultiplier = 1.2f; // Cuánto se agranda
    
    private Vector3 originalScale;
    private Vector3 targetScale;
    private SpriteRenderer spriteRenderer;
    private int originalSortingOrder;
    private bool isSelected = false;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalSortingOrder = spriteRenderer.sortingOrder;
        }
    }

    void Update()
    {
        // Escalado suave hacia el tamaño objetivo en su lugar
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 12f);
    }

    void OnMouseDown()
    {
        isSelected = true;
        targetScale = originalScale * scaleMultiplier;

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = originalSortingOrder + 50; // Se pone al frente
            StartCoroutine(BlinkEffect()); // Arranca el titileo
        }
    }

    void OnMouseUp()
    {
        isSelected = false;
        targetScale = originalScale;

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = originalSortingOrder;
            // Forzamos a que recupere el color y opacidad original al soltar
            spriteRenderer.color = Color.white; 
        }
    }

    // Corrutina para el efecto de titileo (parpadeo)
    IEnumerator BlinkEffect()
    {
        while (isSelected)
        {
            if (spriteRenderer != null)
            {
                // Baja la opacidad al 40%
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.4f);
                yield return new WaitForSeconds(0.12f); // Espera un instante
                
                // Vuelve a la opacidad total (100%)
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.12f);
            }
            else
            {
                yield return null;
            }
        }
    }
}