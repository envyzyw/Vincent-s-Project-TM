using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ImprovisedPlayerScript player = other.gameObject.GetComponent<ImprovisedPlayerScript>();
            Cursor.lockState = CursorLockMode.None;
            player.ShowShop();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ImprovisedPlayerScript player = other.gameObject.GetComponent<ImprovisedPlayerScript>();
            player.HideShop();
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
