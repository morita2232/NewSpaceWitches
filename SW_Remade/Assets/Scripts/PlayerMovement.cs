using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb2D;

    [SerializeField] private Animator animator;

    private Transform platformParent;
    private Vector2 velocidadSuavizado = Vector2.zero;
    private bool mirandoDerecha = true;

    [Header("Movimiento")]
    [SerializeField] private float velocidadDeMovimiento = 8f;
    [Range(0f, 0.3f)]
    [SerializeField] private float suavizadoDeMovimiento = 0.05f;

    private float movimientoHorizontal;

    [Header("Salto")]
    [SerializeField] private float fuerzaDeSalto = 10f;
    [SerializeField] private LayerMask queEsSuelo;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector2 dimensionesCaja = new Vector2(0.8f, 0.2f);
    [SerializeField] private bool enSuelo;

    private bool saltando;

    [Header("Ataque")]
    [SerializeField] private KeyCode teclaDeAtaque = KeyCode.J;
    [SerializeField] private GameObject prefabProyectil;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float tiempoEntreAtaques = 0.3f;

    private float siguienteAtaque;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && enSuelo)
        {
            saltando = true;
        }

        if (Input.GetKeyDown(teclaDeAtaque) && Time.time >= siguienteAtaque)
        {
            Atacar();
        }
    }

    private void FixedUpdate()
    {
        enSuelo = Physics2D.OverlapBox(
            controladorSuelo.position,
            dimensionesCaja,
            0f,
            queEsSuelo
        ) != null;

        Mover();
        saltando = false;
    }

    private void Mover()
    {
        Vector2 velocidadObjetivo = new Vector2(
            movimientoHorizontal * velocidadDeMovimiento,
            rb2D.linearVelocity.y
        );

        rb2D.linearVelocity = Vector2.SmoothDamp(
            rb2D.linearVelocity,
            velocidadObjetivo,
            ref velocidadSuavizado,
            suavizadoDeMovimiento
        );

        if (movimientoHorizontal > 0f && !mirandoDerecha)
        {
            Girar();
        }
        else if (movimientoHorizontal < 0f && mirandoDerecha)
        {
            Girar();
        }

        if (enSuelo && saltando)
        {
            enSuelo = false;
            rb2D.AddForce(Vector2.up * fuerzaDeSalto, ForceMode2D.Impulse);
        }

        animator.SetBool("isJumping", !enSuelo);
        animator.SetBool("isMoving", movimientoHorizontal != 0f);
    }

    private void Atacar()
    {
        if (prefabProyectil == null || puntoDisparo == null)
        {
            Debug.LogWarning("Asigna Prefab Proyectil y Punto Disparo en el Inspector.");
            return;
        }

        siguienteAtaque = Time.time + tiempoEntreAtaques;
        animator.SetTrigger("Attack");

        float direccion = mirandoDerecha ? 1f : -1f;

        GameObject objetoProyectil = Instantiate(
            prefabProyectil,
            puntoDisparo.position,
            Quaternion.identity
        );

        Proyectil proyectil = objetoProyectil.GetComponent<Proyectil>();

        if (proyectil != null)
        {
            proyectil.Inicializar(direccion);
        }
        else
        {
            Debug.LogError("El prefab del proyectil necesita el script Proyectil.");
            Destroy(objetoProyectil);
        }
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;

        Vector3 escala = transform.localScale;
        escala.x *= -1f;
        transform.localScale = escala;
    }

    private void OnDrawGizmos()
    {
        if (controladorSuelo == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(controladorSuelo.position, dimensionesCaja);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            platformParent = collision.transform;
            transform.SetParent(platformParent);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform") &&
            transform.parent == collision.transform)
        {
            transform.SetParent(null);
            platformParent = null;
        }
    }
}