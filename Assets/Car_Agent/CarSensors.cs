using UnityEngine;
using TMPro;

public class CarSensors : MonoBehaviour
{
    [Header("Ustawienia Zasięgu")]
    public float sideSensorLength = 10f; 
    public float frontBackSensorLength = 3f;

    [Header("Dane dla Maszyny Stanów")]
    public bool isGapDetected = false;
    public bool isGapValid = false;
    public bool isFrontBlocked = false;
    public int parkingMode = 0; // 0 = brak, 1 = koperta, 2 = prostopadłe
    public TextMeshProUGUI uiText;

    private Vector3 gapStartPosition;
    private bool hasSeenFirstCar = false; 

    // Lasery boczne podniesione na Y = 0.5f, żeby nie trafiały w asfalt
    public Vector3[] rightSensors = { new Vector3(1, 0.5f, 2), new Vector3(1, 0.5f, 0.6f), new Vector3(1, 0.5f, -0.6f), new Vector3(1, 0.5f, -2) };
    public Vector3[] leftSensors = { new Vector3(-1, 0.5f, 2), new Vector3(-1, 0.5f, 0.6f), new Vector3(-1, 0.5f, -0.6f), new Vector3(-1, 0.5f, -2) };
    
    public Vector3[] frontSensors = { new Vector3(0.8f, 0, 2.25f), new Vector3(0.3f, 0, 2.25f), new Vector3(-0.3f, 0, 2.25f), new Vector3(-0.8f, 0, 2.25f) };
    public Vector3[] backSensors = { new Vector3(0.8f, 0, -2.25f), new Vector3(0.3f, 0, -2.25f), new Vector3(-0.3f, 0, -2.25f), new Vector3(-0.8f, 0, -2.25f) };

    void FixedUpdate() 
    {
        bool rightSideClear = ScanDirection(rightSensors, transform.right, sideSensorLength, Color.green);
        ScanDirection(leftSensors, -transform.right, sideSensorLength, Color.green);
        
        bool frontClear = ScanDirection(frontSensors, transform.forward, frontBackSensorLength, Color.blue);
        isFrontBlocked = !frontClear;
        
        ScanDirection(backSensors, -transform.forward, frontBackSensorLength, Color.yellow);

        MeasureParkingGap(rightSideClear);
    }

    bool ScanDirection(Vector3[] sensors, Vector3 direction, float length, Color color)
    {
        bool isClear = true;
        foreach (Vector3 pos in sensors)
        {
            Vector3 startPos = transform.position + (transform.rotation * pos);
            if (Physics.Raycast(startPos, direction, length)) { Debug.DrawRay(startPos, direction * length, Color.red); isClear = false; }
            else { Debug.DrawRay(startPos, direction * length, color); }
        }
        return isClear;
    }

    void MeasureParkingGap(bool isSideClear)
    {
        // 1. Zdjęcie blokady po zauważeniu pierwszego zaparkowanego auta
        if (!isSideClear) 
        {
            hasSeenFirstCar = true;
        }

        // 2. Start pomiaru luki
        if (isSideClear && !isGapDetected && hasSeenFirstCar)
        {
            isGapDetected = true; isGapValid = false; parkingMode = 0;
            gapStartPosition = transform.position;
            if (uiText != null) { uiText.text = "Skanowanie..."; uiText.color = Color.yellow; }
        }
        // 3. Koniec luki i weryfikacja wymiarów
        else if (!isSideClear && isGapDetected)
        {
            isGapDetected = false;
            
            // KOREKTA INŻYNIERSKA: Dystans fizyczny luki to przejechany odcinek + rozstaw skanerów na pojeździe (4 metry)
            float traveledDistance = Vector3.Distance(gapStartPosition, transform.position);
            float actualGap = traveledDistance + 4.0f; 
            
            Debug.Log($"Wykryto lukę! Przejechano: {traveledDistance}m | Fizyczny rozmiar luki: {actualGap}m");

            // Rozpoznawanie trybu parkowania na podstawie skorygowanego rozmiaru luki
            if (actualGap >= 6.0f) { 
                isGapValid = true; parkingMode = 1; 
                if (uiText != null) { uiText.text = "Koperta!"; uiText.color = Color.green; } 
            }
            else if (actualGap >= 3.0f) { 
                isGapValid = true; parkingMode = 2; 
                if (uiText != null) { uiText.text = "Prostopadłe!"; uiText.color = Color.cyan; } 
            }
            else { 
                isGapValid = false; parkingMode = 0; 
                if (uiText != null) { uiText.text = "Za mała!"; uiText.color = Color.red; } 
            }
        }
    }
}