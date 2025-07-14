using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Node _node;

    void SetPosition(Vector3 pos)
    {
        pos.y = transform.position.y;
        transform.position = pos;
    }

    public void SetNode(Node node)
    {
        if (_node != null) _node.isTaken = false;

        _node = node;
        _node.isTaken = true;
        SetPosition(node.othersPos.position);
    }
}
