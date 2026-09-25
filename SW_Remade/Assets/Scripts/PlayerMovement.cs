using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D rb2D;
    public Animator animator;
    private Transform platformParent;

    [Header("Movimiento")]

    private float movimientoHorizontal = 0f;

    [SerializeField] private float velocidadDeMovimiento;
    [Range(0, 0.3f)][SerializeField] private float SuavizadoDeMovimiento;

    private Vector3 velocidad = Vector3.zero;

    private bool mirandoDerecha = true;

    [Header("Salto")]

    [SerializeField] private float fuerzaDeSalto;
    [SerializeField] private LayerMask queEsSuelo;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector3 dimensionesCaja;
    [SerializeField] private bool enSuelo;
    [SerializeField] private bool saltando = false;


    [Header("Ataque")]

    [SerializeField] private KeyCode teclaDeAtaque;
    [SerializeField] private bool atacando;


    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        movimientoHorizontal = Input.GetAxis("Horizontal") * velocidadDeMovimiento;



        if (Input.GetButtonDown("Jump") && enSuelo)
        {
            saltando = true;
            Debug.Log("En aire");
        }

        if (Input.GetKeyDown(teclaDeAtaque))
        {
            atacando = true;
            Debug.Log("Atacando");
        }



    }


    private void FixedUpdate()
    {
        enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionesCaja, 0f, queEsSuelo);
        //mover
        Mover(movimientoHorizontal * Time.fixedDeltaTime, saltando, atacando);
        saltando = false;
        atacando = false;
        animator.SetBool("isAttacking", false);
    }

    private void Mover(float moviendo, bool saltando, bool atacando)
    {
        Vector3 velocidadObjetivo = new Vector2(moviendo, rb2D.linearVelocity.y);
        rb2D.linearVelocity = Vector3.SmoothDamp(rb2D.linearVelocity, velocidadObjetivo, ref velocidad, SuavizadoDeMovimiento);

        if (moviendo > 0 && !mirandoDerecha)
        {
            //girar
            Girar();
        }
        else if (moviendo < 0 && mirandoDerecha)
        {
            //girar
            Girar();
        }
        if (enSuelo && saltando)
        {
            enSuelo = false;
            rb2D.AddForce(new Vector2(0f, fuerzaDeSalto));
        }
        if(atacando)
        {
            animator.SetBool("isAttacking", true);
        }
        animator.SetBool("isJumping", !enSuelo);
        animator.SetBool("isMoving", moviendo != 0);
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(controladorSuelo.position, dimensionesCaja);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            platformParent = collision.transform;
            transform.SetParent(platformParent);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.SetParent(null);
        }
    }

}
