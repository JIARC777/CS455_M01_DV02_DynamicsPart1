using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinematic : MonoBehaviour
{
    // position from gameObject transform
    // rotation comes from transform as well
    public Vector3 linearVelocity;
    public float angularVelocity; // in degrees
    public GameObject target;
    public float maxSpeed = 40f;
    public bool seek;
    public bool arrive;
    public bool align;
    public bool lookWhereGoing;

    // Update is called once per frame
    void Update()
    {
        transform.position += linearVelocity * Time.deltaTime;
        // adding angular velocity to current transform rotation y component
        transform.eulerAngles += new Vector3(0, angularVelocity * Time.deltaTime, 0);

        // control to switch to proper steering behavior
        if (!arrive)
        {
            Seek mySeek = new Seek();
            mySeek.ai = this;
            // if seek is false set seek property on class to false to activate flee
            if (!seek)
                mySeek.seek = false;
            else
                mySeek.seek = true;
            mySeek.target = target;
            SteeringOutput steering = mySeek.getSteering();
            linearVelocity += steering.linear * Time.deltaTime;
            angularVelocity += steering.angular * Time.deltaTime;
            if (linearVelocity.magnitude > maxSpeed)
            {
                linearVelocity.Normalize();
                linearVelocity *= maxSpeed;
            }
        } else
        {
            Arrive myArrive = new Arrive();
            myArrive.ai = this;
            myArrive.target = target;
            SteeringOutput steering = myArrive.getSteering();
          
            linearVelocity += steering.linear * Time.deltaTime;
            angularVelocity += steering.angular * Time.deltaTime;

        }
        /* commented out after turn in - non-relevant code for assignment was throwing errors
        if (align)
        {
            Align myAlign = new Align();
            myAlign.ai = this;
            myAlign.target = target;
            SteeringOutput steering = myAlign.getSteering();
            if (steering != null)
            {
                linearVelocity += steering.linear * Time.deltaTime;
                angularVelocity += steering.angular * Time.deltaTime;
            }
        }
        if (lookWhereGoing && !align)
        {
            LookWhereGoing myLook = new LookWhereGoing();
            myLook.ai = this;
            target = new GameObject();
            SteeringOutput steering = myLook.getSteering();
            linearVelocity += steering.linear * Time.deltaTime;
            angularVelocity += steering.angular * Time.deltaTime;
            Destroy(target);
        }
        */
    }

}
