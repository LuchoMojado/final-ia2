using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class WorldCreator : MonoBehaviour
{
    [SerializeField] GameObject _mapSize, _addObstacles, _placeCharacter, _placeEnemy, _placeArrows, _characterState;
    [SerializeField] Button _continueButton;
    [SerializeField] TMP_InputField _widthInput, _lengthInput;

    [SerializeField] Grid _grid;
    [SerializeField] Arrow _arrowPrefab;

    [SerializeField] int _sizeCap;

    WorldState _initialState;

    bool _continue = false;

    int _gridWidth = 5, _gridLength = 5;

    Action _clickCheck;

    GameManager _gm;
    GOAPManager _goapManager;

    private void Start()
    {
        _gm = GameManager.instance;

        _goapManager = new GOAPManager(_gm);

        _initialState = new();

        _initialState.state.detected = true;
        _initialState.state.arrows = 0;
        _initialState.state.enemyHp = 50;
        _initialState.state.equippedWeapon = WeaponType.Dagger;

        StartCoroutine(SetUpWorld());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _clickCheck != null) _clickCheck();

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Space))
            if (_continueButton.interactable == true) Continue();
    }

    public void SetGridWidth(string width)
    {
        var value = int.Parse(width);

        if (value < 1)
        {
            value = 1;

            _widthInput.SetTextWithoutNotify("1");
        }
        else if (value > _sizeCap)
        {
            value = _sizeCap;

            _widthInput.SetTextWithoutNotify(_sizeCap.ToString());
        }

        _gridWidth = value;
    }

    public void SetGridLength(string length)
    {
        var value = int.Parse(length);

        if (value < 1)
        {
            value = 1;

            _lengthInput.SetTextWithoutNotify("1");
        }
        else if (value > _sizeCap)
        {
            value = _sizeCap;

            _lengthInput.SetTextWithoutNotify(_sizeCap.ToString());
        }

        _gridLength = value;
    }

    public void Continue()
    {
        _continue = true;
    }

    void ObstacleClick()
    {
        Node node = GetNodeOnCursor();
        node?.SetBlock(!node.isBlocked);
    }

    void CharacterClick()
    {
        ChangeAgentNode(GetNodeOnCursor());
    }

    void EnemyClick()
    {
        ChangeEnemyNode(GetNodeOnCursor());
    }

    void ArrowClick()
    {
        ToggleArrowNode(GetNodeOnCursor());
    }

    void ChangeAgentNode(Node node)
    {
        if (node == null || node.isBlocked) return;
        
        _gm.agent.SetStartingNode(node);

        _continueButton.interactable = true;
    }

    void ChangeEnemyNode(Node node)
    {
        if (node == null || node.isBlocked || node.isTaken) return;
        
        _gm.enemy.SetNode(node);

        _continueButton.interactable = true;
    }

    void ToggleArrowNode(Node node)
    {
        if (node == null || node.isBlocked || node.isTaken) return;

        if (node.arrow != null)
        {
            Destroy(node.arrow.gameObject);
            node.arrow = null;
        }
        else
        {
            node.arrow = Instantiate(_arrowPrefab);
            node.arrow.transform.position = node.othersPos.position;
        }
    }

    public GameObject GetObjectOnCursor()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity))
        {
            return hit.collider.gameObject;
        }

        return default;
    }

    Node GetNodeOnCursor()
    {
        return GetObjectOnCursor()?.GetComponent<Node>();
    }

    public void SetStartingWeapon(int index)
    {
        _initialState.state.equippedWeapon = (WeaponType)index;
    }

    public void SetArrowCount(string input)
    {
        var value = int.Parse(input);

        if (value < 0)
        {
            value = 0;

            _lengthInput.SetTextWithoutNotify("0");
        }

        _initialState.state.arrows = value;
    }

    public void SetInvisibility(bool invisible)
    {
        _initialState.state.detected = !invisible;
    }

    public void SetEnemyHP(string input)
    {
        var value = int.Parse(input);

        if (value < 1)
        {
            value = 1;

            _lengthInput.SetTextWithoutNotify("1");
        }

        _initialState.state.enemyHp = value;

        if (_initialState.state.arrows != 0) _continueButton.interactable = true;
    }

    IEnumerator SetUpWorld()
    {
        _mapSize.SetActive(true);

        yield return new WaitUntil(() => _continue);

        _grid.GenerateGrid(_gridWidth, _gridLength);

        Camera.main.transform.position += new Vector3(_gridWidth * 1.05f, 0); 

        _clickCheck = ObstacleClick;

        _continue = false;

        _mapSize.SetActive(false);
        _addObstacles.SetActive(true);

        yield return new WaitUntil(() => _continue);

        _clickCheck = CharacterClick;

        _continue = false;
        _continueButton.interactable = false;

        _addObstacles.SetActive(false);
        _placeCharacter.SetActive(true);

        yield return new WaitUntil(() => _continue);

        _clickCheck = EnemyClick;

        _continue = false;
        _continueButton.interactable = false;

        _placeCharacter.SetActive(false);
        _placeEnemy.SetActive(true);

        yield return new WaitUntil(() => _continue);

        _clickCheck = ArrowClick;

        _continue = false;

        _placeEnemy.SetActive(false);
        _placeArrows.SetActive(true);

        yield return new WaitUntil(() => _continue);

        _clickCheck = null;

        _continue = false;

        _placeArrows.SetActive(false);
        _characterState.SetActive(true);

        yield return new WaitUntil(() => _continue);

        _characterState.SetActive(false);
        _continueButton.gameObject.SetActive(false);

        var arrowNodes = _gm.allNodes.Where(x => x.arrow != null);
        int count = 0;
        var enemyNode = _gm.allNodes.Where(x => x.isTaken).First();

        foreach (var node in arrowNodes)
        {
            if (_gm.agent.IsNodeAccesible(node)) count++;
        }

        _initialState.state.arrowsAvailable = count;
        _initialState.state.arrowNearby = _gm.agent.ArrowNearby();
        _initialState.state.enemyReachable = _gm.agent.IsNodeAccesible(enemyNode);
        _initialState.state.enemyNearby = _gm.agent.EnemyNearby();
        _initialState.state.canRetreat = enemyNode.neighbors.Any();

        print(_initialState.state.enemyHp);
        print(_initialState.state.arrows);
        print(_initialState.state.detected);
        print(_initialState.state.arrowsAvailable);
        print(_initialState.state.arrowNearby);
        print(_initialState.state.enemyReachable);
        print(_initialState.state.enemyNearby);
        print(_initialState.state.canRetreat);
        print(_initialState.state.equippedWeapon);

        if (_goapManager.TryGetPlan(_initialState, out var plan)) StartCoroutine(_gm.agent.ActionExecution(plan));
        else print("No se puede derrotar al enemigo");
    }
}
