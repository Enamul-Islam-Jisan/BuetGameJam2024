using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Gameplay : SingletonMonoBehaviour<Gameplay>
{
    [SerializeField]
    private PlayerController playerPrefab;
    public PlayerController Player { get; private set; }
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
            End();
        }
    }
    private void UpdateStatus(Status status)
    {
        if (this.status == status) return;
        this.status = status;
        statusUpdated?.Invoke(status);
    }

    private void SpawnPlayer()
    {
        Player = Instantiate(playerPrefab);
    }

    public void SetPause(bool pause)
    {
        if (status != Status.Paused || status != Status.Running) return;
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
        UpdateStatus(Status.Ended);
    }
    public enum Status
    {
        None,
        Started,
        Running,
        Paused,
        Ended
    }
}
