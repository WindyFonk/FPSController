
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    public float health = 50f;

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
