using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailingTarget : MonoBehaviour
{
    [SerializeField] private GameObject trailingTarget;
    [SerializeField] private float trailingDistance = 1f;

    public GameObject target { get { return trailingTarget; } }

    private Vector3 cachedPosition;
    private void Update()
    {
        
    }
}
