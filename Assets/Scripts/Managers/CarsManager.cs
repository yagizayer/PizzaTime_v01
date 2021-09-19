/// <summary>
/// This file used for managing all cars movements
/// </summary>
using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

public class Car
{
    public PositionCell CarPosition;
    public CarMotions CarMotion;
    public int SpawnTick;
    public float SpawnTime;
    public Car(PositionCell carPosition, CarMotions carMotion, int spawnTick, float spawnTime = 0)
    {
        CarPosition = carPosition;
        CarMotion = carMotion;
        SpawnTick = spawnTick;
        SpawnTime = spawnTime;
    }
}


public class CarsManager : MonoBehaviour
{
    public float CarSpawnRate = 2;
    public Orientation CurrentMotion = Orientation.Vertical;
    public bool OrientationActive = false;
    public List<Car> CurrentCars = new List<Car>();
    [SerializeField, Range(.01f, 1)] private float _fickerEffectDuration = .1f;
    [SerializeField] private Transform _leftSpawner;
    [SerializeField] private Transform _rightSpawner;
    [SerializeField] private Transform _topLeftSpawner;
    [SerializeField] private Transform _topRightSpawner;


    private CarSpawnerDict _spawners;
    private float _currentCooldown; // in tickCount
    private int _tickCount = 0;

    private GameManager _gameManager;
    private EventManager _eventManager;
    private PlayerManager _playerManager;
    [HideInInspector]
    private SerializableDictionaryBase<CarMotions, Sprite> _carSprites = new SerializableDictionaryBase<CarMotions, Sprite>();

    public SerializableDictionaryBase<CarMotions, Sprite> CarSprites { get => _carSprites; private set => _carSprites = value; }


    //------------------------    

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _spawners = _gameManager.CarSpawners;
        _playerManager = _gameManager.GamePlayerManager;
        _eventManager = _gameManager.GameEventManager;

        _carSprites[CarMotions.LeftToRight] = _gameManager.SpriteDatabase[AllSprites.CarHorizontalToRight];
        _carSprites[CarMotions.RightToLeft] = _gameManager.SpriteDatabase[AllSprites.CarHorizontalToLeft];
        _carSprites[CarMotions.FrontToBack] = _gameManager.SpriteDatabase[AllSprites.CarVerticalToBack];
        _carSprites[CarMotions.BackToFront] = _gameManager.SpriteDatabase[AllSprites.CarVerticalToFront];

