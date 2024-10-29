using System;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public ShootingController parent;
    private Rigidbody rb;

    [Header("Projectile Settings")]
    [SerializeField] float shootForce = 7f;
    [SerializeField] float explosionRadius = 3f;
    [SerializeField] float explosionDamage = 66f;
    [SerializeField] float explosionForce = 2f;
    [SerializeField] float lifeTime = 2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Apply the shooting force if we are the server
        if (IsServer)
        {
            Invoke(nameof(Expload), lifeTime);
        }
        else
        {
            rb.isKinematic = true;  // Disable physics on the client to avoid conflicts
        }
    }

    private void Update()
    {
        MoveServerRpc();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            Expload();
        }
    }

    void Expload()
    {
        if (!IsServer) { return; }  // Ensure only the server handles explosion and destruction

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody nearbyRb = nearbyObject.GetComponent<Rigidbody>();
            HealthController healthController = nearbyObject.GetComponent<HealthController>();
            
            // Add force and apply damage if applicable
            if (nearbyRb != null)
            {
                nearbyRb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
            if (healthController != null)
            {
                healthController.TakeDamage(CalculateDamage(nearbyObject));
            }
        }

        // Destroy the projectile through the parent ShootingController
        if (parent != null)
        {
            parent.DestroyServerRpc(GetComponent<NetworkObject>());
        }
        else
        {
            Debug.LogWarning("Projectile has no parent ShootingController reference!");
            GetComponent<NetworkObject>().Despawn(true); // Fallback if parent is missing
        }
    }

    private int CalculateDamage(Collider target)
    {
        Vector3 closestPoint = target.ClosestPoint(transform.position);
        float distance = Vector3.Distance(transform.position, closestPoint);
        float damage = explosionDamage * (1 - (distance / explosionRadius));
        return Mathf.Max(Mathf.RoundToInt(damage), 0);
    }

    [ServerRpc(RequireOwnership = false)]
    private void MoveServerRpc()
    {
        // Only the server should apply force/movement to avoid client desync
        if (IsServer && GetComponent<NetworkObject>().IsSpawned)
        {
            rb.AddForce(transform.forward * shootForce, ForceMode.Impulse);
        }
    }

    // Draw the explosion radius for visualization in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
