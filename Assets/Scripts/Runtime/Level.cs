using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class Level : MonoBehaviour
{
    internal IEnumerable<Checkpoint> Points { get; private set; }

    private void Awake()
    {
        Points = GetComponentsInChildren<Checkpoint>().AsEnumerable();
    }

    private void OnDisable()
    {
        IEnumerable<Checkpoint> reachedPoints = Points.Where(c => c.HasReached);
        foreach (var point in Points)
        {
            point.Clear();
        }
    }
}
