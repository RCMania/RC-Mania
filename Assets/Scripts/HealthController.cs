using Unity.Netcode;
using UnityEngine;

public class HealthController : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] int maxHealth = 100;
    [SerializeField] int startHealth = 100;
    private int currentHealth;

    private void Start()
    {
        currentHealth = startHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);

        if (currentHealth <= 0)
        {
            DieServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DieServerRpc()
    {
        Debug.Log("HealthController: Die");

        if (IsServer)
        {
           
            if (NetworkObject != null && NetworkObject.IsSpawned)
            {
                NetworkObject.Despawn(true);
            }
            
            Destroy(gameObject);
        }
    }
}
