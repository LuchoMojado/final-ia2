using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] float _speed;
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

    public void Shoot(Vector3 target)
    {
        transform.forward = target - transform.position;

        StartCoroutine(Flying(target));
    }

    IEnumerator Flying(Vector3 endPos)
    {
        while (Vector3.Distance(transform.position, endPos) > 1f)
        {
            transform.position += transform.forward * _speed * Time.deltaTime;

            yield return null;
        }

        Destroy(gameObject);
    }
}
