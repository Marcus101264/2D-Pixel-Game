using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("基本屬性")]
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;

    [Header("受傷無敵")]
    public float invulnerableDuration;
    [HideInInspector]public float invulnerableCounter;
    public bool invulnerable;

    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;

    private void Start()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }

    private void Update()
    {
        if (invulnerable)
            invulnerableCounter -= Time.deltaTime;

            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }

        if(currentPower < maxPower)
        {
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if(other.CompareTag("Water"))
        {
            //死亡。更新血量
            currentHealth = 0;
            OnHealthChange?.Invoke(this);
            OnDie?.Invoke();
        }
    }

    public void TakeDamage(Attack attacker)
    {
        if(invulnerable)
            return;

        //Debug.Log(attacker.damage);
        if (currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
            TriggerInvulnerable();
            //執行受傷
            OnTakeDamage?.Invoke(attacker.transform);
        }
        else
        {
            currentHealth = 0;
            //觸發死亡
            OnDie?.Invoke();
        }

        OnHealthChange?.Invoke(this);
    }

    /// <summary>
    /// 觸發受傷無敵
    /// </summary>
    private void TriggerInvulnerable()
    {
        if(!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

    public void OnSlide(int cost)
    {
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }
}
