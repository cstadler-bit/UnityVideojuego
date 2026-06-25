using UnityEngine;

public class AudioTioBorracho : MonoBehaviour
{
    [Header("Configuración del Audio")]
    [SerializeField] private AudioClip sonidoTio;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = true;

        if (sonidoTio != null)
        {
            audioSource.clip = sonidoTio;
        }
    }

    public void ReproducirBalbuceo()
    {
        if (audioSource == null) return;

        if (sonidoTio != null)
        {
            audioSource.clip = sonidoTio;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void DetenerBalbuceo()
    {
        if (audioSource == null) return;

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}