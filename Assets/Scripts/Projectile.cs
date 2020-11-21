using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Transform Target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, Target.position, 7 * Time.deltaTime);
        if (Vector3.Distance(transform.position, Target.position) < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}
