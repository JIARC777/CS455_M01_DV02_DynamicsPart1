using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek
{
    public Kinematic ai;
    // Pseudocode has target as Kinematic, but it only needs to be a gameobject with transform.
    public GameObject target;
    float maxAcceleration = 5f;
    public bool seek = true;

    public SteeringOutput getSteering()
    {
        SteeringOutput result = new SteeringOutput();

        //get direction to target
        if (seek)
            result.linear = target.transform.position - ai.transform.position;
        else
            result.linear = ai.transform.position - target.transform.position;
        result.linear.Normalize();
        // give full acceleration
        result.linear *= maxAcceleration;
        result.angular = 0;
        return result;
    }
}

// Notes on arrive
// 2 radii - need add negative acceleration when you hit slow radius (sign missing in pseudocode)
// Line 33 on page 62
// targetSpeed = -1 * (maxSpeed * distance/slowRadius) - scales the velocity based on where you are between two radii
// 7 settings on top of arrive - maxSpeed, max acceleration, and slow radius need to be adjusted to give good results
// Line 25: if distance < targetRadius -> return null (useful when combining steering outputs) - set kinematic to adjust velocity
public class Arrive
{
    public Kinematic ai;
    public GameObject target;
    float maxAcceleration = 100f;
    float targetRadius = 1.5f;
    float slowRadius = 3f;
    float timeToTarget = 0.1f;
    float maxSpeed = .1f;

    public SteeringOutput getSteering()
    {
        SteeringOutput result = new SteeringOutput();
        result.linear = target.transform.position - ai.transform.position;
        float distance = result.linear.magnitude;
        float targetSpeed;
        Vector3 targetVelocity;
       // if (distance < targetRadius)
       //     return null;
        if (distance > slowRadius)
        {
            targetSpeed = maxSpeed;
        } else
        {
            targetSpeed = maxSpeed * (distance - targetRadius) / targetRadius;
        }
        targetVelocity = result.linear;
        targetVelocity.Normalize();
        targetVelocity *= targetSpeed;

        result.linear = targetVelocity - ai.linearVelocity;
        result.linear /= timeToTarget;

        if (result.linear.magnitude > maxAcceleration)
        {
            result.linear.Normalize();
            result.linear *= maxAcceleration;
        }
        result.angular = 0;
        return result;
        
    }
    
}

public class Align
{
    public Kinematic ai;
    public GameObject target;
    float maxAngularAcceleration = 100f;
    float maxRotation = 20f;
    float targetRadius = 1f;
    float slowRadius = 10f;
    float timeToTarget = .1f;

    public SteeringOutput getSteering()
    {
        SteeringOutput result = new SteeringOutput();
        float rotation = Mathf.DeltaAngle(target.transform.rotation.eulerAngles.y, ai.transform.rotation.eulerAngles.y);
        //Debug.Log(rotation);
        float rotationSize = Mathf.Abs(rotation);
        float targetRotation;
        //if (rotationSize < targetRadius)
        // {
        //    result.angular = 0;
        //     Debug.Log("Test");
        //     return null;
        // }    
        if (rotationSize > slowRadius)
        {
            targetRotation = maxRotation;
        }
        else
        {
         //   targetRotation = maxRotation * (rotationSize / targetRadius);
              targetRotation = maxRotation * (rotationSize - targetRadius) / targetRadius;
        }   
        targetRotation *= rotation / rotationSize;

        result.angular = targetRotation - rotation;
        
        result.angular /= timeToTarget;
        Debug.Log(result.angular);
        float angularAcceleration = Mathf.Abs(result.angular);
        if (angularAcceleration > maxAngularAcceleration)
        {
            result.angular /= angularAcceleration;
            result.angular *= maxAngularAcceleration;
        }
       // Debug.Log(result.angular);
        result.linear = Vector3.zero;
        return result;
    }
}

public class LookWhereGoing: Align {
    public SteeringOutput GetSteering()
    {
        Vector3 velocity = ai.linearVelocity;
        if (velocity.magnitude == 0)
        {
            return null;
        }
       // float angleVector = Mathf.Atan2(-velocity.x, velocity.z);
        target.transform.position = new Vector3(-velocity.x, 1, velocity.z);
        Align rotate = new Align();
        rotate.target = target;
        return rotate.getSteering();
    }
}

public class Face: Align
{
    public GameObject target;

    SteeringOutput GetSteering()
    {
        Vector3 direction = (target.transform.position - ai.transform.position);
        if (direction.magnitude == 0)
        {
            return target;
        }
        Align rotate = new Align();
        
        rotate.target = target;

    }
}