using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableButton : Interactable
{
    public Actable actable;
    // Start is called before the first frame update
    void Start()
    {
        cursor = actable.cursor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnClick(){
        

        actable.Act();
    }
}
