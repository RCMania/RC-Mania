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

    [Header("Drive Settings")]
    [SerializeField] float motorForce = 1000f;
    [SerializeField] float brakeForce = 1000f;
    [SerializeField] float fastBrakeMultiplier = 2.5f;
    [SerializeField] float forwardMultiplier = 5f;
    [SerializeField] float backwardsMultiplier = 3f;
    [SerializeField] float maxForwardsSpeed = 180;
    [SerializeField] float maxBackwardsSpeed = 60;
    private NetworkVariable<float> KPH = new NetworkVariable<float>();
    private NetworkVariable<float> movingDirection = new NetworkVariable<float>();

    [Header("Steer Settings")]
    [SerializeField] float baseTurnRadius = 7.5f;
   

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
        driveSystem = new DriveSystem(driveType, wheelColliders, motorForce, brakeForce, rb);
        steeringSystem = new SteerSystem(wheelColliders, wheelBase, rearTrack, baseTurnRadius);
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

    public float GetKPH()
    {
        return KPH.Value;
    }

    public float GetMaxForwardSpeed()
    {
        return maxForwardsSpeed;
    }

    #endregion

    #region server

    [ServerRpc]
    private void DriveServerRpc(float driveInput, float steerInput)
    {
        //Debug.Log("DriveServerRpc");
        movingDirection.Value = transform.InverseTransformDirection(rb.linearVelocity).z;
        KPH.Value = rb.linearVelocity.magnitude * 3.6f;

        if ((movingDirection.Value < -0.2f && driveInput > 0.1f) || (movingDirection.Value > 0.2f && driveInput < -0.1f))
        {
            driveSystem.FastBrake(fastBrakeMultiplier * brakeForce);
        }
        else if (driveInput > 0 && KPH.Value < maxForwardsSpeed)
        {
            driveSystem.Drive(driveInput, forwardMultiplier);
        }
        else if (driveInput < 0 && KPH.Value < maxBackwardsSpeed)
        {
            driveSystem.Drive(driveInput, backwardsMultiplier);
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
        rb.AddForce(-transform.up * downForce * rb.linearVelocity.magnitude);
    }

    #endregion
}
