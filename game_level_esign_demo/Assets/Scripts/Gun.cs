using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float damage = 10f;      // Damage per shot
    public float range = 100f;     // Raycast distance
    public float fireRate = 15f;      // Rounds per second

    [Header("References")]
    public Camera fpsCamera;        // Your player’s camera
    public ParticleSystem muzzleFlashPrefab; // Prefab for your muzzle‐flash
    public GameObject impactEffect;      // Prefab for spark/smoke on impact
    public Transform muzzlePoint;       // Empty at barrel tip

    private float nextTimeToFire = 0f;

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        // 1) Muzzle flash at barrel
        if (muzzleFlashPrefab != null && muzzlePoint != null)
        {
            var flash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
            flash.Play();
            Destroy(flash.gameObject, flash.main.duration + flash.main.startLifetime.constantMax);
        }

        // 2) Raycast forward from the camera
        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        {
            Debug.Log($"Gun hit: {hit.collider.name}");

            // 3) Damage any EnemyAI it hits
            var enemy = hit.collider.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(Mathf.RoundToInt(damage));
                Debug.Log($"→ Damaged {hit.collider.name} for {damage} HP");
            }

            // 4) Spawn impact VFX
            if (impactEffect != null)
            {
                var impactGO = Instantiate(
                    impactEffect,
                    hit.point,
                    Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2f);
            }
        }
    }
}
