using UnityEngine;
using System.Collections;

public class QuinceBoost : MonoBehaviour
{
    private bool efectoActivo = false;

    [Header("Configuración del Boost")]
    [SerializeField] private float duracionBoost = 15f;
    [SerializeField] private float multiplicadorVelocidad = 1.5f;

    [Header("Configuración del Brillo Blanco (Bombeo)")]
    [SerializeField] private float escalaBaseBombeo = 1.1f; // El tamaño base del aura por fuera
    [SerializeField] private float intensidadBombeo = 0.05f; 
    [SerializeField] private float velocidadBombeo = 8f; 
    
    [Tooltip("Asigná acá tu Material_BrilloBlanco.")]
    [SerializeField] private Material materialDeBrillo; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (efectoActivo) return;

        if (other.CompareTag("Player"))
        {
            Movimiento jugador = other.GetComponent<Movimiento>();
            if (jugador != null)
            {
                StartCoroutine(DarBoostConBrilloBombeante(jugador));
            }
        }
    }

    IEnumerator DarBoostConBrilloBombeante(Movimiento jugador)
    {
        efectoActivo = true;
        float velocidadOriginal = jugador.velocidad;
        jugador.velocidad = velocidadOriginal * multiplicadorVelocidad;

        SpriteRenderer spriteOriginal = jugador.GetComponent<SpriteRenderer>();
        
        // Creamos el objeto hijo asignándole el nombre correcto
        GameObject objetoBrillo = new GameObject("ContornoBlanco_Bombeante");
        objetoBrillo.transform.SetParent(jugador.transform); 
        objetoBrillo.transform.localPosition = Vector3.zero; 
        
        // Configuramos el SpriteRenderer del hijo
        SpriteRenderer spriteBrillo = objetoBrillo.AddComponent<SpriteRenderer>();
        spriteBrillo.sprite = spriteOriginal.sprite;
        
        // Color blanco fijo
        spriteBrillo.color = Color.white; 
        
        // Orden de capas (atrás del personaje)
        spriteBrillo.sortingLayerID = spriteOriginal.sortingLayerID;
        spriteBrillo.sortingOrder = spriteOriginal.sortingOrder - 1;

        // Asignamos el material desde el casillero
        if (materialDeBrillo != null)
            spriteBrillo.material = materialDeBrillo;
        else
            spriteBrillo.material = spriteOriginal.material; 

        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionBoost)
        {
            if (objetoBrillo != null && spriteOriginal != null)
            {
                // Sincroniza la animación por si camina
                spriteBrillo.sprite = spriteOriginal.sprite;

                // Calculamos el pulso de bombeo
                float factorBombeo = Mathf.Sin(Time.time * velocidadBombeo) * intensidadBombeo;
                float escalaActual = escalaBaseBombeo + factorBombeo;

                // Modificamos la escala ÚNICAMENTE del hijo brillante
                objetoBrillo.transform.localScale = new Vector3(escalaActual, escalaActual, 1f);
            }
            
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        jugador.velocidad = velocidadOriginal;
        if (objetoBrillo != null) Destroy(objetoBrillo); 
        efectoActivo = false;
    }
}