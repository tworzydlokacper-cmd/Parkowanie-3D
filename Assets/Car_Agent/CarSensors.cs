using UnityEngine;

public class CarSensors : MonoBehaviour
{
    [Header("Ustawienia Zasięgu")]
    public float sideSensorLength = 5f;       // Dłuższy zasięg do szukania miejsca
    public float frontBackSensorLength = 3f;  // Krótszy zasięg do unikania kolizji (zderzaki)

    [Header("Pozycje Czujników (16 sztuk)")]
    // Wymiary auta to 2 x 4.5m, więc wychylamy promienie na krawędzie karoserii

    public Vector3[] rightSensors = new Vector3[] {
        new Vector3(1f, 0.5f, 2f),   // Prawy przód
        new Vector3(1f, 0.5f, 0.6f), // Prawy środek-przód
        new Vector3(1f, 0.5f, -0.6f),// Prawy środek-tył
        new Vector3(1f, 0.5f, -2f)   // Prawy tył
    };

    public Vector3[] leftSensors = new Vector3[] {
        new Vector3(-1f, 0.5f, 2f),   // Lewy przód
        new Vector3(-1f, 0.5f, 0.6f), // Lewy środek-przód
        new Vector3(-1f, 0.5f, -0.6f),// Lewy środek-tył
        new Vector3(-1f, 0.5f, -2f)   // Lewy tył
    };

    public Vector3[] frontSensors = new Vector3[] {
        new Vector3(0.8f, 0.5f, 2.25f),  // Przód prawy
        new Vector3(0.3f, 0.5f, 2.25f),  // Przód środek-prawy
        new Vector3(-0.3f, 0.5f, 2.25f), // Przód środek-lewy
        new Vector3(-0.8f, 0.5f, 2.25f)  // Przód lewy
    };

    public Vector3[] backSensors = new Vector3[] {
        new Vector3(0.8f, 0.5f, -2.25f),  // Tył prawy
        new Vector3(0.3f, 0.5f, -2.25f),  // Tył środek-prawy
        new Vector3(-0.3f, 0.5f, -2.25f), // Tył środek-lewy
        new Vector3(-0.8f, 0.5f, -2.25f)  // Tył lewy
    };

    void Update()
    {
        // Prawy bok - zielone promienie szukające luki
        ScanDirection(rightSensors, transform.right, sideSensorLength, Color.green);
        
        // Lewy bok - zielone promienie szukające luki
        ScanDirection(leftSensors, -transform.right, sideSensorLength, Color.green);
        
        // Przód - niebieskie promienie (zderzak)
        ScanDirection(frontSensors, transform.forward, frontBackSensorLength, Color.blue);
        
        // Tył - żółte promienie (zderzak)
        ScanDirection(backSensors, -transform.forward, frontBackSensorLength, Color.yellow);
    }

    // Uniwersalna funkcja do obsługi każdej grupy czujników
    void ScanDirection(Vector3[] sensors, Vector3 direction, float length, Color freeColor)
    {
        foreach (Vector3 sensorPos in sensors)
        {
            // Przeliczamy lokalne koordynaty z tablicy na globalną pozycję w świecie
            Vector3 startPos = transform.position + (transform.rotation * sensorPos);
            RaycastHit hit;

            if (Physics.Raycast(startPos, direction, out hit, length))
            {
                // Trafienie w przeszkodę zawsze rysujemy na czerwono
                Debug.DrawRay(startPos, direction * hit.distance, Color.red);
            }
            else
            {
                // Brak przeszkody - rysujemy wolną przestrzeń zdefiniowanym kolorem
                Debug.DrawRay(startPos, direction * length, freeColor);
            }
        }
    }
}