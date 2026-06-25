using UnityEngine;

public class TioMovimiento : MonoBehaviour
{
    public Transform[] puntos;
    public float velocidad = 2f;

    private int puntoActual = 0;
    private Animator animator;
    private bool detenido = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveY", -1);
        animator.SetBool("Moving", true);
        animator.speed = 1f;
    }

    void Update()
    {
        if (detenido)
        {
            return;
        }

        if (puntos.Length == 0)
            return;

        Vector2 destino = puntos[puntoActual].position;

        Vector2 direccion =
            (destino - (Vector2)transform.position).normalized;

        transform.position = Vector2.MoveTowards(
            transform.position,
            destino,
            velocidad * Time.deltaTime
        );

        animator.SetFloat("MoveX", direccion.x);
        animator.SetFloat("MoveY", direccion.y);
        animator.SetBool("Moving", true);

        if (Vector2.Distance(transform.position, destino) < 0.1f)
        {
            puntoActual++;

            if (puntoActual >= puntos.Length)
            {
                puntoActual = 0;
            }
        }
    }

    public void DetenerTio()
    {
        detenido = true;

        // Congela la animación en el frame exacto donde quedó.
        animator.speed = 0f;
    }

    public void ReanudarTio()
    {
        detenido = false;

        // Reactiva la animación.
        animator.speed = 1f;
        animator.SetBool("Moving", true);
    }
}