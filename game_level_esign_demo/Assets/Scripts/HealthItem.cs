using UnityEngine;

public class HealthItem : MonoBehaviour
{
    public int healAmount = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FPSController player = other.GetComponent<FPSController>();
            if (player != null)
            {
                player.Heal(healAmount);
                Debug.Log("Player healed for " + healAmount + " HP");
                Destroy(gameObject);
            }
        }
    }
}
