using UnityEngine;
using System.Collections;

// Poner este script en el GameObject de la PISTA DE BAILE (con un Collider2D en modo Trigger).
// No modifica Movimiento.cs: controla el Animator y el SpriteRenderer del jugador desde afuera.
// También necesita un AudioSource en este mismo GameObject (Unity lo agrega solo si falta).
[RequireComponent(typeof(AudioSource))]
public class PistaDeBaile : MonoBehaviour
{
    [Header("Sprites de la animación de baile (6 frames)")]
    [SerializeField] private Sprite[] spritesBaile; // Arrastrá acá los 6 sprites, en orden

    [Header("Velocidad de la animación")]
    [SerializeField] private float fpsBaile = 8f;

    [Header("Audio de la pista de baile")]
    [SerializeField] private AudioClip clipMusica; // El audio a reproducir (suena UNA vez)

    private Coroutine coroutineBaile;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clipMusica;
        audioSource.loop = false; // 🔧 Ya no repite en loop: suena una sola vez
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Animator animator = other.GetComponent<Animator>();
        SpriteRenderer sr = other.GetComponent<SpriteRenderer>();

        if (sr == null) return; // Sin SpriteRenderer no hay nada para animar

        // 🔧 Ya NO congelamos el movimiento: el jugador puede seguir caminando con WASD
        // mientras se reproduce la animación de baile encima.

        // Apagamos el Animator para que no compita con el cambio manual de sprites de acá abajo
        // (igual seguís moviéndote: Movimiento.cs mueve el transform.position sin depender del Animator)
        if (animator != null)
            animator.enabled = false;

        if (coroutineBaile != null) StopCoroutine(coroutineBaile);
        coroutineBaile = StartCoroutine(Bailar(sr));

        // Arrancamos el audio de la pista de baile (una sola vez)
        if (clipMusica != null)
        {
            audioSource.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (coroutineBaile != null)
        {
            StopCoroutine(coroutineBaile);
            coroutineBaile = null;
        }

        Animator animator = other.GetComponent<Animator>();

        // Reactivamos el Animator para que Movimiento.cs vuelva a controlar la animación normal
        if (animator != null)
            animator.enabled = true;

        // 🔧 Ya no hay nada que descongelar (el jugador nunca se congeló).

        // 🔧 Ya no cortamos el audio acá: como ahora suena una sola vez (sin loop),
        // lo dejamos terminar aunque el jugador ya haya salido de la pista.
    }

    private IEnumerator Bailar(SpriteRenderer sr)
    {
        if (spritesBaile == null || spritesBaile.Length == 0)
        {
            Debug.LogWarning($"{gameObject.name}: 'Sprites Baile' está vacío en el Inspector. Asigná los 6 sprites.");
            yield break;
        }

        int frame = 0;
        float espera = 1f / fpsBaile;

        while (true)
        {
            sr.sprite = spritesBaile[frame % spritesBaile.Length];
            frame++;
            yield return new WaitForSeconds(espera);
        }
    }
}