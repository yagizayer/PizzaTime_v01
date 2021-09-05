using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private EventManager _eventManager;
    private GameManager _gameManager;
    private List<MenuSelectable> _allSelectables = new List<MenuSelectable>();
    private (RectTransform, MenuSelectable) _currentSelection;
    [SerializeField] private RectTransform _indicator;
    [SerializeField] private Vector2 _indicatorOffset = new Vector2(0, -30);

    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _eventManager = _gameManager.GameEventManager;
        if(_gameManager.CurrentGameState == GameState.Ended)
            InitializeMenu();
    }

    void Update()
    {
        if (_gameManager.CurrentGameState == GameState.Ended)
        {
            // Menu Movements
            if (Input.GetKeyDown(KeyCode.W))
                _eventManager.InvokeMenuMovementEvent(direction: CellDirection.Next);
            if (Input.GetKeyDown(KeyCode.S))
                _eventManager.InvokeMenuMovementEvent(direction: CellDirection.Previous);
            if (Input.GetKeyDown(KeyCode.A))
                _eventManager.InvokeMenuMovementEvent(direction: CellDirection.Left);
            if (Input.GetKeyDown(KeyCode.D))
                _eventManager.InvokeMenuMovementEvent(direction: CellDirection.Right);
            if (Input.GetKeyDown(KeyCode.E))
                _eventManager.InvokeMenuSelectEvent();
        }
    }

    public void ChangeSelection(CellDirection direction)
    {
        if (_currentSelection.Item2.SelectionDirections.ContainsKey(direction) == false) return;

        _currentSelection.Item1 = _currentSelection.Item2.SelectionDirections[direction].GetComponent<RectTransform>();
        _currentSelection.Item2 = _currentSelection.Item2.SelectionDirections[direction].GetComponent<MenuSelectable>();

        MoveIndicator();
    }

    public void MoveIndicator()
    {
        _indicator.anchoredPosition = _currentSelection.Item1.anchoredPosition + _indicatorOffset;
    }
    public void SelectSelection()
    {
        _currentSelection.Item1.GetComponent<Button>().onClick.Invoke();
    }

    public void InitializeMenu()
    {
        foreach (MenuSelectable item in FindObjectsOfType<MenuSelectable>())
        {
            if (item.SelectionDefault) _currentSelection = (item.GetComponent<RectTransform>(), item);
            _allSelectables.Add(item);
        }
        MoveIndicator();
    }
}
