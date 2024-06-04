using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Semaforo : MonoBehaviour
{
    public float greenLightDuration = 5f;  // Duración de la luz verde
    public float redLightDuration = 5f;    // Duración de la luz roja

    private GameObject redLight;
    private GameObject greenLight;
    private GameObject colliderUp;
    private GameObject colliderDown;
    private GameObject colliderLeft;
    private GameObject colliderRight;

    //Hay que asignar el coche desde el editor de Unity, creo que estó hará que solo pueda haber un coche
    //(De esta manera), pero se podría hacer una lista de coches o que al colisionar se pase la info del coche
    //que ha colisionado si queremos poner mas de uno
    public List<CarPathFollower> coches = null;

    private float timer;
    public bool isGreenLight;

    void Start()
    {
        // Encuentra los objetos hijos por nombre
        redLight = transform.Find("Red").gameObject;
        greenLight = transform.Find("Green").gameObject;
        colliderUp = transform.Find("Collider Up").gameObject;
        colliderDown = transform.Find("Collider Down").gameObject;
        colliderLeft = transform.Find("Collider Left").gameObject;
        colliderRight = transform.Find("Collider Right").gameObject;

        // Inicializa el temporizador y la luz verde
        timer = greenLightDuration;
        isGreenLight = true;
        UpdateTrafficLights();
    }

    void Update()
    {
        // Actualiza el temporizador
        timer -= Time.deltaTime;

        // Alterna las luces cuando el temporizador llega a cero
        if (timer <= 0)
        {
            isGreenLight = !isGreenLight;
            timer = isGreenLight ? greenLightDuration : redLightDuration;
            UpdateTrafficLights();
        }
    }

    void UpdateTrafficLights()
    {
        if (isGreenLight)
        {
            // Activa la luz verde y desactiva la luz roja
            greenLight.SetActive(true);
            redLight.SetActive(false);

            // Desactiva los colisionadores Up y Down, y activa Left y Right
            colliderUp.SetActive(false);
            colliderDown.SetActive(false);
            colliderLeft.SetActive(true);
            colliderRight.SetActive(true);

            foreach (CarPathFollower c in coches)
            {
                if (c != null && !c.isMoving && !c.isBreak)
                {
                    //Se necesita esta linea aqui para actualizar el estado de movimiento
                    c.isMoving = true;
                }
            }
            
            
        }
        else
        {
            // Activa la luz roja y desactiva la luz verde
            redLight.SetActive(true);
            greenLight.SetActive(false);

            // Activa los colisionadores Up y Down, y desactiva Left y Right
            colliderUp.SetActive(true);
            colliderDown.SetActive(true);
            colliderLeft.SetActive(false);
            colliderRight.SetActive(false);
        }
    }

    // Método para cambiar manualmente la luz a verde
    public void SetGreenLight()
    {
        isGreenLight = true;
        timer = greenLightDuration;
        UpdateTrafficLights();
    }

    // Método para cambiar manualmente la luz a roja
    public void SetRedLight()
    {
        isGreenLight = false;
        timer = redLightDuration;
        UpdateTrafficLights();
    }

}
