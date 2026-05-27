using UnityEngine;

public class WheelAligner : MonoBehaviour
{
    public WheelCollider wheelCollider; // Niewidzialna fizyka
    public Transform visualWheel;       // Widzialna grafika koła

    void Update()
    {
        // Jeśli nie przypisaliśmy kół w Edytorze, nic nie rób
        if (wheelCollider == null || visualWheel == null) return;

        Vector3 position;
        Quaternion rotation;

        // Pobieramy aktualną pozycję i obrót z WheelCollidera
        wheelCollider.GetWorldPose(out position, out rotation);

        // Aplikujemy te dane na nasz wizualny model koła
        visualWheel.position = position;
        visualWheel.rotation = rotation;
    }
}