using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Player player;
    

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        CenterCameraOnPlayer();
    }

    private void CenterCameraOnPlayer()
    {
        Vector3 cameraPos = mainCamera.transform.position;
        cameraPos.x = player.transform.position.x;
        cameraPos.y = player.transform.position.y;
        mainCamera.transform.position = cameraPos + offset;
    }
}
