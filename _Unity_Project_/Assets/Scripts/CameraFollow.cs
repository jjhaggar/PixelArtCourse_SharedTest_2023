using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // El objetivo que la camara seguira
    public Vector3 offset; // Desplazamiento de la camara respecto al objetivo

    void Update()
    {
        // Actualiza la posicion de la camara para que siga al objetivo con el desplazamiento dado
        transform.position = target.position + offset;
    }
}
