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

    void Expload()
    {
        if (!IsOwner) { return; }

        Debug.Log("Projectile: Expload");
        Collider[] _colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObjects in _colliders)
        {
            /*Rigidbody rb = nearbyObjects.GetComponent<Rigidbody>();
            HealthController hc = nearbyObjects.GetComponent<HealthController>();
           
            AddForce(rb);
            DamageObject(hc, CalculateDamage());*/
        }
        parent.DestroyServerRpc(GetComponent<NetworkObject>());
    }

    /*void AddForce(Rigidbody rb)
    {
        if (rb == null) {  return; }
    }

    void CalculateDamage()
    {

    }

    void DamageObject(HealthController hp, float amountOfDamage)
    {
        if (hp == null) { return; }
    }*/

    private void OnCollisionEnter(Collision collision)
    {
        Expload();
    }

    [ServerRpc(RequireOwnership = false)]
    private void MoveServerRpc()
    {
        if (!GetComponent<NetworkObject>().IsSpawned) { return; } // Ensure it's spawned
        //rb.linearVelocity = transform.forward * shootForce;
        rb.AddForce(transform.forward * shootForce);
    }

    
}
