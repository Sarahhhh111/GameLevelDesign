using UnityEngine;

public class GoldItem : MonoBehaviour
{
    public int goldAmount = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FPSController player = other.GetComponent<FPSController>();
            if (player != null)
            {
                player.Heal(goldAmount);
                Debug.Log("Player gained " + goldAmount + "g");
                Destroy(gameObject);
            }
        }
    }
}
