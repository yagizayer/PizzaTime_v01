using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine.UI;
using UnityEngine;
using System;

public class Car
{
    public PositionCell CarPosition;
    public CarMotions CarMotion;
    public Car(PositionCell carPosition, CarMotions carMotion)
    {
        CarPosition = carPosition;
        CarMotion = carMotion;
    }
}


public class CarsManager : MonoBehaviour
{
    public float CarSpawnRate = 2;
    public Orientation CurrentMotion = Orientation.Vertical;
    public bool OrientationActive = false;
    [SerializeField, Range(.01f, 1)] private float _fickerEffectDuration = .1f;
    private CarSpawnerDict _spawners;
    private GameManager _gameManager;
    private EventManager _eventManager;
    private PlayerManager _playerManager;
    [HideInInspector]
    public List<Car> CurrentCars = new List<Car>();
    private SerializableDictionaryBase<CarMotions, Sprite> _carSprites = new SerializableDictionaryBase<CarMotions, Sprite>();

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _spawners = _gameManager.CarSpawners;
        _playerManager = _gameManager.GamePlayerManager;
        _eventManager = _gameManager.GameEventManager;

        if (_gameManager.IsGameRunning)
            StartCoroutine(SpawnCar());

        _carSprites[CarMotions.LeftToRight] = _gameManager.SpriteDatabase[AllSprites.CarHorizontalToRight];
        _carSprites[CarMotions.RightToLeft] = _gameManager.SpriteDatabase[AllSprites.CarHorizontalToLeft];
        _carSprites[CarMotions.FrontToBack] = _gameManager.SpriteDatabase[AllSprites.CarVerticalToBack];
        _carSprites[CarMotions.BackToFront] = _gameManager.SpriteDatabase[AllSprites.CarVerticalToFront];

        if (CarSpawnRate % _gameManager.TimeStep == 0)
        {
            Debug.LogWarning($"Car Spawn rate({CarSpawnRate}) is multpile of Timestep({_gameManager.TimeStep}), this will cause cars rapidly proceeding to second cell, giving player no room for dodge.\n Changing Car Spawn rate to : {CarSpawnRate - .01f}");
            CarSpawnRate = CarSpawnRate - .01f;
        }


    }

    private IEnumerator SpawnCar()
    {
        while (_gameManager.IsGameRunning)
        {
            yield return new WaitForSecondsRealtime(CarSpawnRate);
            Car spawnerCell = SelectRandomSpawner();
            CurrentCars.Add(spawnerCell);
            ShowCar(spawnerCell.CarPosition, _carSprites[spawnerCell.CarMotion]);
        }
    }

    public void StopCarSpawning()
    {
        StopCoroutine(SpawnCar());
    }

    public void ProceedCars()
    {
        List<Car> carsToRemove = new List<Car>();
        for (int i = 0; i < CurrentCars.Count; i++)
        {
            Car car = CurrentCars[i];
            CellDirection direction = CellDirection.Right; // placeholder

            if (car.CarMotion == CarMotions.LeftToRight) direction = CellDirection.Right; // to right
            if (car.CarMotion == CarMotions.RightToLeft) direction = CellDirection.Left; // to left
            if (car.CarMotion == CarMotions.FrontToBack) direction = CellDirection.Next; // to top
            if (car.CarMotion == CarMotions.BackToFront) direction = CellDirection.Previous; // to bottom

            if (car.CarPosition.GetNeighbor(direction))
            {
                // not end of the road yet
                HideCar(car.CarPosition);
                car.CarPosition = car.CarPosition.GetNeighbor(direction);
                ShowCar(car.CarPosition, _carSprites[car.CarMotion]);
                StartCoroutine(_gameManager.CheckCollisionLater());
            }
            else
            {
                // end of the road
                HideCar(car.CarPosition);
                carsToRemove.Add(car);
            }
            CurrentCars[i] = car;
        }
        foreach (Car car in carsToRemove)
            CurrentCars.Remove(car);
    }


    //------------------------

    private Car SelectRandomSpawner()
    {
        System.Random r = new System.Random();
        int haltControl = 0;
        while (true)
        {
            if (haltControl++ == 500)
            {
                Debug.LogError("There is a problem");
                Application.Quit();
            }
            int selectedSpawnerIndex = r.Next(0, _spawners.Count);
            int counter = 0;
            foreach (KeyValuePair<Transform, CarMotions> spawner in _spawners)
            {
                if (selectedSpawnerIndex == counter)
                {
                    // orientation control
                    if (OrientationActive)
                    {
                        if (CurrentMotion == Orientation.Vertical)
                            if (spawner.Value == CarMotions.LeftToRight || spawner.Value == CarMotions.RightToLeft) continue;
                        if (CurrentMotion == Orientation.Horizontal)
                            if (spawner.Value == CarMotions.FrontToBack || spawner.Value == CarMotions.BackToFront) continue;
                    }

                    return new Car(spawner.Key.GetComponent<PositionCell>(), spawner.Value);
                }
                counter++;
            }
        }
    }
    private void ShowCar(PositionCell targetCell, Sprite sprite)
    {
        targetCell.MyImage.sprite = sprite;
        targetCell.MyImage.enabled = true;
        targetCell.MyImage.preserveAspect = true;
    }
    public void HideCar(PositionCell targetCell)
    {
        StartCoroutine(HidingCar(targetCell));
    }

    private IEnumerator HidingCar(PositionCell targetCell)
    {
        yield return new WaitForSecondsRealtime(_fickerEffectDuration);
        targetCell.MyImage.sprite = null;
        targetCell.MyImage.enabled = false;
        targetCell.MyImage.preserveAspect = false;
    }
}
