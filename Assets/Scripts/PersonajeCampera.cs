using UnityEngine;

public class PersonajeCampera : MonoBehaviour
{
    [Header("Movimiento hacia la salida")]
    public Transform[] puntosSalida;
    public float velocidad = 2f;

    private Animator animator;
    private int puntoActual = 0;
    private bool seEstaRetirando = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetBool("TieneCampera", false);
            animator.SetBool("SeRetira", false);
            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", -1);
        }
    }

    void Update()
    {
        if (!seEstaRetirando)
            return;

        if (puntosSalida == null || puntosSalida.Length == 0)
            return;

        Transform destino = puntosSalida[puntoActual];

        Vector2 direccion = 
            (destino.position - transform.position).normalized;

        transform.position = Vector2.MoveTowards(
            transform.position,
            destino.position,
            velocidad * Time.deltaTime
        );

        if (animator != null)
        {
            animator.SetFloat("Horizontal", direccion.x);
            animator.SetFloat("Vertical", direccion.y);
            animator.SetBool("SeRetira", true);
        }

        if (Vector2.Distance(transform.position, destino.position) < 0.1f)
        {
            puntoActual++;

            if (puntoActual >= puntosSalida.Length)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void RecibirCamperaYRetirarse()
    {
        Debug.Log($"👋 {gameObject.name}: RecibirCamperaYRetirarse() fue llamado."); // 🔧 DIAGNÓSTICO Bug 2

        if (seEstaRetirando)
            return;

        // 🔧 DIAGNÓSTICO Bug 2: si esto aparece en la consola, el problema NO está en el código,
        // sino en que no le asignaste "Puntos Salida" a esta tía en el Inspector. Sin puntos de
        // salida, seEstaRetirando queda en true pero el Update() de abajo corta en el primer "return"
        // y el personaje nunca se mueve ni se desactiva.
        if (puntosSalida == null || puntosSalida.Length == 0)
        {
            Debug.LogWarning($"⚠️ {gameObject.name} no tiene 'Puntos Salida' asignados en el Inspector. " +
                              "La tía nunca se va a mover hacia la salida.");
        }

        if (animator != null)
        {
            animator.SetBool("TieneCampera", true);
            animator.SetBool("SeRetira", true);
        }

        seEstaRetirando = true;
    }
}