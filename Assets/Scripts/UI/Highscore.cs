using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Highscore : MonoBehaviour
{
    private List<UserDetails> _dailyUserDetails = new List<UserDetails>();
    private List<UserDetails> _weeklyUserDetails = new List<UserDetails>();
    private List<UserDetails> _monthlyUserDetails = new List<UserDetails>();

    private TextMeshProUGUI[] _allScores;
    private GameObject[] _skinSpheres;
    [SerializeField]
    private RectTransform _referenceTextPlace;
    [SerializeField] private GameObject _referenceSkinPlace;
    [SerializeField]
    private Material _otherTextMaterial;

    private int _playerPosition;
    private void Awake()
    {
        _allScores = CreateRectTransform(_referenceTextPlace);
        _skinSpheres = CreateRectTransform(_referenceSkinPlace);
    }
    private TextMeshProUGUI[] CreateRectTransform (RectTransform reference)
    {
        TextMeshProUGUI[] objs = new TextMeshProUGUI[FirebaseDB.Instance.GetMaxDictCount()];
        
        for (int i = 0; i < objs.Length; i++)
        {
            GameObject obj = Instantiate(reference, gameObject.transform).gameObject;
            RectTransform rect = obj.GetComponent<RectTransform>();
            TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
            obj.name = (i + 1).ToString() + ".Place_Text";
            rect.anchoredPosition = reference.anchoredPosition - new Vector2(0, reference.rect.height * i);
            if(text.fontSize > 20f) text.fontSize = 20f;
            objs[i] = text;

        }
        return objs ?? null;

    }
    private GameObject[] CreateRectTransform(GameObject reference)
    {
        GameObject[] skinSphere = new GameObject[FirebaseDB.Instance.GetMaxDictCount()];
        RectTransform refrect = reference.GetComponent<RectTransform>();

        for (int i = 0; i < skinSphere.Length; i++)
        {
            RectTransform rect = Instantiate(reference, gameObject.transform).GetComponent<RectTransform>();
            rect.name = (i + 1).ToString() + ".Place_Skin";
            rect.anchoredPosition = refrect.anchoredPosition - new Vector2(0, refrect.rect.height * i);
            skinSphere[i] = rect.transform.GetChild(0).gameObject;
        }
        return skinSphere ?? null;
    }
    private void Show(List<UserDetails> details)
    {
        //tüm textleri temizle
        for (int i = 0; i < _allScores.Length; i++)
        {
            _allScores[i].SetText("");
        }
        //textleri yerleştir
        for (int i = 0; i < _allScores.Length && details.Count != i; i++)
        {
            _allScores[i].SetText((i + 1) + ". " + details[i].username + " = " + details[i].score);
        }
        //skinleri yerleştir.
        for (int i = 0; i < _skinSpheres.Length; i++)
        {
            if (i == _skinSpheres.Length) break;
            if (details[i].sid == null) return;
            Material mat = _skinSpheres[i].GetComponent<MeshRenderer>().material;
            mat.SetTexture("_BaseMap", PlayerData.Instance.GetSkinsList()[Convert.ToInt16(details[i].sid)].texture);
        }
    }
    public void OnDailyButtonClick() { Show(_dailyUserDetails); }
    public void OnWeeklyButtonClick() { Show(_weeklyUserDetails); }
    public void OnMonthlyButtonClick() { Show(_monthlyUserDetails); }
    public void OnHighscoresButtonClick()
    {
        EnableScoreboard();
        var obj = FirebaseDB.Instance.GetValues();

        //obj["d"] // daily list
        //obj["w"] // weekly list
        //obj["m"] // monthly list
        _dailyUserDetails = obj["d"];
        _weeklyUserDetails = obj["w"];
        _monthlyUserDetails = obj["m"];

    }
    public void EnableScoreboard()
    {
        foreach (TextMeshProUGUI t in _allScores) { t.gameObject.SetActive(true); }
        foreach (GameObject g in _skinSpheres) { g.SetActive(true); }
    }
    public void DisableScoreboard()
    {
        foreach (TextMeshProUGUI t in _allScores) { t.gameObject.SetActive(false); }
        foreach (GameObject g in _skinSpheres) { g.SetActive(false); }
    }
}




