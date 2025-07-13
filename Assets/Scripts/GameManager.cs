using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public Grid currentGraph;
    [SerializeField] Agent _agent;
    public Enemy enemy;

    public static GameManager instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ChangeAgentNode(GetNodeOnCursor());
        }
        if (Input.GetMouseButtonDown(1))
        {
            ChangeEnemyNode(GetNodeOnCursor());
        }
        if (Input.GetMouseButtonDown(2))
        {
            Node node = GetNodeOnCursor();
            node?.SetBlock(!node.isBlocked);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Node node = GetNodeOnCursor();
            if (node != null) _agent.Run(node);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Node node = GetNodeOnCursor();
            if (node != null) _agent.Sneak(node);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _agent.EquipBow();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _agent.EquipDagger();
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _agent.BowAttack();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            //if (_startNode != null) _pf.BFS(_startNode);
            // if (startNode != null) StartCoroutine(_pf.PaintAStar(startNode, endNode));
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    void ChangeAgentNode(Node node)
    {
        if (node == null) return;
        _agent.SetCurrentNode(node);
    }

    void ChangeEnemyNode(Node node)
    {
        if (node == null) return;
        enemy.SetNode(node);
    }

    public GameObject GetObjectOnCursor()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity))
        {
            return hit.collider.gameObject;
        }

        return default;

        /* return Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity) ? hit.collider.gameObject : default;*/
    }

    Node GetNodeOnCursor()
    {
        return GetObjectOnCursor()?.GetComponent<Node>();
    }
}
