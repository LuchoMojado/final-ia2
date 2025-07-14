using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //[SerializeField] public Grid currentGraph;
    public Agent agent;
    public Enemy enemy;
    
    [HideInInspector] public List<Node> allNodes;

    public static GameManager instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            agent.EquipBow();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            agent.EquipDagger();
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            agent.BowAttack();
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

    
}
