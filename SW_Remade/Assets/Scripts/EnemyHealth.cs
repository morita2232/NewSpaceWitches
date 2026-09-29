using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int vidaMaxima = 3;

    private int vidaActual;

    private void Awake()
    {
        vidaActual = vidaMaxima;
    }

    public void RecibirDano(int cantidad)
    {
        vidaActual -= cantidad;

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        Destroy(gameObject);
    }
}