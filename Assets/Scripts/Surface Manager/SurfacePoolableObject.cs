using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SurfacePoolableObject : MonoBehaviour
{
    public ObjectPool<GameObject> Parent;

    private void OnDisable()
    {
        if(Parent != null)
        {
            Parent.Release(gameObject);
        }
    }
}
