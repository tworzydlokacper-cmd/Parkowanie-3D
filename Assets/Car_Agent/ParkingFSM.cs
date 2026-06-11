using UnityEngine;

// 1. INTERFEJS STANU
public interface ICarState
{
    void Enter(ParkingFSM fsm);
    void Execute(ParkingFSM fsm);
    void Exit(ParkingFSM fsm);
}

// 2. KLASA KONTEKSTU (Główny skrypt)
public class ParkingFSM : MonoBehaviour
{
    public CarController controller;
    public CarSensors sensors;

    private ICarState currentState;

    public PatrolState patrolState = new PatrolState();
    public ParallelParkState parallelState = new ParallelParkState();
    public PerpendicularParkState perpendicularState = new PerpendicularParkState();
    public EmergencyState emergencyState = new EmergencyState();

    void Start()
    {
        controller = GetComponent<CarController>();
        sensors = GetComponent<CarSensors>();
        
        ChangeState(patrolState);
    }

    void FixedUpdate()
    {
        // Globalne przerwanie - uwaga, to zaraz rozwiniemy o HFSM!
        if (sensors.isFrontBlocked && currentState != emergencyState)
        {
            ChangeState(emergencyState);
        }

        if (currentState != null)
        {
            currentState.Execute(this);
        }
    }

    public void ChangeState(ICarState newState)
    {
        if (currentState != null)
            currentState.Exit(this);

        currentState = newState;
        currentState.Enter(this);
    }
}

// ==========================================================
// 3. OSOBNE KLASY STANÓW 
// ==========================================================

public class PatrolState : ICarState
{
    public void Enter(ParkingFSM fsm) { }

    public void Execute(ParkingFSM fsm)
    {
        fsm.controller.MoveCar(0.3f, 0f);

        if (fsm.sensors.isGapValid && !fsm.sensors.isGapDetected)
        {
            if (fsm.sensors.parkingMode == 1)
                fsm.ChangeState(fsm.parallelState);
            else if (fsm.sensors.parkingMode == 2)
                fsm.ChangeState(fsm.perpendicularState);
        }
    }

    public void Exit(ParkingFSM fsm) { }
}

public class ParallelParkState : ICarState
{
    private float initialYAngle;
    private int phase;
    private float timer;
    private Vector3 phase2StartPosition; // Nowy czujnik (Odometria)

    public void Enter(ParkingFSM fsm)
    {
        initialYAngle = fsm.transform.eulerAngles.y;
        phase = 0;
        timer = 0f;
    }

    public void Execute(ParkingFSM fsm)
    {
        if (phase == 0) {
            fsm.controller.StopCar();
            timer += Time.fixedDeltaTime;
            if (timer > 1.0f) { timer = 0f; phase = 1; }
        }
        else if (phase == 1) {
            fsm.controller.MoveCar(-0.4f, 1f);
            float currentAngle = Mathf.DeltaAngle(initialYAngle, fsm.transform.eulerAngles.y);
            
            if (currentAngle < -35f) { 
                phase = 2; 
                // Zapisujemy pozycję, w której auto skończyło łamać się do zatoki
                phase2StartPosition = fsm.transform.position; 
            }
        }
        else if (phase == 2) {
            fsm.controller.MoveCar(-0.4f, 0f);
            
            // ROZWIĄZANIE UNIWERSALNE: Cofa prosto przez równe 1.8 metra (odometria), niezależnie od pozycji na mapie!
            float distanceTraveled = Vector3.Distance(phase2StartPosition, fsm.transform.position);
            if (distanceTraveled > 1.8f) { phase = 3; } 
        }
        else if (phase == 3) {
            fsm.controller.MoveCar(-0.4f, -1f);
            float currentAngle = Mathf.DeltaAngle(initialYAngle, fsm.transform.eulerAngles.y);
            if (currentAngle > -2f) { phase = 4; }
        }
        else if (phase == 4) {
            fsm.controller.StopCar();
            if (fsm.sensors.uiText != null) {
                fsm.sensors.uiText.text = "Koperta perfekcyjna!";
                fsm.sensors.uiText.color = Color.magenta;
            }
        }
    }

    public void Exit(ParkingFSM fsm) { }
}

public class PerpendicularParkState : ICarState
{
    private float initialYAngle;
    private int phase;
    private float timer;
    private Vector3 phase2StartPosition;

    public void Enter(ParkingFSM fsm)
    {
        initialYAngle = fsm.transform.eulerAngles.y;
        phase = 0;
        timer = 0f;
    }

    public void Execute(ParkingFSM fsm)
    {
        if (phase == 0) {
            fsm.controller.StopCar();
            timer += Time.fixedDeltaTime;
            if (timer > 1.0f) { timer = 0f; phase = 1; }
        }
        else if (phase == 1) {
            fsm.controller.MoveCar(-0.4f, 1f);
            float currentAngle = Mathf.DeltaAngle(initialYAngle, fsm.transform.eulerAngles.y);
            
            if (currentAngle < -85f) { 
                phase = 2; 
                phase2StartPosition = fsm.transform.position;
            }
        }
        else if (phase == 2) {
            fsm.controller.MoveCar(-0.4f, 0f);
            
            // UNIWERSALNE: Wjeżdża na głębokość 2.0 metrów od momentu naprostowania się
            float distanceTraveled = Vector3.Distance(phase2StartPosition, fsm.transform.position);
            if (distanceTraveled > 2.0f) { phase = 3; } 
        }
        else if (phase == 3) {
            fsm.controller.StopCar();
            if(fsm.sensors.uiText != null) {
                fsm.sensors.uiText.text = "Prostopadłe perfekcyjne!";
                fsm.sensors.uiText.color = Color.cyan;
            }
        }
    }

    public void Exit(ParkingFSM fsm) { }
}

public class EmergencyState : ICarState
{
    public void Enter(ParkingFSM fsm)
    {
        fsm.controller.StopCar();
        if (fsm.sensors.uiText != null)
        {
            fsm.sensors.uiText.text = "KOLIZJA!";
            fsm.sensors.uiText.color = Color.red;
        }
    }

    public void Execute(ParkingFSM fsm)
    {
        fsm.controller.StopCar(); 
    }

    public void Exit(ParkingFSM fsm) { }
}