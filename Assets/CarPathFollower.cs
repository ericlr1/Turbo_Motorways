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

    private List<Transform> anchorPoints; // Puntos de anclaje del recorrido seleccionado
    private int currentAnchorIndex;

    void Start()
    {
        SelectRandomRoute();
    }

    void Update()
    {
        if (anchorPoints == null || anchorPoints.Count == 0) return; // Si no hay puntos, no hacer nada

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
    }
}
