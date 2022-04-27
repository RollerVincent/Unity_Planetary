using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actable : MonoBehaviour
{

    public Sprite cursor;
  

    public virtual void Act(){
        Debug.Log("Clicked");
    }
    
}
