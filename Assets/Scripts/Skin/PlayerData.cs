
#define DEBUG

using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Random = UnityEngine.Random;

public enum SetStatus
{
    SS_Error = 0,
    SS_Success
}

public struct SkinInfoStc
{
    public string id;
    public string name;
    public Texture texture;
    public string info;
    public int adToUnlockCount;
}

public class PlayerData
{
    private struct _mInfoStc
    {
        public string _id;
        public string _name;
        public string _info;
        public int _adToUnlockCount;
    }
    
    [Serializable]
    private class PlayerInfo
    {
        public string playerUniqueId;
        public string playerUserName;
        public UInt64 playerBestScore;
        public int playerChoosenSkinIdx;
        public int playerCurrentAdWatchedCount;
        public bool[] buyInfo = new bool[25];
    }

    public static int skinCount = 25;
    /*All skin info will stored here*/
    private SkinInfoStc[] skinInfo;

    /*Singleton instance*/
    private static PlayerData instance = new PlayerData();
    
    /*Instance getter*/
    public static PlayerData Instance => instance;

    /*Private internal data (not saved to local memory)*/
    private List<_mInfoStc> _mList;

    /*Local memory data*/
    private PlayerInfo _playerInfo;
    private string path;
    private BinaryFormatter formatter = new BinaryFormatter();
    
    /*Private constructor*/
    private PlayerData()
    {
        if (FirebaseDB.Instance == null) ; //dummy call
        
        _playerInfo = new PlayerInfo();
         skinInfo = new SkinInfoStc[skinCount];
         path = Application.persistentDataPath + "iogamesworldballgame.dat";
        _mList = new List<_mInfoStc>()
        {
            new _mInfoStc{ _id = "default",     _name="Default Ball",   _info = "Default Skin.",     _adToUnlockCount = 0   },
            new _mInfoStc{ _id = "ar",     _name="Argentina Ball",   _info = "Argentina Flag Skin.",     _adToUnlockCount = 10000   },
            new _mInfoStc{ _id = "aus",     _name="Austria Ball",   _info = "Austria Flag Skin.",     _adToUnlockCount = 10000   },
            new _mInfoStc{ _id = "bng",     _name="Bangladesh Ball",   _info = "Bangladesh Flag Skin.",     _adToUnlockCount = 10000   },
            new _mInfoStc{ _id = "blg",     _name="Belgium Ball",   _info = "Belgium Flag Skin.",     _adToUnlockCount = 10000   },
            new _mInfoStc{ _id = "br",     _name="Brazil Ball",   _info = "Brazil Flag Skin.",     _adToUnlockCount = 10000   },
            new _mInfoStc{ _id = "bul",     _name="Bulgaria Ball",   _info = "Bulgaria Flag Skin.",     _adToUnlockCount = 10000   },
            new _mInfoStc{ _id = "ca",     _name="Canada Ball",   _info = "Canada Flag Skin.",     _adToUnlockCount = 10000   },
            new _mInfoStc{ _id = "doge",   _name="Doge Ball",   _info = "Doge Skin.",        _adToUnlockCount = 10000  },
            new _mInfoStc{ _id = "fr",   _name="France Ball",   _info = "France Flag Skin.",        _adToUnlockCount = 10000  },
            new _mInfoStc{ _id = "de",     _name="German Ball",   _info = "German Flag Skin.",     _adToUnlockCount = 10000   },
            new _mInfoStc{ _id = "hg",     _name="Hungary Ball",   _info = "Hungary Flag Skin.",     _adToUnlockCount = 10000   },
            new _mInfoStc{ _id = "ind",     _name="India Ball",   _info = "India Flag Skin.",     _adToUnlockCount = 10000   },
            new _mInfoStc{ _id = "indo",     _name="Indonesia Ball",   _info = "Indonesia Flag Skin.",     _adToUnlockCount = 10000   },
            new _mInfoStc{ _id = "it",     _name="Italy Ball",   _info = "Italy Flag Skin.",      _adToUnlockCount = 10000   },
            new _mInfoStc{ _id = "jp",     _name="Japan Ball",   _info = "Japan Flag Skin.",     _adToUnlockCount = 10000   },
            new _mInfoStc{ _id = "mx",     _name="Mexico Ball",   _info = "Mexico Flag Skin.",     _adToUnlockCount = 10000   },
            new _mInfoStc{ _id = "nth",     _name="Netherlands Ball",   _info = "Netherlands Flag Skin.",     _adToUnlockCount = 10000   },
            new _mInfoStc{ _id = "pepe",   _name="Pepe Ball",   _info = "Sad Pepe Skin.",            _adToUnlockCount = 10000  },
            new _mInfoStc{ _id = "poke",   _name="Poke Ball",   _info = "Pokeball Skin.",        _adToUnlockCount = 10000  },
            new _mInfoStc{ _id = "pl",   _name="Poland Ball",   _info = "Poland Flag Skin.",        _adToUnlockCount = 10000  },
            new _mInfoStc{ _id = "ro",   _name="Romania Ball",   _info = "Romania Flag Skin.",        _adToUnlockCount = 10000  },
            new _mInfoStc{ _id = "rus",   _name="Russia Ball",   _info = "Russia Flag Skin.",        _adToUnlockCount = 10000  },
            new _mInfoStc{ _id = "tr",     _name="Turkey Ball",   _info = "Turkey Flag Skin.",     _adToUnlockCount = 10000   },
            new _mInfoStc{ _id = "ukr",     _name="Ukraine Ball",   _info = "Ukraine Flag Skin.",     _adToUnlockCount = 10000   }

            //new _mInfoStc{ _id = "fr",     _name="France Ball",   _info = "France Flag Skin.",     _adToUnlockCount = 5   },
            //new _mInfoStc{ _id = "en",     _name="England Ball",   _info = "England Flag Skin.",    _adToUnlockCount = 5   },
            //new _mInfoStc{ _id = "us",     _name="USA Ball",   _info = "USA Flag Skin.",        _adToUnlockCount = 5   },
            //...    
        };
        
        /*Load textures*/
        Texture[] textures = Resources.LoadAll<Texture>("SkinTextures");

        /*Initialize everything to default values*/
        for (int i = 0; i < _mList.Count; ++i)
        {
            skinInfo[i].id = _mList[i]._id;
            skinInfo[i].info = _mList[i]._info;
            skinInfo[i].name = _mList[i]._name;
             skinInfo[i].adToUnlockCount = _mList[i]._adToUnlockCount;
            skinInfo[i].texture = textures[i];
        }
        
        if (!File.Exists(path))
        {
            InitializePlayerInfoForFirstTime();
        }
        else
        {
            LoadUserDetailsFromMemory();
        }
        
    }

