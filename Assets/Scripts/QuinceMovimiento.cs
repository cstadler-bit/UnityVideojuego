using UnityEngine;

public class QuinceMovimiento : MonoBehaviour
{
    public int prueba = 123;
    public Transform[] puntos;
    public float velocidad = 2f;

    private int puntoActual = 0;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        animator.SetFloat("Horizontal", 0);
        animator.SetFloat("Vertical", -1);
        animator.SetBool("Moving", true);
    }

    void Update()
    {
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

        animator.SetFloat("Horizontal", direccion.x);
        animator.SetFloat("Vertical", direccion.y);
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
}