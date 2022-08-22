using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStatus : MonoBehaviour
{
    [Header("Entity Settings")]
    [SerializeField] private int _maxHP = 5;
    [SerializeField] private float dropRadius = 1f;
    [SerializeField] private ParticleSystem hitVfx;
    [SerializeField] private GameObject[] drops;
    [SerializeField] private GameObject[] loots;

    [Header("Entity Debugs")]
    [SerializeField]private int _currentHP = 5;
    public int currentHP { get { return _currentHP; } }
    public int maxHP { get { return _maxHP; } }

    private bool _invincible = false;

    protected virtual void Start()
    {
        _currentHP = _maxHP;   
    }

    public virtual void RecieveDamage(int damage)
    {
        if(_currentHP > 0)
        {
            _currentHP -= damage;

            hitVfx?.Play();
            OnHit();
        }
        else
        {
            return;
        }

        if(_currentHP <= 0)
        {
            //death VFX
            foreach(GameObject drop in drops)
                Instantiate(drop, transform.position, Quaternion.identity);

            //droploot
            foreach (GameObject loot in loots)
                Instantiate(loot,
                    transform.position + 
                    new Vector3(Random.Range(-dropRadius, dropRadius),
                    Random.Range(-dropRadius, dropRadius)), 
                    Quaternion.identity);

            OnDeath();
        }
    }

    public void Heal(int point)
    {
        _currentHP += point;
        if (_currentHP > _maxHP)
            _currentHP = _maxHP;
    }

    public virtual void OnHit()
    {

    }

    public virtual void OnDeath()
    {
        GameManager.NotifyKill();
        Destroy(gameObject);
    }

    public void SetInvincible(bool value)
    {
        _invincible = value;
    }
}
