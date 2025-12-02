using UnityEngine;

public class GravedadPersonalizada : MonoBehaviour
{
    public float fuerzaGravedad = 9.81f; // Puedes ajustar la fuerza si quieres

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Muy importante desactivar la gravedad normal
    }

    void FixedUpdate()
    {
        if (transform.parent != null)
        {
            Vector3 direccionDeCaida = -transform.parent.up;
            rb.AddForce(direccionDeCaida * fuerzaGravedad, ForceMode.Acceleration);
        }
    }
}
