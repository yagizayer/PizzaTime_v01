using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine.UI;
using UnityEngine;
using System;

public class CarsManager : MonoBehaviour
{
    public float CoolDown = 2;
    public Orientation _currentMotion = Orientation.Vertical;
    public bool OrientationActive = false;
    private CarSpawnerDict _spawners;
    private GameManager _gameManager;
    private EventManager _eventManager;
    private List<(PositionCell, CarMotions)> _currentCars = new List<(PositionCell, CarMotions)>();
    private SerializableDictionaryBase<CarMotions, Sprite> _carSprites = new SerializableDictionaryBase<CarMotions, Sprite>();

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _spawners = _gameManager.CarSpawners;
        _eventManager = _gameManager.GameEventManager;
        StartCoroutine(SpawnCar());
        _carSprites[CarMotions.LeftToRight] = _gameManager.SpriteDatabase[AllSprites.CarHorizontalToRight];
        _carSprites[CarMotions.RightToLeft] = _gameManager.SpriteDatabase[AllSprites.CarHorizontalToLeft];
        _carSprites[CarMotions.FrontToBack] = _gameManager.SpriteDatabase[AllSprites.CarVerticalToBack];
        _carSprites[CarMotions.BackToFront] = _gameManager.SpriteDatabase[AllSprites.CarVerticalToFront];
    }

    private IEnumerator SpawnCar()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(CoolDown);
            (PositionCell, CarMotions) spawnerCell = SelectRandomSpawner();
            _currentCars.Add(spawnerCell);
            ShowCar(spawnerCell.Item1, _carSprites[spawnerCell.Item2]);
        }
    }


    public void ProceedCars()
    {
        List<(PositionCell, CarMotions)> carsToRemove = new List<(PositionCell, CarMotions)>();
        for (int i = 0; i < _currentCars.Count; i++)
        {
            (PositionCell, CarMotions) car = _currentCars[i];
            CellDirection direction = CellDirection.Right; // placeholder

            if (car.Item2 == CarMotions.LeftToRight) direction = CellDirection.Right;
            if (car.Item2 == CarMotions.RightToLeft) direction = CellDirection.Left;
            if (car.Item2 == CarMotions.FrontToBack) direction = CellDirection.Next;
            if (car.Item2 == CarMotions.BackToFront) direction = CellDirection.Previous;

            if (car.Item1.GetNeighbor(direction))
            {
                // not end of the road yet
                HideCar(car.Item1);
                car.Item1 = car.Item1.GetNeighbor(direction);
                ShowCar(car.Item1, _carSprites[car.Item2]);
            }
            else
            {
                // end of the road
                HideCar(car.Item1);
                carsToRemove.Add(car);
            }
            _currentCars[i] = car;
        }
        foreach ((PositionCell, CarMotions) car in carsToRemove)
            _currentCars.Remove(car);
    }


    //------------------------

    private (PositionCell, CarMotions) SelectRandomSpawner()
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
                        if (_currentMotion == Orientation.Vertical)
                            if (spawner.Value == CarMotions.LeftToRight || spawner.Value == CarMotions.RightToLeft) continue;
                        if (_currentMotion == Orientation.Horizontal)
                            if (spawner.Value == CarMotions.FrontToBack || spawner.Value == CarMotions.BackToFront) continue;
                    }

                    return (spawner.Key.GetComponent<PositionCell>(), spawner.Value);
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
        targetCell.MyImage.sprite = null;
        targetCell.MyImage.enabled = false;
        targetCell.MyImage.preserveAspect = false;
    }
}
