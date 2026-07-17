using UnityEngine;
using UnityEngine.UI;

public class EvolucionAbuelo : MonoBehaviour
{
    [Header("Objetos de la UI (Jerarquía Canvas)")]
    [SerializeField] private Image filtroRojoUI;
    [SerializeField] private GameObject fondoIncognito;

    [Header("Objetos de las Caras en la UI")]
    [SerializeField] private GameObject caraFelizUI;
    [SerializeField] private GameObject caraPreocupadaUI;
    [SerializeField] private GameObject caraEnojadaUI;

    [Header("Tiempos de enojo")]
    [SerializeField] private float tiempoParaPreocupado = 35f;
    [SerializeField] private float tiempoParaEnojado = 70f;
    [SerializeField] private float tiempoParaFinal = 100f;

    private int faseActual = 0;
    private float cronometro = 0f;
    private bool temporizadorActivo = false;
    private bool estaTitilando = false;

    void Start()
    {
        if (fondoIncognito != null)
            fondoIncognito.SetActive(true);

        if (caraFelizUI != null)
            caraFelizUI.SetActive(false);

        if (caraPreocupadaUI != null)
            caraPreocupadaUI.SetActive(false);

        if (caraEnojadaUI != null)
            caraEnojadaUI.SetActive(false);

        if (filtroRojoUI != null)
        {
            filtroRojoUI.gameObject.SetActive(true);
            filtroRojoUI.fillAmount = 0f;
        }

        faseActual = 0;
        cronometro = 0f;
        temporizadorActivo = false;
        estaTitilando = false;
    }

    void Update()
    {
        if (!temporizadorActivo) return;

        cronometro += Time.deltaTime;

        // Fase 2: pasa a preocupado más lento
        if (faseActual == 1 && cronometro >= tiempoParaPreocupado)
        {
            faseActual = 2;

            if (caraFelizUI != null)
                caraFelizUI.SetActive(false);

            if (caraPreocupadaUI != null)
                caraPreocupadaUI.SetActive(true);

            if (filtroRojoUI != null)
                filtroRojoUI.fillAmount = 0.66f;

            Debug.Log("Abuelo preocupado.");
        }

        // Fase 3: pasa a enojado más lento
        if (faseActual == 2 && cronometro >= tiempoParaEnojado)
        {
            faseActual = 3;

            if (caraPreocupadaUI != null)
                caraPreocupadaUI.SetActive(false);

            if (caraEnojadaUI != null)
                caraEnojadaUI.SetActive(true);

            if (filtroRojoUI != null)
                filtroRojoUI.fillAmount = 1f;

            estaTitilando = true;

            Debug.Log("Abuelo enojado y titilando.");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegistrarTiaEnojoTotal();
            }
        }

        // Titileo mientras está enojado
        if (estaTitilando && cronometro < tiempoParaFinal)
        {
            if (caraEnojadaUI != null)
            {
                bool mostrar = Mathf.PingPong(Time.time * 9f, 1f) > 0.5f;
                caraEnojadaUI.SetActive(mostrar);
            }
        }

        // Estado final
        if (cronometro >= tiempoParaFinal && faseActual == 3)
        {
            faseActual = 4;
            estaTitilando = false;

            if (caraEnojadaUI != null)
            {
                caraEnojadaUI.SetActive(true);

                Image imgEnojada = caraEnojadaUI.GetComponent<Image>();

                if (imgEnojada != null)
                {
                    imgEnojada.material = new Material(Shader.Find("UI/Default"));
                    imgEnojada.color = new Color(0.25f, 0.25f, 0.25f, 1f);
                }
            }

            if (filtroRojoUI != null)
                filtroRojoUI.gameObject.SetActive(false);

            Debug.Log("Abuelo llegó al estado final de enojo.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !temporizadorActivo && faseActual == 0)
        {
            RevelarAbuelo();
        }
    }

    private void RevelarAbuelo()
    {
        if (caraEnojadaUI != null)
        {
            Image imgEnojada = caraEnojadaUI.GetComponent<Image>();

            if (imgEnojada != null)
                imgEnojada.color = Color.white;
        }

        temporizadorActivo = true;
        faseActual = 1;
        cronometro = 0f;

        if (fondoIncognito != null)
            fondoIncognito.SetActive(false);

        if (caraFelizUI != null)
            caraFelizUI.SetActive(true);

        if (caraPreocupadaUI != null)
            caraPreocupadaUI.SetActive(false);

        if (caraEnojadaUI != null)
            caraEnojadaUI.SetActive(false);

        if (filtroRojoUI != null)
        {
            filtroRojoUI.gameObject.SetActive(true);
            filtroRojoUI.fillAmount = 0.33f;
        }

        Debug.Log("Inicio: Abuelo feliz.");
    }

    public void ForzarCaraFeliz()
    {
        temporizadorActivo = false;
        estaTitilando = false;
        faseActual = 5;

        if (fondoIncognito != null)
            fondoIncognito.SetActive(false);

        if (caraPreocupadaUI != null)
            caraPreocupadaUI.SetActive(false);

        if (caraEnojadaUI != null)
            caraEnojadaUI.SetActive(false);

        if (caraFelizUI != null)
        {
            caraFelizUI.SetActive(true);

            Image imgFeliz = caraFelizUI.GetComponent<Image>();

            if (imgFeliz != null)
                imgFeliz.color = Color.white;
        }

        if (filtroRojoUI != null)
        {
            filtroRojoUI.gameObject.SetActive(true);
            filtroRojoUI.fillAmount = 0f;
        }

        Debug.Log("ForzarCaraFeliz ejecutado: El abuelo quedó feliz.");
    }
}