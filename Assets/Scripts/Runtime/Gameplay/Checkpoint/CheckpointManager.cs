using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class CheckpointManager : SingletonMonoBehaviour<CheckpointManager>
{
    [SerializeField]
    private float checkRadius = 0.2f;
    [SerializeField]
    private LayerMask playerLayer;
    private Transform playerTransform;
    private bool isEnabled = false;
    private int currentIndex = 0;
    private IEnumerable<Checkpoint> points;

    protected override void Awake()
    {
        base.Awake();
        Gameplay.levelLoaded += LoadCheckPoints;

    }

    private void Update()
    {
        CheckpointDetection();
    }

    private void CheckpointDetection()
    {
        if (!isEnabled) return;
        Checkpoint activatedCheckpoint = null;
        for (int i = 0; i < points.Count(); i++)
        {
            Checkpoint point = points.ElementAt(i);
            bool isHit = Physics2D.CircleCast(point.transform.position, checkRadius, Vector2.zero, 1f, playerLayer);
            if(!isHit || point.IsActive || !point.IsEnabled) continue;
            activatedCheckpoint = point;
            currentIndex = i;
            if (i == points.Count() - 1)
            {
                Gameplay.Instance.LoadNextLevel();
                return;
            }
        }
        if (activatedCheckpoint)
        {
            for (int i = 0; i < points.Count(); i++)
            {
                Checkpoint point = points.ElementAt(i);
                point.SetActive(activatedCheckpoint == point);
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
        return GoToCurrent();
    }

    public Checkpoint GoToCurrent()
    {
        Checkpoint targetPoint = points.ElementAtOrDefault(currentIndex);
        targetPoint.SetActive(true);
        IEnumerable<Checkpoint> otherPoints = points.Take(currentIndex).Where(c => c != targetPoint);
        foreach (Checkpoint point in otherPoints)
        {
            point.SetActive(false);
        }
        playerTransform.position = targetPoint.transform.position;
        return targetPoint;
    }

    public Checkpoint GoToStart()
    {
        currentIndex = 0;
        return GoToCurrent();
    }

    public Checkpoint GoToEnd()
    {
        currentIndex = points.Count() - 1;
        return GoToCurrent();
    }
    
    public Checkpoint GoBackward(int count)
    {
        int destinationIndex = currentIndex - count;
        if (destinationIndex <= 0) 
            destinationIndex = 0;
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
        Gameplay.Instance.QuickLookAt(points.LastOrDefault().transform, 2,3);
    }

    private void OnDestroy()
    {
        Gameplay.levelLoaded -= LoadCheckPoints;
    }

}
