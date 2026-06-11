using UnityEngine;

public class ParkingManager : MonoBehaviour
{
    [Header("TUTAJ WEPNIJ SWOJE AUTO (PREFAB Z CANVAS)")]
    public GameObject playerCarPrefab;

    [Header("TUTAJ WEPNIJ MODEL ZAPARKOWANEGO AUTA (PREFAB) - OPCJONALNIE")]
    public GameObject parkedCarPrefab;

    private bool showMenu = true;
    private GameObject currentMap;
    private GameObject spawnedCar;

    void OnGUI()
    {
        if (showMenu)
        {
            GUI.Box(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100, 300, 200), "Wybierz Tryb Parkowania");

            if (GUI.Button(new Rect(Screen.width / 2 - 125, Screen.height / 2 - 50, 250, 40), "1. Parkowanie Równoległe (Koperta)"))
                StartGame(true);

            if (GUI.Button(new Rect(Screen.width / 2 - 125, Screen.height / 2 + 10, 250, 40), "2. Parkowanie Prostopadłe"))
                StartGame(false);
        }
    }

    void StartGame(bool isParallel)
    {
        showMenu = false;
        GenerateMap(isParallel);
        
        if (playerCarPrefab != null)
        {
            // Auto startuje na bezpiecznej pozycji środka pasa drogi (X = -2.5f)
            spawnedCar = Instantiate(playerCarPrefab, new Vector3(-2.5f, 0.5f, -10f), Quaternion.identity);
            
            Rigidbody rb = spawnedCar.GetComponent<Rigidbody>();
            if (rb != null) 
            {
                rb.isKinematic = false; 
                rb.mass = 1500f; 
                rb.centerOfMass = new Vector3(0f, -1.5f, 0f); 
            }
            
            Camera carCam = spawnedCar.GetComponentInChildren<Camera>();
            if (carCam != null) carCam.gameObject.SetActive(true);
        }
    }

    void GenerateMap(bool isParallel)
    {
        if (currentMap != null) Destroy(currentMap);
        currentMap = new GameObject("Wygenerowana_Mapa");

        Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
        Material asphaltMat = new Material(urpShader); asphaltMat.SetColor("_BaseColor", new Color(0.2f, 0.2f, 0.2f));
        Material lineMat = new Material(urpShader); lineMat.SetColor("_BaseColor", Color.white);
        Material carMat = new Material(urpShader); carMat.SetColor("_BaseColor", new Color(0.8f, 0.1f, 0.1f));

        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
        road.transform.SetParent(currentMap.transform);
        road.transform.localScale = new Vector3(25f, 0.1f, 60f);
        road.transform.position = new Vector3(-3f, 0f, 20f); 
        road.GetComponent<MeshRenderer>().material = asphaltMat;

        if (isParallel)
        {
            // KOPERTA: Odpowiednio powiększone miejsca (szeroka luka)
            float[] carPositionsZ = { 0f, 11.5f, 18f }; 
            
            foreach (float z in carPositionsZ)
            {
                if (parkedCarPrefab != null)
                {
                    // Generowanie z Twojego modelu 3D (Y=0, bo modele zwykle mają pivot na spodzie)
                    GameObject parkedCar = Instantiate(parkedCarPrefab, new Vector3(3.0f, 0f, z), Quaternion.identity);
                    parkedCar.transform.SetParent(currentMap.transform);
                    if (parkedCar.GetComponent<Collider>() == null) parkedCar.AddComponent<BoxCollider>();
                }
                else
                {
                    // Rezerwowy sześcian w razie braku modelu
                    GameObject parkedCar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    parkedCar.transform.SetParent(currentMap.transform);
                    parkedCar.transform.localScale = new Vector3(2.5f, 1.5f, 5.0f);
                    parkedCar.transform.position = new Vector3(3.0f, 0.75f, z);
                    parkedCar.GetComponent<MeshRenderer>().material = carMat;
                }
            }

            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            line.transform.SetParent(currentMap.transform);
            line.transform.localScale = new Vector3(0.2f, 0.15f, 50f);
            line.transform.position = new Vector3(0.5f, 0.05f, 15f);
            line.GetComponent<MeshRenderer>().material = lineMat;
            Destroy(line.GetComponent<BoxCollider>());
        }
        else
        {
            // PROSTOPADŁE: Każde miejsce poszerzone (szeroka luka 4.5m)
            int spots = 8;
            float spotWidth = 4.5f; 
            float spotLength = 6.0f;
            int emptySpot = 3; 

            for (int i = 0; i < spots; i++)
            {
                float zPos = i * spotWidth;

                GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
                line.transform.SetParent(currentMap.transform);
                line.transform.localScale = new Vector3(spotLength, 0.15f, 0.15f);
                line.transform.position = new Vector3(spotLength / 2f + 1f, 0.05f, zPos);
                line.GetComponent<MeshRenderer>().material = lineMat;
                Destroy(line.GetComponent<BoxCollider>());

                if (i != emptySpot)
                {
                    if (parkedCarPrefab != null)
                    {
                        // Generowanie z Twojego modelu 3D (obróconego o 90 stopni w poprzek drogi)
                        GameObject parkedCar = Instantiate(parkedCarPrefab, new Vector3(spotLength / 2f + 1f, 0f, zPos + (spotWidth / 2f)), Quaternion.Euler(0, -90, 0));
                        parkedCar.transform.SetParent(currentMap.transform);
                        if (parkedCar.GetComponent<Collider>() == null) parkedCar.AddComponent<BoxCollider>();
                    }
                    else
                    {
                        // Rezerwowy sześcian w razie braku modelu
                        GameObject parkedCar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        parkedCar.transform.SetParent(currentMap.transform);
                        parkedCar.transform.localScale = new Vector3(5.0f, 1.5f, 2.5f);
                        parkedCar.transform.position = new Vector3(spotLength / 2f + 1f, 0.75f, zPos + (spotWidth / 2f));
                        parkedCar.GetComponent<MeshRenderer>().material = carMat;
                    }
                }
            }
        }
    }
}