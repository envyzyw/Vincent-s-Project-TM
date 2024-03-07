using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class Projectile : NetworkBehaviour, Attackable
{
    public float forceAmount;
    public float throwPower;
    private MeshRenderer mesh;
    public  float attackPower;
    private float originalAttackPower;
    public float despawnTime;
    private Rigidbody body;
    // Start is called before the first frame update
    void Start()
    {
        originalAttackPower = attackPower;
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
        GetComponent<NetworkObject>().Despawn();
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

    public void Fire(Vector3 direction)
    {
        GetComponent<Rigidbody>().AddForce(direction * throwPower);
        ProjectileServerRpc(direction);
        StartCoroutine(Despawn());
    }

    [ServerRpc]
    private void ProjectileServerRpc(Vector3 cameraForward)
    {
        GetComponent<NetworkObject>().Spawn();
    }

    public void Attacked(float forceAmount, Vector3 forceDirection, float attackPower)
    {
        StopAllCoroutines();
        StartCoroutine(Despawn());
        attackPower += originalAttackPower;
        body.AddForce(forceDirection * forceAmount);
    }
}
