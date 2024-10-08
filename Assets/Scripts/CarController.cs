using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CarController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private WheelCollider[] wheelColliders = new WheelCollider[4];
    [SerializeField] private GameObject[] wheelMeshes = new GameObject[4];


    private float driveInput = 0f;
    private float steerInput = 0f;


    private void FixedUpdate() {

        Inputs();
        FWDrive();
    }

    private void Inputs() {

        driveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }

    private void FWDrive() {

    }
   
}
