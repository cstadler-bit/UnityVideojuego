using UnityEngine;

public class Movimiento : MonoBehaviour
{
    public float velocidad = 5f;

    private Animator animator;

    private Vector2 ultimaDireccion = new Vector2(0, -1);

    void Start()
    {
        animator = GetComponent<Animator>();

        animator.SetFloat("Horizontal", 0);
        animator.SetFloat("Vertical", -1);
        animator.SetBool("Moving", false);
    }

    void Update()
    {
        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyCode.D))
            h = 1f;
        else if (Input.GetKey(KeyCode.A))
            h = -1f;
        else if (Input.GetKey(KeyCode.W))
            v = 1f;
        else if (Input.GetKey(KeyCode.S))
            v = -1f;

        Vector2 movimiento = new Vector2(h, v);

        transform.position += (Vector3)movimiento * velocidad * Time.deltaTime;

        if (movimiento != Vector2.zero)
        {
            ultimaDireccion = movimiento;
        }

        animator.SetFloat("Horizontal", ultimaDireccion.x);
        animator.SetFloat("Vertical", ultimaDireccion.y);
        animator.SetBool("Moving", movimiento != Vector2.zero);
    }
}