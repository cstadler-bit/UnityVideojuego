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

        // 🔧 FIX: YA NO forzamos DropCoat() acá.
        // Regla actual del juego: si el saco es incorrecto, el niño se lo tiene que QUEDAR encima
        // (eso ya lo maneja GestionMision.../MostrarErrorSacoEquivocado con el cartel de error).
        // Antes, este script llamaba a scriptSaco.DropCoat() para tirar el saco al piso, pero
        // DropCoat() no reseteaba la bandera isWithKid de DraggableObject. Resultado: el saco
        // quedaba tirado en el piso pero "trabado" como si un niño lo siguiera cargando, y nunca
        // más se podía volver a agarrar. Como ahora no debe soltarse, directamente sacamos la
        // llamada. (El empujón físico de abajo tampoco tenía efecto real: mientras el saco está
        // con el niño, su Rigidbody2D es Kinematic, así que AddForce no lo movía).

        // 🔥 PENALIZACIÓN DE ESTRELLAS: Solo le avisa al GameManager la primera vez que se enoja ESTA tía
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