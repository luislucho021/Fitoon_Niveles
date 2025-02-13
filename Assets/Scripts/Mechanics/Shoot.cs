using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileSpeed;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ShootProjectile()
    {
        if (projectilePrefab != null) {
            GameObject projectile = Instantiate(projectilePrefab, transform);
            projectile.GetComponent<Rigidbody>().velocity = -transform.forward * projectileSpeed;
            animator.SetTrigger("Shoot");
        }
    }
}
