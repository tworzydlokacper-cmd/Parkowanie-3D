using UnityEngine;
using TMPro; 

public class CarSensors : MonoBehaviour
{
    [Header("Ustawienia Zasięgu")]
    public float sideSensorLength = 5f;       
    public float frontBackSensorLength = 3f;  

    [Header("Dane dla Maszyny Stanów (Osoba 2)")]
    public bool isGapDetected = false; 
    public float lastMeasuredGapLength = 0f; 
    public float minimumRequiredGap = 6.75f; 

    [Header("Interfejs Ekranu")]
    public TextMeshProUGUI uiText; // TO POLE NAM SIĘ POJAWI W UNITY!

    private Vector3 gapStartPosition;

    public Vector3[] rightSensors = new Vector3[] {
        new Vector3(1f, 0.0f, 2f),   
        new Vector3(1f, 0.0f, 0.6f), 
        new Vector3(1f, 0.0f, -0.6f),
        new Vector3(1f, 0.0f, -2f)   
    };

    public Vector3[] leftSensors = new Vector3[] {
        new Vector3(-1f, 0.0f, 2f),   
        new Vector3(-1f, 0.0f, 0.6f), 
        new Vector3(-1f, 0.0f, -0.6f),
        new Vector3(-1f, 0.0f, -2f)   
    };

    public Vector3[] frontSensors = new Vector3[] {
        new Vector3(0.8f, 0.0f, 2.25f),  
        new Vector3(0.3f, 0.0f, 2.25f),  
        new Vector3(-0.3f, 0.0f, 2.25f), 
        new Vector3(-0.8f, 0.0f, 2.25f)  
    };

    public Vector3[] backSensors = new Vector3[] {
        new Vector3(0.8f, 0.0f, -2.25f),  
        new Vector3(0.3f, 0.0f, -2.25f),  
        new Vector3(-0.3f, 0.0f, -2.25f), 
        new Vector3(-0.8f, 0.0f, -2.25f)  
    };

    void Update()
    {
        bool rightSideClear = ScanDirection(rightSensors, transform.right, sideSensorLength, Color.green);
        ScanDirection(leftSensors, -transform.right, sideSensorLength, Color.green);
        ScanDirection(frontSensors, transform.forward, frontBackSensorLength, Color.blue);
        ScanDirection(backSensors, -transform.forward, frontBackSensorLength, Color.yellow);

        MeasureParkingGap(rightSideClear);
    }

    bool ScanDirection(Vector3[] sensors, Vector3 direction, float length, Color freeColor)
    {
        bool isCompletelyClear = true;
        foreach (Vector3 sensorPos in sensors)
        {
            Vector3 startPos = transform.position + (transform.rotation * sensorPos);
            RaycastHit hit;

            if (Physics.Raycast(startPos, direction, out hit, length))
            {
                Debug.DrawRay(startPos, direction * hit.distance, Color.red);
                isCompletelyClear = false; 
            }
            else
            {
                Debug.DrawRay(startPos, direction * length, freeColor);
            }
        }
        return isCompletelyClear;
    }

    void MeasureParkingGap(bool isSideClear)
    {
        if (uiText == null) return; 

        if (isSideClear && !isGapDetected)
        {
            isGapDetected = true;
            gapStartPosition = transform.position; 
            uiText.text = "Skanowanie luki...";
            uiText.color = Color.yellow;
        }
        else if (!isSideClear && isGapDetected)
        {
            isGapDetected = false;
            lastMeasuredGapLength = Vector3.Distance(gapStartPosition, transform.position);
            
            float roundedGap = Mathf.Round(lastMeasuredGapLength * 100f) / 100f;

            if (lastMeasuredGapLength >= minimumRequiredGap)
            {
                uiText.text = $"Luka znaleziona!\nRozmiar: {roundedGap}m\nMozna parkowac.";
                uiText.color = Color.green;
            }
            else
            {
                uiText.text = $"Luka za mala!\nRozmiar: {roundedGap}m\nSzukam dalej...";
                uiText.color = Color.red;
            }
        }
    }
}