using UnityEngine;

public class ParkingFSM : MonoBehaviour
{
    public enum ParkingState { Patrol, Maneuver, Emergency }
    public ParkingState currentState = ParkingState.Patrol;

    private float initialYAngle;
    private int phase = 0;
    private float timer = 0f;

    private CarController controller;
    private CarSensors sensors;

    void Start() 
    {
        controller = GetComponent<CarController>();
        sensors = GetComponent<CarSensors>();
    }

    void FixedUpdate() 
    {
        if (sensors.isFrontBlocked) currentState = ParkingState.Emergency;

        switch (currentState) 
        {
            case ParkingState.Patrol:
                // Gaz 0.3f, żeby powoli skanował
                controller.MoveCar(0.3f, 0f);
                if (sensors.isGapValid && !sensors.isGapDetected) 
                {
                    initialYAngle = transform.eulerAngles.y;
                    phase = 0;
                    timer = 0f;
                    currentState = ParkingState.Maneuver;
                }
                break;

            case ParkingState.Maneuver:
                
                // === TRYB 1: KOPERTA (Twoja wersja) ===
                if (sensors.parkingMode == 1) 
                {
                    if (phase == 0) {
                        controller.MoveCar(0f, 0f);
                        timer += Time.fixedDeltaTime;
                        if (timer > 1.0f) { timer = 0f; phase = 1; }
                    }
                    else if (phase == 1) {
                        controller.MoveCar(-0.2f, 1f);
                        float currentAngle = Mathf.DeltaAngle(initialYAngle, transform.eulerAngles.y);
                        if (currentAngle < -35f) { phase = 2; timer = 0f; }
                    }
                    else if (phase == 2) {
                        controller.MoveCar(-0.2f, 0f);
                        timer += Time.fixedDeltaTime;
                        // Zachowano Twoje 1.8f wjazdu w głąb luki
                        if (timer > 1.8f) { phase = 3; } 
                    }
                    else if (phase == 3) {
                        controller.MoveCar(-0.2f, -1f);
                        float currentAngle = Mathf.DeltaAngle(initialYAngle, transform.eulerAngles.y);
                        if (currentAngle > -5f) { phase = 4; }
                    }
                    else if (phase == 4) {
                        controller.MoveCar(0f, 0f);
                        if(sensors.uiText != null) {
                            sensors.uiText.text = "Koperta perfekcyjna!";
                            sensors.uiText.color = Color.magenta;
                        }
                    }
                }
                
                // === TRYB 2: PROSTOPADŁE ===
                else if (sensors.parkingMode == 2) 
                {
                    if (phase == 0) {
                        controller.MoveCar(0f, 0f);
                        timer += Time.fixedDeltaTime;
                        if (timer > 1.0f) { timer = 0f; phase = 1; }
                    }
                    else if (phase == 1) {
                        controller.MoveCar(-0.2f, 1f);
                        float currentAngle = Mathf.DeltaAngle(initialYAngle, transform.eulerAngles.y);
                        // Złamanie do -85 stopnia
                        if (currentAngle < -85f) { phase = 2; timer = 0f; }
                    }
                    else if (phase == 2) {
                        controller.MoveCar(-0.2f, 0f);
                        timer += Time.fixedDeltaTime;
                        // Wjazd w głąb na 2 sekundy
                        if (timer > 2.0f) { phase = 3; } 
                    }
                    else if (phase == 3) {
                        controller.MoveCar(0f, 0f);
                        if(sensors.uiText != null) {
                            sensors.uiText.text = "Prostopadłe perfekcyjne!";
                            sensors.uiText.color = Color.cyan;
                        }
                    }
                }
                break;

            case ParkingState.Emergency:
                controller.MoveCar(0f, 0f);
                if(sensors.uiText != null) {
                    sensors.uiText.text = "KOLIZJA!";
                    sensors.uiText.color = Color.red;
                }
                break;
        }
    }
}