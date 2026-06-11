using UnityEngine;

// 1. INTERFEJS STANU (Zgodnie z wymaganiami z dokumentacji!)
public interface ICarState
{
    void Enter(ParkingFSM fsm);
    void Execute(ParkingFSM fsm);
    void Exit(ParkingFSM fsm);
}

// 2. KLASA KONTEKSTU (Główny skrypt FSM podpięty do auta)
public class ParkingFSM : MonoBehaviour
{
    public CarController controller;
    public CarSensors sensors;

    private ICarState currentState;

    // Instancje stanów gotowe do użycia
    public PatrolState patrolState = new PatrolState();
    public ParallelParkState parallelState = new ParallelParkState();
    public PerpendicularParkState perpendicularState = new PerpendicularParkState();
    public EmergencyState emergencyState = new EmergencyState();

    void Start()
    {
        controller = GetComponent<CarController>();
        sensors = GetComponent<CarSensors>();
        
        // Zaczynamy od szukania luki
        ChangeState(patrolState);
    }

    void FixedUpdate()
    {
        // Globalne przerwanie - awaryjne hamowanie (wymóg dla HFSM i Mapy C)
        if (sensors.isFrontBlocked && currentState != emergencyState)
        {
            ChangeState(emergencyState);
        }

        // Wykonywanie logiki aktywnego stanu
        if (currentState != null)
        {
            currentState.Execute(this);
        }
    }

    // Funkcja zarządzająca przejściami (Tranzycjami)
    public void ChangeState(ICarState newState)
    {
        if (currentState != null)
            currentState.Exit(this);

        currentState = newState;
        currentState.Enter(this);
    }
}

// ==========================================================
// 3. OSOBNE KLASY STANÓW (Wzorzec State / Gang of Four)
// ==========================================================

public class PatrolState : ICarState
{
    public void Enter(ParkingFSM fsm) { }

    public void Execute(ParkingFSM fsm)
    {
        fsm.controller.MoveCar(0.3f, 0f); // Patroluj powoli

        // Tranzycja do odpowiedniego manewru
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
            fsm.controller.MoveCar(-0.2f, 1f);
            float currentAngle = Mathf.DeltaAngle(initialYAngle, fsm.transform.eulerAngles.y);
            if (currentAngle < -35f) { phase = 2; timer = 0f; }
        }
        else if (phase == 2) {
            fsm.controller.MoveCar(-0.2f, 0f);
            timer += Time.fixedDeltaTime;
            if (timer > 2.3f) { phase = 3; }
        }
        else if (phase == 3) {
            fsm.controller.MoveCar(-0.2f, -1f);
            float currentAngle = Mathf.DeltaAngle(initialYAngle, fsm.transform.eulerAngles.y);
            if (currentAngle > -5f) { phase = 4; }
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
            fsm.controller.MoveCar(-0.2f, 1f);
            float currentAngle = Mathf.DeltaAngle(initialYAngle, fsm.transform.eulerAngles.y);
            if (currentAngle < -85f) { phase = 2; timer = 0f; }
        }
        else if (phase == 2) {
            fsm.controller.MoveCar(-0.2f, 0f);
            timer += Time.fixedDeltaTime;
            if (timer > 2.0f) { phase = 3; }
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
        fsm.controller.StopCar(); // Utrzymaj hamulec
    }

    public void Exit(ParkingFSM fsm) { }
}