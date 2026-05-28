using UnityEngine;
using TMPro;

public class CarSensors : MonoBehaviour
{
    [Header("Ustawienia Zasięgu")]
    public float sideSensorLength = 5f;
    public float frontBackSensorLength = 3f;

    [Header("Dane dla Maszyny Stanów")]
    public bool isGapDetected = false;
    public bool isGapValid = false;
    public bool isFrontBlocked = false;
    public int parkingMode = 0; // NOWA ZMIENNA: 0 = brak, 1 = koperta, 2 = prostopadłe
    public TextMeshProUGUI uiText;

    private Vector3 gapStartPosition;

    public Vector3[] rightSensors = { new Vector3(1, 0, 2), new Vector3(1, 0, 0.6f), new Vector3(1, 0, -0.6f), new Vector3(1, 0, -2) };
    public Vector3[] leftSensors = { new Vector3(-1, 0, 2), new Vector3(-1, 0, 0.6f), new Vector3(-1, 0, -0.6f), new Vector3(-1, 0, -2) };
    public Vector3[] frontSensors = { new Vector3(0.8f, 0, 2.25f), new Vector3(0.3f, 0, 2.25f), new Vector3(-0.3f, 0, 2.25f), new Vector3(-0.8f, 0, 2.25f) };
    public Vector3[] backSensors = { new Vector3(0.8f, 0, -2.25f), new Vector3(0.3f, 0, -2.25f), new Vector3(-0.3f, 0, -2.25f), new Vector3(-0.8f, 0, -2.25f) };

    void Update()
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
        if (uiText == null) return;

        if (isSideClear && !isGapDetected)
        {
            isGapDetected = true; isGapValid = false; parkingMode = 0; // Dodano parkingMode
            gapStartPosition = transform.position;
            uiText.text = "Skanowanie..."; uiText.color = Color.yellow;
        }
        else if (!isSideClear && isGapDetected)
        {
            isGapDetected = false;
            float gap = Vector3.Distance(gapStartPosition, transform.position);
            
            if (gap >= 5.5f) { isGapValid = true; parkingMode = 1; uiText.text = "Koperta!"; uiText.color = Color.green; } // Dodano parkingMode = 1
            else if (gap >= 2.5f) { isGapValid = true; parkingMode = 2; uiText.text = "Prostopadłe!"; uiText.color = Color.cyan; } // Dodano parkingMode = 2
            else { isGapValid = false; parkingMode = 0; uiText.text = "Za mała!"; uiText.color = Color.red; } // Dodano parkingMode = 0
        }
    }
}