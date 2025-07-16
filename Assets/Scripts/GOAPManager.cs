using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GOAPManager
{
    Pathfinding _pf;
    GameManager _gm;

    public GOAPManager(GameManager gm)
    {
        _pf = new();
        _gm = gm;
    }

    List<GOAPActions> GetActions()
    {
        return new List<GOAPActions>()
        {
            new GOAPActions("Switch to dagger")
            .SetCost(1)
            .SetBehaviour(_gm.agent.EquipDagger)
            .Precondition(x => x.state.equippedWeapon != WeaponType.Dagger && x.state.enemyReachable)
            .Effect(x =>
            {
                x.state.equippedWeapon = WeaponType.Dagger;
                return x;
            }),
            new GOAPActions("Switch to hammer")
            .SetCost(1)
            .SetBehaviour(_gm.agent.EquipHammer)
            .Precondition(x => x.state.equippedWeapon != WeaponType.Hammer && x.state.enemyReachable)
            .Effect(x =>
            {
                x.state.equippedWeapon = WeaponType.Hammer;
                return x;
            }),
            new GOAPActions("Switch to bow")
            .SetCost(1)
            .SetBehaviour(_gm.agent.EquipBow)
            .Precondition(x => x.state.equippedWeapon != WeaponType.Bow && !x.state.enemyNearby)
            .Effect(x =>
            {
                x.state.equippedWeapon = WeaponType.Bow;
                return x;
            }),
            new GOAPActions("Dagger attack")
            .SetCost(1)
            .SetBehaviour(_gm.agent.DaggerAttack)
            .Precondition(x => x.state.equippedWeapon == WeaponType.Dagger && x.state.enemyNearby)
            .Effect(x => 
            {
                x.state.enemyHp -= 5;
                x.state.detected = true;
                return x;
            }),
            new GOAPActions("Sneaky dagger attack")
            .SetCost(2)
            .SetBehaviour(_gm.agent.SneakyDagger)
            .Precondition(x => x.state.equippedWeapon == WeaponType.Dagger && x.state.enemyNearby && !x.state.detected)
            .Effect(x =>
            {
                x.state.enemyHp -= 50;
                x.state.detected = true;
                return x;
            }),
            new GOAPActions("Hammer attack")
            .SetCost(2)
            .SetBehaviour(_gm.agent.HammerAttack)
            .Precondition(x => x.state.equippedWeapon == WeaponType.Hammer && x.state.enemyNearby)
            .Effect(x =>
            {
                x.state.enemyHp -= 15;
                x.state.detected = true;
                return x;
            }),
            new GOAPActions("Sneaky hammer attack")
            .SetCost(2)
            .SetBehaviour(_gm.agent.SneakyHammer)
            .Precondition(x => x.state.equippedWeapon == WeaponType.Hammer && x.state.enemyNearby && !x.state.detected)
            .Effect(x =>
            {
                x.state.enemyHp -= 30;
                x.state.detected = true;
                return x;
            }),
            new GOAPActions("Bow attack")
            .SetCost(1)
            .SetBehaviour(_gm.agent.BowAttack)
            .Precondition(x => x.state.equippedWeapon == WeaponType.Bow && !x.state.enemyNearby && x.state.arrows > 0)
            .Effect(x =>
            {
                x.state.enemyHp -= 15;
                x.state.arrows--;
                x.state.detected = true;
                return x;
            }),
            new GOAPActions("Sneaky bow attack")
            .SetCost(2)
            .SetBehaviour(_gm.agent.SneakyBow)
            .Precondition(x => x.state.equippedWeapon == WeaponType.Bow && !x.state.enemyNearby && x.state.arrows > 0 && !x.state.detected)
            .Effect(x =>
            {
                x.state.enemyHp -= 30;
                x.state.arrows--;
                x.state.detected = true;
                return x;
            }),
            new GOAPActions("Run to arrow")
            .SetCost(1)
            .SetBehaviour(_gm.agent.RunToArrow)
            .Precondition(x => x.state.arrowsAvailable > 0 && !x.state.arrowNearby && x.state.equippedWeapon == WeaponType.Bow)
            .Effect(x =>
            {
                x.state.arrowNearby = true;
                x.state.detected = true;
                x.state.enemyNearby = false;
                return x;
            }),
            new GOAPActions("Sneak to arrow")
            .SetCost(2)
            .SetBehaviour(_gm.agent.SneakToArrow)
            .Precondition(x => x.state.arrowsAvailable > 0 && !x.state.arrowNearby && !x.state.detected && x.state.equippedWeapon == WeaponType.Bow)
            .Effect(x =>
            {
                x.state.arrowNearby = true;
                x.state.enemyNearby = false;
                return x;
            }),
            new GOAPActions("Run to enemy")
            .SetCost(1)
            .SetBehaviour(_gm.agent.RunToEnemy)
            .Precondition(x => x.state.enemyReachable && !x.state.enemyNearby && x.state.equippedWeapon != WeaponType.Bow)
            .Effect(x =>
            {
                x.state.enemyNearby = true;
                x.state.detected = true;
                x.state.arrowNearby = false;
                return x;
            }),
            new GOAPActions("Sneak to enemy")
            .SetCost(2)
            .SetBehaviour(_gm.agent.SneakToEnemy)
            .Precondition(x => x.state.enemyReachable && !x.state.enemyNearby && !x.state.detected && x.state.equippedWeapon != WeaponType.Bow)
            .Effect(x =>
            {
                x.state.enemyNearby = true;
                x.state.arrowNearby = false;
                return x;
            }),
            new GOAPActions("Run away from enemy")
            .SetCost(1)
            .SetBehaviour(_gm.agent.RunFromEnemy)
            .Precondition(x => x.state.canRetreat && x.state.enemyNearby)
            .Effect(x =>
            {
                x.state.enemyNearby = false;
                x.state.detected = true;
                return x;
            }),
            new GOAPActions("Pick up arrow")
            .SetCost(1)
            .SetBehaviour(_gm.agent.PickArrow)
            .Precondition(x => x.state.arrowNearby && x.state.equippedWeapon == WeaponType.Bow)
            .Effect(x =>
            {
                x.state.arrows++;
                x.state.arrowsAvailable--;
                x.state.arrowNearby = false;
                return x;
            }),
            new GOAPActions("Turn invisible")
            .SetCost(2)
            .SetBehaviour(_gm.agent.TurnInvisible)
            .Precondition(x => x.state.detected && !x.state.enemyNearby)
            .Effect(x =>
            {
                x.state.detected = false;
                return x;
            }),
        };
    }

    public bool TryGetPlan(WorldState initialState, out List<GOAPActions> plan)
    {
        Func<WorldState, int> heuristic = x =>
        {
            return Mathf.RoundToInt(x.state.enemyHp);
        };

        Func<WorldState, bool> objective = x =>
        {
            return x.state.enemyHp <= 0;
        };

        var worldStatePath = _pf.AStarGOAP(initialState, null, GetActions(), heuristic, objective);

        if (worldStatePath != default)
        {
            plan = worldStatePath.Skip(1).Select(x => x.generatingAction).ToList();
            return true;
        }
        else
        {
            plan = default;
            return false;
        }
    }
}
