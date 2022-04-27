using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : Interactable
{

    public string id;
    public Mesh mesh;
    public float weight;

    public float inventoryScale;
    public Vector3 inventoryRotation;
    public Vector3 inventoryOffset;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnClick(){
        if(GameObject.Find("InventoryManager").GetComponent<InventoryManager>().activeInventory.SetNext(this)){
            DestroyImmediate(gameObject);
        }
        
    }
}
