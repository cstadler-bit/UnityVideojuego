using UnityEngine;

public class NinoInteraction : MonoBehaviour
{
    [Header("Configuración de Turnos")]
    [Tooltip("1 = Toma el saco primero (Nene), 2 = Toma el saco segundo (Nena)")]
    public int prioridad = 1;
}