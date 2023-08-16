
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    public float health;

    private void Start()
    {
        health = 1000f;
    }
    public void TakeDamage (float amount)
    {
        health -= amount;
        Debug.Log(">>> Target health: " + health);
        if (health <= 0f)
        {
            Debug.Log(">>> Target Die ");
            Destroy(gameObject);
        }
    }

    
}
