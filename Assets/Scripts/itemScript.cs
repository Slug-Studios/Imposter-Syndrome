using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemScript : MonoBehaviour
{
    public walterBrain Spawner;
    public int upgrade;
    public GameObject SpawnedItem;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if a item has been picked, destroy itself
        if (Spawner.Itempicked)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<playerController>() != null)
        {
            other.GetComponent<playerController>().upgradeEquiped = upgrade;
            GameObject Item = Instantiate(SpawnedItem, other.transform.position, other.transform.rotation, other.transform);
            Item.transform.localPosition = SpawnedItem.transform.position;
            Item.transform.localRotation = SpawnedItem.transform.rotation;
            Spawner.Itempicked = true;
            Spawner.player.gameObject.GetComponent<playerController>().equipedItem = Item;
        }
    }
}
