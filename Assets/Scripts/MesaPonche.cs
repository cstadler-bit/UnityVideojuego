using UnityEngine;
using System.Collections;

public class MesaPonche : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip sonidoPonche;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Movimiento movimiento = other.GetComponent<Movimiento>();

        if (movimiento == null)
            return;

        // Reproduce el sonido
        if (audioSource != null && sonidoPonche != null)
        {
            audioSource.clip = sonidoPonche;
            audioSource.volume = 1f;
            audioSource.Play();
        }

        // Ejecuta la animación
        movimiento.TomarPonche();

        // Calcula cuánto dura la animación
        float duracionAnimacion =
            movimiento.loops *
            movimiento.drinkFrames.Length *
            movimiento.frameTime;

        // Empieza a bajar el volumen un poco antes de terminar
        StartCoroutine(EsperarYDesvanecer(Mathf.Max(0f, duracionAnimacion - 0.4f)));
    }

    private IEnumerator EsperarYDesvanecer(float espera)
    {
        yield return new WaitForSeconds(espera);

        yield return StartCoroutine(FadeOutAudio(0.4f));
    }

    private IEnumerator FadeOutAudio(float duracion)
    {
        if (audioSource == null || !audioSource.isPlaying)
            yield break;

        float volumenInicial = audioSource.volume;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(volumenInicial, 0f, tiempo / duracion);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = volumenInicial;
    }
}