using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Fizyczne Koła (Podepnij w Inspektorze)")]
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
    }

    public void MoveCar(float gasInput, float steeringInput)
    {
        if (gasInput == 0f)
        {
            rearLeftWheel.motorTorque = 0f;
            rearRightWheel.motorTorque = 0f;

            frontLeftWheel.brakeTorque = maxBrakeForce;
            frontRightWheel.brakeTorque = maxBrakeForce;
            rearLeftWheel.brakeTorque = maxBrakeForce;
            rearRightWheel.brakeTorque = maxBrakeForce;
        }
        else
        {
            frontLeftWheel.brakeTorque = 0f;
            frontRightWheel.brakeTorque = 0f;
            rearLeftWheel.brakeTorque = 0f;
            rearRightWheel.brakeTorque = 0f;

            float torque = gasInput * maxMotorForce;
            rearLeftWheel.motorTorque = torque;
            rearRightWheel.motorTorque = torque;
        }

        float speed = rb.linearVelocity.magnitude;
        if (speed > 0.1f || Mathf.Abs(gasInput) > 0.1f) 
        {
            float targetAngle = steeringInput * maxSteerAngle;
            float steerSpeed = (maxSteerAngle * 2) / timeToFullLock; 
            currentSteerAngle = Mathf.MoveTowards(currentSteerAngle, targetAngle, steerSpeed * Time.deltaTime);
        }

        frontLeftWheel.steerAngle = currentSteerAngle;
        frontRightWheel.steerAngle = currentSteerAngle;
    }
}