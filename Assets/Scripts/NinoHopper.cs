using System.Collections.Generic; // 🔥 Obligatorio para usar listas de huellas
using UnityEngine;

public class MovimientoCaminante : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    [Tooltip("El objetivo a seguir (Player para el nene, Nene para la nena)")]
    public Transform jugador; 
    public float velocidad = 4f;
    [Tooltip("Distancia fija que se mantendrá entre cada miembro de la fila")]
    public float distanciaEntrePatitos = 1.2f; 

    [Header("Animación")]
    public float fpsAnimacion = 8f;
    public Sprite[] spritesAbajo;
    public Sprite[] spritesArriba;
    public Sprite[] spritesDerecha;
    public Sprite[] spritesIzquierda;

    private SpriteRenderer sr;
    private Vector2 direccion = Vector2.zero;
    private Vector2 ultimaDireccion = Vector2.zero;
    private int frameActual = 0;
    private float tiempoAnimacion = 0f;

    // 🔥 La lista mágica que guarda el historial de pasos del líder
    private List<Vector3> historialHuellas = new List<Vector3>();

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();

        if (jugador != null)
        {
            // Registramos la posición inicial del líder para arrancar el camino
            historialHuellas.Add(jugador.position);
        }
    }

    void Update()
    {
        if (jugador == null) return;

        // 1. Registrar los pasos del líder si se movió una distancia mínima de la última huella
        Vector3 ultimaHuellaRegistrada = historialHuellas[historialHuellas.Count - 1];
        if (Vector3.Distance(jugador.position, ultimaHuellaRegistrada) > 0.05f)
        {
            historialHuellas.Add(jugador.position);
        }

        // 2. Calcular cuánto camino total hay acumulado en el historial de huellas
        float distanciaTotalCamino = Vector3.Distance(transform.position, historialHuellas[0]);
        for (int i = 0; i < historialHuellas.Count - 1; i++)
        {
            distanciaTotalCamino += Vector3.Distance(historialHuellas[i], historialHuellas[i + 1]);
        }

        // 3. Si el líder se alejó más que la distancia permitida, empezamos a seguir las huellas
        if (distanciaTotalCamino > distanciaEntrePatitos && historialHuellas.Count > 0)
        {
            Vector3 huellaObjetivo = historialHuellas[0];

            // Si ya estamos re cerca de la huella actual, la borramos y pasamos a la siguiente del camino
            if (Vector3.Distance(transform.position, huellaObjetivo) < 0.1f)
            {
                historialHuellas.RemoveAt(0);
                if (historialHuellas.Count > 0) huellaObjetivo = historialHuellas[0];
            }

            // Calcular delta hacia la huella para que sigan funcionando tus animaciones cardinales
            Vector2 delta = huellaObjetivo - transform.position;
            
            if (delta.magnitude > 0.01f)
            {
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    direccion = new Vector2(Mathf.Sign(delta.x), 0);
                else
                    direccion = new Vector2(0, Mathf.Sign(delta.y));

                // 🔥 MOVIMIENTO SEGURO: Nos movemos hacia el punto exacto de la huella, sin cortar camino
                transform.position = Vector3.MoveTowards(transform.position, huellaObjetivo, velocidad * Time.deltaTime);

                // Resetear animación si cambia la dirección cardinal
                if (direccion != ultimaDireccion)
                {
                    frameActual = 0;
                    ultimaDireccion = direccion;
                }

                // Actualizar los frames de animación
                tiempoAnimacion += Time.deltaTime;
                if (tiempoAnimacion >= (1f / fpsAnimacion))
                {
                    frameActual++;
                    tiempoAnimacion = 0f;
                }
            }
        }
        else
        {
            // Si la fila se detuvo o está compacta, el niño se queda quieto
            direccion = Vector2.zero;
        }

        AplicarSprite();
    }

    void AplicarSprite()
    {
        if (sr == null) return; 

        Sprite[] setActual = ElegirArraySegunDireccion();
        if (setActual.Length > 0)
        {
            sr.sprite = setActual[frameActual % setActual.Length];
        }
    }

    Sprite[] ElegirArraySegunDireccion()
    {
        if (direccion == Vector2.zero) 
        {
            if (Mathf.Abs(ultimaDireccion.y) > Mathf.Abs(ultimaDireccion.x))
                return ultimaDireccion.y > 0 ? spritesArriba : spritesAbajo;
            else
                return ultimaDireccion.x > 0 ? spritesDerecha : spritesIzquierda;
        }

        if (Mathf.Abs(direccion.y) > Mathf.Abs(direccion.x))
            return direccion.y > 0 ? spritesArriba : spritesAbajo;
        else
            return ultimaDireccion.x > 0 ? spritesDerecha : spritesIzquierda;
    }
}