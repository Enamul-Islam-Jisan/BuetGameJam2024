using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Gameplay : SingletonMonoBehaviour<Gameplay>
{
    [SerializeField]
    private PlayerController player;
    public Status status { get; private set; } = Status.None;

    public static event StatusUpdated statusUpdated;

    public delegate void StatusUpdated(Status status);

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        SpawnPlayer(); 
        UpdateStatus(Status.Started);
        UpdateStatus(Status.Running);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            SetPause(true);
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            SetPause(false);
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            End();
        }
    }
    private void UpdateStatus(Status status)
    {
        this.status = status;
        statusUpdated?.Invoke(status);
    }

    private void SpawnPlayer()
    {
        player = Instantiate(player);
    }

    public void SetPause(bool pause)
    {
        if (status != Status.Paused && status != Status.Running) return;
        if (pause)
        {
            UpdateStatus(Status.Paused);
        }
        else
        {
            UpdateStatus(Status.Running);
        }
    }

    public void End()
    {
        UpdateStatus(Status.Started);
    }

    public enum Status
    {
        None,
        Started,
        Running,
        Paused
    }
}
