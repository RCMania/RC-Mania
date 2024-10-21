using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Projectile Settings")]
    [SerializeField] float shootForce = 10f;
    [SerializeField] float explosionRadius = 3f;
    [SerializeField] float lifeTime = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
    }

    
    void Update()
    {
        //rb.linearVelocity = transform.forward * shootForce;
        rb.AddForce(transform.forward * shootForce);
    }
}
