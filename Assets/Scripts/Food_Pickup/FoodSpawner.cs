using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [Header("Ground and Walls")]
    [SerializeField]
    private GameObject _spawnArea;
    [SerializeField]
    private bool _createInvisibleWalls = false;
    [SerializeField] private GameObject _wallPrefab;
    private GameObject[] _walls = new GameObject[4];
    [SerializeField] private GameObject _wallContainer;
    [Header("Spawn Object")]
    [SerializeField]
    private GameObject _spawnObject;
    [SerializeField]
    private int _initialSpawnAmount;
    [SerializeField]
    private int _sustainedSpawnAmount;
    [SerializeField]
    private int _currentSpawnCount;

    private Vector3 _spawnPosition;
    private GameObject _objectContainer;



    private float Xoffset, Yoffset, Zoffset;

    private void Awake()
    {
        //Add food container as a child object to this object to keep track of foods.
        _objectContainer = new GameObject("ObjectContainer");
        _objectContainer.transform.parent = this.transform;
        
    }
    // Start is called before the first frame update
    void Start()
    {

        //offset for each different spawn area object.
        Yoffset = (_spawnArea.transform.localScale.y / 2f);
        Xoffset = (_spawnArea.transform.localScale.x / 2f);
        Zoffset = (_spawnArea.transform.localScale.z / 2f);

        for (int i = 0; i < _initialSpawnAmount; i++)
        {
            //Select Spawn position        
            _spawnPosition = _spawnArea.transform.position +
                new Vector3(Random.Range(0f, _spawnArea.transform.localScale.x) - Xoffset,
                (_spawnObject.transform.localScale.y / 2) + Yoffset,
                Random.Range(0f, _spawnArea.transform.localScale.z) - Zoffset);

            //spawnla
            GameObject obj = Instantiate(_spawnObject, _spawnPosition, Quaternion.identity);

            //add all foods to food container.
            switch(_spawnObject.name)
            {
                case "Food":
                    obj.name = (i + 1) + ".Food";
                    _objectContainer.name = "FoodContainer";
                    break;
                case "Fuze_Pickup_Parent":
                    obj.name = (i + 1) + ".FuzePickup";
                    _objectContainer.name = "MissilePickupContainer";
                    break;
                case "Time_Pickup_Parent":
                    obj.name = (i + 1) + ".TimePickup";
                    _objectContainer.name = "TimePickupContainer";
                    break;
                case "SmallBronzeCoin":
                    obj.name = (i + 1) + ".BronzeCoin";
                    _objectContainer.name = "BronzeCoinContainer";
                    break;
                case "SmallSilverCoin":
                    obj.name = (i + 1) + ".SilverCoin";
                    _objectContainer.name = "SilverCoinContainer";
                    break;
                case "SmallGoldCoin":
                    obj.name = (i + 1) + ".GoldCoin";
                    _objectContainer.name = "GoldCoinContainer";
                    break;
            }
            obj.transform.parent = _objectContainer.transform;


        }

        //Create walls;
        if (_createInvisibleWalls) 
        {
            CreateInvisibleWalls();
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

    private void CreateInvisibleWalls()
    {

        for (int i = 0; i < _walls.Length; i++)
        {
            _walls[i] = Instantiate(_wallPrefab, _wallContainer.transform);
            _walls[i].transform.localScale = _spawnArea.transform.localScale;
        }
        _walls[0].transform.localRotation = Quaternion.Euler(90, 0, 180);
        _walls[1].transform.localRotation = Quaternion.Euler(90, 0, 0);
        _walls[2].transform.localRotation = Quaternion.Euler(90, 0, 90);
        _walls[3].transform.localRotation = Quaternion.Euler(90, 0, -90);
        _walls[0].transform.position = new Vector3(0, 0, (Yoffset + Zoffset));
        _walls[1].transform.position = new Vector3(0, 0, (Yoffset + Zoffset) * -1f);
        _walls[2].transform.position = new Vector3((Yoffset + Zoffset), 0, 0);
        _walls[3].transform.position = new Vector3((Yoffset + Zoffset) * -1f, 0, 0);
    }
    private void SpawnObject()
    {
        //Select Spawn position        
        _spawnPosition = _spawnArea.transform.position +
            new Vector3(Random.Range(0f, _spawnArea.transform.localScale.x) - Xoffset,
            (_spawnObject.transform.localScale.y / 2) + Yoffset,
            Random.Range(0f, _spawnArea.transform.localScale.z) - Zoffset);

        //spawnla
        GameObject obj = Instantiate(_spawnObject, _spawnPosition, Quaternion.identity);


        //add all foods to food container.
        obj.transform.parent = _objectContainer.transform;
    }
 
}
