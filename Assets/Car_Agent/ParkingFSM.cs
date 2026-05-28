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
                
                // ==========================================
                // TRYB 1: KOPERTA (Twoja działająca wersja)
                // ==========================================
                if (sensors.parkingMode == 1) 
                {
                    if (phase == 0) 
                    {
                        // ETAP 1: Ostre hamowanie! Zatrzymujemy się przed manewrem na 1 sekundę.
                        controller.MoveCar(0f, 0f);
                        timer += Time.fixedDeltaTime;
                        if (timer > 1.0f) { timer = 0f; phase = 1; }
                    }
                    else if (phase == 1) 
                    {
                        // ETAP 2: Wolne łamanie w prawo na wstecznym
                        controller.MoveCar(-0.2f, 1f);
                        float currentAngle = Mathf.DeltaAngle(initialYAngle, transform.eulerAngles.y);
                        if (currentAngle < -35f) { phase = 2; timer = 0f; }
                    }
                    else if (phase == 2) 
                    {
                        // ETAP 3: Wolne cofanie w głąb luki na prostych kołach
                        controller.MoveCar(-0.2f, 0f);
                        timer += Time.fixedDeltaTime;
                        
                        // Czas wjazdu w głąb luki (ustawiony na 1.8f)
                        if (timer > 1.8f) { phase = 3; } 
                    }
                    else if (phase == 3) 
                    {
                        // ETAP 4: Wolne prostowanie w lewo
                        controller.MoveCar(-0.2f, -1f);
                        float currentAngle = Mathf.DeltaAngle(initialYAngle, transform.eulerAngles.y);
                        if (currentAngle > -5f) { phase = 4; }
                    }
                    else if (phase == 4) 
                    {
                        // ETAP 5: Zaciągnięcie hamulca
                        controller.MoveCar(0f, 0f);
                        if(sensors.uiText != null) {
                            sensors.uiText.text = "Koperta perfekcyjna!";
                            sensors.uiText.color = Color.magenta;
                        }
                    }
                }
                
                // ==========================================
                // TRYB 2: PARKOWANIE PROSTOPADŁE
                // ==========================================
                else if (sensors.parkingMode == 2) 
                {
                    if (phase == 0) 
                    {
                        // ETAP 1: Ostre hamowanie i sekunda przerwy przed manewrem
                        controller.MoveCar(0f, 0f);
                        timer += Time.fixedDeltaTime;
                        if (timer > 1.0f) { timer = 0f; phase = 1; }
                    }
                    else if (phase == 1) 
                    {
                        // ETAP 2: Cofamy z maksymalnym skrętem w prawo (aż złapiemy kąt 90 stopni)
                        controller.MoveCar(-0.2f, 1f);
                        float currentAngle = Mathf.DeltaAngle(initialYAngle, transform.eulerAngles.y);
                        
                        // Ustawiamy -85 zamiast -90, żeby uwzględnić bezwładność auta
                        if (currentAngle < -85f) { phase = 2; timer = 0f; }
                    }
                    else if (phase == 2) 
                    {
                        // ETAP 3: Prostujemy kierownicę i jedziemy do tyłu w głąb luki
                        controller.MoveCar(-0.2f, 0f);
                        timer += Time.fixedDeltaTime;
                        
                        // Ten czas decyduje, jak głęboko w lukę wjedzie auto
                        if (timer > 2.0f) { phase = 3; } 
                    }
                    else if (phase == 3) 
                    {
                        // ETAP 4: Zaciągnięcie hamulca
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