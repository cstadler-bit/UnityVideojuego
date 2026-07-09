using UnityEngine;
using System.Collections;

public class ContactoQuinceanera : MonoBehaviour
{
    [Header("Configuración de Audio")]
    [SerializeField] private AudioClip audioChoque;
    [Range(0f, 1f)] [SerializeField] private float volumen = 1f;

    [Header("Ajustes del Sonido")]
    [SerializeField] private float cooldownSonido = 1.5f; 

    [Header("Configuración de Frenado")]
    [SerializeField] private float tiempoFrenado = 3f;

    // 🔥 CORREGIDO: Ahora sí coincide con 'QuinceMovimiento' de tu Inspector
    private QuinceMovimiento scriptMovimientoQuinceanera; 
    private AudioSource audioSource;
    private Rigidbody2D rbQuinceanera;
    private float tiempoUltimoSonido;
    private bool estaFrenada = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        rbQuinceanera = GetComponent<Rigidbody2D>();

        // 🔥 CORREGIDO: Busca el componente con el nombre exacto
        scriptMovimientoQuinceanera = GetComponent<QuinceMovimiento>();

        if (scriptMovimientoQuinceanera == null)
        {
            Debug.LogError($"[ERROR] No encontré el script 'QuinceMovimiento' en {gameObject.name}. Asegurate de que el script de la foto esté pegado en este mismo objeto.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !estaFrenada)
        {
            if (audioChoque != null && Time.time >= tiempoUltimoSonido + cooldownSonido)
            {
                audioSource.PlayOneShot(audioChoque, volumen);
                tiempoUltimoSonido = Time.time;
            }
            StartCoroutine(FrenarQuinceaneraRoutine());
        }
    }

    IEnumerator FrenarQuinceaneraRoutine()
    {
        estaFrenada = true;

        if (scriptMovimientoQuinceanera != null) scriptMovimientoQuinceanera.enabled = false;

        if (rbQuinceanera != null)
        {
            rbQuinceanera.linearVelocity = Vector2.zero;
            rbQuinceanera.angularVelocity = 0f;
        }

        yield return new WaitForSeconds(tiempoFrenado);

        if (scriptMovimientoQuinceanera != null) scriptMovimientoQuinceanera.enabled = true;
        estaFrenada = false;
    }
}