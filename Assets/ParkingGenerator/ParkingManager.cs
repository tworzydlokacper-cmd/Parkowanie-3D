using UnityEngine;

public class ParkingManager : MonoBehaviour
{
    [Header("TUTAJ WEPNIJ SWOJE AUTO (PREFAB)")]
    public GameObject playerCarPrefab;

    [Header("TUTAJ WEPNIJ MODEL ZAPARKOWANEGO AUTA (PREFAB)")]
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
            {
                StartGame(true);
            }

            if (GUI.Button(new Rect(Screen.width / 2 - 125, Screen.height / 2 + 10, 250, 40), "2. Parkowanie Prostopadłe"))
            {
                StartGame(false);
            }
        }
    }

    void StartGame(bool isParallel)
    {
        showMenu = false;
        GenerateMap(isParallel);
        
        if (playerCarPrefab != null)
        {
            spawnedCar = Instantiate(playerCarPrefab, new Vector3(-3f, 0.5f, -10f), Quaternion.identity);
            
            Camera carCam = spawnedCar.GetComponentInChildren<Camera>();
            if (carCam != null) carCam.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("NIE PODPIĄŁEŚ PREFABU GŁÓWNEGO AUTA!");
        }
    }

    void GenerateMap(bool isParallel)
    {
        if (currentMap != null) Destroy(currentMap);
        currentMap = new GameObject("Wygenerowana_Mapa");

        // Materiały URP
        Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
        
        Material asphaltMat = new Material(urpShader);
        asphaltMat.SetColor("_BaseColor", new Color(0.2f, 0.2f, 0.2f));
        
        Material lineMat = new Material(urpShader);
        lineMat.SetColor("_BaseColor", Color.white);

        // Asfalt
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
        road.transform.SetParent(currentMap.transform);
        road.transform.localScale = new Vector3(20f, 0.1f, 50f);
        road.transform.position = new Vector3(0f, 0f, 15f);
        road.GetComponent<MeshRenderer>().material = asphaltMat;

        if (isParallel)
        {
            // === GENEROWANIE KOPERTY ===
            float[] carPositionsZ = { 0f, 12f, 20f }; 
            
            foreach (float z in carPositionsZ)
            {
                if (parkedCarPrefab != null)
                {
                    // Wrzucamy Twój model zamiast sześcianu
                    GameObject parkedCar = Instantiate(parkedCarPrefab, new Vector3(2.5f, 0f, z), Quaternion.identity);
                    parkedCar.transform.SetParent(currentMap.transform);
                }
            }

            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            line.transform.SetParent(currentMap.transform);
            line.transform.localScale = new Vector3(0.2f, 0.15f, 40f);
            line.transform.position = new Vector3(0f, 0.05f, 10f);
            line.GetComponent<MeshRenderer>().material = lineMat;
            Destroy(line.GetComponent<BoxCollider>());
        }
        else
        {
            // === GENEROWANIE PROSTOPADŁEGO ===
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

                if (i != emptySpot && parkedCarPrefab != null)
                {
                    // Wrzucamy Twój model, obrócony o -90 stopni w osi Y!
                    GameObject parkedCar = Instantiate(parkedCarPrefab, new Vector3(spotLength / 2f + 1f, 0f, zPos + (spotWidth / 2f)), Quaternion.Euler(0, -90, 0));
                    parkedCar.transform.SetParent(currentMap.transform);
                }
            }
        }
    }
}