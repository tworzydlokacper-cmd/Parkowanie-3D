using UnityEngine;

public class ParkingFSM : MonoBehaviour
{
    public enum ParkingState 
    { 
        Patrol, 
        Prepare, 
        Maneuver, 
        Emergency 
    }

    [Header("Status Mózgu")]
    public ParkingState currentState = ParkingState.Patrol;

    private CarController controller;
    private CarSensors sensors;

    void Start()
    {
        controller = GetComponent<CarController>();
        sensors = GetComponent<CarSensors>();
    }

    void FixedUpdate()
    {
        // --- 1. PRIORYTET: SYSTEM AWARYJNY ---
        // Bez względu na to, co robimy, jeśli z przodu jest przeszkoda -> HAMUJ!
        if (sensors.isFrontBlocked && currentState != ParkingState.Emergency)
        {
            currentState = ParkingState.Emergency;
        }

        // --- 2. GŁÓWNA MASZYNA STANÓW ---
        switch (currentState)
        {
            case ParkingState.Patrol:
                PatrolState();
                break;
            case ParkingState.Prepare:
                PrepareState();
                break;
            case ParkingState.Maneuver:
                ManeuverState();
                break;
            case ParkingState.Emergency:
                EmergencyState();
                break;
        }
    }

    void PatrolState()
    {
        controller.MoveCar(0.3f, 0f);

        if (sensors.isGapValid && !sensors.isGapDetected)
        {
            currentState = ParkingState.Prepare;
        }
    }

    void PrepareState()
    {
        controller.MoveCar(0f, 0f);
    }

    void ManeuverState()
    {
        // Zostawiamy na później
    }

    void EmergencyState()
    {
        // 1. Odcięcie gazu i potężny hamulec
        controller.MoveCar(0f, 0f);
        
        // 2. Wymuszamy czerwony komunikat na ekranie
        if (sensors.uiText != null)
        {
            sensors.uiText.text = "AWARYJNE HAMOWANIE!\nPrzeszkoda z przodu!";
            sensors.uiText.color = Color.red;
        }
    }
}