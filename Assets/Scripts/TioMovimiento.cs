using UnityEngine;

public class TioMovimiento : MonoBehaviour
{
    public Transform[] puntos;
    public float velocidad = 2f;

    private int puntoActual = 0;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (puntos.Length == 0) return;

        Vector2 direccion =
            (puntos[puntoActual].position - transform.position).normalized;

        transform.position = Vector2.MoveTowards(
            transform.position,
            puntos[puntoActual].position,
            velocidad * Time.deltaTime
        );

        animator.SetBool("Moving", true);

        animator.SetFloat("MoveX", direccion.x);
        animator.SetFloat("MoveY", direccion.y);

        if (Vector2.Distance(
            transform.position,
            puntos[puntoActual].position) < 0.1f)
        {
            puntoActual++;

            if (puntoActual >= puntos.Length)
            {
                puntoActual = 0;
            }
        }
    }
}