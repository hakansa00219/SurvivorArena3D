using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinsPanel : MonoBehaviour
{
    public RectTransform panelPrefab;
    public Material refMat;
    
    private SkinInfoStc[] _ls;
    private GameObject _skinPrefab;
    private float _skinDist = 150f;
    void Awake()
    {
        /*Get list from singleton*/
        _ls = PlayerData.Instance.GetSkinsList();
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(PlayerData.Instance.GetSkinCount() * _skinDist, GetComponent<RectTransform>().sizeDelta.y);

        for (int i = 0; i < PlayerData.Instance.GetSkinCount(); i++)
        {
            RectTransform obj = Instantiate(panelPrefab, this.transform);
            obj.name = i.ToString(); //eskisi Prefab + i ydi değiştirdim.
            obj.anchoredPosition = new Vector2(i * _skinDist, -15f);
            _skinPrefab = obj.Find("SkinOverview").Find("Sphere").gameObject;
            Material m = new Material(refMat);
            m.SetTexture("_BaseMap", _ls[i].texture);
            _skinPrefab.GetComponent<MeshRenderer>().sharedMaterial = m;
            obj.Find("skinNamePanel").Find("skinName").GetComponent<TextMeshProUGUI>().SetText(_ls[i].name);
            obj.Find("SkinOverview").Find("skinInfo").GetComponent<TextMeshProUGUI>().SetText(_ls[i].info);
        }
        
    }

}
