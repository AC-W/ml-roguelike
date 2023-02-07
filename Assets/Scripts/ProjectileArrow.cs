using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileArrow : MonoBehaviour, Projectiles
{   
    private Vector3 shootDir;
    private float moveSpeed = 2f;
    public float damage = 10f;

    public void SetUp(Vector3 shootDirection)
    {
        this.shootDir = shootDirection;
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddForce(shootDirection * moveSpeed, ForceMode2D.Impulse);
        
        Vector3 dir = shootDirection.normalized;
        float n = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        transform.eulerAngles = new Vector3(0,0,n-90);

            Destroy(gameObject,5f);
    }

    private void Update()
    {
        // transform.position += shootDir *moveSpeed* Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        IDamageable damageableObject = col.GetComponent<IDamageable>();
        Projectiles projectile = col.GetComponent<Projectiles>();
        if (projectile != null) return;

        try 
            {
                MyAgents agent = transform.parent.GetComponent<MyAgents>();
                agent.GetResult(col,transform);
            }
            catch {}

        if (damageableObject != null)
        {
            transform.parent = col.transform;
            col.GetComponent<Rigidbody2D>().AddForce(gameObject.GetComponent<Rigidbody2D>().velocity,ForceMode2D.Impulse);
            damageableObject.TakeHit(damage);
        }
        
        Destroy (gameObject.GetComponent<Rigidbody2D>(),0.03f);
        Destroy (gameObject.GetComponent<Collider2D>());
    }
    
    private void OnTriggerExit2D(Collider2D col)
    {
        Destroy(gameObject,5f);
    }
}
