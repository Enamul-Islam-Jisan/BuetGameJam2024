using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PanelNavigator : MonoBehaviour
{
    [SerializeField]
    private List<Panel> panels = new List<Panel>();
    [SerializeField]
    private bool canGoBack = true;
    [SerializeField]
    private int defaultSelected;
    private bool initialNavigation = false;
    public int CurrentPanelIndex { get; private set; }
    public int NextPanelIndex { get; private set; }
    public Panel CurrentPanel  { get; private set; }
    public Panel NextPanel { get; private set; }
    public List<int> lastPanelsIndexs { get; private set; } = new List<int>();

    private static List<PanelNavigator> instances = new List<PanelNavigator>();
    private static bool Paused;


    private void Awake()
    {
        lastPanelsIndexs.Clear();

        initialNavigation = true;
        GoToView(defaultSelected);
    }

    private void OnEnable()
    {
        if (!instances.Contains(this))
        {
            instances.Add(this);
        }
    }

    private void OnDisable()
    {
        if (instances.Contains(this))
        {
            instances.Remove(this);
        }
    }

    private void OnDestroy()
    {
        if (instances.Contains(this))
        {
            instances.Remove(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !Paused)
        {
            var instance = instances.LastOrDefault(i => i.canGoBack);
            if (instance) instance.GoToLastView();
        }
    }
    public void GoToLastView()
    {
        if (!canGoBack) return;
        if (lastPanelsIndexs.Count < 1) return;
        int index = lastPanelsIndexs.Last();
        if (index == CurrentPanelIndex) return;
        GoToView(index);
    }

    public void GoToView(int index)
    {
        StartCoroutine(GoToViewInternal(index));
    }

    private IEnumerator GoToViewInternal(int index)
    {
        if (index >= 0 && index < panels.Count)
        {
            if(!initialNavigation)
            {
                CurrentPanel = panels[CurrentPanelIndex];

                if (CurrentPanel)
                {
                    yield return CurrentPanel.HideAsync();
                }
            }
            else
            {
                initialNavigation = false;
            }
            NextPanelIndex = index;
            NextPanel = panels[NextPanelIndex];
            yield return NextPanel.ShowAsync();
            if (lastPanelsIndexs.Count > 0)
            {
                if (NextPanelIndex == 0)
                {
                    lastPanelsIndexs.Clear();
                }
                else
                {
                    if (NextPanelIndex == lastPanelsIndexs.Last())
                    {
                        lastPanelsIndexs.Remove(NextPanelIndex);
                    }
                    else
                    {
                        lastPanelsIndexs.Add(CurrentPanelIndex);
                    }
                }
            }
            else
            {
                lastPanelsIndexs.Add(CurrentPanelIndex);
            }
            CurrentPanelIndex = NextPanelIndex;
        }
        else yield break;
    }

    public static void Pause()
    {
        if (Paused) return;
        Paused = true;
    }
    public static void Resume()
    {
        if (!Paused) return;
        Paused = false;
    }
}
