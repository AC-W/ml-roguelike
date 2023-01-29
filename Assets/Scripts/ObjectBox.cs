using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBox : MonoBehaviour
{
    private float health;
    // Start is called before the first frame update
    void Start()
    {
        this.health = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate() 
    {
        if (this.health <= 0) 
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.name == "arrow(Clone)")
        {
            Debug.Log("arrow hit damagable object");
            this.health -= col.gameObject.GetComponent<ProjectileArrow>().damage;
            Debug.Log(health);
        }
        
    }
}
