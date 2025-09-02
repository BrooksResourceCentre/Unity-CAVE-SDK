using System.Collections;
using MMUCAVE;
using UnityEngine;

public class ProjectileCannon : InteractionObject
{
    [Header("References")]

    [Tooltip("The spawn point transform")]
    [SerializeField] private Transform spawnPoint;

    [Tooltip("The prefab to fire from the spawnPoint")]
    [SerializeField] private GameObject projectilePrefab;


    [Header("Fire Settings")]
    [Tooltip("The force to apply to the projectile")]
    [SerializeField] private float force = 50f;

    [Tooltip("The upper bound of how many may randomly fire. Keep 1 for just 1. Must be above 0")]
    [SerializeField] private int maxRandomFireCount = 1;

    [Header("Rate Limit")]
    [Tooltip("Allows for rate limiting the projectile cannon.")]
    [SerializeField] private bool allowRateLimiting = false;
    [Tooltip("The cooldown between uses of the projectile cannon.")]
    [SerializeField] private float cooldownTimer = 0.1f;
    
    private bool isRateLimited = false;
    public override void OnTouch()
    {
        // Error handling
        if (spawnPoint == null)
            Debug.LogError($"{name} - Spawn point is not assigned!");
        if (projectilePrefab == null)
            Debug.LogError($"{name} - Projectile prefab is not assigned!");
        if (maxRandomFireCount < 1)
            Debug.LogError($"{name} - Fire count must be greater than 0!");
        if (isRateLimited) 
            return;
        if (allowRateLimiting)
            StartCoroutine(HandleRateLimiting());

        // Generate a random ammount to fire
        int randomFireCount = Random.Range(1, maxRandomFireCount + 1);

        for (int i = 0; i < randomFireCount; i++)
        {
            // Create the projectile
            GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
            // Get the Rigidbody component
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            // Apply force to the projectile
            rb.AddForce(spawnPoint.up * force, ForceMode.Impulse);
        }
    }

    // Allows for the cannon to only fire every 0.1s.
    private IEnumerator HandleRateLimiting()
    {
        isRateLimited = true;
        yield return new WaitForSeconds(cooldownTimer);
        isRateLimited = false;
    }
}