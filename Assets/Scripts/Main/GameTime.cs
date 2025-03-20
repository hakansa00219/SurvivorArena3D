using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    [SerializeField ]private TextMeshProUGUI _playerScore;
    private int min = 5;
    private int sec = 0;
    float totalTime = 361; // seconds
    float timeLeft;
    bool finished = false;
    [Header("How long should game wait for 3,2,1,go animation.")]
    [SerializeField] private float _startTime;

    public static event Action GameFinishedEvent; // subscribe this to finish game.
    
    private void Awake()
    {
        _playerScore.SetText("-:-");
        StartCoroutine(ControlPhysics(_startTime));
    }

    void Start()
    {
        timeLeft = totalTime;
        finished = false;
        FoodCollision.TimeIncreasedEvent += IncreaseTimeleft;
    }
    
    void Update()
    {
        if(Physics.autoSimulation)
        {
            timeLeft -= Time.deltaTime;
        }


        if (!finished)
        {
            if (timeLeft <= 0f)
            {
                _playerScore.color = new Color32(234, 53, 32, 255); // red
                _playerScore.SetText("00:00");
                finished = true;
                GameFinishedEvent?.Invoke();
            }
            else
            {
                _playerScore.SetText(Mathf.Floor(timeLeft / 60).ToString("00") +
                    ":" +
                  Mathf.Floor(timeLeft % 60).ToString("00"));

                if (timeLeft > totalTime / 2)
                {
                    _playerScore.color = new Color32(63, 210, 35, 255); // green
                }
                else if (timeLeft < totalTime / 5)
                {
                    _playerScore.color = new Color32(234, 53, 32, 255); // red
                }
                else
                {
                    _playerScore.color = new Color32(233, 154, 32, 255); // orange
                }
            }
        }
    }
    public void IncreaseTimeleft(float time)
    {
        timeLeft += time;
    }
    public IEnumerator ControlPhysics(float time)
    {
        Physics.autoSimulation = false;
        yield return new WaitForSeconds(time);
        Physics.autoSimulation = true;
    }
    private void OnDestroy()
    {
        FoodCollision.TimeIncreasedEvent -= IncreaseTimeleft;
    }
}
