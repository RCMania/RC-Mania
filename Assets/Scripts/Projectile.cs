using System;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public ShootingController parent;
    private Rigidbody rb;

    [Header("Projectile Settings")]
    [SerializeField] float shootForce = 10f;
    [SerializeField] float explosionRadius = 3f;
    [SerializeField] float explosionDamage = 50f;
    [SerializeField] float explosionForce = 20f;
    [SerializeField] float lifeTime = 2f;

    void Start()
    {
        if (IsServer)
        {
            Invoke(nameof(Expload), lifeTime);
        }

        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        MoveServerRpc();
    }

    void OnDrawGizmos()
    {
        // Draw a wire sphere in the scene view for the explosion radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    void Expload()
    {
        if (!IsOwner) { return; }

        //Debug.Log("Projectile: Expload");
        Collider[] _colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in _colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            HealthController hc = nearbyObject.GetComponent<HealthController>();
           
            // ++ AddForce(rb);
            DamageObject(hc, CalculateDamage(nearbyObject));
        }
        parent.DestroyServerRpc(GetComponent<NetworkObject>());
    }

  
    private float CalculateDamage(Collider target)
    {
        Vector3 closestPoint = target.ClosestPoint(transform.position);

        float distance = Vector3.Distance(transform.position, closestPoint);

        float damage = explosionDamage * (1 - (distance / explosionRadius));

        //Debug.Log("Collided With:" + target.name + "Distance from explosion: " + distance + " Damage dealt: " + damage);

        return Mathf.Max(damage, 0);
    }

    void DamageObject(HealthController hp, float amountOfDamage)
    {
        if (hp == null) { return; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Expload();
    }

    [ServerRpc(RequireOwnership = false)]
    private void MoveServerRpc()
    {
        if (!GetComponent<NetworkObject>().IsSpawned) { return; } 
        
        //rb.linearVelocity = transform.forward * shootForce;
        rb.AddForce(transform.forward * shootForce);
    }

    
}
