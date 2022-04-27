using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameter : MonoBehaviour
{
    public string name;
    public Sprite icon;
    public float normal;
    public float low;
    public bool limited;
    public float current;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetOffset(){
        return (current-low) / (normal-low);
    }
}
