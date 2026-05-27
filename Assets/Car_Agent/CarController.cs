using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Fizyczne Koła (Podepnij w Inspektorze)")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("Parametry Jazdy")]
    public float maxMotorForce = 500f; // Moc silnika
    public float maxSteerAngle = 35f;  // Maksymalny skręt z dokumentacji
    public float timeToFullLock = 1f;  // Czas obrotu kół od lewej do prawej

    private float currentSteerAngle = 0f;
    private Rigidbody rb;

    void Start()
    {
        // Pobieramy fizykę auta, żeby móc sprawdzać jego prędkość
        rb = GetComponent<Rigidbody>(); 
    }

    // Tę funkcję będzie w przyszłości wywoływać Maszyna Stanów
    // gasInput: od -1 (wsteczny) do 1 (gaz do dechy)
    // steeringInput: od -1 (lewo) do 1 (prawo)
    public void MoveCar(float gasInput, float steeringInput)
    {
        // --- 1. SILNIK (Napęd na tył) ---
        float torque = gasInput * maxMotorForce;
        rearLeftWheel.motorTorque = torque;
        rearRightWheel.motorTorque = torque;

        // --- 2. UKŁAD KIEROWNICZY Z OGRANICZENIAMI ---
        float speed = rb.linearVelocity.magnitude;
        
        // Zgodnie z dokumentacją: kręcimy tylko, jeśli auto jedzie (lub dostaje gaz)
        if (speed > 0.1f || Mathf.Abs(gasInput) > 0.1f) 
        {
            float targetAngle = steeringInput * maxSteerAngle;
            
            // Prędkość kręcenia kierownicą (od -35 do 35 to 70 stopni)
            float steerSpeed = (maxSteerAngle * 2) / timeToFullLock; 
            
            // Płynne, mechaniczne zmienianie kąta (symulacja serwomechanizmu)
            currentSteerAngle = Mathf.MoveTowards(currentSteerAngle, targetAngle, steerSpeed * Time.deltaTime);
        }

        // Aplikujemy wyliczony kąt na przednie koła
        frontLeftWheel.steerAngle = currentSteerAngle;
        frontRightWheel.steerAngle = currentSteerAngle;
    }
}