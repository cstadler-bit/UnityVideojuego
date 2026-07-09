using UnityEngine;

public class EfectoBombeo : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 2f;    // Qué tan rápido late
    public float intensidad = 0.1f; // Qué tanto crece (0.1 = 10% más)

    private Vector3 escalaOriginal;

    void Start()
    {
        // Guardamos el tamaño original para no perderlo nunca
        escalaOriginal = transform.localScale;
    }

    void Update()
    {
        // El seno oscila entre -1 y 1. Lo multiplicamos por la intensidad.
        float pulso = Mathf.Sin(Time.time * velocidad) * intensidad;

        // Aplicamos el tamaño original + el pulso en X e Y
        transform.localScale = escalaOriginal + new Vector3(pulso, pulso, 0);
    }
}