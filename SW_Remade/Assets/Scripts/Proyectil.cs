using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Proyectil : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 12f;
    [SerializeField] private float tiempoDeVida = 1.5f;

    [Header("Daño")]
    [SerializeField] private int dano = 1;
    [SerializeField] private LayerMask capaEnemigos;

    private Rigidbody2D rb2D;
    private bool yaImpacto;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Inicializar(float direccion)
    {
        spriteRenderer.flipX = direccion < 0f;
        rb2D.linearVelocity = new Vector2(direccion * velocidad, 0f);
        Destroy(gameObject, tiempoDeVida);
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (yaImpacto)
            return;

        // Solo considera objetos cuya capa esté incluida en "capaEnemigos".
        if ((capaEnemigos.value & (1 << otro.gameObject.layer)) == 0)
            return;

        EnemyHealth enemigo = otro.GetComponentInParent<EnemyHealth>();

        if (enemigo == null)
            return;

        yaImpacto = true;
        enemigo.RecibirDano(dano);
        Destroy(gameObject);
    }
}