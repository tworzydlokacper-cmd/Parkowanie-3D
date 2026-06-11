using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Fizyczne Koła")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("Parametry Jazdy")]
    public float maxMotorForce = 2000f;
    public float maxBrakeForce = 3000f;
    public float maxSteerAngle = 35f;

    [Header("Geometria Ackermanna")]
    [Tooltip("Dystans między przednią a tylną osią (L)")]
    public float wheelbase = 2.7f; 
    [Tooltip("Dystans między lewym a prawym kołem (W)")]
    public float trackWidth = 1.5f; 

    [Header("Regulator PID (P-Controller)")]
    [Tooltip("Wzmocnienie - jak mocno auto reaguje na błąd odległości")]
    public float pGain = 1500f; 

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 1500f;
        rb.centerOfMass = new Vector3(0f, -1.0f, 0f);
    }

    public void MoveCar(float gasInput, float steeringInput)
    {
        ApplyMotorTorque(gasInput * maxMotorForce);
        ApplyAckermannSteering(steeringInput * maxSteerAngle);
    }

    public void MoveWithPID(float distanceError, float steeringInput)
    {
        float targetForce = distanceError * pGain;
        targetForce = Mathf.Clamp(targetForce, -maxMotorForce, maxMotorForce);

        ApplyMotorTorque(targetForce);
        ApplyAckermannSteering(steeringInput * maxSteerAngle);
    }

    public void StopCar()
    {
        rearLeftWheel.motorTorque = rearRightWheel.motorTorque = 0f;
        frontLeftWheel.brakeTorque = frontRightWheel.brakeTorque = maxBrakeForce;
        rearLeftWheel.brakeTorque = rearRightWheel.brakeTorque = maxBrakeForce;
    }

    private void ApplyMotorTorque(float force)
    {
        if (Mathf.Abs(force) < 10f) 
        {
            StopCar();
            return;
        }

        frontLeftWheel.brakeTorque = frontRightWheel.brakeTorque = 0f;
        rearLeftWheel.brakeTorque = rearRightWheel.brakeTorque = 0f;

        rearLeftWheel.motorTorque = force;
        rearRightWheel.motorTorque = force;
    }

    private void ApplyAckermannSteering(float steerAngle)
    {
        if (Mathf.Abs(steerAngle) < 0.1f)
        {
            frontLeftWheel.steerAngle = 0f;
            frontRightWheel.steerAngle = 0f;
            return;
        }

        float angleRad = Mathf.Abs(steerAngle) * Mathf.Deg2Rad;

        float insideAngle = Mathf.Atan(wheelbase / ((wheelbase / Mathf.Tan(angleRad)) - (trackWidth / 2f))) * Mathf.Rad2Deg;
        float outsideAngle = Mathf.Atan(wheelbase / ((wheelbase / Mathf.Tan(angleRad)) + (trackWidth / 2f))) * Mathf.Rad2Deg;

        if (steerAngle > 0) 
        {
            frontRightWheel.steerAngle = insideAngle;  
            frontLeftWheel.steerAngle = outsideAngle;
        }
        else 
        {
            frontLeftWheel.steerAngle = -insideAngle;  
            frontRightWheel.steerAngle = -outsideAngle;
        }
    }
}