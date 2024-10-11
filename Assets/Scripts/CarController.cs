using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CarController : NetworkBehaviour
{
    #region variables

    [Header("DriveType")]
    [SerializeField] DriveSystem.DriveType driveType;

    [Header("References")]
    [SerializeField] WheelCollider[] wheelColliders = new WheelCollider[4];
    [SerializeField] GameObject[] wheelMeshes = new GameObject[4];
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject centerOfMass;

    [Header("Car Settings")]
    [SerializeField] float wheelBase = 2.55f;
    [SerializeField] float rearTrack = 1.5f;
    [SerializeField] float downForce = 50f;

    [Header("Drive and Steer Settings")]
    [SerializeField] float motorForce = 1000f;
    [SerializeField] float brakeForce = 1000f;
    [SerializeField] float forwardFactor = 5f;
    [SerializeField] float backwardsFactor = 3f;
    [SerializeField] float maxForwardsSpeed = 180;
    [SerializeField] float maxBackwardsSpeed = 60;

    private float driveInput = 0f;
    private float steerInput = 0f;

    private DriveSystem driveSystem;
    private SteerSystem steeringSystem;

    #endregion

    #region client

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        centerOfMass = GameObject.Find("Mass");
        rb.centerOfMass = centerOfMass.transform.localPosition;

        // Initialize systems
        driveSystem = new DriveSystem(driveType, wheelColliders, motorForce, brakeForce);
        steeringSystem = new SteerSystem(wheelColliders, wheelBase, rearTrack);
    }

    private void Update()
    {
        if (!IsOwner) return;

        Inputs();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        DriveServerRpc(driveInput, steerInput);
        AddDownForceServerRpc();
    }

    private void Inputs()
    {
        driveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }

    #endregion

    #region server

    [ServerRpc]
    private void DriveServerRpc(float driveInput, float steerInput)
    {
        if (Mathf.Abs(driveInput) > 0.1f)
        {
            Debug.Log("car controller Drive");
            driveSystem.Drive(driveInput);
        }
        else
        {
            driveSystem.Brake();
        }

        steeringSystem.AckermanSteering(steerInput);
    }

    [ServerRpc]
    private void AddDownForceServerRpc()
    {
        rb.AddForce(-transform.up * downForce * rb.velocity.magnitude);
    }

    #endregion
}
