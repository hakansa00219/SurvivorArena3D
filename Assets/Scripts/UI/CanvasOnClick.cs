using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CanvasOnClick : MonoBehaviour
{
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;


    GameObject secilenSkin;
    GameObject oldresult;
    string oldresultname;
    GameObject[] _allSkinPanels;

    bool skinsecmekapalı = false;

    [SerializeField] private GameObject _content;

    void Start()
    {
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();
        oldresult = new GameObject("DontMindMe(Useless)");
        _allSkinPanels = GameObject.FindGameObjectsWithTag("skinsPanel");
        SkinSecili(PlayerData.Instance.GetPlayerChoosenSkinIdx());
    }

    void Update()
    {
        if (skinsecmekapalı) return;
        //Check if the left Mouse button is clicked
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray

            foreach (RaycastResult result in results)
            {              
                if(result.gameObject.tag == "skinsPanel")
                {
                    //secili olan satın alınmışmı diye bak alınmışsa aşağıdaki methodlar çağırılcak.
                    //degilse continue?
                    List<int> skinList = new List<int>();
                    skinList = PlayerData.Instance.GetPlayerBuyInfoIdxList();
                    if(PlayerData.Instance.GetPlayerBuyInfoIdxList().Contains(System.Convert.ToInt32(result.gameObject.name)))
                    {
                        //it contains so I bought it.
                        //do other sutff
                        if (result.gameObject.name == oldresult.name)
                        {
                            //aynısına tıklandı.
                            oldresult.name = result.gameObject.name;
                        }
                        else
                        {
                            //eski seçili olanı kaldır
                            Color oldcolor = oldresult.GetComponent<Image>().color;
                            oldcolor.a = 0.392f;
                            oldresult.GetComponent<Image>().color = oldcolor;
                            //eski secili olanın selectionu kaldır
                            oldresult.transform.Find("Selection").gameObject.SetActive(false);
                            //eski seçilenin indexini tut.
                            string forclosinghighlight = oldresultname;
                            //yeni seçileni highlight et.
                            SkinSecili(System.Convert.ToInt16(result.gameObject.name));
                        }
                        SecilenSkiniKaydet();
                        SkinSecmeAc();
                    }                    
                            
                }               
            }
        }
    }
    public void SkinSecili(int seciliSkinIdx)
    {

        //secilen skin
        secilenSkin = _allSkinPanels[seciliSkinIdx];
        //highlight
        Color oldColor = _allSkinPanels[seciliSkinIdx].GetComponent<Image>().color;
        oldColor.a = 1;
        _allSkinPanels[seciliSkinIdx].GetComponent<Image>().color = oldColor;
        //selection text
        _allSkinPanels[seciliSkinIdx].transform.Find("Selection").gameObject.SetActive(true);
        //seçilen skin artık eski skin oldu.
        oldresult = _allSkinPanels[seciliSkinIdx];
    }
   
    public void SecilenSkiniKaydet()
    {
        if (secilenSkin == null)
        {
            Debug.Log("secilen skin null");
            return;
        }
        PlayerData.Instance.SetPlayerChoosenSkinIdx(System.Convert.ToInt16(secilenSkin.name));
        skinsecmekapalı = true;
    }
    public void SkinSecmeAc()
    {
        skinsecmekapalı = false;
    }
}