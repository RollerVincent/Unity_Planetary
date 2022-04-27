using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : Interactable
{

    public string currentCollectable;

    Vector3 dragStartPosition;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void createEntry(string key){

        currentCollectable = key;

        Collectable targetCollectable = null;
        foreach(Collectable collectable in GameObject.Find("InventoryManager").GetComponent<InventoryManager>().collectablesParent.GetComponentsInChildren<Collectable>(true)){
            if(collectable.id == key){
                targetCollectable = collectable;
            }

        }

        GameObject entryObject = new GameObject("Entry " + key);
        MeshFilter mf = entryObject.AddComponent<MeshFilter>();
        MeshRenderer mr = entryObject.AddComponent<MeshRenderer>();
        mr.material = GlobalSettings.InventoryMaterial;
        

        mf.mesh = targetCollectable.mesh;


        entryObject.transform.parent = transform;
        

        entryObject.transform.localPosition = Vector3.zero + targetCollectable.inventoryOffset;
        entryObject.transform.localScale = entryObject.transform.localScale * targetCollectable.inventoryScale;
        entryObject.transform.localEulerAngles = targetCollectable.inventoryRotation;




    }

    public override void OnDrag(){

        GetComponent<Collider>().enabled = false;

        Vector3 pivPos = GameObject.Find("InteractionManager").GetComponent<InteractionManager>().pivotObject.transform.position;
        Vector3 camPos = GameObject.Find("InteractionManager").GetComponent<InteractionManager>().camera.transform.position;
        Vector3 pivDir = (pivPos - camPos).normalized;

        transform.position = pivPos +  GameObject.Find("FocusManager").GetComponent<FocusManager>().pivotOffset * pivDir;


        GameObject hitObject = null;
        RaycastHit hit;
       // if (Physics.Raycast(pivPos, pivDir, out hit, 10f))
        //{


        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit)) {
            hitObject = hit.collider.gameObject;
            Debug.Log(hitObject.name);
            
        }

        GetComponent<Collider>().enabled = false;
    }

    public override void OnDragStart(){
        dragStartPosition = transform.localPosition;
    }

    public override void OnDragEnd(){
        transform.localPosition = dragStartPosition;
    }
}
