using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Accion para GOAP. Tiene precondiciones (Los requerimientos para una accion) y efectos (lo que va a pasar cuando se ejecute la funcion).
/// </summary>
public class GOAPActions
{
    public string Name { get; private set; }

    public Func<WorldState, bool> Preconditions = delegate { return true; };
    public Func<WorldState, WorldState> Effects = delegate { return default; };

    public int Cost { get; private set; }

    //public ActionTypes RelatedAction { get; private set; }

    public GOAPActions(string name)
    {
        Name = name;
    }

    public GOAPActions SetCost(int cost)
    {
        Cost = cost;

        return this;
    }

    //public GoapAction SetAction(ActionTypes newAction)
    //{
    //    RelatedAction = newAction;
    //    return this;
    //}

    public GOAPActions Precondition(Func<WorldState, bool> prec)
    {
        Preconditions = prec;
        return this;
    }

    public GOAPActions Effect(Func<WorldState, WorldState> effe)
    {
        Effects = effe;
        return this;
    }
}
