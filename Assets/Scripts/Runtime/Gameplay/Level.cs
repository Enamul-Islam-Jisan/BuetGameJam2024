using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class Level : MonoBehaviour
{
    [field:SerializeField]
    public Collider2D CameraBound { get; private set; }
    internal IEnumerable<Checkpoint> Points { get; private set; }


    private void Awake()
    {
        Points = GetComponentsInChildren<Checkpoint>().AsEnumerable();
    }

    private void OnDisable()
    {
        IEnumerable<Checkpoint> reachedPoints = Points.Where(c => c.IsActive);
        foreach (var point in Points)
        {
            point.SetActive(false);
        }
    }
}
