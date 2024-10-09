using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CarController : NetworkBehaviour
{
    #region utils

    internal enum DriveType
    {
        frontWheelDrive,
        rearWheelDrive,
        fourWheelDrive
    }

    #endregion

    #region variables

    [Header("DriveType")]
    [SerializeField] DriveType driveType;

    [Header("References")]
    [SerializeField] WheelCollider[] wheelColliders = new WheelCollider[4];
    [SerializeField] GameObject[] wheelMeshes = new GameObject[4];
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject centerOfMass;

    [Header("Car Settings")]
    [SerializeField] float wheelBase = 2.55f; // in metters
    [SerializeField] float rearTrack = 1.5f; // in metters
    [SerializeField] float downForce = 50f;

    [Header("Drive Settings")]
    [SerializeField] float motorForce = 500f;

    [Header("Steer Settings")]
    [SerializeField] float baseTurnRadius = 7.5f;
    private float currentTurnRadius;

    [Header("Inputs")]
    private float driveInput = 0f;
    private float steerInput = 0f;


    private float KPH = 0f; // ?? where to calc it in user/server space. ( = rb.velocity.mangitude * 3.6 )

    #endregion

    #region client

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        centerOfMass = GameObject.Find("Mass");

        rb.centerOfMass = centerOfMass.transform.localPosition;
    }


    private void FixedUpdate() {

        if (!IsOwner) { return; }
        
        Inputs();
        DriveServerRpc(driveInput, steerInput);
        AddDownForceServerRpc(); // for better car grip
    }

    private void Inputs() {

        driveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }

    #endregion


    #region server

    [ServerRpc]
    private void DriveServerRpc(float driveInput, float steerInput)
    {

        if (driveType == DriveType.frontWheelDrive)
        {
            FrontWheelDrive(driveInput);
        } 
        else if (driveType == DriveType.rearWheelDrive)
        {
            // ++add RWDrive
        }
        else
        {
            // ++add FourWDrive
        }

        AckermanSteering(steerInput);
    }

    [ServerRpc]
    private void AddDownForceServerRpc()
    {
        rb.AddForce(-transform.up * downForce * rb.velocity.magnitude);

        SyncRigidbodyStateClientRpc(rb.position, rb.rotation);
    }

    [ClientRpc]
    private void SyncRigidbodyStateClientRpc(Vector3 position, Quaternion rotation)
    {
        // Update clients' Rigidbody position and rotation to match the server's
        rb.position = position;
        rb.rotation = rotation;
    }

    private void FrontWheelDrive(float driveInput)
    {
        for (int i = 0; i < 2; i++)
        {
            wheelColliders[i].motorTorque = motorForce * driveInput;
        }
    }

    private void AckermanSteering(float steerInput)
    {
        // ++ change the turn radius based on the car speed;
        currentTurnRadius = baseTurnRadius;

        float innerWheelAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (currentTurnRadius + (rearTrack / 2))) * steerInput;
        float outterWheelAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (currentTurnRadius - (rearTrack / 2))) * steerInput;

        if (steerInput > 0) 
        {
            wheelColliders[0].steerAngle = innerWheelAngle;
            wheelColliders[1].steerAngle = outterWheelAngle;
        }
        else if (steerInput < 0)
        {
            wheelColliders[0].steerAngle = outterWheelAngle;
            wheelColliders[1].steerAngle = innerWheelAngle;
        }
        else
        {
            wheelColliders[0].steerAngle = 0;
            wheelColliders[1].steerAngle = 0;
        }

    }
    #endregion
}
