using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseComponent : MonoBehaviour
{
    public bool isLaunched = false;
    public float maxTime = 2.0f;
    private float tiempoEnColision = 0f;
    private bool enColision = false;

    private bool firstContact = false;
    private void OnCollisionEnter(Collision collision)
    {
        // Si la casa aún no ha sido lanzada, ignoramos la colisión.
        if (!isLaunched) return;

        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("House"))
        {
            firstContact = true;
            enColision = true;
            tiempoEnColision = 0f;
        }
    }


    void Update()
    {
        if (enColision)
        {
            tiempoEnColision += Time.deltaTime;
            if (tiempoEnColision >= maxTime)
            {
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
                this.enabled = false;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("House"))
        {
            enColision = false;
        }
    }

    public bool GetIfCollide()
    {
        return firstContact;
    }
}
