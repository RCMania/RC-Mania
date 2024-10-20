using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteerSystem
{
    private WheelCollider[] wheelColliders;
    private float wheelBase;
    private float rearTrack;
    private float currentTurnRadius;
    private float baseTurnRadius;

    public SteerSystem(WheelCollider[] wheelColliders, float wheelBase, float rearTrack, float baseTurnRadius)
    {
        this.wheelColliders = wheelColliders;
        this.wheelBase = wheelBase;
        this.rearTrack = rearTrack;
        this.baseTurnRadius = baseTurnRadius;
    }

    public void AckermanSteering(float steerInput)
    {
        currentTurnRadius = baseTurnRadius;

        float innerWheelAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (currentTurnRadius + (rearTrack / 2))) * steerInput;
        float outterWheelAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (currentTurnRadius - (rearTrack / 2))) * steerInput;

        if (steerInput > 0)
        {
            // Left wheel is outer, right wheel is inner
            wheelColliders[0].steerAngle = outterWheelAngle;
            wheelColliders[1].steerAngle = innerWheelAngle;
        }
        else if (steerInput < 0)
        {
            // Left wheel is inner, right wheel is outer
            wheelColliders[0].steerAngle = innerWheelAngle;
            wheelColliders[1].steerAngle = outterWheelAngle;
        }
        else
        {
            wheelColliders[0].steerAngle = 0;
            wheelColliders[1].steerAngle = 0;
        }
    }
}
