using UnityEngine;
using System.Collections; // Necesario para las corrutinas

public class InterruptorCuadrado : MonoBehaviour
{
    [Header("Referencia del Cuadrado")]
    [SerializeField] private GameObject cuadradoObjeto; 

    [Header("Configuración del Degradé")]
    [SerializeField] private float tiempoTransicion = 1.5f; // Tus 1.5 segundos
    [SerializeField] private float opacidadMinima = 0.2f;   // Tu intensidad mínima de 0.2
    [SerializeField] private float opacidadMaxima = 1.0f;   // Tu intensidad máxima de 1.0

    private bool cuadradoOculto = false; 
    private Coroutine corrutinaDesvanecer;
    
    // Soportamos tanto Sprites del mapa como Imágenes del Canvas
    private SpriteRenderer spriteRenderer;
    private UnityEngine.UI.Image imagenUI;

    void Start()
    {
        if (cuadradoObjeto != null)
        {
            // Buscamos si el cuadrado es un objeto del mapa o de la UI
            spriteRenderer = cuadradoObjeto.GetComponent<SpriteRenderer>();
            imagenUI = cuadradoObjeto.GetComponent<UnityEngine.UI.Image>();

            // Arranca al mango (1.0 de opacidad)
            SetOpacidad(opacidadMaxima);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cuadradoOculto = !cuadradoOculto;

            // Frenamos cualquier transición vieja para que no tironee
            if (corrutinaDesvanecer != null) StopCoroutine(corrutinaDesvanecer);

            if (cuadradoOculto)
            {
                // CAMINO DE IDA: Se desvanece de 1 a 0.2
                corrutinaDesvanecer = StartCoroutine(TransicionOpacidad(opacidadMaxima, opacidadMinima));
                Debug.Log("Desvaneciendo cuadrado a 0.2...");
            }
            else
            {
                // CAMINO DE VUELTA: Reaparece de 0.2 a 1
                corrutinaDesvanecer = StartCoroutine(TransicionOpacidad(opacidadMinima, opacidadMaxima));
                Debug.Log("Restaurando cuadrado a 1.0...");
            }
        }
    }

    // Corrutina que hace el cálculo frame a frame
    private IEnumerator TransicionOpacidad(float inicio, float fin)
    {
        float tiempoPasado = 0f;

        while (tiempoPasado < tiempoTransicion)
        {
            tiempoPasado += Time.deltaTime;
            float porcentaje = tiempoPasado / tiempoTransicion;

            // Interpola suavemente el valor alpha
            float alphaActual = Mathf.Lerp(inicio, fin, porcentaje);
            SetOpacidad(alphaActual);

            yield return null;
        }

        // Aseguramos el valor final exacto
        SetOpacidad(fin);
    }

    // Método auxiliar para aplicar el alpha sea lo que sea el cuadrado
    private void SetOpacidad(float valorAlpha)
    {
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            spriteRenderer.color = new Color(c.r, c.g, c.b, valorAlpha);
        }
        else if (imagenUI != null)
        {
            Color c = imagenUI.color;
            imagenUI.color = new Color(c.r, c.g, c.b, valorAlpha);
        }
    }
}