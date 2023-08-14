using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolScript : MonoBehaviour
{
    public Transform gunPivot;
    public int maxRange;
    public float damage;

    public int maxBullet;
    public int currentBullet;

    public float bulletSpreadAngle;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        maxRange = 100;
        damage = 10f;
        maxBullet = 15;

        bulletSpreadAngle = 1;
        currentBullet = maxBullet;
    }

    private void Update()
    {
     
    }

    public void Reload()
    {
        currentBullet = maxBullet;
        Debug.Log(">>> Reload mag !");
        Debug.Log(">>> currentBullet after reload: " + currentBullet);
    }

    public void Fire()
    {
        if (currentBullet > 0)
        {
            currentBullet--;
           
            // Tạo độ lệch ngẫu nhiên khi bắn
            float randomSpread = Random.Range(-bulletSpreadAngle / 2, bulletSpreadAngle / 2);
            Quaternion rotation = Quaternion.Euler(0, randomSpread, 0);
            Vector3 rayDirection = rotation * gunPivot.transform.forward;

            // Bắn một Raycast từ vị trí súng
            RaycastHit hit;
            if (Physics.Raycast(gunPivot.transform.position, gunPivot.transform.forward, out hit, maxRange))
            {
                Debug.Log(hit.transform.name);

                TargetScript target = hit.transform.GetComponent<TargetScript>();

                if (target != null)
                {
                    target.TakeDamage(damage);
                    Debug.Log(">>> damage: " + damage);
                }
            }
            animator.SetInteger("Bullet", currentBullet);
        }
       
    }
}
