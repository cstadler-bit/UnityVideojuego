using UnityEngine;

public class MovimientoNino : MonoBehaviour
{
    [Header("Configuración")]
    public Transform jugador;
    public float velocidad = 4f;
    public float distanciaParaParar = 1.0f;
    public float distanciaParaEmpezar = 1.5f;
    public float fpsAnimacion = 8f;

    [Header("Sprites Caminando (Saltando)")]
    public Sprite[] spritesAbajo;
    public Sprite[] spritesArriba;
    public Sprite[] spritesDerecha;
    public Sprite[] spritesIzquierda;

    [Header("Sprites Idle (Quieto)")]
    public Sprite idleAbajo;
    public Sprite idleArriba;
    public Sprite idleDerecha;
    public Sprite idleIzquierda;

    private SpriteRenderer sr;
    private Vector2 ultimaDir = new Vector2(0, -1); // Mira abajo al iniciar
    private bool estaCaminando = false;
    private int frameActual = 0;
    private float tiempoAcumulado = 0f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        // Lógica de Histéresis (Evita el parpadeo)
        if (distancia > distanciaParaEmpezar) estaCaminando = true;
        else if (distancia < distanciaParaParar) estaCaminando = false;

        if (estaCaminando)
        {
            MoverseSaltando();
        }
        else
        {
            MostrarIdle();
        }
    }

    void MoverseSaltando()
    {
        Vector2 vectorHaciaJugador = (Vector2)jugador.position - (Vector2)transform.position;
        
        // Movimiento Cardinal (Sin diagonales)
        Vector2 direccion;
        if (Mathf.Abs(vectorHaciaJugador.x) > Mathf.Abs(vectorHaciaJugador.y))
            direccion = new Vector2(vectorHaciaJugador.x, 0).normalized;
        else
            direccion = new Vector2(0, vectorHaciaJugador.y).normalized;

        transform.position += (Vector3)direccion * velocidad * Time.deltaTime;
        ultimaDir = direccion;

        // Animación de Salto (Caminar)
        tiempoAcumulado += Time.deltaTime;
        if (tiempoAcumulado >= (1f / fpsAnimacion))
        {
            frameActual++;
            tiempoAcumulado = 0f;
        }

        Sprite[] setActual = ElegirArraySegunDireccion();
        frameActual %= setActual.Length;
        sr.sprite = setActual[frameActual];
    }

    void MostrarIdle()
    {
        frameActual = 0; // Reseteamos la animación
        tiempoAcumulado = 0f;

        // Elegimos el Idle según la última dirección
        if (Mathf.Abs(ultimaDir.y) > Mathf.Abs(ultimaDir.x))
            sr.sprite = ultimaDir.y > 0 ? idleArriba : idleAbajo;
        else
            sr.sprite = ultimaDir.x > 0 ? idleDerecha : idleIzquierda;
    }

    Sprite[] ElegirArraySegunDireccion()
    {
        if (Mathf.Abs(ultimaDir.y) > Mathf.Abs(ultimaDir.x))
            return ultimaDir.y > 0 ? spritesArriba : spritesAbajo;
        else
            return ultimaDir.x > 0 ? spritesDerecha : spritesIzquierda;
    }
}