using UnityEngine;

public class GoldItem : MonoBehaviour
{
    [Tooltip("How much gold this strawberry grants.")]
    public int value = 1;

    private void OnTriggerEnter(Collider other)
    {
        // Only the player picks up strawberries:
        var fps = other.GetComponent<FPSController>();
        if (fps != null)
        {
            fps.AddGold(value);
            Destroy(gameObject);
        }
    }
}