    private void InitializePlayerInfoForFirstTime()
    {
#if (DEBUG)
        Debug.Log("Local memory file is created.");
#endif
        _playerInfo.playerUniqueId = CreateUniqueID();
        _playerInfo.playerUserName = "Username";
        _playerInfo.playerCurrentAdWatchedCount = 0;
        _playerInfo.playerChoosenSkinIdx = 0;
        _playerInfo.playerBestScore = 0;
        for (int i = 0; i < PlayerData.skinCount; ++i)
        {
            _playerInfo.buyInfo[i] = false;
        }
        _playerInfo.buyInfo[0] = true;
        bool r = SaveUserDetailsToMemory();
    }

    private bool SaveUserDetailsToMemory()
    {
        FileStream fs = new FileStream(path, FileMode.Create);
        using (fs)
        {
            formatter.Serialize(fs, _playerInfo);
#if (DEBUG)
            Debug.Log("Data saved to local.");
#endif
        }

        return FirebaseDB.Instance.HandleMyScore(new UserDetails() {
            score = _playerInfo.playerBestScore.ToString(),
            sid = _playerInfo.playerChoosenSkinIdx.ToString(),
            uid = GetPlayerUniqueId(),
            username = _playerInfo.playerUserName
        });
    }

    private void LoadUserDetailsFromMemory()
    {
        FileStream fs = new FileStream(path, FileMode.Open);
        _playerInfo = formatter.Deserialize(fs) as PlayerInfo;
        fs.Close();
        _playerInfo.buyInfo[0] = true;
    }

    public SkinInfoStc[] GetSkinsList()
    {
        return skinInfo;
    }

    public int GetSkinCount()
    {
        return skinInfo.Length;
    }

    private string CreateUniqueID()
    {
        char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        char[] digits = "0123456789".ToCharArray();
        string unique_id = "";
        for (int i = 0; i < 5; ++i)
        {
            unique_id += alpha[Random.Range(0, alpha.Length)];
            unique_id += digits[Random.Range(0, digits.Length)];
        }
        return unique_id;
    }

    public string GetPlayerUniqueId()
    {
        return _playerInfo.playerUniqueId;
    }

    public string GetPlayerUserName()
    {
        return _playerInfo.playerUserName;
    }

    public SetStatus SetPlayerUserName(string username)
    {
        /*max username length check*/
        if (username.Length > 64)
            return SetStatus.SS_Error;

        _playerInfo.playerUserName = username;
        bool r = SaveUserDetailsToMemory();

        return r ? SetStatus.SS_Success : SetStatus.SS_Error;
    }

    public UInt64 GetPlayerBestScore()
    {
        return _playerInfo.playerBestScore;
    }

    public SetStatus SetPlayerBestScore(UInt64 score)
    {
        /*score cannot be lower than previous high score*/
        if (score <= _playerInfo.playerBestScore)
            return SetStatus.SS_Error;

        _playerInfo.playerBestScore = score;
        bool r = SaveUserDetailsToMemory();

        return r ? SetStatus.SS_Success : SetStatus.SS_Error;
    }

    public int GetPlayerChoosenSkinIdx()
    {
        return _playerInfo.playerChoosenSkinIdx;
    }

    public SetStatus SetPlayerChoosenSkinIdx(int idx)
    {
        /*skin count check*/
        if (idx > (PlayerData.skinCount - 1))
            return SetStatus.SS_Error;

        _playerInfo.playerChoosenSkinIdx = idx;
        bool r = SaveUserDetailsToMemory();

        return r ? SetStatus.SS_Success : SetStatus.SS_Error;
    }

    public int GetPlayerCurrentAdWatchedCount()
    {
        return _playerInfo.playerCurrentAdWatchedCount;
    }

    public SetStatus SetPlayerCurrentAdWatchedCount(int count)
    {
        _playerInfo.playerCurrentAdWatchedCount = count;
        bool r = SaveUserDetailsToMemory();

        return r ? SetStatus.SS_Success : SetStatus.SS_Error;
    }

    public List<int> GetPlayerBuyInfoIdxList()
    {
        List<int> tmp = new List<int>();
        for(int i = 0; i< PlayerData.skinCount; ++i)
        {
            if (_playerInfo.buyInfo[i])
            {
                tmp.Add(i);
            }
        }
        return tmp;
    }

    public SetStatus SetPlayerBuyInfoIdx(int idx)
    {
        /*skin count check*/
        if (idx > (PlayerData.skinCount - 1))
            return SetStatus.SS_Error;

        _playerInfo.buyInfo[idx] = true;
        bool r = SaveUserDetailsToMemory();

        return r ? SetStatus.SS_Success : SetStatus.SS_Error;
    }
    public void Initialize()
    {

    }
}
