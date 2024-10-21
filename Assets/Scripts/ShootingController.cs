using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Shooting : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] Transform shootPoint;      
    [SerializeField] GameObject rocketPrefab;
    [SerializeField] List<GameObject> spawnedRockets = new List<GameObject>();

    [Header("Shooting Settings")]
    [SerializeField] float reloadTime = 1f;
    [SerializeField] private NetworkVariable<float> lastShootTime = new NetworkVariable<float>();

    private void Start()
    {
        if (IsServer)
        {
            lastShootTime.Value = Time.time;
        }
    }

    private void Update()
    {
        if (!IsOwner) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            ShootServerRpc();  
        }
    }

    [ServerRpc] 
    private void ShootServerRpc()
    {
        if (Time.time < (lastShootTime.Value + reloadTime)) { return; }  // Prevent shooting before reload time

        lastShootTime.Value = Time.time;  // Update the last shot time

        SpawnRocket();
    }
    
    private void SpawnRocket()
    {
        GameObject rocketInstance = Instantiate(rocketPrefab, shootPoint.position, shootPoint.rotation);
        spawnedRockets.Add(rocketInstance);
        
        rocketInstance.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc] //(RequireOwnership = false)
    public void DestroyServerRpc()
    {
        //GameObject _toDestroy = spawnedRockets[0]; (by not 0 maybe with an id but for later fix)
        // _toDestroy.GetComponent<NetworkObject>().DeSpawn();
        //spawnedRockets.Remove(_toDestroy);
        //Destroy(_toDestroy);

    }

}
