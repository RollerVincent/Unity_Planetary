using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryObject : Placeable
{


    public float weightCapacity=100;
    public float currentWeight;
    public int xSlots;
    public int ySlots;
    public float slotZOffset;

    public string[,] content;

    public bool log;

    
    


    // Start is called before the first frame update
    void Start()
    {
        content = new string[xSlots, ySlots];
        currentWeight = 0;
    }


    public bool SetNext(Collectable collectable){

        string key = collectable.id;
        for (int i=0;i<xSlots;i++){
            for (int j=0;j<ySlots;j++){
                if(content[i,j] == null){
                    if(currentWeight <= weightCapacity - collectable.weight){
                        content[i,j] = key;
                        currentWeight += collectable.weight;
                        return true;
                    }else{
                        return false;
                    }
                    
                }
            }
        }
        return false;
    }



    void logContent(){
        Debug.Log("Inventory:");
        for (int i=0;i<xSlots;i++){
            for (int j=0;j<ySlots;j++){
                
                if(content[i,j] != null){
                    Debug.Log(content[i,j]);
                    
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(log){
            logContent();
            log = false;
        }
    }

    public override void OnClick(){
        logContent();



        GameObject focusObject = GameObject.Find("FocusManager").GetComponent<FocusManager>().Focus(gameObject);
        if(focusObject != null){
            DestroyImmediate(focusObject.GetComponent<InventoryObject>());

            for(int x=0; x<xSlots; x++){
                for(int y=0; y<ySlots; y++){
                    createSlot(focusObject, x, y);
                }
            }


            
        }
        


    }

    void createSlot(GameObject focusObject, int x, int y){

        GameObject slot = GameObject.Instantiate(GameObject.Find("InventorySlotInstance"));

        //DestroyImmediate(slot.GetComponent<MeshRenderer>());
        //DestroyImmediate(slot.GetComponent<MeshFilter>());
        //SpriteRenderer sr = slot.AddComponent<SpriteRenderer>();
        //sr.sprite = GlobalSettings.InventorySlotSprite;
        //sr.material = GlobalSettings.InventorySlotMaterial;
        
        Vector3 pivPos = GameObject.Find("InteractionManager").GetComponent<InteractionManager>().pivotObject.transform.position;
        Vector3 camPos = GameObject.Find("InteractionManager").GetComponent<InteractionManager>().camera.transform.position;
        Vector3 pivDir = (pivPos - camPos).normalized;

        slot.name = "InventorySlot";
        
        
        slot.transform.rotation = focusObject.transform.rotation;

        slot.transform.parent = focusObject.transform;

        //slot.transform.localScale = slot.transform.localScale * GlobalSettings.InventorySlotSize * 0.1f;
        


        slot.transform.localPosition = Vector3.zero;
        slot.transform.localPosition += new Vector3(x*GlobalSettings.InventorySlotSpacing, y*GlobalSettings.InventorySlotSpacing, slotZOffset);
        slot.transform.localPosition -= new Vector3(((xSlots-1)*GlobalSettings.InventorySlotSpacing)*0.5f, ((xSlots-1)*GlobalSettings.InventorySlotSpacing)*0.5f, 0);

        slot.transform.localPosition = new Vector3(slot.transform.localPosition.x/slot.transform.parent.localScale.x, slot.transform.localPosition.y/slot.transform.parent.localScale.y, slot.transform.localPosition.z);

        InventorySlot slotComponent = slot.GetComponent<InventorySlot>();


        if(content[x,y] != null){
            slotComponent.createEntry(content[x,y]);
        }


        


    }


   



  
}
