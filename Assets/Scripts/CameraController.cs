using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraController : NetworkBehaviour
{
    [Header("Refernces")]
    private CarController carController;
    [SerializeField] Camera cam;
    private AudioListener audioListener;

    [Header("Camera Rotation")]
    [SerializeField] Transform focusPoint;
    [SerializeField] Transform cameraHolder, cameraAnchor;
    [SerializeField] float minAngle = -5, maxAngle = 30;
    [SerializeField] float cameraSensitivity = 200f;
    private float mouseX, mouseY;
    private bool scrollWheelClicked = false;
    private float xRotation;

    [Header("Camera FOV")]
    [SerializeField] float minFOV = 60;
    [SerializeField] float maxFOV = 90;
    [SerializeField] float fovSpeedChangeMultiplier = 2f;


    [SerializeField] bool lockMouse = true;

    #region Client

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            cam.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if (!IsOwner) return;

        carController = GetComponentInParent<CarController>();
        if (cam == null) return;
        cam.fieldOfView = minFOV;

        if (lockMouse)
        {
            // Lock and hide the cursor when the game starts
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
       
    }


    private void Update()
    {
        if (!IsOwner) return;
        Inputs();
        UpdateFreeLookCamRotation();
        UpdateCameraFOV();
        FastLookBack();
    }

    private void UpdateFreeLookCamRotation()
    {
        if (IsOwner)
        {
            cam.transform.LookAt(focusPoint);
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, minAngle, maxAngle); // Clamp up/down rotation
            cameraHolder.Rotate(Vector3.up * mouseX);
            cameraAnchor.localRotation = Quaternion.Euler(xRotation, 0, 0);
        }
    }

    private void UpdateCameraFOV()
    {
        if (IsOwner)
        {
            Debug.Log(carController.GetKPH());
            cam.fieldOfView = Mathf.Lerp(minFOV, maxFOV, (carController.GetKPH() * fovSpeedChangeMultiplier) / carController.GetMaxForwardSpeed());
        }
    }


    private void FastLookBack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2))
            cameraHolder.localRotation = Quaternion.Euler(0, 180, 0);
        if (Input.GetKeyUp(KeyCode.Mouse2))
            cameraHolder.localRotation = Quaternion.Euler(0, 0, 0);
    }

    private void Inputs()
    {
        mouseX = Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime; // mouse movement sideways
        mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime; // mouse movement up and down
    }

    #endregion
}
