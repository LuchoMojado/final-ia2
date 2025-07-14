using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] float _speed;

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
