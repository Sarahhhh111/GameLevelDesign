using UnityEngine;

public class GemItem : MonoBehaviour
{
    [Tooltip("How many gold (or points) this gem gives.")]
    public int value = 1;

    private void OnTriggerEnter(Collider other)
    {
        // only let the player pick it up:
        var fps = other.GetComponent<FPSController>();
        if (fps != null)
        {
            fps.AddGold(value);
            Destroy(gameObject);
        }
    }
}
