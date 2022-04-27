using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GlobalSettings : MonoBehaviour
{

    public bool updateSettings;
    

    [SerializeField]
    private float placeHolderDistance;
    public static float PlaceHolderDistance;

    [SerializeField]
    private Material lineMaterial;
    public static Material LineMaterial;


    [SerializeField]
    private float uiMargin;
    public static float UiMargin;

    [SerializeField]
    private float uiSize;
    public static float UiSize;

    [SerializeField]
    private float uiSpeed;
    public static float UiSpeed;

    [SerializeField]
    private Color uiColor;
    public static Color UiColor;

    [SerializeField]
    private Color uiActiveColor;
    public static Color UiActiveColor;

    [SerializeField]
    private Color uiSelectedColor;
    public static Color UiSelectedColor;

    [SerializeField]
    private float sunBrightDistance;
    public static float SunBrightDistance;





    [SerializeField]
    [Range(1, 100)]
    private int playBackSpeed;
    public static int PlayBackSpeed;

    [SerializeField]
    [Range(1, 10)]
    private float orbitSpeed;
    public static float OrbitSpeed;

    [SerializeField]
    private bool applyOrbits;
    public static bool ApplyOrbits;


    [SerializeField]
    [Range(100, 1000)]
    private int traceLength;
    public static int TraceLength;

    [SerializeField]
    [Range(1, 1000)]
    private int menuExpansion;
    public static int MenuExpansion;

    [SerializeField]
    [Range(0f, 1f)]
    private float inventorySlotSpacing;
    public static float InventorySlotSpacing;

    

    [SerializeField]
    private Material inventorySlotMaterial;
    public static Material InventorySlotMaterial;

    [SerializeField]
    private Material inventoryMaterial;
    public static Material InventoryMaterial;

    [SerializeField]
    private Sprite inventorySlotSprite;
    public static Sprite InventorySlotSprite;

    

    
    




    private bool isUpdated = false;

    
    public void Start(){
        UpDateSettings();
    }


    public void Update(){

        if(updateSettings || !isUpdated){


            UpDateSettings();

            updateSettings = false;
            isUpdated = true;
        }

    }

    void UpDateSettings(){
        PlaceHolderDistance = placeHolderDistance;
        Shader.SetGlobalFloat("_PlaceHolderDistance", PlaceHolderDistance);

        LineMaterial = lineMaterial;

        UiMargin = uiMargin;
        UiSize = uiSize;
        UiSpeed = uiSpeed;
        UiColor = uiColor;
        UiActiveColor = uiActiveColor;
        UiSelectedColor = uiSelectedColor;
        
        SunBrightDistance = sunBrightDistance;
        Shader.SetGlobalFloat("_SunBrightDistance", SunBrightDistance);

        PlayBackSpeed = playBackSpeed;
        TraceLength = traceLength;
        ApplyOrbits = applyOrbits;
        OrbitSpeed = orbitSpeed;
        MenuExpansion = menuExpansion;
        InventorySlotSpacing = inventorySlotSpacing;
        InventorySlotMaterial = inventorySlotMaterial;
        InventoryMaterial = inventoryMaterial;
        InventorySlotSprite = inventorySlotSprite;
        
    }



}
