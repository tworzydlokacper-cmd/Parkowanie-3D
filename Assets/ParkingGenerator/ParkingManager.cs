using UnityEngine;

public class ParkingManager : MonoBehaviour
{
    [Header("TUTAJ WEPNIJ SWOJE AUTO (PREFAB Z CANVAS)")]
    public GameObject playerCarPrefab;

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
            // Zespawnuj auto na środku drogi (X=0) i zrównaj z pasem
            spawnedCar = Instantiate(playerCarPrefab, new Vector3(0f, 0.5f, -10f), Quaternion.identity);
            
            // KLUCZOWE ZABEZPIECZENIE: Zdejmij isKinematic na Rigidbody, żeby ruszył!
            Rigidbody rb = spawnedCar.GetComponent<Rigidbody>();
            if (rb != null) 
            {
                rb.isKinematic = false; 
                rb.mass = 1500f; // Wymuś 1.5 tony
                rb.centerOfMass = new Vector3(0f, -1.5f, 0f); // Wymuś środek ciężkości
            }
            
            // Aktywacja kamery auta
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
        road.transform.localScale = new Vector3(15f, 0.1f, 50f);
        road.transform.position = new Vector3(-2f, 0f, 15f);
        road.GetComponent<MeshRenderer>().material = asphaltMat;

        if (isParallel)
        {
            // KOPERTA (Luka między 0 a 12, auta na X=3, laser 8m sięgnie)
            float[] carPositionsZ = { 0f, 12f, 20f }; 
            
            foreach (float z in carPositionsZ)
            {
                GameObject parkedCar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                parkedCar.transform.SetParent(currentMap.transform);
                parkedCar.transform.localScale = new Vector3(2.5f, 1.5f, 5.0f);
                parkedCar.transform.position = new Vector3(3.0f, 0.75f, z);
                parkedCar.GetComponent<MeshRenderer>().material = carMat;
            }

            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            line.transform.SetParent(currentMap.transform);
            line.transform.localScale = new Vector3(0.2f, 0.15f, 40f);
            line.transform.position = new Vector3(0.5f, 0.05f, 10f);
            line.GetComponent<MeshRenderer>().material = lineMat;
            Destroy(line.GetComponent<BoxCollider>());
        }
        else
        {
            // PROSTOPADŁE (Luka 3.5m - tryb 2, auta bliżej X=4, laser sięgnie)
            int spots = 8;
            float spotWidth = 3.5f; 
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