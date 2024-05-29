using UnityEngine;
using Vuforia;

public class RaycastHandler : MonoBehaviour
{
    private Camera vuforiaCamera;

    void Start()
    {
        // Encuentra la cámara de Vuforia en la escena
        vuforiaCamera = FindObjectOfType<VuforiaBehaviour>().GetComponent<Camera>();
    }

    void Update()
    {
#if UNITY_EDITOR
        HandleInput(Input.GetMouseButtonDown(0), Input.mousePosition);
#else
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            HandleInput(true, Input.touches[0].position);
        }
#endif
    }

    void HandleInput(bool inputDetected, Vector3 inputPosition)
    {
        if (inputDetected)
        {
            Debug.Log("Input detected");

            Ray ray = vuforiaCamera.ScreenPointToRay(inputPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    Semaforo semaforo;
                    if (hit.collider.gameObject.TryGetComponent<Semaforo>(out semaforo))
                    {
                        if (semaforo.isGreenLight)
                        {
                            Debug.Log("SetRedLight");
                            semaforo.SetRedLight();
                        }
                        else
                        {
                            Debug.Log("SetGreenLight");
                            semaforo.SetGreenLight();
                        }
                    }
                }
            }
        }
    }
}
