using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb; 
    
    private bool isBeingCarried = false;
    private bool isBlinking = false; 
    private bool isWithKid = false; // Candado de posesión de este saco

    [Header("Configuración de Cercanía")]
    [SerializeField] private float distanciaParaInteractuar = 1.5f; 

    [Header("Configuración de Posición al Cargar (Jugador)")]
    [SerializeField] private Vector3 posicionRelativaAlJugador = new Vector3(0.2f, 0.1f, 0f); 

    [Header("Configuración de Posición al Cargar (Niños)")]
    [Tooltip("Ajustá el eje Y para bajar o subir el saco respecto al centro del nene/nena")]
    [SerializeField] private Vector3 posicionRelativaAlNino = new Vector3(0f, 0.2f, 0f); 

    [Header("Ajuste Anti-Trabas")]
    [SerializeField] private Vector3 offsetAlSoltar = new Vector3(0f, -0.4f, 0f); 

    [Header("Sprites de Estado")]
    [SerializeField] private Sprite spriteAbollado; 
    private Sprite spriteNormal; 

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>(); 
        if (spriteRenderer != null) spriteNormal = spriteRenderer.sprite;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    void Update()
    {
        // 🔥 SOLUCIÓN AL BUG: Si el saco lo tiene un nene, SOLO se suelta si el jugador está al lado de ese nene
        if (isWithKid) 
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) 
            {
                // Verificamos si estamos físicamente cerca del niño que carga ESTE saco
                if (playerTransform != null && transform.parent != null)
                {
                    float distanciaAlAyudante = Vector2.Distance(playerTransform.position, transform.parent.position);
                    
                    if (distanciaAlAyudante <= distanciaParaInteractuar)
                    {
                        DropCoatFromKid();
                    }
                }
            }
            return; // Bloquea el resto del código de este saco mientras lo tenga el niño
        }

        // Movimiento suave cuando lo lleva el jugador
        if (isBeingCarried && playerTransform != null)
        {
            transform.position = playerTransform.position + posicionRelativaAlJugador;
        }

        // Lógica de interactuar en el piso
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
                NinoInteraction[] todosLosNinos = Object.FindObjectsByType<NinoInteraction>(FindObjectsSortMode.None);
                NinoInteraction ninoElegido = null;

                // 🔧 DIAGNÓSTICO: lista todos los NinoInteraction que Unity encuentra en la escena,
                // su prioridad, si están activos y si ya tienen un saco encima. Esto nos va a decir
                // en la consola por qué el segundo niño no está siendo elegido.
                Debug.Log($"[Diagnóstico sacos] Se encontraron {todosLosNinos.Length} NinoInteraction en la escena:");
                foreach (NinoInteraction n in todosLosNinos)
                {
                    bool ocupado = n.GetComponentInChildren<DraggableObject>() != null;
                    Debug.Log($"  - {n.gameObject.name} | prioridad={n.prioridad} | activo={n.gameObject.activeInHierarchy} | ocupado={ocupado}");
                }

                // INTENTO 1: Buscamos primero al Nene (Prioridad 1) si está libre
                foreach (NinoInteraction nino in todosLosNinos)
                {
                    if (nino.gameObject.activeInHierarchy && nino.prioridad == 1)
                    {
                        if (nino.GetComponentInChildren<DraggableObject>() == null)
                        {
                            ninoElegido = nino;
                            break;
                        }
                    }
                }

                // INTENTO 2: Si el nene está ocupado, buscamos a la Nena (Prioridad 2)
                if (ninoElegido == null)
                {
                    foreach (NinoInteraction nino in todosLosNinos)
                    {
                        if (nino.gameObject.activeInHierarchy && nino.prioridad == 2)
                        {
                            if (nino.GetComponentInChildren<DraggableObject>() == null)
                            {
                                ninoElegido = nino;
                                break;
                            }
                        }
                    }
                }

                // Entregamos el saco según el veredicto
                if (ninoElegido != null)
                {
                    Debug.Log("Saco entregado a: " + ninoElegido.gameObject.name);
                    TransferirA(ninoElegido.transform);
                }
                else
                {
                    // Si ambos niños están full ocupados, lo agarra el jugador
                    Debug.Log("Todos los niños ocupados. Lo lleva el jugador.");
                    IntentarAgarrarJugador();
                }
            }
        }
    }

    void IntentarAgarrarJugador()
    {
        DraggableObject camperaEquipada = playerTransform.GetComponentInChildren<DraggableObject>();
        if (camperaEquipada == null) GrabCoat();
    }

    void GrabCoat()
    {
        isBeingCarried = true;
        if (rb != null) { rb.bodyType = RigidbodyType2D.Kinematic; rb.linearVelocity = Vector2.zero; }
        if (spriteRenderer != null) { spriteRenderer.sortingOrder = 12; if (spriteAbollado != null) spriteRenderer.sprite = spriteAbollado; }
        if (playerTransform != null) transform.SetParent(playerTransform);
    }

    public void DropCoat()
    {
        isBeingCarried = false;
        isWithKid = false; // 🔧 FIX: si el saco lo tenía un niño y se fuerza el drop desde afuera
                            // (ej. DetectorCamperaEquivocada), había que liberar también este candado.
                            // Si no, el saco quedaba tirado en el piso pero con isWithKid = true para
                            // siempre, y el Update() de este script lo bloqueaba con el "return" del
                            // bloque isWithKid, haciendo que nunca más se pudiera volver a agarrar.
        transform.SetParent(null);
        transform.position += offsetAlSoltar;
        if (spriteRenderer != null) { spriteRenderer.sortingOrder = 2; if (spriteNormal != null) spriteRenderer.sprite = spriteNormal; }
    }

    public void TransferirA(Transform nuevoPadre)
    {
        isWithKid = true;      
        isBeingCarried = false; 
        
        transform.SetParent(nuevoPadre); 
        transform.localPosition = posicionRelativaAlNino; 

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }
        
        if (spriteRenderer != null) 
        { 
            spriteRenderer.sortingOrder = 12; 
            if (spriteAbollado != null) spriteRenderer.sprite = spriteAbollado; 
        }
    }

    public void DropCoatFromKid()
    {
        isWithKid = false; 
        transform.SetParent(null); 
        transform.position += offsetAlSoltar; 

        if (spriteRenderer != null) 
        { 
            spriteRenderer.sortingOrder = 2; 
            if (spriteNormal != null) spriteRenderer.sprite = spriteNormal; 
        }
        Debug.Log("Un niño soltó el saco.");
    }
}