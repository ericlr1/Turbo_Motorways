using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Route
{
    public List<Transform> anchorPoints; // Lista de puntos de anclaje para un recorrido
}

public class CarPathFollower : MonoBehaviour
{
    public List<Route> routes; // Lista de recorridos
    public float speed; // Velocidad del coche
    public float rotationSpeed; // Velocidad de rotación del coche
    public float stopDistance; // Distancia mínima para considerar que el coche ha llegado a un punto
    public ParticleSystem carBreakddownParticles;

    private List<Transform> anchorPoints; // Puntos de anclaje del recorrido seleccionado
    private int currentAnchorIndex;
    public bool isMoving = true;
    public bool isBreak = false;

    public Semaforo semaforo;

    void Start()
    {
        SelectRandomRoute();
    }

    void Update()
    {
        if (!isMoving || anchorPoints == null || anchorPoints.Count == 0) return; // Si el coche no debe moverse o no hay puntos, no hacer nada

        // Obtener el punto de destino actual
        Transform targetPoint = anchorPoints[currentAnchorIndex];

        // Dirección hacia el punto de destino
        Vector3 direction = (targetPoint.position - transform.position).normalized;

        // Movimiento del coche hacia el punto de destino
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        // Rotación del coche hacia el punto de destino
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Verificar si el coche ha llegado al punto de destino
        if (Vector3.Distance(transform.position, targetPoint.position) < stopDistance)
        {
            // Pasar al siguiente punto de anclaje
            currentAnchorIndex++;
            if (currentAnchorIndex >= anchorPoints.Count)
            {
                // Si se llega al final de la lista de puntos, seleccionar una nueva ruta aleatoria y teletransportar al primer punto
                SelectRandomRoute();
            }
        }

        if (Random.value < 0.001f) // La probablidad de que el coche se rompa
        {
            BreakDown();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Detector"))
        {
            //El OnTriggerStay no te deja actualizar cuando la traffic light es roja o verde
            //Si cuando ha entrado el coche era roja, la dejará en rojo todo el rato
            //Ahora va CLINICO pero creo que esta manera en especifico solo servirá con UN solo coche
            if (semaforo.isGreenLight)
            {
                Debug.Log("Semaforo en verde, el coche pasa");
                isMoving = true;
            }
            else
            {
                Debug.Log("Semaforo en rojo, el coche se detiene");
                isMoving = false;
            }

            //if (other.CompareTag("Detector") && isMoving)
            //{
            //    Debug.Log("Detector activado, deteniendo el coche.");
            //    isMoving = false;
            //}
        }
    }
    void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("Detector") && !isMoving)
        //{
        //    Debug.Log("Detector activado, reanudando el movimiento del coche.");
        //    isMoving = true;
        //}
    }

    void SelectRandomRoute()
    {
        int randomRouteIndex = Random.Range(0, routes.Count);
        anchorPoints = routes[randomRouteIndex].anchorPoints;
        currentAnchorIndex = 0;
        if (anchorPoints.Count > 0)
        {
            // Teletransportar al primer punto de la nueva ruta
            transform.position = anchorPoints[0].position;
            // Ajustar la rotación del coche hacia el primer punto
            Vector3 direction = (anchorPoints[0].position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }
        isMoving = true; // Asegurar que el coche se mueva al seleccionar una nueva ruta
    }

    private void OnMouseDown()
    {
        if (!isMoving && isBreak) 
        { 
            FixCar(); 
        }
    }

    //Funcion que hace que el coche se pare
    void BreakDown()
    {
        Debug.Log("Car Break Down! Please fix it!");
        isMoving = false;
        isBreak = true;
        carBreakddownParticles.Play();
    }

    //Funcion para arreglar el coche
    void FixCar()
    {
        Debug.Log("Car Fixed! Thank you!");
        isMoving = true;
        isBreak = false;
        carBreakddownParticles.Stop();
    }
}
