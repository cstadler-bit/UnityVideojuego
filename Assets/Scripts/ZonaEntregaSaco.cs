using UnityEngine;

public class ZonaEntregaSaco : MonoBehaviour
{
    [Header("Referencias a hacer desaparecer")]
    [SerializeField] private GameObject signoExclamacion; // El signo que queremos borrar
    [SerializeField] private GameObject panelPopUpDialogo; // Por si querés que no vuelva a salir el cartel de Gladys

    private bool misionCumplida = false;

    // Este método lo vamos a llamar desde el script que arrastra el saco cuando el jugador lo SUELTE adentro del área
    public void EntregarAbrigo(GameObject objetoSoltado)
    {
        // Si ya ganamos, no hacemos nada
        if (misionCumplida) return;

        // VERIFICACIÓN: Nos fijamos si el objeto que soltó tiene el nombre exacto de tu saco amarillo
        // (Fijate en tu Hierarchy si se llama "sacoamarillo_0" o similar y ponelo igual acá)
        if (objetoSoltado.name.Contains("sacoamarillo"))
        {
            misionCumplida = true;
            
            // 1. Hacemos desaparecer el signo de exclamación por completo
            if (signoExclamacion != null) signoExclamacion.SetActive(false);

            // 2. Desactivamos el script del Pop-up viejo para que Gladys no vuelva a quejarse del taxi si pasás por encima
            TriggerPopUpDirecto scriptViejo = GetComponent<TriggerPopUpDirecto>();
            if (scriptViejo != null) scriptViejo.enabled = false;
            
            // 3. Apagamos el diálogo si había quedado abierto en pantalla
            if (panelPopUpDialogo != null) panelPopUpDialogo.SetActive(false);

            // 4. Hacemos desaparecer el saco de la mano del jugador porque ya lo entregó
            objetoSoltado.SetActive(false);

            Debug.Log("¡Misión completa! Entregaste el saco amarillo y el signo desapareció.");
        }
        else
        {
            Debug.Log("Este no es el saco de la Tía Gladys, es otro objeto.");
        }
    }
}