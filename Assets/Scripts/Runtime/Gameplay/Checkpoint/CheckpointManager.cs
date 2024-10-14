using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointManager : SingletonMonoBehaviour<CheckpointManager>
{
    [SerializeField]
    private float checkRadius = 0.2f;
    [SerializeField]
    private LayerMask playerLayer;
    private List<Checkpoint> reachedPoints = new List<Checkpoint>();
    private Transform playerTransform;
    private bool isEnabled = false;
    private int currentIndex = 0;
    private IEnumerable<Checkpoint> points;

    protected override void Awake()
    {
        base.Awake();
        Gameplay.levelLoaded += LoadCheckPoints;
        Gameplay.characterReverted += () => GoToCurrent();
    }


    private void Update()
    {
        CheckpointDetection();
    }

    private void CheckpointDetection()
    {
        if (!isEnabled) return;
        for (int i = currentIndex; i < points.Count(); i++)
        {
            if(i > currentIndex)
            {
                Checkpoint lastPoint = points.ElementAtOrDefault(i - 1);
                if (lastPoint && !lastPoint.HasReached) break;
            }
            Checkpoint point = points.ElementAt(i);
            if (point.HasReached) continue;
            bool isHit = Physics2D.CircleCast(point.transform.position, checkRadius, Vector2.zero, 1f, playerLayer);
            if(!isHit) continue;
            point.MarkReached();
            reachedPoints.Add(point);
            currentIndex = i;
            if (i == points.Count() - 1)
            {
                Gameplay.Instance.LoadNextLevel();
                break;
            }
        }
    }


    public Checkpoint GoForward(int count)
    {
        int destinationIndex = currentIndex + count;
        if (destinationIndex >= points.Count())
        {
            destinationIndex = points.Count() - 1;
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
        playerTransform.position = targetPoint.transform.position;
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
            foreach (Checkpoint point in _reachedPoints)
            {
                point.Clear();
                reachedPoints.Remove(point);
            }
        }
        currentIndex = destinationIndex;
        return GoToCurrent();
    }

    private void LoadCheckPoints(Level level)
    {
        points = level.Points;
        currentIndex = 0;
        playerTransform = Gameplay.Instance.player.transform;
        isEnabled = true;
        GoToCurrent();
    }

    private void OnDestroy()
    {
        Gameplay.levelLoaded -= LoadCheckPoints;
    }

}
