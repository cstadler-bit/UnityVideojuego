using UnityEngine;

public class Charco : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Movimiento movimiento = other.GetComponent<Movimiento>();

        if (movimiento != null)
            movimiento.Resbalar();
    }
}