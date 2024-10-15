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
    private UnityEvent onEnable;
    [SerializeField]
    private UnityEvent onDisable;
    [SerializeField]
    public bool HasReached { get; private set; }
    [SerializeField]
    private bool disableByDefault;
    public bool IsEnabled { get; private set; }
    private void Awake()
    {
        SetEnabled(!disableByDefault);
    }
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
        if (!HasReached) return;
        HasReached = false;
        cleared?.Invoke();
    }

    public void SetEnabled(bool enabled)
    {
        if(IsEnabled == enabled) return;
        IsEnabled = enabled;
        switch (enabled)
        {
            case true:
                onEnable?.Invoke();
                break;
            case false:
                onDisable?.Invoke();
                break;
        }
    }
}