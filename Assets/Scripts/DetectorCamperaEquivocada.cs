using UnityEngine;

public class DetectorCamperaEquivocada : MonoBehaviour
{
    [Header("Configuración de la Misión")]
    [SerializeField] private string nombreSacoCorrecto = "sacopeludo"; // O "sacoamarillo" según el personaje

    private bool yaSeEnojo = false; // Evita que una misma tía sume múltiples errores si el jugador la spamea

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si el objeto que entra es el jugador directamente, lo ignoramos
        if (other.CompareTag("Player")) return;

        string nombreObjeto = other.gameObject.name.ToLower();

        // Si entra un saco, pero NO contiene el nombre del saco correcto de este personaje... ¡Se enoja!
        if (nombreObjeto.Contains("saco") && !nombreObjeto.Contains(nombreSacoCorrecto.ToLower()))
        {
            ProcesarCamperaEquivocada(other.gameObject);
        }
    }

    private void ProcesarCamperaEquivocada(GameObject sacoEquivocado)
    {
        Debug.Log($"❌ ¡Campera equivocada! Se intentó entregar: {sacoEquivocado.name} a {gameObject.name}. Se esperaba: {nombreSacoCorrecto}");

        // 1. 🔥 LA CLAVE: Forzamos al saco a soltarse y caer al piso usando su propio script
        // ¡YA NO lo apagamos! Así el jugador puede levantarlo del piso y llevárselo a la tía correcta.
        DraggableObject scriptSaco = sacoEquivocado.GetComponent<DraggableObject>();
        if (scriptSaco != null)
        {
            scriptSaco.DropCoat(); 
        }

        // [OPCIONAL] Le metemos un pequeño empujón físico para que el saco "rebote" de la tía y se note el rechazo
        Rigidbody2D rbSaco = sacoEquivocado.GetComponent<Rigidbody2D>();
        if (rbSaco != null)
        {
            Vector2 direccionEmpuje = (sacoEquivocado.transform.position - transform.position).normalized;
            rbSaco.AddForce(direccionEmpuje * 4f, ForceMode2D.Impulse);
        }

        // 2. 🔥 PENALIZACIÓN DE ESTRELLAS: Solo le avisa al GameManager la primera vez que se enoja ESTA tía
        if (!yaSeEnojo)
        {
            yaSeEnojo = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegistrarPersonajeEnojado();
            }
            else
            {
                Debug.LogWarning("¡Ojo! No se encontró el GameManager en la escena para restar la estrella.");
            }

            // 3. 👀 FEEDBACK VISUAL: Descomentá esto si querés que tu script de evolución cambie su apariencia a enojada
            /*
            EvolucionGladys evo = GetComponent<EvolucionGladys>();
            if (evo != null)
            {
                // evo.ForzarCaraEnojada(); // O el método que maneje sus expresiones
            }
            */
        }
    }
}