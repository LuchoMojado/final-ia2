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
        _node = node;
        SetPosition(node.transform.position);
    }
}
