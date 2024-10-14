using System;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    private UnityEvent started;
    [SerializeField]
    private UnityEvent reached;
    [SerializeField]
    private UnityEvent cleared;
    [SerializeField]
    public bool HasReached { get; private set; }

    internal void MarkStarting()
    {
        if (HasReached) return;
        MarkReached();
        started?.Invoke();
    }

    internal void MarkReached()
    {
        if (HasReached) return;
        HasReached = true;
        reached?.Invoke();
    }

    internal void Clear()
    {
        Debug.Log(HasReached);
        if (!HasReached) return;
        HasReached = false;
        Debug.Log(HasReached);
        cleared?.Invoke();
    }

}