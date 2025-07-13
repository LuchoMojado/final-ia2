using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GOAPManager : MonoBehaviour
{
    Pathfinding _pf;

    private void Start()
    {
        _pf = new Pathfinding();   
    }

    List<GOAPActions> GetActions()
    {
        return new List<GOAPActions>()
        {
            new GOAPActions("SwitchToDagger")
            .SetCost(1)
            .Precondition(x => x.state.equippedWeapon != WeaponType.Dagger)
            .Effect(x =>
            {
                x.state.equippedWeapon = WeaponType.Dagger;
                return x;
            }),
            new GOAPActions("SwitchToHammer")
            .SetCost(1)
            .Precondition(x => x.state.equippedWeapon != WeaponType.Hammer)
            .Effect(x =>
            {
                x.state.equippedWeapon = WeaponType.Hammer;
                return x;
            }),
            new GOAPActions("SwitchToBow")
            .SetCost(1)
            .Precondition(x => x.state.equippedWeapon != WeaponType.Bow)
            .Effect(x =>
            {
                x.state.equippedWeapon = WeaponType.Bow;
                return x;
            }),
            new GOAPActions("DaggerAttack")
            .SetCost(1)
            .Precondition(x => x.state.equippedWeapon == WeaponType.Dagger && x.state.enemyNearby)
            .Effect(x => 
            {
                x.state.enemyHp -= 5;
                x.state.detected = true;
                return x;
            }),
            new GOAPActions("SneakyDaggerAttack")
            .SetCost(2)
            .Precondition(x => x.state.equippedWeapon == WeaponType.Dagger && x.state.enemyNearby && !x.state.detected)
            .Effect(x =>
            {
                x.state.enemyHp -= 50;
                x.state.detected = true;
                return x;
            }),
            new GOAPActions("HammerAttack")
            .SetCost(2)
            .Precondition(x => x.state.equippedWeapon == WeaponType.Hammer && x.state.enemyNearby)
            .Effect(x =>
            {
                x.state.enemyHp -= 15;
                x.state.detected = true;
                return x;
            }),
            new GOAPActions("SneakyHammerAttack")
            .SetCost(2)
            .Precondition(x => x.state.equippedWeapon == WeaponType.Hammer && x.state.enemyNearby && !x.state.detected)
            .Effect(x =>
            {
                x.state.enemyHp -= 30;
                x.state.detected = true;
                return x;
            }),
            new GOAPActions("BowAttack")
            .SetCost(1)
            .Precondition(x => x.state.equippedWeapon == WeaponType.Bow && !x.state.enemyNearby && x.state.arrows > 0)
            .Effect(x =>
            {
                x.state.enemyHp -= 15;
                x.state.arrows--;
                x.state.detected = true;
                return x;
            }),
            new GOAPActions("SneakyBowAttack")
            .SetCost(2)
            .Precondition(x => x.state.equippedWeapon == WeaponType.Bow && !x.state.enemyNearby && x.state.arrows > 0 && !x.state.detected)
            .Effect(x =>
            {
                x.state.enemyHp -= 30;
                x.state.arrows--;
                x.state.detected = true;
                return x;
            }),
            new GOAPActions("RunToArrow")
            .SetCost(1)
            .Precondition(x => x.state.arrowReachable && !x.state.arrowNearby)
            .Effect(x =>
            {
                x.state.arrowNearby = true;
                x.state.detected = true;
                return x;
            }),
            new GOAPActions("SneakToArrow")
            .SetCost(2)
            .Precondition(x => x.state.arrowReachable && !x.state.arrowNearby && !x.state.detected)
            .Effect(x =>
            {
                x.state.arrowNearby = true;
                return x;
            }),
            new GOAPActions("RunToEnemy")
            .SetCost(1)
            .Precondition(x => x.state.enemyReachable && !x.state.enemyNearby)
            .Effect(x =>
            {
                x.state.enemyNearby = true;
                x.state.detected = true;
                return x;
            }),
            new GOAPActions("SneakToEnemy")
            .SetCost(2)
            .Precondition(x => x.state.enemyReachable && !x.state.enemyNearby && !x.state.detected)
            .Effect(x =>
            {
                x.state.enemyNearby = true;
                return x;
            }),
            new GOAPActions("RunAwayFromEnemy")
            .SetCost(1)
            .Precondition(x => x.state.canRetreat && x.state.enemyNearby)
            .Effect(x =>
            {
                x.state.enemyNearby = false;
                x.state.detected = true;
                return x;
            }),
            new GOAPActions("PickUpArrow")
            .SetCost(1)
            .Precondition(x => x.state.arrowNearby)
            .Effect(x =>
            {
                x.state.arrows++;
                return x;
            }),
            new GOAPActions("TurnInvisible")
            .SetCost(2)
            .Precondition(x => x.state.detected && !x.state.enemyNearby)
            .Effect(x =>
            {
                x.state.detected = false;
                return x;
            }),
        };
    }

    public List<GOAPActions> ExecuteActions()
    {
        var initialState = new WorldState();

        Func<WorldState, int> heuristic = x =>
        {
            return Mathf.RoundToInt(x.state.enemyHp);
        };

        Func<WorldState, bool> objective = x =>
        {
            return x.state.enemyHp <= 0;
        };

        var worldStatePath = _pf.AStarGOAP(initialState, null, GetActions(), heuristic, objective);

        return worldStatePath.Skip(1).Select(x => x.generatingAction).ToList();
    }
}
