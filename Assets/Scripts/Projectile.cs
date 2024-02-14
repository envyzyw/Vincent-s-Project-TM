using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour, Attackable
{
    public float forceAmount;
    public float throwPower;
    private MeshRenderer mesh;
    public  float attackPower;
    public float despawnTime;
    private Rigidbody body;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        body = GetComponent<Rigidbody>();
        StartCoroutine(Despawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }

    public float GetForce() { return throwPower; }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<Attackable>() != null)
        {
            Attackable attackable = collision.gameObject.GetComponent<Attackable>();
            Vector3 enemyPos = collision.gameObject.transform.position;
            
            Vector3 forceDirection = enemyPos - transform.position;

            attackable.Attacked(forceAmount, forceDirection.normalized, attackPower);
        }
    }

    public void Attacked(float forceAmount, Vector3 forceDirection, float attackPower)
    {
        StopAllCoroutines();
        StartCoroutine(Despawn());
        body.AddForce(forceDirection * forceAmount);
    }
}
