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

    public DriveSystem(DriveType driveType, WheelCollider[] wheelColliders, float motorForce, float brakeForce)
    {
        this.driveType = driveType;
        this.wheelColliders = wheelColliders;
        this.motorForce = motorForce;
        this.brakeForce = brakeForce;
    }

    public void Drive(float driveInput)
    {
        Debug.Log("Drive System Drive");
        foreach (WheelCollider wc in wheelColliders)
        {
            wc.brakeTorque = 0;
        }

        if (driveType == DriveType.frontWheelDrive)
        {
            FrontWheelDrive(driveInput);
        }
        else if (driveType == DriveType.rearWheelDrive)
        {
            // ++ Implement rear-wheel drive
        }
        else
        {
            // ++ Implement four-wheel drive
        }
    }

    private void FrontWheelDrive(float driveInput)
    {
        Debug.Log("Drive System FWD");
        for (int i = 0; i < 2; i++)
        {
            wheelColliders[i].motorTorque = motorForce * driveInput;
        }
    }

    public void Brake()
    {
        Debug.Log("Drive System Brake");
        foreach (WheelCollider wc in wheelColliders)
        {
            wc.brakeTorque = brakeForce;
            wc.motorTorque = 0;
        }
    }
}
