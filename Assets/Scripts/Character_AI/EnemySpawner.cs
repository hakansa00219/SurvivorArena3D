using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Header("Ground and Walls")]
    [SerializeField]
    private GameObject _spawnArea;
    [Header("Spawn Object")]
    [SerializeField]
    private Enemy _enemyObject;
    [SerializeField]
    private int _currentSpawnCount;

    private string[] enemyNames = new string[]
    {
        "Hakan","Evillious Energy","Duke of Doom","Eat Bullets","Monstrous Michel","Collateral Damage",
        "Gabriel Groomer","Shoot 2 Kill","Annihilator","Veteran of Death","Walk Alone Bravely",
        "Tonight Gamer","Walking Pegasus","Dead Deal","Killer Romeo","Overkill","Best Bluster",
        "Brute Fact","Green Ghost","Bloss Flop","Terrific Tornado","Tango Boss","Accurate Arrow",
        "Kill Spree","Optimal Aces","Dark Knight","Inimical Thugs","Knuckle Duster","Local Back Stabber",
        "Happy Killing","Fear Butchers","Guncap Slingbad","Highlander Monk","Left Divide","Jack The Ripper",
        "Hog Butcher","Psychedelic Servicemen","Militaristic Fighting Machine","Keen Team Six",
        "Junkyard Dog","Fuzzy Pack","Straight Gangsters","Mortified Coercion","Lyrical Armed Services",
        "Outrageous Dominance","Brash Thugs","Complex Slayers","Faulty Devils","Odd Hooligans","Organic Punks",
        "Hungry Admirals","Cloudy Perpetrator","Keen Team Six","PUBJESUS","Inimical Thugs",
        "Agent Hercules","Metal Star","Night Magnet","Headshooter","Optimal Aces","Alpha Returns","Dancing Madman",
        "Abnormal Vigor","Ball Blaster","Kill Switch","Pixie Soldier","Pro Headshot","Grave Digger",
        "Cool Shooter","Dead Shot","Thunderbeast","Bad soldier","Local Grim Reaper","Captain Jack Sparrow",
        "Outrageous Dominance","Agent 47","Militaristic Fighting Machine","PUBGian","Lyrical Armed Services",
        "Quarrelsome Strategy","Hungry Admirals","Cloudy Perpetrator","Fuzzy Pack","Straight Gangsters","Psychedelic Servicemen",
        "Homely Sharpshooters","Plain Privileg","Annoyed Power","Demonic Criminals","Fear Butchers","Left Divide",
        "Spicy Senorita","Girl Royale","Blade Woman","Giggle Fluff","Candy Cough","Dexterous Queen","Panda Heart",
        "Princess Pickney","Magic Peach","Tiger Kitty","Lady Killer","Dangerous Damsel","Koi Diva","PubgPie",
        "Auspicious Olivia","Luna Star","Ancient Ambrosia","Curious Caroline","Crazy Cinderella","Tragic Girl",
        "Girls of Neptune","Broken Paws","Anonymous Girl","Tiny Hunter","Leading Light","Acid Queen",
        "Video Game Heroine","Cool Whip","Claudia Clouds","Princess of PUBG","Gun Digger","TeKilla Sunrise",
        "Little Drunk Girl","Digital Goddess","Peanut Butter Woman","Sleek Assassin","Treasure Devil",
        "Lady Fantastic","Opulent Gamer","Wildcat Talent","Pink Nightmare","Miss Fix It","Feral Filly",
        "Troubled Chick","Freeze Queen","Eye Candy Kitten","Romance Princess","Titanium Ladybug",
        "Emerald Goddess","Marshmallow Treat","Queen Bee","Microwave Chardonnay","Gamer Bean","Mafia Princess",
        "Woodland Beauty","Darkside Hooker","Saturn Extreme","Battle Mistress","Sassy Muffin","Canary Apple Red"
    };

    [Range(0, 150)] // enemynames size
    [SerializeField]
    private int _initialSpawnAmount;
    [Range(0, 150)] // enemynames size
    [SerializeField]
    private int _sustainedSpawnAmount;

    private Vector3 _spawnPosition;
    private GameObject _objectContainer;
    private GameObject[] _walls;

    private float Xoffset, Yoffset, Zoffset;

    private Queue<string> deadEnemyNames;

    private void Awake()
    {
        //Add food container as a child object to this object to keep track of foods.
        _objectContainer = new GameObject("EnemyContainer");
        _objectContainer.transform.parent = this.transform;
        deadEnemyNames = new Queue<string>();
        Enemy.IamDeadEvent += EnemyIsDead;
    }
    // Start is called before the first frame update
    void Start()
    {
        //offset for each different spawn area object.
        Yoffset = (_spawnArea.transform.localScale.y / 2f);
        Xoffset = (_spawnArea.transform.localScale.x / 2f);
        Zoffset = (_spawnArea.transform.localScale.z / 2f);
        
        List<int> idxList = GetRandomNName();
        
        for (int i = 0; i < _initialSpawnAmount; i++)
        {
            //Select Spawn position        
            _spawnPosition = _spawnArea.transform.position +
                new Vector3(Random.Range(0f, _spawnArea.transform.localScale.x) - Xoffset,
                (_enemyObject.transform.localScale.y / 2) + Yoffset,
                Random.Range(0f, _spawnArea.transform.localScale.z) - Zoffset);

            //spawnla
            Enemy obj = Instantiate(_enemyObject, _spawnPosition, Quaternion.identity);
            
            obj.name = enemyNames[idxList[i]];
            obj.transform.parent = _objectContainer.transform;
        }
    }

    private void Update()
    {
        //food count tracker
        _currentSpawnCount = _objectContainer.transform.childCount;
        if (_currentSpawnCount < _sustainedSpawnAmount)
        {
            SpawnObject();
        }
    }
    
    private void SpawnObject()
    {
        for(int i = 0; i < _sustainedSpawnAmount - _currentSpawnCount; i++)
        {
            //Select Spawn position        
            _spawnPosition = _spawnArea.transform.position +
                new Vector3(Random.Range(0f, _spawnArea.transform.localScale.x) - Xoffset,
                (_enemyObject.transform.localScale.y / 2) + Yoffset,
                Random.Range(0f, _spawnArea.transform.localScale.z) - Zoffset);
            Enemy obj = Instantiate(_enemyObject, _spawnPosition, Quaternion.identity);
            obj.name = deadEnemyNames.Dequeue();
            obj.transform.parent = _objectContainer.transform;
        }   
    }

    private void EnemyIsDead(string enemyName)
    {
        deadEnemyNames.Enqueue(enemyName);
    }

    private List<int> GetRandomNName()
    {
        List<int> indexList = new List<int>();
        List<int> choosenIndexes = new List<int>();
        int idx = Random.Range(0, enemyNames.Length);

        do
        {
            while (choosenIndexes.Contains(idx))
            {
                idx = Random.Range(0, enemyNames.Length);
            }
            indexList.Add(idx);
            choosenIndexes.Add(idx);

        } while (indexList.Count < _initialSpawnAmount);

        return indexList;
    }
    private void OnDestroy()
    {
        Enemy.IamDeadEvent -= EnemyIsDead;
    }
}
