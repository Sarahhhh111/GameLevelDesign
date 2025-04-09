using UnityEngine;

public class HealthItem : MonoBehaviour
{
    public int healAmount = 5; // Amount of health to restore

    private void OnTriggerEnter(Collider other)
    {
        FPSController player = other.GetComponent<FPSController>();

        if (player != null)
        {
            player.Heal(healAmount); // Call the healing function
            Debug.Log("Player healed for " + healAmount + " HP");
            Destroy(gameObject); // Remove the shroom from scene
        }
    }
}
