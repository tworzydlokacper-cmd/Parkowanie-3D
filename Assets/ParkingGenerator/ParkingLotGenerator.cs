using UnityEngine;

public class ParkingLotGenerator : MonoBehaviour
{
    [Header("Ustawienia Parkingu")]
    public int numberOfSpots = 6;           // Ilość miejsc
    public float spotWidth = 3.5f;          // Szerokość miejsca (3.5m = Prostopadłe dla Twojego radaru)
    public float spotLength = 6.0f;         // Długość miejsca
    public int emptySpotIndex = 3;          // Które miejsce ma być puste (luka)

    [ContextMenu("Generuj Parking")]
    public void GenerateParking()
    {
        // Tworzymy główny folder na parking, żeby utrzymać porządek na scenie
        GameObject parkingRoot = new GameObject("Wygenerowany_Parking");
        parkingRoot.transform.position = Vector3.zero;

        // Materiały
        Material asphaltMat = new Material(Shader.Find("Standard")) { color = new Color(0.2f, 0.2f, 0.2f) };
        Material lineMat = new Material(Shader.Find("Standard")) { color = Color.white };
        Material carMat = new Material(Shader.Find("Standard")) { color = new Color(0.8f, 0.1f, 0.1f) };

        // 1. GENEROWANIE DROGI
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
        road.name = "Asfalt";
        road.transform.SetParent(parkingRoot.transform);
        road.transform.localScale = new Vector3(15f, 0.1f, numberOfSpots * spotWidth + 10f);
        road.transform.position = new Vector3(-5f, 0f, (numberOfSpots * spotWidth) / 2f);
        road.GetComponent<MeshRenderer>().material = asphaltMat;

        // 2. GENEROWANIE MIEJSC I LINII
        for (int i = 0; i <= numberOfSpots; i++)
        {
            float zPos = i * spotWidth;

            // Malowanie białej linii
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            line.name = "Biala_Linia_" + i;
            line.transform.SetParent(parkingRoot.transform);
            line.transform.localScale = new Vector3(spotLength, 0.15f, 0.15f);
            line.transform.position = new Vector3(spotLength / 2f, 0.05f, zPos);
            line.GetComponent<MeshRenderer>().material = lineMat;
            DestroyImmediate(line.GetComponent<BoxCollider>()); // Usuwamy kolider z linii, żeby radar go nie widział

            // Generowanie zaparkowanych aut (kwadratów), pomijając wyznaczoną lukę
            if (i < numberOfSpots && i != emptySpotIndex)
            {
                GameObject parkedCar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                parkedCar.name = "Zajete_Auto_" + i;
                parkedCar.transform.SetParent(parkingRoot.transform);
                parkedCar.transform.localScale = new Vector3(4.5f, 1.5f, 2.0f); // Rozmiar "innego auta"
                parkedCar.transform.position = new Vector3(spotLength / 2f, 0.75f, zPos + (spotWidth / 2f));
                parkedCar.GetComponent<MeshRenderer>().material = carMat;
            }
        }

        Debug.Log("Parking wygenerowany pomyślnie!");
    }
}