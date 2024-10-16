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
    private IEnumerable<Switch> switches;
    private IEnumerable<Door> doors;

    private void Awake()
    {
        Points = GetComponentsInChildren<Checkpoint>().AsEnumerable();

        switches = GetComponentsInChildren<Switch>().AsEnumerable();
        doors = GetComponentsInChildren<Door>().AsEnumerable();


        for (int i = 0; i < switches.Count(); i++)
        {
            Switch s = switches.ElementAtOrDefault(i);
            Door d = doors.ElementAtOrDefault(i);
            if (!s) continue;
            s.turnedOn += () =>
            {
                d.Open();
            };
        }
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
