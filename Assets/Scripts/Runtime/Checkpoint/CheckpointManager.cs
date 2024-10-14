using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class CheckpointManager : SingletonMonoBehaviour<CheckpointManager>
{
    [SerializeField]
    private float checkRadius = 0.2f;
    [SerializeField]
    private LayerMask playerLayer;
    private List<Checkpoint> points = new List<Checkpoint>();
    private List<Checkpoint> reachedPoints = new List<Checkpoint>();
    private PlayerController player;
    private bool isEnabled = false;
    private int currentIndex = 0;

    protected override void Awake()
    {
        Gameplay.statusUpdated += GameplayStatusUpdated;
        base.Awake();
        points = GetComponentsInChildren<Checkpoint>().ToList();
    }

    private void Start()
    {
    }

    private void GameplayStatusUpdated(Gameplay.Status status)
    {
        switch (status)
        {
            case Gameplay.Status.Started:

                if (!player)
                    player = Gameplay.Instance.Player;

                GoToStart();
                break;
            case Gameplay.Status.Running:
                isEnabled = true;
                break;
            case Gameplay.Status.Paused:
                isEnabled = false;
                break;
        }
    }


    private void Update()
    {
        CheckpointDetection();
    }

    private void CheckpointDetection()
    {
        if(!isEnabled) return;
        for (int i = currentIndex; i < points.Count; i++)
        {
            if(i > currentIndex)
            {
                Checkpoint lastPoint = points.ElementAtOrDefault(i - 1);
                if (lastPoint && !lastPoint.HasReached) break;
            }
            Checkpoint point = points[i];
            if (point.HasReached) continue;
            bool isHit = Physics2D.CircleCast(point.transform.position, checkRadius, Vector2.zero, 1f, playerLayer);
            if(!isHit) continue;
            point.MarkReached();
            reachedPoints.Add(point);
            currentIndex = i;
        }
    }


    public Checkpoint GoForward(int count)
    {
        int destinationIndex = currentIndex + count;
        if (destinationIndex >= points.Count)
        {
            destinationIndex = points.Count - 1;
            if (destinationIndex == currentIndex)
                return GoToCurrent();
        }
        currentIndex = destinationIndex;
        Checkpoint targetPoint = points.ElementAtOrDefault(currentIndex);
        targetPoint.MarkStarting();
        if (currentIndex > 0)
        {
            IEnumerable<Checkpoint> unreachedPoints = points.Take(currentIndex).Where(c => !c.HasReached);
            foreach (Checkpoint point in unreachedPoints)
            {
                point.MarkReached();
                reachedPoints.Add(point);
            }
        }
        reachedPoints.Add(targetPoint);
        return GoToCurrent();
    }
    public Checkpoint GoToCurrent()
    {
        Checkpoint targetPoint = points.ElementAtOrDefault(currentIndex);
        player.transform.position = targetPoint.transform.position;
        return targetPoint;
    }

    public Checkpoint GoToStart()
    {
        return GoBackward(reachedPoints.Count);
    }
    
    public Checkpoint GoBackward(int count)
    {
        int destinationIndex = currentIndex - count;
        if (destinationIndex <= 0) 
            destinationIndex = 0;
        if (currentIndex > 0)
        {
            IEnumerable<Checkpoint> _reachedPoints = points.Take(currentIndex + 1).TakeLast(count + 1);
            Debug.Log(_reachedPoints.Count());
            foreach (Checkpoint point in _reachedPoints)
            {
                point.Clear();
                reachedPoints.Remove(point);
            }
        }
        currentIndex = destinationIndex;
        return GoToCurrent();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("GO Forward"))
        {
            GoForward(1);
        }
        if (GUILayout.Button("GO Current"))
        {
            GoBackward(0);
        }
        if (GUILayout.Button("GO Backward"))
        {
            GoBackward(1);
        }
    }
}
