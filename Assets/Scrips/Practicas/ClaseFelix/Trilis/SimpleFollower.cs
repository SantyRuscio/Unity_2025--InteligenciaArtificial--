using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollower : MonoBehaviour
{
   Vector3 dir = Vector3.zero;
    [SerializeField] float speed = 5f;

    private void Update()
    {
        dir = Target.Position - transform.position;

        transform.position = transform.position + dir.normalized * Time.deltaTime;
    }

}
