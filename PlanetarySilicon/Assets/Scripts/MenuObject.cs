using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuObject : MonoBehaviour
{

    public string name;
    public List<string> actions;
    public List<Sprite> sprites;
    
    int xIndex;
    GameObject menuGameObject;
    public bool expanded = false;
    int counter = 0;

    public bool create;

    int restingFrames;



    // Start is called before the first frame update
    void Start()
    {
         StartCoroutine(LateStart());
    }
 
    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.2f);
        Create();
    }

    // Update is called once per frame
    void Update()
    {
        if(create){
            Create();
            create = false;
        }



    /*
        if(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == null){

            if(GameObject.Find("Canvas").transform.childCount>0){
                if(GameObject.Find("Canvas").GetComponent<MenuManager>().lastSelected == null){
                    GameObject.Find("Canvas").GetComponent<MenuManager>().lastSelected = GameObject.Find("Canvas").transform.GetChild(0).gameObject;
                }
                GameObject.Find("Canvas").GetComponent<MenuManager>().lastSelected.gameObject.GetComponent<Button>().Select();
            }
            
        }else{

            if(counter > 0){
                if(expanded){
                    
                    Collapse();
                }else{
                    Expand();
                }
                counter -= 1;
            }

            bool isSelected = false;
            
            Button button = menuGameObject.transform.GetComponent<Button>();
            if(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == button.gameObject)
            {
                isSelected = true;
            }
            if(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.parent.gameObject == button.gameObject)
            {
                isSelected = true;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            { 
                
                if(isSelected)
                {
                    GameObject.Find("Canvas").GetComponent<MenuManager>().menuActive = true;
                    OnClick();
                }
                
                
            } 
            if (Input.GetKeyUp(KeyCode.LeftShift))
            { 
                if(isSelected)
                {
                    GameObject.Find("Canvas").GetComponent<MenuManager>().menuActive = false;
                    OnClick();
                }
            }

            if(isSelected){


                bool goup = false;
                bool godown = false;
                bool goright = false;
                bool goleft = false;


                float vertical = Input.GetAxis("Mouse Y");
                float horizontal = Input.GetAxis("Mouse X");

            
                if(vertical>0.005f && restingFrames == 0){
                    goup = true;
                    restingFrames = 30;//(int)(500f/(vertical/0.01f+1));
                }
                if(vertical<-0.005f && restingFrames == 0){
                    godown = true;
                    restingFrames = 30;//(int)(500f/(-vertical/0.01f+1));
                }

                if(horizontal>0.005f && restingFrames == 0){
                    goright = true;
                    restingFrames = 30;//(int)(500f/(horizontal/0.01f+1));
                }
                if(horizontal<-0.005f && restingFrames == 0){
                    goleft = true;
                    restingFrames = 30;//(int)(500f/(-horizontal/0.01f+1));
                }

                

                if(Input.GetMouseButton(0) && restingFrames == 0){// && GameObject.Find("Canvas").GetComponent<MenuManager>().menuActive == true){

                    Debug.Log("click");
                    restingFrames = 60;

                    GameObject selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
                    selected.GetComponent<Button>().Select();
                    selected.GetComponent<Button>().onClick.Invoke();


                }

            
                
                if(goup && GameObject.Find("Canvas").GetComponent<MenuManager>().menuActive == true){

                    GameObject selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
                    if(selected != menuGameObject){
                        int index = selected.transform.GetSiblingIndex();

                        if(index < selected.transform.parent.childCount-1){
                            selected.transform.parent.GetChild(index+1).gameObject.GetComponent<Button>().Select();
                        }
                        goup=false;
                    }else{
                        
                        selected.transform.GetChild(0).gameObject.GetComponent<Button>().Select();
                        
                        goup=false;
                    }
                    
                }

                if(godown && GameObject.Find("Canvas").GetComponent<MenuManager>().menuActive == true){

                    GameObject selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
                    if(selected != menuGameObject.transform.GetChild(0).gameObject && selected != menuGameObject){
                        int index = selected.transform.GetSiblingIndex();

                        if(index < selected.transform.parent.childCount){
                            selected.transform.parent.GetChild(index-1).gameObject.GetComponent<Button>().Select();
                        }
                        godown=false;
                    }else{
                        if(selected != menuGameObject){
                            menuGameObject.GetComponent<Button>().Select();
                        }
                        
                        
                        
                        godown=false;
                    }
                    
                }

                if(goright && GameObject.Find("Canvas").GetComponent<MenuManager>().menuActive == true){

                    int index = menuGameObject.transform.GetSiblingIndex();

                    if(index < menuGameObject.transform.parent.childCount-1){

                        menuGameObject.transform.parent.GetChild(index+1).gameObject.GetComponent<Button>().Select();
                        UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();

                    }

                    goright = false;


                    
                }

                if(goleft && GameObject.Find("Canvas").GetComponent<MenuManager>().menuActive == true){

                    int index = menuGameObject.transform.GetSiblingIndex();

                    if(index > 0){

                        menuGameObject.transform.parent.GetChild(index-1).gameObject.GetComponent<Button>().Select();
                        UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();

                    }
                    goleft = false;
                }
                
                GameObject.Find("Canvas").GetComponent<MenuManager>().lastSelected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

            }

            restingFrames -= 1;
            restingFrames = Mathf.Max(0,restingFrames);



        }

    */

        
    }

    

    public void Create(){

        xIndex = GameObject.Find("Canvas").transform.childCount;

        GameObject newObj = new GameObject(name + " Menu"); //Create the GameObject
        menuGameObject = newObj;
        newObj.SetActive(true);
        Image newImage = newObj.AddComponent<Image>();
        newImage.sprite = sprites[0];

        Button newButton = newObj.AddComponent<Button>();
        //MenuEntry entry = newObj.AddComponent<MenuEntry>();



        //newButton.onClick.AddListener(OnClick);



        ColorBlock colors = ColorBlock.defaultColorBlock;
        colors.normalColor = GlobalSettings.UiColor;
        colors.selectedColor = GlobalSettings.UiColor;
        colors.highlightedColor = GlobalSettings.UiColor;
        newButton.colors = colors;

        
        RectTransform rt = newObj.GetComponent<RectTransform>();
        rt.SetParent(GameObject.Find("Canvas").transform);

        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchorMin = new Vector2(0.5f, 0.5f);

        rt.anchoredPosition = Vector3.zero;//new Vector2(GlobalSettings.UiSize + GlobalSettings.UiMargin, 0) * xIndex + new Vector2(GlobalSettings.UiMargin, GlobalSettings.UiMargin);
        rt.sizeDelta = new Vector2(GlobalSettings.UiSize, GlobalSettings.UiSize);


       

        int c = 1;
        foreach(string action in actions){
            CreateAction(action, sprites[c], (c-1)*1f/actions.Count*360f);
            c += 1;
        }


    }

    void SetActionMethod(Button button, string action){
        if(action == "Expand"){
            //button.onClick.AddListener(gameObject.GetComponent<MenuObject>().OnClick);
        }else if(action == "RocketUp"){
            button.onClick.AddListener(delegate{gameObject.GetComponent<FlyingObject>().TogglePointUp(button.GetComponent<Image>());});
        }else if(action == "RocketAutoHeight"){
            button.onClick.AddListener(delegate{gameObject.GetComponent<FlyingObject>().ToggleAutoHeight(button.GetComponent<Image>());});
        }
        else if(action == "CameraStar"){
            button.onClick.AddListener(gameObject.GetComponent<CameraManager>().EnableStarViewMode);
        }
        else if(action == "CameraObject"){
            button.onClick.AddListener(gameObject.GetComponent<CameraManager>().EnableObjectViewMode);
        }
        else if(action == "CameraPlayer"){
            button.onClick.AddListener(gameObject.GetComponent<CameraManager>().EnablePlayerViewMode);
        }
        else if(action == "OrbitView"){
            button.onClick.AddListener(delegate{GameObject.Find("Universe").GetComponent<Universe>().ToggleOrbitView(button.GetComponent<Image>());});
        }
        else if(action == "VelocityPreview"){
            button.onClick.AddListener(delegate{gameObject.GetComponent<AttractedObject>().ToggleVelocityPreview(button.GetComponent<Image>());});
        }
        else if(action == "Seat"){
            //button.onClick.AddListener(delegate{gameObject.GetComponentInChildren<PlayerSeat>().ToggleSitting(button.GetComponent<Image>());});
            button.onClick.AddListener(delegate{gameObject.GetComponentInChildren<Beamer>().ToggleSitting(button.GetComponent<Image>());});
        }
        else if(action == "HelmetLight"){
            //button.onClick.AddListener(delegate{gameObject.GetComponentInChildren<PlayerSeat>().ToggleSitting(button.GetComponent<Image>());});
            button.onClick.AddListener(delegate{gameObject.GetComponentInChildren<WalkController>().ToggleLight(button.GetComponent<Image>());});
        }
        else if(action == "Struts"){
            //button.onClick.AddListener(delegate{gameObject.GetComponentInChildren<PlayerSeat>().ToggleSitting(button.GetComponent<Image>());});
            button.onClick.AddListener(delegate{gameObject.GetComponentInChildren<StrutManager>().Toggle(button.GetComponent<Image>());});
        }
        else if(action == "FocusBackPack"){
            //button.onClick.AddListener(delegate{gameObject.GetComponentInChildren<PlayerSeat>().ToggleSitting(button.GetComponent<Image>());});
            button.onClick.AddListener(delegate{gameObject.GetComponentInChildren<BackPack>().ToggleView(button.GetComponent<Image>());});
        }
    }

    void CreateAction(string action, Sprite sprite, float angle){
        GameObject newObj = new GameObject(action + " Action"); //Create the GameObject
        
        Image newImage = newObj.AddComponent<Image>();
        newImage.raycastTarget = true;
        newImage.sprite = sprite;
        //newImage.color = new Color(newImage.color.r, newImage.color.g, newImage.color.b, 0);

        Button newButton = newObj.AddComponent<Button>();
        ColorBlock colors = ColorBlock.defaultColorBlock;
        colors.normalColor = GlobalSettings.UiSelectedColor;
        colors.selectedColor = GlobalSettings.UiSelectedColor;
        colors.highlightedColor = GlobalSettings.UiColor;
        newButton.colors = colors;
        



        SetActionMethod(newButton, action);

        
        RectTransform rt = newObj.GetComponent<RectTransform>();
        rt.SetParent(menuGameObject.transform);

        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchorMin = new Vector2(0.5f, 0.5f);


        Vector3 pos = new Vector3(GlobalSettings.UiMargin,0,0);
        pos = Quaternion.AngleAxis(angle, new Vector3(0,0,1)) * pos;

                
            

        rt.anchoredPosition = pos;//new Vector2(GlobalSettings.UiSize + GlobalSettings.UiMargin, 0) * xIndex + new Vector2(GlobalSettings.UiMargin, GlobalSettings.UiMargin);
        rt.sizeDelta = new Vector2(GlobalSettings.UiSize*0.6f, GlobalSettings.UiSize*0.6f);

    }

    void Expand(){


        


        for(int i=0; i<menuGameObject.transform.childCount; i++){
            RectTransform child_rt = menuGameObject.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
            Image child_image = menuGameObject.transform.GetChild(i).gameObject.GetComponent<Image>();

            RectTransform parent_rt = menuGameObject.GetComponent<RectTransform>();

            child_rt.anchoredPosition = new Vector2(child_rt.anchoredPosition.x, (GlobalSettings.UiMargin+GlobalSettings.UiSize)*(i+1)*(counter/GlobalSettings.UiSpeed));
            //child_rt.sizeDelta = new Vector2(GlobalSettings.UiSize, GlobalSettings.UiSize*(counter/100f));
            float a = (counter/GlobalSettings.UiSpeed);
            a = Mathf.Pow(a, 3);
            child_image.color = new Color(child_image.color.r, child_image.color.g, child_image.color.b, a);




        }
    }

    void Collapse(){
        for(int i=0; i<menuGameObject.transform.childCount; i++){
            RectTransform child_rt = menuGameObject.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
            Image child_image = menuGameObject.transform.GetChild(i).gameObject.GetComponent<Image>();


            RectTransform parent_rt = menuGameObject.GetComponent<RectTransform>();

            child_rt.anchoredPosition = new Vector2(child_rt.anchoredPosition.x, (GlobalSettings.UiMargin+GlobalSettings.UiSize)*(i+1)*((GlobalSettings.UiSpeed-counter)/GlobalSettings.UiSpeed));
            //child_rt.sizeDelta = new Vector2(GlobalSettings.UiSize, GlobalSettings.UiSize*((100-counter)/100f));
            float a = ((GlobalSettings.UiSpeed-counter)/GlobalSettings.UiSpeed);
            a = Mathf.Pow(a, 3);
            child_image.color = new Color(child_image.color.r, child_image.color.g, child_image.color.b, a);


        }
    }

    public void OnClick(){
        if(expanded){
            for(int i=0; i<menuGameObject.transform.childCount; i++){
                Image im = menuGameObject.transform.GetChild(i).GetComponent<Image>();
                im.raycastTarget = false;

                
            }

            counter = (int) GlobalSettings.UiSpeed;
            expanded = false;
        }else{
            MenuObject[] components = GameObject.FindObjectsOfType<MenuObject>();
            foreach(MenuObject menuObject in components){
               
                if(menuObject != this && menuObject.expanded){
                    menuObject.OnClick();
                }
            }

            for(int i=0; i<menuGameObject.transform.childCount; i++){
                Image im = menuGameObject.transform.GetChild(i).GetComponent<Image>();
                im.raycastTarget = true;
            }

            counter = (int) GlobalSettings.UiSpeed;

            expanded = true;
        }
    }
}
