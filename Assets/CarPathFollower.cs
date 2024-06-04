using System.Collections.Generic;
using System.Threading;
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

    private float elapsedTime = 0f;
    private float checkInterval = 5f;

    public Semaforo semaforo;

    public List<AudioClip> audios;
    public AudioSource audioSource;

    public CarManager carManager;

    void Start()
    {
        semaforo = GameObject.Find("Traffic Light Variant").GetComponent<Semaforo>();
        carManager = GameObject.Find("Car Spawner Manager").GetComponent<CarManager>();

        FindAndAddRoutes();

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
                //Si llega al final destruir el GameObject
                Destroy(gameObject);

                // Si se llega al final de la lista de puntos, seleccionar una nueva ruta aleatoria y teletransportar al primer punto
                //SelectRandomRoute();
            }
        }

        // Actualizar el tiempo transcurrido
        elapsedTime += Time.deltaTime;

        // Solo verificar la posibilidad de avería cada X segundos
        if (elapsedTime >= checkInterval)
        {
            elapsedTime = 0f; // Reiniciar el temporizador
            if (Random.value < 0.15f) // La probabilidad de que el coche se rompa (15%)
            {
                BreakDown();
                audioSource.PlayOneShot(audios[GenerateRandom01()]);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Detector"))
        {
            semaforo.coches.Add(this);

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

        if (other.CompareTag("Car"))
        {
            if(other.GetComponent<CarPathFollower>().isMoving == false && other.GetComponent<CarPathFollower>().isBreak == true)
            {
                isMoving = false;
            }
            else if (other.GetComponent<CarPathFollower>().isMoving == true && other.GetComponent<CarPathFollower>().isBreak == false)
            {
                isMoving = true;
            }

        }

    }
    void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("Detector") && !isMoving)
        //{
        //    Debug.Log("Detector activado, reanudando el movimiento del coche.");
        //    isMoving = true;
        //}

        //if (other.CompareTag("Car"))
        //{
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
        else
        {
            Debug.Log("No hay rutas asignadas al GameObject: " + gameObject.name);
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

    private void OnDestroy()
    {
        carManager.SpawnCar();
        carManager.carAmount--;
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
    public int GenerateRandom01()
    {
        // Generate a random number between the minimum and maximum (exclusive)
        float randomNumber = Random.value;


        if (randomNumber < 0.5)
        {
            return 0;
        }
        else
        {
            return 1;
        }

    }
    private void FindAndAddRoutes()
    {
        // Buscar el objeto padre que contiene todas las rutas
        GameObject routesParent = GameObject.Find("Alfombra");
        if (routesParent != null)
        {
            // Encontrar todas las rutas como hijos del objeto padre
            for (int i = 0; ; i++)
            {
                GameObject routeObj = GameObject.Find("Ruta " + i);
                if (routeObj == null)
                {
                    // No se encontró la ruta, asumir que ya no hay más rutas
                    break;
                }

                // Crear una nueva ruta y buscar todos los 'Cubes' que son hijos de la ruta
                Route newRoute = new Route();
                newRoute.anchorPoints = new List<Transform>();

                foreach (Transform child in routeObj.transform)
                {
                    // Asumir que todos los hijos son puntos de anclaje
                    if (child.name.StartsWith("Cube"))
                    {
                        newRoute.anchorPoints.Add(child);
                    }
                }

                // Añadir la nueva ruta a la lista de rutas si tiene puntos de anclaje
                if (newRoute.anchorPoints.Count > 0)
                {
                    routes.Add(newRoute);
                }
            }
        }
        else
        {
            Debug.LogError("No se encontró el objeto 'Alfombra'. Asegúrate de que el nombre esté escrito correctamente y de que exista en la jerarquía.");
        }
    }

}
