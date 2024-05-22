using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCamera : MonoBehaviour
{
    public Camera frontCamera; // Asigna la c�mara frontal en el Inspector
    public Camera rearCamera; // Asigna la c�mara trasera en el Inspector

    private bool isFrontCameraActive = true;

    private void Start()
    {
        // Activa la c�mara frontal al inicio
        frontCamera.enabled = true;
        rearCamera.enabled = false;
    }

    public void SwitchCamera()
    {
        // Alterna entre c�maras
        isFrontCameraActive = !isFrontCameraActive;
        frontCamera.enabled = isFrontCameraActive;
        rearCamera.enabled = !isFrontCameraActive;
    }
}
