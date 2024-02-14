using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum pickupType
    {
        health,
        gold,
        weapon
    }

    public pickupType type;

    private Transform target;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        Vector3 direction = new Vector3(target.position.x, target.position.y+1, target.position.z) - transform.position;
        transform.position += direction.normalized * 6 * Time.deltaTime;
    }

    public void Follow(Transform target)
    {
        this.target = target;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            ImprovisedPlayerScript player = other.gameObject.GetComponent<ImprovisedPlayerScript>();
            if(type == pickupType.health)
            {
                //Heal player
                player.Heal(25);
                Destroy(gameObject);
            }
            else if(type == pickupType.gold)
            {
                player.AddGold();
                Destroy(gameObject);
            }
        }
    }
}
