using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveSystem
{
    public enum DriveType
    {
        frontWheelDrive,
        rearWheelDrive,
        fourWheelDrive
    }

    private DriveType driveType;
    private WheelCollider[] wheelColliders;
    private float motorForce;
    private float brakeForce;
    private float numberOfWheels;

    public DriveSystem(DriveType driveType, WheelCollider[] wheelColliders, float motorForce, float brakeForce)
    {
        this.driveType = driveType;
        this.wheelColliders = wheelColliders;
        this.motorForce = motorForce;
        this.brakeForce = brakeForce;

        if (driveType == DriveType.fourWheelDrive)
        {
            this.numberOfWheels = 4;
        }
        else
        {
            this.numberOfWheels = 2;
        }

    }

    public void Drive(float driveInput, float directionMultiplier)
    {
        Debug.Log("Drive System Drive");
        foreach (WheelCollider wc in wheelColliders)
        {
            wc.brakeTorque = 0;
        }

        if (driveType == DriveType.frontWheelDrive)
        {
            FrontWheelDrive(driveInput, directionMultiplier);
        }
        else if (driveType == DriveType.rearWheelDrive)
        {
           RearWheelDrive(driveInput, directionMultiplier);
        }
        else
        {
            FourWheelDrive(driveInput, directionMultiplier);
        }
    }

    private void FrontWheelDrive(float driveInput, float directionMultiplier)
    {
        Debug.Log("Drive System FWD");
        for (int i = 0; i < 2; i++)
        {
            wheelColliders[i].motorTorque = (motorForce * directionMultiplier * driveInput) / numberOfWheels;
        }
    }

    private void RearWheelDrive(float driveInput, float directionMultiplier)
    {
        Debug.Log("Drive System RWD");
        for (int i = 2; i < 4; i++)
        {
            wheelColliders[i].motorTorque = (motorForce * directionMultiplier * driveInput) / numberOfWheels;
        }
    }

    private void FourWheelDrive(float driveInput, float directionMultiplier)
    {
        Debug.Log("Drive System FourWD");
        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].motorTorque = (motorForce * directionMultiplier * driveInput) / numberOfWheels;
        }
    }

    public void Brake()
    {
        Debug.Log("Drive System Brake");
        foreach (WheelCollider wc in wheelColliders)
        {
            wc.brakeTorque = brakeForce / numberOfWheels;
            wc.motorTorque = 0;
        }
    }

    public void FastBrake(float _brakeForce)
    {
        Debug.Log("Drive System Fast Brake");
        Debug.Log("Drive System Fast Brake Force:" + _brakeForce);
        foreach (WheelCollider wc in wheelColliders)
        {
            wc.brakeTorque = _brakeForce / numberOfWheels;
            wc.motorTorque = 0;
        }
    }
}
