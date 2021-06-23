using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootTargetBehavior : MonoBehaviour
{
    public Transform Node; 
    
    void Update()
    {
        this.transform.position = new Vector3(Node.position.x, 0, Node.position.z);
        this.transform.rotation = Node.rotation;
    }
}