        if (CarSpawnRate % _gameManager.TimeStepStartValue == 0)
        {
            Debug.LogWarning($"Car Spawn rate({CarSpawnRate}) is multpile of Timestep({_gameManager.TimeStepStartValue}), this will cause cars rapidly proceeding to second cell, giving player no room for dodge.\n Changing Car Spawn rate to : {CarSpawnRate - .01f}");
            CarSpawnRate = CarSpawnRate - .01f;
        }
        StartCoroutine(SpawningCar());
    }

    /// <summary>
    /// Spawns Cars in a determined Time span
    /// </summary>
    private IEnumerator SpawningCar()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(CarSpawnRate * _gameManager.TimestepPercentage());
            if (_gameManager.CurrentGameState != GameState.Started) continue;
            SpawnCar();


        }
    }

    /// <summary>
    /// Moves Cars on the scene
    /// </summary>
    public void ProceedCars()
    {
        if (_gameManager.CurrentGameState != GameState.Started)
            return;

        List<Car> carsToRemove = new List<Car>();
        for (int i = 0; i < CurrentCars.Count; i++)
        {
            Car car = CurrentCars[i];
            CellDirection direction = CellDirection.Right; // placeholder
            if (Time.time - car.SpawnTime < .5f) continue;

            if (car.CarMotion == CarMotions.LeftToRight) direction = CellDirection.Right; // to right
            if (car.CarMotion == CarMotions.RightToLeft) direction = CellDirection.Left; // to left
            if (car.CarMotion == CarMotions.FrontToBack) direction = CellDirection.Next; // to top
            if (car.CarMotion == CarMotions.BackToFront) direction = CellDirection.Previous; // to bottom

            PositionCell targetCell = car.CarPosition.GetNeighbor(direction);
            if (targetCell)
            {
                // not end of the road yet

                // orientation control
                if (car.CarMotion == CarMotions.LeftToRight || car.CarMotion == CarMotions.RightToLeft)
                    if (CurrentMotion == Orientation.Vertical)
                    {
                        // Show side car when cars movements are vertical
                        ShowCar(car.CarPosition, _carSprites[car.CarMotion]);
                        continue;
                    }
                    else
                    {
                        // Remove Side cars when Cars movements are horizontal
                        if (car.SpawnTick + 3 < _tickCount)// remove in 3 tick
                        {
                            UnblockRoads(car.CarPosition);
                            HideCar(car.CarPosition);
                            carsToRemove.Add(car);
                        }
                        continue;
                    }

                // proceeding part
                // test_2 
                if (_gameManager.GamePlayerManager.PlayerCell == targetCell)
                    StartCoroutine(_gameManager.CheckCollisionLater(true));
                else
                {
                    if (targetCell.TeleportCar)
                    {
                        PositionCell teleportCellPos = targetCell.GetNeighbor(direction);
                        if (teleportCellPos)
                        {
                            HideCar(car.CarPosition);
                            car.CarPosition = teleportCellPos;
                            ShowCar(car.CarPosition, _carSprites[car.CarMotion]);
                            if (_gameManager.GamePlayerManager.PlayerCell == car.CarPosition)
                                StartCoroutine(_gameManager.CheckCollisionLater(true));
                        }
                        else
                        {
                            // teleport to end of road
                            HideCar(car.CarPosition);
                            carsToRemove.Add(car);
                        }
                    }
                    else
                    {
                        HideCar(car.CarPosition);
                        car.CarPosition = targetCell;
                        ShowCar(car.CarPosition, _carSprites[car.CarMotion]);
                        if (_gameManager.GamePlayerManager.PlayerCell == car.CarPosition)
                            StartCoroutine(_gameManager.CheckCollisionLater(true));
                    }
                }
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

    /// <summary>
    /// Spawns Cars
    /// </summary>
    private void SpawnCar()
    {
        List<Car> result = new List<Car>();

        // Spawn Vertical Car
        // is totalPoint below threshold
        if (_gameManager.TotalPoint % _gameManager.SpeedResetThreshold <= _gameManager.DoubleCarSpawnThreshold)
        {
            // normally select between 3 choices
            System.Random r = new System.Random();
            int verticalCarType = r.Next(3);
            /*
                1- left spawner
                2- right spawner
                3- both spawners
            */
            if (verticalCarType == 1)
            {
                result.Add(new Car(_topLeftSpawner.GetComponent<PositionCell>(), CarMotions.BackToFront, _tickCount, Time.time));
            }
            if (verticalCarType == 2)
            {
                result.Add(new Car(_topRightSpawner.GetComponent<PositionCell>(), CarMotions.BackToFront, _tickCount, Time.time));
            }
            if (verticalCarType == 3)
            {
                result.Add(new Car(_topLeftSpawner.GetComponent<PositionCell>(), CarMotions.BackToFront, _tickCount, Time.time));
                result.Add(new Car(_topRightSpawner.GetComponent<PositionCell>(), CarMotions.BackToFront, _tickCount, Time.time));
            }
        }
        else
        {
            result.Add(new Car(_topLeftSpawner.GetComponent<PositionCell>(), CarMotions.BackToFront, _tickCount, Time.time));
            result.Add(new Car(_topRightSpawner.GetComponent<PositionCell>(), CarMotions.BackToFront, _tickCount, Time.time));
        }

        // Spawn Horizontal Car
        if (CurrentMotion == Orientation.Horizontal)
        {
            List<(CarMotions, Transform)> horizontalSpawners = new List<(CarMotions, Transform)>(){
                (CarMotions.LeftToRight,_leftSpawner),
                (CarMotions.RightToLeft,_rightSpawner)
            };
            (CarMotions, Transform) randomHorizontalSpawner = horizontalSpawners.Choice();

            result.Add(new Car(randomHorizontalSpawner.Item2.GetComponent<PositionCell>(), randomHorizontalSpawner.Item1, _tickCount, Time.time));
            BlockRoad(randomHorizontalSpawner.Item2.GetComponent<PositionCell>());
        }
        foreach (Car car in result)
        {
            CurrentCars.Add(car);
            ShowCar(car.CarPosition, _carSprites[car.CarMotion]);
        }
    }

    /// <summary>
    /// Shows car object in scene
    /// </summary>
    /// <param name="targetCell">Car Position</param>
    /// <param name="sprite">related Sprite</param>
    public void ShowCar(PositionCell targetCell, Sprite sprite)
    {
        targetCell.MyImage.sprite = sprite;
        targetCell.MyImage.enabled = true;
        targetCell.MyImage.preserveAspect = true;
    }

    /// <summary>
    /// Hides car object in scene
    /// </summary>
    /// <param name="targetCell">Car Position</param>
    public void HideCar(PositionCell targetCell)
    {
        StartCoroutine(HidingCar(targetCell));
    }

    /// <summary>
    /// Hides car object in scene
    /// </summary>
    /// <param name="targetCell">Car Position</param>
    private IEnumerator HidingCar(PositionCell targetCell)
    {
        yield return new WaitForSecondsRealtime(_fickerEffectDuration);
        targetCell.MyImage.sprite = null;
        targetCell.MyImage.enabled = false;
        targetCell.MyImage.preserveAspect = false;
    }

    /// <summary>
    /// Change current orientation of road
    /// </summary>
    public void ChangeDirection()
    {
        UnblockRoads();
        if (CurrentMotion == Orientation.Vertical)
        {
            CurrentMotion = Orientation.Horizontal;
        }
        else
        {
            CurrentMotion = Orientation.Vertical;
            List<Car> carsToRemove = new List<Car>();
            foreach (Car car in CurrentCars)
                if (car.CarMotion == CarMotions.LeftToRight || car.CarMotion == CarMotions.RightToLeft)
                    carsToRemove.Add(car);
            foreach (Car car in carsToRemove)
            {
                HideCar(car.CarPosition);
                CurrentCars.Remove(car);
            }
            if (_gameManager.Mode == GameMode.B)
                SpawnSideCar();
        }

    }

    /// <summary>
    /// Spawns a Side Car 
    /// </summary>
    private void SpawnSideCar()
    {
        // randomly decide left or right
        (Transform, CarMotions) tempValues = UnityEngine.Random.Range(0, 1) < .5f ? (_leftSpawner, CarMotions.LeftToRight) : (_rightSpawner, CarMotions.RightToLeft);

        Car tempCar = new Car(tempValues.Item1.GetComponent<PositionCell>(), tempValues.Item2, _tickCount);
        CurrentCars.Add(tempCar);
        ShowCar(tempCar.CarPosition, _carSprites[tempValues.Item2]);

        BlockRoad(tempCar.CarPosition);
    }

    /// <summary>
    /// Blocks road for going up or downside of game area
    /// </summary>
    /// <param name="carCell">Blocking Cars position</param>
    private void BlockRoad(PositionCell carCell)
    {
        PositionCell carTopCell = carCell.GetNeighbor(CellDirection.Next);
        PositionCell carBottomCell = carCell.GetNeighbor(CellDirection.Previous);

        if (carTopCell) carTopCell.ForbidPlayerOnInput = CellDirection.Previous;
        if (carBottomCell) carBottomCell.ForbidPlayerOnInput = CellDirection.Next;
    }

    /// <summary>
    /// Removes all Blocking cars from scene and opens roads to player
    /// </summary>
    private void UnblockRoads()
    {
        PositionCell leftSpawnerCell = _leftSpawner.GetComponent<PositionCell>();
        PositionCell rightSpawnerCell = _rightSpawner.GetComponent<PositionCell>();

        PositionCell leftTopCell = leftSpawnerCell.GetNeighbor(CellDirection.Next);
        PositionCell rightTopCell = rightSpawnerCell.GetNeighbor(CellDirection.Next);
        PositionCell leftBottomCell = leftSpawnerCell.GetNeighbor(CellDirection.Previous);
        PositionCell rightBottomCell = rightSpawnerCell.GetNeighbor(CellDirection.Previous);

        if (leftTopCell) leftTopCell.ForbidPlayerOnInput = CellDirection.Null;
        if (rightTopCell) rightTopCell.ForbidPlayerOnInput = CellDirection.Null;
        if (leftBottomCell) leftBottomCell.ForbidPlayerOnInput = CellDirection.Null;
        if (rightBottomCell) rightBottomCell.ForbidPlayerOnInput = CellDirection.Null;
    }

    /// <summary>
    /// Removes given Blocking car from scene and opens roads to player
    /// </summary>
    /// <param name="carCell">Blocking Cars position</param>
    private void UnblockRoads(PositionCell carCell)
    {
        PositionCell carTopCell = carCell.GetNeighbor(CellDirection.Next);
        PositionCell carBottomCell = carCell.GetNeighbor(CellDirection.Previous);

        if (carTopCell) carTopCell.ForbidPlayerOnInput = CellDirection.Null;
        if (carBottomCell) carBottomCell.ForbidPlayerOnInput = CellDirection.Null;
    }

    /// <summary>
    /// counts every tick
    /// </summary>
    public void IncreaseTickCounter() => _tickCount++;

    /// <summary>
    /// Remove all cars from scene
    /// </summary>
    public void ClearAllCars()
    {
        foreach (Car car in CurrentCars)
            HideCar(car.CarPosition);

        CurrentCars.Clear();
    }
}
