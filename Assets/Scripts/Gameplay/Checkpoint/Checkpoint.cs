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
    public bool IsActive { get; private set; }
    [SerializeField]
    private bool disableByDefault;
    public bool IsEnabled { get; private set; }
    private void Awake()
    {
        SetEnabled(!disableByDefault);
    }

    internal void SetActive(bool active)
    {
        if (IsActive == active) return;
        IsActive = active;
        switch (IsActive)
        {
            case true:
                name += " - Active";
                reached?.Invoke();
                break;
            case false:
                name = name.Replace(" - Active", "");
                cleared?.Invoke();
                break;
        }
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