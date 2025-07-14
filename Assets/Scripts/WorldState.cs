using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Dagger,
    Hammer,
    Bow
}

public class WorldState
{
    public struct State
    {
        public float enemyHp;
        public int arrows, arrowsAvailable;
        public bool detected, arrowNearby, enemyReachable, enemyNearby, canRetreat;
        public WeaponType equippedWeapon;

        public State Clone()
        {
            return new State()
            {
                enemyHp = this.enemyHp,
                arrows = this.arrows,
                detected = this.detected,
                arrowsAvailable = this.arrowsAvailable,
                arrowNearby = this.arrowNearby,
                enemyReachable = this.enemyReachable,
                enemyNearby = this.enemyNearby,
                canRetreat = this.canRetreat,
                equippedWeapon = this.equippedWeapon,
            };
        }
    }

    public State state;

    public WorldState(GOAPActions action = null)
    {
        generatingAction = action;
        state = new();
    }
    
    public WorldState(WorldState newState, GOAPActions action = null)
    {
        generatingAction = action;
        state = newState.state.Clone();
    }

    public GOAPActions generatingAction;
}
