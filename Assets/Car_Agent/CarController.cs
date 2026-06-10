using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Fizyczne Koła")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("Parametry Jazdy")]
    // ZWIĘKSZONO MOC (500f na 2000f), żeby na pewno ruszył z masą 1500kg
    public float maxMotorForce = 2000f; 
    public float maxBrakeForce = 3000f;
    public float maxSteerAngle = 35f;
    public float timeToFullLock = 1f;

    private float currentSteerAngle = 0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Zabezpieczenie przed wywrotkami i masa 1.5 tony
        rb.mass = 1500f; 
        rb.centerOfMass = new Vector3(0f, -1.5f, 0f); 
    }

    public void MoveCar(float gasInput, float steeringInput)
    {
        // Hamowanie, gdy nie ma gazu
        if (gasInput == 0f)
        {
            rearLeftWheel.motorTorque = rearRightWheel.motorTorque = 0f;
            frontLeftWheel.brakeTorque = frontRightWheel.brakeTorque = maxBrakeForce;
            rearLeftWheel.brakeTorque = rearRightWheel.brakeTorque = maxBrakeForce;
        }
        else
        {
            frontLeftWheel.brakeTorque = frontRightWheel.brakeTorque = 0f;
            rearLeftWheel.brakeTorque = rearRightWheel.brakeTorque = 0f;
            
            float torque = gasInput * maxMotorForce;
            rearLeftWheel.motorTorque = rearRightWheel.motorTorque = torque;
        }

        // Układ kierowniczy
        float targetAngle = steeringInput * maxSteerAngle;
        float steerSpeed = (maxSteerAngle * 2) / timeToFullLock;
        
        currentSteerAngle = Mathf.MoveTowards(currentSteerAngle, targetAngle, steerSpeed * Time.deltaTime);

        frontLeftWheel.steerAngle = frontRightWheel.steerAngle = currentSteerAngle;
    }

    public float GetSteeringAngleTowards(Vector3 targetPosition)
    {
        Vector3 relativePos = transform.InverseTransformPoint(targetPosition);
        float angle = Mathf.Atan2(relativePos.x, relativePos.z) * Mathf.Rad2Deg;
        return Mathf.Clamp(angle / maxSteerAngle, -1f, 1f);
    }
}