using UnityEngine;
using System.Collections;

public class AudioTioBorracho : MonoBehaviour
{
    [Header("Configuración del Audio")]
    // 🚨 Arrastrá tu archivo de sonido (.mp3 o .wav) a este casillero en el Inspector
    [SerializeField] private AudioClip sonidoTio; 
    
    // 🔥 Configurado en false para que el audio se pueda reactivar en cada choque
    [SerializeField] private bool sonarUnaSolaVez = false; 
    [SerializeField] private float duracionAudio = 5f; 

    private AudioSource audioSource;
    private bool estaReproduciendo = false;

    void Start()
    {
        // Buscamos o agregamos el componente AudioSource automáticamente en el objeto
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Desactivamos el Play On Awake por código para evitar que suene solo al arrancar la escena
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Si está configurado para sonar una sola vez y ya se ejecutó, frena acá
            if (sonarUnaSolaVez && estaReproduciendo) return;

            if (sonidoTio != null && audioSource != null)
            {
                // Si el jugador lo vuelve a chocar mientras ya sonaba, frenamos la corutina anterior
                // para que el sonido se reinicie limpiamente desde el principio
                StopAllCoroutines();
                StartCoroutine(ReproducirAudioPorTiempo());
            }
            else if (sonidoTio == null)
            {
                Debug.LogWarning($"¡Mora, acordate de arrastrar el sonido al casillero 'Sonido Tio' en {gameObject.name}!");
            }
        }
    }

    IEnumerator ReproducirAudioPorTiempo()
    {
        estaReproduciendo = true;
        Debug.Log("🔊 Choque con el Tío: Reproduciendo sonido.");
        
        audioSource.clip = sonidoTio;
        audioSource.Play();

        // Controla el tiempo de fondo (5 segundos) sin colgar el juego
        yield return new WaitForSeconds(duracionAudio);

        audioSource.Stop();
        estaReproduciendo = false;
        Debug.Log("🔇 Pasaron los 5 segundos y el audio se detuvo.");
    }
}