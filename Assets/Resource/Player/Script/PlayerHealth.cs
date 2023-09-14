using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header(">>> Player HP <<<")]
    [SerializeField] private float maxHP;
    [SerializeField] private float currentHP;

    // done healling
    [Header(">>> Restore HP <<<")]
    [SerializeField] private int maxUses;
    [SerializeField] private int remainingUses;

    //[Header(">>> new Restore <<<")]
    //[SerializeField] private FirstKitScript firstKitScript;
    //[SerializeField] private int fullUses;
    //[SerializeField] private int currentUses;

    // Start is called before the first frame update
    void Start()
    {
        maxHP = 100f;
        currentHP = maxHP;

        // done healling
        maxUses = 3;
        remainingUses = maxUses;

        //// new healling
        //fullUses = 3;
        //currentUses = fullUses;
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.E))
        //{
        //    NewRestoreHP();
        //}
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        Debug.Log(">>> Player HP: " + currentHP);
        if (currentHP <= 0f)
        {
            Debug.Log(">>> Player Die ");
            Debug.Log(">>> Play animation die");
        }
    }

    // new healling
    //private void NewRestoreHP()
    //{
    //    currentHP = Mathf.Min(maxHP, currentHP + firstKitScript.healAmount);
    //    Debug.Log(">>> Pressed E");
    //    Debug.Log($"{currentHP}");
    //}



    // Done Healling
    private void RestoreHP()
    {
        if (remainingUses > 0)
        {
            currentHP += 60;
            remainingUses--;
        }
        else
        {
            Debug.Log(">>> You haven't First and Kit");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FirstKit") && remainingUses < maxUses)
        {
            remainingUses++;
            if (remainingUses == maxUses)
            {
                Debug.Log(">>> You've reached the maximum number of First Aid Kits.");
                return;
            }
        }
    }

}
