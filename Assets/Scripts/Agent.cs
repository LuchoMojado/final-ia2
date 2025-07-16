using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Agent : MonoBehaviour
{
    [SerializeField] float _runSpeed = 3, _sneakSpeed = 1.5f;
    float _currentSpeed;

    Pathfinding _pf;
    Node _currentNode;
    [SerializeField] Arrow _arrowPrefab;

    [SerializeField] Transform _weaponSpawnPos;
    [SerializeField] GameObject _bowPrefab, _daggerPrefab;
    GameObject _currentWeaponRef;

    GameManager _gm { get => GameManager.instance; }

    bool _busy = false;

    List<Vector3> _pathToFollow = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        _pf = new Pathfinding();
    }

    void Update()
    {
        TravelThroughPath();
    }

    void SetPath(List<Vector3> path)
    {
        _pathToFollow = path;
    }

    void TravelThroughPath()
    {
        if (_pathToFollow == null || _pathToFollow.Count == 0) return;
        Vector3 posTarget = _pathToFollow[0];
        posTarget.y = transform.position.y;
        Vector3 dir = posTarget - transform.position;
        if (dir.magnitude < 0.05f)
        {
            SetPosition(posTarget);
            _pathToFollow.RemoveAt(0);

            if (_pathToFollow.Count == 0) _busy = false;
        }

        Move(dir);
    }
    void Move(Vector3 dir)
    {
        //transform.forward = dir;
        transform.LookAt(transform.position + dir, Vector3.up);
        transform.position += dir.normalized * _currentSpeed * Time.deltaTime;
    }

    void SetPosition(Vector3 pos)
    {
        pos.y = transform.position.y;
        transform.position = pos;
    }

    public void SetStartingNode(Node node)
    {
        _currentNode = node;
        SetPosition(node.characterPos.position);
    }

    public bool IsNodeAccesible(Node node)
    {
        var path = _pf.AStar(_currentNode, node);

        return path != null;
    }

    public bool ArrowNearby()
    {
        return _currentNode.arrow != null;
    }

    public bool EnemyNearby()
    {
        return _currentNode.isTaken;
    }

    public void Run(Node node)
    {
        _pathToFollow = _pf.AStar(_currentNode, node);
        _currentSpeed = _runSpeed;
        _currentNode = node;
    }

    public void Sneak(Node node)
    {
        _pathToFollow = _pf.AStar(_currentNode, node);
        _currentSpeed = _sneakSpeed;
        _currentNode = node;
    }

    public void EquipDagger()
    {
        if (_currentWeaponRef != null) Destroy(_currentWeaponRef);
        var dagger = Instantiate(_daggerPrefab, _weaponSpawnPos);
        _currentWeaponRef = dagger;
    }

    public void EquipHammer()
    {
        //if (_currentWeaponRef != null) Destroy(_currentWeaponRef);
        //var dagger = Instantiate(_daggerPrefab, _weaponSpawnPos);
        //_currentWeaponRef = dagger;
    }

    public void EquipBow()
    {
        if (_currentWeaponRef != null) Destroy(_currentWeaponRef);
        var bow = Instantiate(_bowPrefab, _weaponSpawnPos);
        _currentWeaponRef = bow;
    }
    
    public void DaggerAttack()
    {

    }

    public void HammerAttack()
    {

    }

    public void BowAttack()
    {
        var arrow = Instantiate(_arrowPrefab, transform.position, Quaternion.identity);
        arrow.Shoot(_gm.enemy.transform.position);
    }

    public void SneakyDagger()
    {

    }

    public void SneakyHammer()
    {

    }

    public void SneakyBow()
    {
        var arrow = Instantiate(_arrowPrefab, transform.position, Quaternion.identity);
        arrow.Shoot(_gm.enemy.transform.position);
    }

    public void RunToArrow()
    {
        Run(_gm.allNodes.Where(x => x.arrow != null).OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).First());
    }

    public void SneakToArrow()
    {
        Sneak(_gm.allNodes.Where(x => x.arrow != null).OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).First());
    }

    public void RunToEnemy()
    {
        Run(_gm.allNodes.Where(x => x.isTaken).First());
    }

    public void SneakToEnemy()
    {
        Sneak(_gm.allNodes.Where(x => x.isTaken).First());
    }

    public void RunFromEnemy()
    {
        Run(_gm.allNodes.Where(x => x.isTaken).First().neighbors.First());
    }

    public void PickArrow()
    {
        Destroy(_currentNode.arrow.gameObject);
        _currentNode.arrow = null;
    }

    public void TurnInvisible()
    {

    }

    public void ExecutePlan()
    {

    }

    public IEnumerator ActionExecution(List<GOAPActions> actions)
    {
        while (actions.Any())
        {
            var action = actions.First();

            action.agentBehaviour();
            print(action.Name);

            yield return new WaitForSeconds(0.25f);

            while (_busy) yield return null;

            actions.Remove(action);
        }

        print("plan completed");
    }
}
