using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System;

public class Scoreboard : MonoBehaviour
{
    public Dictionary<string, float> _list;
    private TextMeshProUGUI[] _allScores;
    private TextMeshProUGUI _playerScore;
    [SerializeField]
    private RectTransform _firstTextTransform;
    [SerializeField]
    private Material _textMaterial;
    [SerializeField]
    private Material _otherTextMaterial;
    public TMP_FontAsset tmpFont;
    public Material fontMat;
    private Material _fontMat;

    private int _playerPosition;
    private int _characterCount; // oyundaki char sayısı;

    private void Start()
    {
        Enemy.IamDeadEvent += EnemyIsDead;
        Enemy.IamSpawnedEvent += CharacterSpawned;
        Character.IEatSomethingEvent += SetScore;
        Player.IamSpawned += CharacterSpawned;
    }

    private void Awake()
    {
        _fontMat = new Material(fontMat);
        _list = new Dictionary<string, float>();
        _characterCount = GameObject.FindGameObjectsWithTag("Enemy").Length / 2 + GameObject.FindGameObjectsWithTag("Player").Length / 2;
        _allScores = new TextMeshProUGUI[10];
        for(int i = 0; i < _allScores.Length; i++)
        {
            GameObject text = new GameObject((i+1).ToString() +".Place_Text");
            _allScores[i] = text.GetComponent<TextMeshProUGUI>() ?? text.AddComponent<TextMeshProUGUI>();
            text.transform.SetParent(gameObject.transform);
            _allScores[i].rectTransform.position = _firstTextTransform.position - new Vector3(0, _firstTextTransform.rect.height*i);
            _allScores[i].rectTransform.sizeDelta = new Vector2(_firstTextTransform.rect.width, _firstTextTransform.rect.height);
            _allScores[i].enableAutoSizing = true;
            _allScores[i].fontSizeMin = 1;
            _allScores[i].alignment = TextAlignmentOptions.Flush;
            //_allScores[i].fontSharedMaterial = _otherTextMaterial;
            _allScores[i].font = tmpFont;
            _allScores[i].fontSharedMaterial = _fontMat;

        }
        _playerScore = new TextMeshProUGUI();
        GameObject text2 = new GameObject("Players_Place_Text");
        text2.transform.SetParent(gameObject.transform);
        _playerScore = text2.GetComponent<TextMeshProUGUI>() ?? text2.AddComponent<TextMeshProUGUI>(); 
        _playerScore.rectTransform.position = _firstTextTransform.position - new Vector3(0, _firstTextTransform.rect.height * (_allScores.Length));
        _playerScore.rectTransform.sizeDelta = new Vector2(_firstTextTransform.rect.width, _firstTextTransform.rect.height);
        //_playerScore.enableAutoSizing = true;
        _playerScore.enableAutoSizing = true;
        _playerScore.fontSizeMin = 1;
        _playerScore.alignment = TextAlignmentOptions.Flush;
        //_playerScore.fontSharedMaterial = _textMaterial;
        _playerScore.font = tmpFont;
        _playerScore.fontSharedMaterial = _fontMat;
        InvokeRepeating("Show", 0f, 1f);
    }

    private void SetScore(string name , UInt32 score, EatType notUsed) //add texture later.
    {
        if (_list.ContainsKey(name)) 
        {
            _list[name] = score;
        } else
        {
            _list.Add(name, score);
        }
    }
    
    private void CharacterSpawned(string name, UInt32 score)
    {
        SetScore(name, score, EatType.ET_NONE);
    }

    private void Sort()
    {
        _list = _list.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
    }
    private void Show()
    {
        Sort();
        for (int i = 0; i < _allScores.Length; i++)
        {
            if (i >= _list.Count)
            {
                _allScores[i].SetText("");
                continue;
            }

            string line = GenerateTextLine((i + 1).ToString(), _list.Keys.ElementAt(i), _list.Values.ElementAt(i).ToString());
            
            _allScores[i].SetText(line);
            
        }
        TekrarBoya();
    }

    private string GenerateTextLine(string order, string name, string score)
    {
        string retval = "";
        /*retval += new String(' ', 3 - order.Length);
        retval += order;
        retval += ". ";*/
        retval += " ";
        retval += TruncateName(name, 15);
        retval += "\t\t";
        retval += score;
        return retval;
    }

    void TekrarBoya()
    {
        string playerUserName = PlayerData.Instance.GetPlayerUserName();
        int playerIdx = 1;
        foreach (KeyValuePair<string, float> pair in _list)
        {
            if (pair.Key == playerUserName)
            {
                _playerPosition = playerIdx;
                
                break;
            }
            ++playerIdx;
        }
        for (int i = 0; i < _allScores.Length; i++)
        {
            _allScores[i].fontSharedMaterial = _fontMat;
            _allScores[i].color = Color.white;
        }
        /*playeri boyamak*/
        if (playerIdx > _allScores.Length)
        {
            _playerScore.enabled = true;
            string line = _playerPosition.ToString() + ".";
            line += GenerateTextLine(_playerPosition.ToString(), playerUserName, _list[playerUserName].ToString());

            _playerScore.SetText(line);            
            _playerScore.fontSharedMaterial = _fontMat;
            

        }
        else
        {
            _playerScore.enabled = false;
            
            _allScores[playerIdx - 1].fontSharedMaterial = _fontMat;
            _allScores[playerIdx - 1].color = Color.red;
            //Debug.Break();
        }
        _playerScore.color = Color.red;
        
    }

    public void EnemyIsDead(string name)
    {
        if (_list.ContainsKey(name))
        {
            _list.Remove(name);
        }
        else
            return;
    }
    
    public string CheckIfSuitableForThrowMissileOrDefault(string[] names, string selfname)
    {
        var firstNElementToLookUp = 3; /*25*/
        string playerUserName = PlayerData.Instance.GetPlayerUserName();
        string retVal = "";
        string tmpkey = "";

        //do
        //{
        //    tmpkey = _list.ElementAt(UnityEngine.Random.Range(0, firstNElementToLookUp)).Key;
        //} while (tmpkey.CompareTo(playerUserName) == 0 || tmpkey.CompareTo(selfname) == 0);

        //retVal = tmpkey;

        if (names != null)
        {
            if (_list != null)
            {
                var sortedFirstNElement = _list.Keys.ToList().GetRange(0, firstNElementToLookUp);
                for (int i = 0; i < names.Length; i++)
                {
                    if (sortedFirstNElement.Contains(names[i]))
                    {
                        retVal = names[i];
                    }
                }
            }
            else
            {
                retVal = names[0];
            }
        }
        
        return retVal;
    }

    public int GetEnemyCount() { return _list.Count ; } //-1: one of them is player

    private string TruncateName(string name, int size)
    {
        return name.Length > size ? (string.Format("{0}", name.Substring(0, size)) + "..") : name;
    }
    private void OnDestroy()
    {
        Enemy.IamDeadEvent -= EnemyIsDead;
        Enemy.IamSpawnedEvent -= CharacterSpawned;
        Character.IEatSomethingEvent -= SetScore;
        Player.IamSpawned -= CharacterSpawned;
    }
}
