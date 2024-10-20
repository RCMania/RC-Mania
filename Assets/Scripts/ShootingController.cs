using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Shooting : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] Transform shootPoint;      
    [SerializeField] GameObject rocketPrefab;   

    [Header("Shooting Settings")]
    [SerializeField] float reloadTime = 1f;
    private NetworkVariable<float> lastShootTime = new NetworkVariable<float>();

    private void Start()
    {
        if (IsOwner) 
        {
            lastShootTime.Value = Time.time;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetMouseButtonDown(0))
        {
            ShootServerRpc();  
        }
    }

    [ServerRpc(RequireOwnership = true)]
    private void ShootServerRpc()
    {
        if (Time.time < (lastShootTime.Value + reloadTime)) return;  // Prevent shooting before reload time

        lastShootTime.Value = Time.time;  // Update the last shot time

        SpawnRocketServerRpc();
    }

    [ServerRpc(RequireOwnership = true)]
    private void SpawnRocketServerRpc()
    {
        GameObject rocketInstance = Instantiate(rocketPrefab, shootPoint.position, shootPoint.rotation);
        rocketInstance.GetComponent<NetworkObject>().Spawn();

        // Optionally, you could return this information to the client
        SpawnRocketClientRpc();
    }

    [ClientRpc]
    private void SpawnRocketClientRpc()
    {
        if (IsOwner) return;  // Ensure the local owner doesn't do this again (redundant check)

        // You can handle client-specific logic here (e.g., visual effects, sounds)
        // Example: Play shoot sound effect or show an animation
    }
}
