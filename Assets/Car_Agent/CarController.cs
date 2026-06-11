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
    public float wheelbase = 1.76f; 
    [Tooltip("Dystans między lewym a prawym kołem (W)")]
    public float trackWidth = 1.0f; 

    [Header("Regulator PID (P-Controller)")]
    [Tooltip("Wzmocnienie - jak mocno auto reaguje na błąd odległości")]
    public float pGain = 1500f; 

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Zabezpieczenie fizyki auta
        if (rb != null)
        {
            rb.mass = 1500f;
            rb.centerOfMass = new Vector3(0f, -1.0f, 0f);
        }
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

        // KOREKTA INŻYNIERSKA: Zabezpieczenie przed polskim przecinkiem/kropką w Unity!
        // Jeśli Unity wyzeruje zmienne, wymuszamy nasze bezpieczne wartości.
        float safeWheelbase = wheelbase <= 0.1f ? 1.76f : wheelbase;
        float safeTrackWidth = trackWidth <= 0.1f ? 1.0f : trackWidth;

        float angleRad = Mathf.Abs(steerAngle) * Mathf.Deg2Rad;

        // Geometria Ackermanna - koło wewnętrzne skręca pod większym kątem
        float insideAngle = Mathf.Atan(safeWheelbase / ((safeWheelbase / Mathf.Tan(angleRad)) - (safeTrackWidth / 2f))) * Mathf.Rad2Deg;
        float outsideAngle = Mathf.Atan(safeWheelbase / ((safeWheelbase / Mathf.Tan(angleRad)) + (safeTrackWidth / 2f))) * Mathf.Rad2Deg;

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