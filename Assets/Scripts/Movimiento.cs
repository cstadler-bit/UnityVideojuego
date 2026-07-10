using UnityEngine;
using System.Collections;

public class Movimiento : MonoBehaviour
{
    public float velocidad = 5f;
    public bool congelado = false;

    [Header("Animación de bebida")]
    public Sprite[] drinkFrames;
    public float frameTime = 0.12f;
    public int loops = 3;

    [Header("Animación de resbalón")]
    public Sprite[] fallFrames;
    public Sprite[] getUpFrames;
    public float slipFrameTime = 0.12f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 ultimaDireccion = new Vector2(0, -1);

    private bool tomando = false;
    private bool resbalando = false;

    private Sprite spriteOriginal;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteOriginal = spriteRenderer.sprite;

        animator.SetFloat("Horizontal", 0);
        animator.SetFloat("Vertical", -1);
        animator.SetBool("Moving", false);
    }

    void Update()
    {
        if (congelado || tomando || resbalando)
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

    //==================================================
    // TOMAR PONCHE
    //==================================================

    public void TomarPonche()
    {
        if (!tomando)
            StartCoroutine(AnimacionTomar());
    }

    private IEnumerator AnimacionTomar()
    {
        tomando = true;

        animator.enabled = false;

        for (int l = 0; l < loops; l++)
        {
            for (int i = 0; i < drinkFrames.Length; i++)
            {
                spriteRenderer.sprite = drinkFrames[i];
                yield return new WaitForSeconds(frameTime);
            }
        }

        animator.enabled = true;
        spriteRenderer.sprite = spriteOriginal;

        tomando = false;
    }

    //==================================================
    // RESBALÓN
    //==================================================

    public void Resbalar()
    {
        if (resbalando || tomando)
            return;

        StartCoroutine(AnimacionResbalon());
    }

    private IEnumerator AnimacionResbalon()
{
    resbalando = true;

    animator.enabled = false;

    // ===== CAÍDA (4 FRAMES) =====

    spriteRenderer.sprite = fallFrames[0];
    yield return new WaitForSeconds(slipFrameTime);

    spriteRenderer.sprite = fallFrames[1];
    yield return new WaitForSeconds(slipFrameTime);

    spriteRenderer.sprite = fallFrames[2];
    yield return new WaitForSeconds(slipFrameTime);

    // Último frame de la caída (tirado en el piso)
    spriteRenderer.sprite = fallFrames[3];
    yield return new WaitForSeconds(0.50f);

    // ===== LEVANTARSE (4 FRAMES) =====

    spriteRenderer.sprite = getUpFrames[0];
    yield return new WaitForSeconds(slipFrameTime);

    spriteRenderer.sprite = getUpFrames[1];
    yield return new WaitForSeconds(slipFrameTime);

    spriteRenderer.sprite = getUpFrames[2];
    yield return new WaitForSeconds(slipFrameTime);

    spriteRenderer.sprite = getUpFrames[3];
    yield return new WaitForSeconds(slipFrameTime);

    animator.enabled = true;
    spriteRenderer.sprite = spriteOriginal;

    resbalando = false;
}

    //==================================================
    // EFECTOS
    //==================================================

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