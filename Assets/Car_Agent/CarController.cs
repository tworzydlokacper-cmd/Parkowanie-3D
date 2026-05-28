using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Fizyczne Koła")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("Parametry Jazdy")]
    public float maxMotorForce = 500f;
    public float maxBrakeForce = 3000f;
    public float maxSteerAngle = 35f;
    public float timeToFullLock = 1f;

    private float currentSteerAngle = 0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Zabezpieczenie przed wywrotkami
        rb.mass = 1500f; 
        rb.centerOfMass = new Vector3(0f, -1.5f, 0f); 
    }

    public void MoveCar(float gasInput, float steeringInput)
    {
        if (gasInput == 0f)
        {
            rearLeftWheel.motorTorque = 0f;
            rearRightWheel.motorTorque = 0f;
            frontLeftWheel.brakeTorque = frontRightWheel.brakeTorque = maxBrakeForce;
            rearLeftWheel.brakeTorque = rearRightWheel.brakeTorque = maxBrakeForce;
        }
        else
        {
            frontLeftWheel.brakeTorque = frontRightWheel.brakeTorque = 0f;
            rearLeftWheel.brakeTorque = rearRightWheel.brakeTorque = 0f;
            
            float torque = gasInput * maxMotorForce;
            rearLeftWheel.motorTorque = torque;
            rearRightWheel.motorTorque = torque;
        }

        float targetAngle = steeringInput * maxSteerAngle;
        float steerSpeed = (maxSteerAngle * 2) / timeToFullLock;
        
        currentSteerAngle = Mathf.MoveTowards(currentSteerAngle, targetAngle, steerSpeed * Time.deltaTime);

        frontLeftWheel.steerAngle = currentSteerAngle;
        frontRightWheel.steerAngle = currentSteerAngle;
    }

    public float GetSteeringAngleTowards(Vector3 targetPosition)
    {
        Vector3 relativePos = transform.InverseTransformPoint(targetPosition);
        float angle = Mathf.Atan2(relativePos.x, relativePos.z) * Mathf.Rad2Deg;
        return Mathf.Clamp(angle / maxSteerAngle, -1f, 1f);
    }

    public float GetDistanceTo(Vector3 targetPosition)
    {
        return Vector3.Distance(transform.position, targetPosition);
    }
}