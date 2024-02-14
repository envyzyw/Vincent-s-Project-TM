using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawn : NetworkBehaviour, Attackable
{
    public GameObject Enemy;
    public float spawnRate;
    public bool isConstant;


    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        if (isConstant) StartCoroutine(SpawnEnemy());
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(SpawnEnemy());

        
    }
    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            if (isConstant)
            {
                yield return new WaitForSeconds(1 / spawnRate);

                Instantiate(Enemy, transform.position, Quaternion.identity);
                if (Enemy.GetComponent<NetworkObject>()) Enemy.GetComponent<NetworkObject>().Spawn();
                else Debug.Log("woww, please setup your prefabs before typing code");
            }
            else
            {
                yield return null;
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !isConstant)
        {
            Instantiate(Enemy, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attacked(float playerForceAmount, Vector3 forceDirection, float attackPower)
    {
        Destroy(gameObject);
    }

}
