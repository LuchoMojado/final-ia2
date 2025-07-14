using System;
using System.Collections;
using System.Collections.Generic;
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

    int _gridWidth, _gridLength;

    Action _clickCheck;

    GameManager gm;

    private void Start()
    {
        gm = GameManager.instance;

        StartCoroutine(SetUpWorld());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _clickCheck != null) _clickCheck();
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

        if (_gridLength != 0) _continueButton.interactable = true;
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

        if (_gridWidth != 0) _continueButton.interactable = true;
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
        
        gm.agent.SetCurrentNode(node);

        _continueButton.interactable = true;
    }

    void ChangeEnemyNode(Node node)
    {
        if (node == null || node.isBlocked || node.isTaken) return;
        
        gm.enemy.SetNode(node);

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

    IEnumerator SetUpWorld()
    {
        _mapSize.SetActive(true);

        yield return new WaitUntil(() => _continue);

        _grid.GenerateGrid(_gridWidth, _gridLength);

        Camera.main.transform.position += new Vector3(_gridWidth, 0); 

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

        _continue = false;
        _continueButton.interactable = false;

        _placeArrows.SetActive(false);
        _characterState.SetActive(true);

        yield return new WaitUntil(() => _continue);

        _characterState.SetActive(false);
    }
}
