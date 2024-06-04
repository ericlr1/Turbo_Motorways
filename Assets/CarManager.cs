using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    public List<GameObject> prefabs;

    public int carAmount = 0;
    public int pendingtoAdd = 0;

    public void Start()
    {
        StartCoroutine(SpawnCarsWithDelay());

    }
    public void Update()
    {
        if (pendingtoAdd > 0 && carAmount < 3)
        {
            SpawnCar();
        }
    }
    public void SpawnCar()
    {
        if (carAmount <= 3)
        {
            Debug.Log("Car spawned");
            // Elegir un prefab aleatorio de la lista
            int randomIndex = Random.Range(0, prefabs.Count);
            GameObject carPrefab = prefabs[randomIndex];

            // Instanciar el prefab
            Instantiate(carPrefab, transform.position, transform.rotation);

            carAmount++;
        }
        else
        {
            Debug.Log("Max number of cars spawned");
        }
    }

    private IEnumerator SpawnCarsWithDelay()
    {
        for (int i = 0; i < 3; i++)
        {
            SpawnCar();
            yield return new WaitForSeconds(1.0f); // Espera 1 segundo
        }
    }
}
