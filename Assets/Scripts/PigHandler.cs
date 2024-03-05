using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    [SerializeField] private float _madHealth = 3f;
    [SerializeField] private float _damageThreshold = 0.2f;
    [SerializeField] private  Animator _myAnimator;
    [SerializeField] private  float _dieAfterSomeTime = 1f;

    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = _madHealth;
    }

    public void Damage(float damageAmount)
    {
        _currentHealth -= damageAmount;
    
        if (_currentHealth <= 0f)
        {
            _myAnimator.SetTrigger("Died");
            StartCoroutine(DieAfterSomeTime());
        }
    }

    public void Die()
    {
        GameManager.instance.RemovePig(this);
        Destroy(gameObject);
    }

    private IEnumerator DieAfterSomeTime()
    {
        yield return new WaitForSeconds(_dieAfterSomeTime);
        Die();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float impactVelocity = collision.relativeVelocity.magnitude;

        if (impactVelocity > _damageThreshold)
        {
            Damage(impactVelocity);
        }
    }
}
