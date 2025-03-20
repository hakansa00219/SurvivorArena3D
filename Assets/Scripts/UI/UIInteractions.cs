using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInteractions : MonoBehaviour
{
    private CanvasOnClick _canvasOnClick;
    [SerializeField]
    private MyAdScript _adManager;
    [SerializeField]
    private Highscore _highscore;
    [SerializeField]
    private RectTransform _canvas;
    [SerializeField]
    private GameObject[] objectsToHide;
    [SerializeField]
    private GameObject _skinMenu;
    [SerializeField]
    private GameObject _skinPrefab;
    private int _skinCount;
    [SerializeField]
    private TextMeshProUGUI _description;
    [SerializeField]
    private TextMeshProUGUI _ticketText;
    [SerializeField]
    private TextMeshProUGUI _denemetext;
    [SerializeField]
    private GameObject _playerBestPanel;
    [SerializeField]
    private TextMeshProUGUI _playerBestText;
    private int _ticketCount = 0;
    [System.Serializable]
    public class EventType : UnityEvent { }
    [Header("Buttons")]
    [SerializeField]
    private Button _playButton;
    [SerializeField]
    private Button _highscoresButton, _dailyButton, _weeklyButton, _monthlyButton, _watchAdButton,_backButton,_skinMenuButton, _soundButton;//add more button objects here if u want to add them with a singleton script.
    private Button[] _buyButtons;
    private Button[] _unlockButtons;
    private List<Button> _allButtons;
    [SerializeField] private Sprite _soundonSprite;
    [SerializeField] private Sprite _soundoffSprite;


    [Header("Events")]
    public EventType[] _eventsInScene; // add events through inspector if the event is based on object that is in scene.
    private void Awake()
    {
        _buyButtons = new Button[PlayerData.Instance.GetSkinCount()];
        _unlockButtons = new Button[PlayerData.Instance.GetSkinCount()];
        _canvasOnClick = _canvas.GetComponent<CanvasOnClick>();
        
    }
    private void Start()
    {
        _skinCount = PlayerData.Instance.GetSkinCount();
        for(int i = 0; i < _skinCount; i++)
        {
            _buyButtons[i] = _skinPrefab.transform.Find(i.ToString()).Find("buyBtn").GetComponent<Button>();
            _unlockButtons[i] = _skinPrefab.transform.Find(i.ToString()).Find("unlockBtn").GetComponent<Button>();
            //_buyButtons[i].GetComponentInChildren<TextMeshProUGUI>().SetText(PlayerData.Instance.GetSkinsList()[i].adToUnlockCount.ToString() + " to Unlock");
            _unlockButtons[i].GetComponentInChildren<TextMeshProUGUI>().SetText(PlayerData.Instance.GetSkinsList()[i].adToUnlockCount.ToString() + " to Unlock");
            int x = i;
            _buyButtons[i].onClick.AddListener(() => BuyButtonClick(x));
            _unlockButtons[i].onClick.AddListener(() => UnlockButtonClick(x));
        }
        
        _ticketText.SetText("Current Coins = " + PlayerData.Instance.GetPlayerCurrentAdWatchedCount());
        _allButtons = new List<Button>() {_playButton,_highscoresButton,_dailyButton,_weeklyButton,_monthlyButton };
        //use add listener for singleton pattern
        _playButton.onClick.AddListener(PlayButtonClick);
        _highscoresButton.onClick.AddListener(HighscoresButtonClick);
        _dailyButton.onClick.AddListener(DailyButtonClick);
        _weeklyButton.onClick.AddListener(WeeklyButtonClick);
        _monthlyButton.onClick.AddListener(MonthlyButtonClick);
        _watchAdButton.onClick.AddListener(AdButtonClick);
        _backButton.onClick.AddListener(BackButtonClick);
        _skinMenuButton.onClick.AddListener(SkinMenuButtonClick);
        _soundButton.onClick.AddListener(SoundButtonClick);
    }
    private void Update()
    {
        
        int money = PlayerData.Instance.GetPlayerCurrentAdWatchedCount();
        for (int i = 0; i< _skinCount; i++)
        {
            int price = PlayerData.Instance.GetSkinsList()[i].adToUnlockCount;
            if (money < price)
            {
                //_buyButtons[i].interactable = false;
                _unlockButtons[i].interactable = false;
            }
            else
            {
                //_buyButtons[i].interactable = true;
                _unlockButtons[i].interactable = true;
            }
            
        }
        foreach (int idx in PlayerData.Instance.GetPlayerBuyInfoIdxList())
        {
            _buyButtons[idx].gameObject.SetActive(false);
            _unlockButtons[idx].gameObject.SetActive(false);

        }




    }
    private void SoundButtonClick()
    {
        GameObject spriteObj = _soundButton.transform.GetChild(0).gameObject;
        if (spriteObj.GetComponent<Image>().sprite.name == "SoundON") 
        {
            AudioListener.volume = 0f;
            spriteObj.GetComponent<Image>().sprite = _soundoffSprite;
        }
        else if (spriteObj.GetComponent<Image>().sprite.name == "SoundOFF") 
        {
            AudioListener.volume = 1.0f;
            spriteObj.GetComponent<Image>().sprite = _soundonSprite;
        }
        else 
        {
            Debug.Log("Sound Button bug - Maybe sprite name is changed?");
        }
    }
    private void PlayButtonClick()
    {
        _playButton.gameObject.SetActive(false);
        if (_highscore.gameObject.activeInHierarchy)
        {
            _highscore.DisableScoreboard();
            _highscore.gameObject.SetActive(false);

        }
        GameManager.Instance.ChangeSceneRoutine("GameScene");      
    }
    bool highscore = true;
    private void HighscoresButtonClick()
    {
        if (_highscore.gameObject.activeInHierarchy) highscore = false;
        else highscore = true;
        _denemetext.SetText(PlayerData.Instance.GetPlayerUniqueId() + " - - - " + PlayerData.Instance.GetPlayerUserName());
        _playerBestText.SetText("Your best score: " + PlayerData.Instance.GetPlayerBestScore().ToString() );
        _dailyButton.gameObject.SetActive(highscore);
        _weeklyButton.gameObject.SetActive(highscore);
        _monthlyButton.gameObject.SetActive(highscore);
        _playerBestPanel.SetActive(highscore);
        _highscore.gameObject.SetActive(highscore);
        _highscore.OnHighscoresButtonClick();
        _highscore.OnDailyButtonClick();
    }
    private void DailyButtonClick() { _highscore.OnDailyButtonClick(); }
    private void WeeklyButtonClick() { _highscore.OnWeeklyButtonClick(); }
    private void MonthlyButtonClick() { _highscore.OnMonthlyButtonClick(); }
    private void BackButtonClick()
    {
        Debug.Log("BackButtonClicked.");
        _skinMenu.GetComponent<Animator>().SetTrigger("BackButtonClicked");
        _skinMenu.transform.Find("TopPart").Find("Skins").GetComponent<ScrollRect>().horizontalNormalizedPosition = 0;
        //seçilen skini kaydet.
        _canvasOnClick.SecilenSkiniKaydet();
        _skinMenu.transform.Find("TopPart").Find("Skins").GetComponent<ScrollRect>().horizontal = false;
        _skinMenuButton.interactable = true;
    }
    private void SkinMenuButtonClick()
    {
        _skinMenu.GetComponent<Animator>().SetTrigger("SkinsMenuClicked");
        _description.SetText("Watch ad to gain 1000 coins.");
        _dailyButton.gameObject.SetActive(false);
        _weeklyButton.gameObject.SetActive(false);
        _monthlyButton.gameObject.SetActive(false);
        if(_highscore.gameObject.activeInHierarchy)
        {
            _highscore.DisableScoreboard();
            _highscore.gameObject.SetActive(false);
            
        }
        _playerBestPanel.SetActive(false);
        _skinMenuButton.interactable = false;
        _canvasOnClick.SkinSecmeAc();
        _skinMenu.transform.Find("TopPart").Find("Skins").GetComponent<ScrollRect>().horizontal = true;
    }
    private void BuyButtonClick(int skinID) 
    { 
        //if payment is confirmed.do this.
        /*
        PlayerData.Instance.SetPlayerBuyInfoIdx(skinID);
        */
    }
    private void UnlockButtonClick(int skinID)
    {
        int price = PlayerData.Instance.GetSkinsList()[skinID].adToUnlockCount;
        int money = PlayerData.Instance.GetPlayerCurrentAdWatchedCount();
        if (money >= price) { PlayerData.Instance.SetPlayerCurrentAdWatchedCount(money - price);
            PlayerData.Instance.SetPlayerBuyInfoIdx(skinID);
            _ticketText.SetText("Current Coins = " + PlayerData.Instance.GetPlayerCurrentAdWatchedCount());
        }
        else 
        {
            _ticketText.SetText("Not enough coins.");
            StartCoroutine(NotEnoughCoin());
        } //TODO uyarı çıkacak yetersiz para diye.
    }
    private void AdButtonClick() {
        if (_adManager.IsRewAdLoaded())
        {
            Debug.Log("ad loaded");
            _adManager.ShowRewAd();
            StartCoroutine(GameManager.Instance.ADLoadingImage(_adManager));
            StartCoroutine(AdButtonTimer());
        }
        else
        {
            Debug.Log("ad didnt load");
            _description.SetText("Try again later.");

        }
    }
    public void GetRewarded() 
    {
        PlayerData.Instance.SetPlayerCurrentAdWatchedCount(PlayerData.Instance.GetPlayerCurrentAdWatchedCount() + 1000);
        _ticketText.SetText("Current Coins = " + PlayerData.Instance.GetPlayerCurrentAdWatchedCount());
        _description.SetText("Watch ad to gain 1000 coins.");
    }
    public void UsernameChanged(TextMeshProUGUI textObject)
    {
        string playername = PlayerData.Instance.GetPlayerUserName();
        if (textObject.text == playername) return;
        PlayerData.Instance.SetPlayerUserName(textObject.text);
        //PlayerData.Instance.SetPlayerBestScore(14000); //TODO:take best score from somewhere for paramater.
        _denemetext.SetText(textObject.text);
    }
    private IEnumerator AdButtonTimer()
    {
        //disable button interactable
        _watchAdButton.interactable = false;

        //make a timer 1 min
        int counter = 60;
        while(counter >= 0)
        {
            yield return new WaitForSeconds(1);
            //change button text to timer 
            counter--;
            _watchAdButton.GetComponentInChildren<TextMeshProUGUI>().SetText(counter.ToString());

        }
        // after timer ends change back to Watch
        _watchAdButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Watch");
        // make button interactable again.
        _watchAdButton.interactable = true;

    }
    private IEnumerator NotEnoughCoin()
    {
        yield return new WaitForSeconds(1);
        _ticketText.SetText("Current Coins = " + PlayerData.Instance.GetPlayerCurrentAdWatchedCount());
    }
}
