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
    [SerializeField] GameObject _bowPrefab, _daggerPrefab, _hammerPrefab;
    [SerializeField] Animator _anim;
    [SerializeField] Renderer[] _renderers;

    GameObject _currentWeaponRef;

    GameManager _gm { get => GameManager.instance; }

    bool _busy = false;

    List<Vector3> _pathToFollow = new List<Vector3>();
    
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

            if (_pathToFollow.Count == 0)
            {
                _anim.SetBool("run", false);
                _anim.SetBool("sneakwalk", false);

                _busy = false;
            }
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
        StartCoroutine(LerpVisibility(true));

        _pathToFollow = _pf.AStar(_currentNode, node);
        _currentSpeed = _runSpeed;
        _currentNode = node;
        _anim.SetBool("run", true);
    }

    public void Sneak(Node node)
    {
        _pathToFollow = _pf.AStar(_currentNode, node);
        _currentSpeed = _sneakSpeed;
        _currentNode = node;
        _anim.SetBool("sneakwalk", true);
    }

    public void EquipDagger()
    {
        if (_currentWeaponRef != null) Destroy(_currentWeaponRef);
        var dagger = Instantiate(_daggerPrefab, _weaponSpawnPos);
        _currentWeaponRef = dagger;
        _anim.SetTrigger("switch");
    }

    public void EquipHammer()
    {
        if (_currentWeaponRef != null) Destroy(_currentWeaponRef);
        var hammer = Instantiate(_hammerPrefab, _weaponSpawnPos);
        _currentWeaponRef = hammer;
        _anim.SetTrigger("switch");
    }

    public void EquipBow()
    {
        if (_currentWeaponRef != null) Destroy(_currentWeaponRef);
        var bow = Instantiate(_bowPrefab, _weaponSpawnPos);
        _currentWeaponRef = bow;
        _anim.SetTrigger("switch");
    }
    
    public void DaggerAttack()
    {
        StartCoroutine(LerpVisibility(true));

        _anim.SetTrigger("dagger");
    }

    public void HammerAttack()
    {
        StartCoroutine(LerpVisibility(true));

        _anim.SetTrigger("hammer");
    }

    public void BowAttack()
    {
        StartCoroutine(LerpVisibility(true));

        var arrow = Instantiate(_arrowPrefab, transform.position, Quaternion.identity);
        arrow.Shoot(_gm.enemy.transform.position);
        _anim.SetTrigger("bow");
    }

    public void SneakyDagger()
    {
        StartCoroutine(LerpVisibility(true));

        _anim.SetTrigger("sneakydagger");
    }

    public void SneakyHammer()
    {
        StartCoroutine(LerpVisibility(true));

        _anim.SetTrigger("sneakyhammer");
    }

    public void SneakyBow()
    {
        StartCoroutine(LerpVisibility(true));

        var arrow = Instantiate(_arrowPrefab, transform.position, Quaternion.identity);
        arrow.Shoot(_gm.enemy.transform.position);
        _anim.SetTrigger("sneakybow");
        _busy = false;
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
        Run(_gm.allNodes.Where(x => x.isTaken).First().Neighbors.First());
    }

    public void PickArrow()
    {
        Destroy(_currentNode.arrow.gameObject);
        _currentNode.arrow = null;
        _anim.SetTrigger("pickup");
    }

    public void TurnInvisible()
    {
        _anim.SetTrigger("invisible");
        StartCoroutine(LerpVisibility(false));
    }

    public void ToggleInvisibility(bool on)
    {
        StartCoroutine(LerpVisibility(on));
    }

    IEnumerator LerpVisibility(bool invisible)
    {
        float timer = 0, startValue = invisible ? 0f : 1f, endValue = invisible ? 1f : 0f;

        if (endValue == _renderers[0].material.color.a) yield break;

        while (timer < 0.5f)
        {
            timer += Time.deltaTime;

            foreach (var item in _renderers)
            {
                item.material.color = new Color(1, 1, 1, Mathf.Lerp(startValue, endValue, timer * 2));
            }

            yield return null;
        }

        foreach (var item in _renderers)
        {
            item.material.color = new Color(1, 1, 1, endValue);
        }
    }

    public void ActionFinished()
    {
        _busy = false;
    }

    public IEnumerator ActionExecution(List<GOAPActions> actions)
    {
        while (actions.Any())
        {
            var action = actions.First();

            _busy = true;

            action.agentBehaviour();
            print(action.Name);

            while (_busy) yield return null;

            actions.Remove(action);
        }

        print("Plan completed");

        _gm.enemy.gameObject.SetActive(false);
    }
}
