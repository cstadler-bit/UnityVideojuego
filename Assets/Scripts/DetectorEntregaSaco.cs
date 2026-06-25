using UnityEngine;

public class DetectorEntregaSaco : MonoBehaviour
{
    [Header("Referencias de la Misión")]
    [SerializeField] private GameObject signoExclamacion;   // El signo flotante que querés borrar
    [SerializeField] private GameObject panelPopUpDialogo;  // El cartel con el diálogo de la Tía Gladys

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name.Contains("sacoamarillo"))
        {
            // Verificamos si el jugador soltó el mouse o la barra espaciadora
            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
            {
                CompletarMision(other.gameObject);
            }
        }
    }

    private void CompletarMision(GameObject saco)
    {
        Debug.Log("¡Misión cumplida! Desactivando componentes...");

        // 1. Apagamos el script del Pop-up automático para que no reactive nada al movernos
        TriggerPopUpDirecto scriptViejo = GetComponent<TriggerPopUpDirecto>();
        if (scriptViejo != null) 
        {
            scriptViejo.enabled = false; 
        }

        // 2. Hacemos desaparecer el signo de exclamación DE RAÍZ
        if (signoExclamacion != null) 
        {
            signoExclamacion.SetActive(false);
            Debug.Log("Signo de exclamación desactivado con éxito.");
        }
        else 
        {
            Debug.LogError("¡Ojo! El casillero del Signo de Exclamación está vacío en el Inspector.");
        }

        // 3. Apagamos el diálogo de la Tía Gladys si estaba abierto
        if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);

        // 4. Hacemos desaparecer el saco de la pantalla
        saco.SetActive(false);
        
        // Desactivamos este script para que no se vuelva a ejecutar
        this.enabled = false;
    }
}
