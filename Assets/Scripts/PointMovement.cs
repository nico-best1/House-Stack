using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMovement : MonoBehaviour
{
    public float speed = 2.0f;

    public Transform firstPoint; // puntos entre los que se mueve
    public Transform secondPoint;

    public float range = 1.0f;

    private bool axisXZ = false; // si es true sera en el eje X sino en el Z
    private bool dirFirst = false; // si es true va hacia first, sino va hacia second

    private void Start()
    {
        ChangePositions();
        transform.localPosition = new Vector3(0,0,0);
        dirFirst = false;
    }

    void Update()
    {

        // Determinar el destino actual
        Transform target = dirFirst ? firstPoint : secondPoint;

        // Mover hacia el destino
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Si llegó al destino, invertir dirección
        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            dirFirst = !dirFirst;
        }
    }

    public void Reiniciate()
    {
        transform.position = firstPoint.position;
        dirFirst = false;
    }

    public void SetPositions(Vector3 posfirst)
    {
        if (firstPoint != null && secondPoint != null)
        {
            firstPoint.transform.localPosition = posfirst * range;
            secondPoint.transform.localPosition = -posfirst * range;
        }
        Reiniciate();
    }

    public void ChangePositions()
    {
        axisXZ = !axisXZ;
        if (axisXZ)
        {
            SetPositions(new Vector3(1, 0, 0));
        }
        else
        {
            SetPositions(new Vector3(0, 0, 1));
        }
    }
}
