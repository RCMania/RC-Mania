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
    private Rigidbody rb;
    private float handBrakeFriction;

    public DriveSystem(DriveType driveType, WheelCollider[] wheelColliders, float motorForce, float brakeForce, Rigidbody rb)
    {
        this.driveType = driveType;
        this.wheelColliders = wheelColliders;
        this.motorForce = motorForce;
        this.brakeForce = brakeForce;
        this.rb = rb;

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
        //Debug.Log("Drive System Drive");
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
        //Debug.Log("Drive System FWD");
        for (int i = 0; i < 2; i++)
        {
            wheelColliders[i].motorTorque = (motorForce * directionMultiplier * driveInput) / numberOfWheels;
        }
    }

    private void RearWheelDrive(float driveInput, float directionMultiplier)
    {
        //Debug.Log("Drive System RWD");
        for (int i = 2; i < 4; i++)
        {
            wheelColliders[i].motorTorque = (motorForce * directionMultiplier * driveInput) / numberOfWheels;
        }
    }

    private void FourWheelDrive(float driveInput, float directionMultiplier)
    {
        //Debug.Log("Drive System FourWD");
        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].motorTorque = (motorForce * directionMultiplier * driveInput) / numberOfWheels;
        }
    }

    public void Brake()
    {
        //Debug.Log("Drive System Brake");
        foreach (WheelCollider wc in wheelColliders)
        {
            wc.motorTorque = 0;
            wc.brakeTorque = brakeForce / numberOfWheels;
        }
    }

    public void FastBrake(float _brakeForce)
    {
        //Debug.Log("Drive System Fast Brake");
        
        foreach (WheelCollider wc in wheelColliders)
        {
            //Debug.Log("wheel" + wc.motorTorque);
            wc.motorTorque = 0;
            wc.brakeTorque = _brakeForce / numberOfWheels;
        }
    }
   
    public void Drift(float steerInput)
    {
        
        
    }

    //public void FastBrake(float _brakeForce, float fastBrakeMultiplier)
    //{
    //    Debug.Log("Drive System Fast Brake");

    //    // Increase brake force on all wheels
    //    foreach (WheelCollider wc in wheelColliders)
    //    {
    //        wc.brakeTorque = _brakeForce * fastBrakeMultiplier / numberOfWheels;
    //        wc.motorTorque = 0;
    //    }

    //    // Increase drag to simulate air resistance during fast brake
    //    rb.drag = 1.5f;  // You can tweak this value based on how fast you want the car to stop
    //    rb.angularDrag = 1f;

    //    // Optional: Increase grip (friction) during fast braking
    //    foreach (WheelCollider wc in wheelColliders)
    //    {
    //        WheelFrictionCurve forwardFriction = wc.forwardFriction;
    //        forwardFriction.stiffness = 2f; // Increase stiffness for more grip during braking
    //        wc.forwardFriction = forwardFriction;

    //        WheelFrictionCurve sidewaysFriction = wc.sidewaysFriction;
    //        sidewaysFriction.stiffness = 2f;
    //        wc.sidewaysFriction = sidewaysFriction;
    //    }
    //}

    //public void ResetCarSettings()
    //{
    //    Debug.Log("Resetting car settings to original values.");

    //    // Reset motor and brake forces
    //    //motorForce = originalMotorForce;
    //    //brakeForce = originalBrakeForce;

    //    // Reset drag and friction settings
    //    rb.drag = 0.1f;  // Set back to normal drag value
    //    rb.angularDrag = 0.05f;

    //    // Reset wheel friction to default
    //    foreach (WheelCollider wc in wheelColliders)
    //    {
    //        WheelFrictionCurve forwardFriction = wc.forwardFriction;
    //        forwardFriction.stiffness = 1f;  // Default stiffness value
    //        wc.forwardFriction = forwardFriction;

    //        WheelFrictionCurve sidewaysFriction = wc.sidewaysFriction;
    //        sidewaysFriction.stiffness = 1f;  // Default stiffness value
    //        wc.sidewaysFriction = sidewaysFriction;

    //        wc.brakeTorque = 0;  // Reset brake force
    //    }
    //}
}
