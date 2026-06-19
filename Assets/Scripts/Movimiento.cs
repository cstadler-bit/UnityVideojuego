using UnityEngine;
using System.Collections;

public class Movimiento : MonoBehaviour
{
    public float velocidad = 5f;
    public bool congelado = false;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 ultimaDireccion = new Vector2(0, -1);

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        animator.SetFloat("Horizontal", 0);
        animator.SetFloat("Vertical", -1);
        animator.SetBool("Moving", false);
    }

    void Update()
    {
        if (congelado)
        {
            animator.SetBool("Moving", false);
            return;
        }

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

    public void AplicarCongeladoVisual()
    {
        spriteRenderer.color = new Color(1f, 1f, 1f, 0.6f);
    }

    public void QuitarCongeladoVisual()
    {
        spriteRenderer.color = Color.white;
    }

    public IEnumerator Parpadear(float duracion)
    {
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.15f);

            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.15f);

            tiempo += 0.3f;
        }

        spriteRenderer.enabled = true;
    }

    public IEnumerator BrillarBoost(float duracion)
    {
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            spriteRenderer.color = new Color(1f, 0.9f, 0.4f, 1f);
            yield return new WaitForSeconds(0.25f);

            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.25f);

            tiempo += 0.5f;
        }

        spriteRenderer.color = Color.white;
    }
}