using UnityEngine;

[DisallowMultipleComponent]
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T: MonoBehaviour
{
    public static T Instance { get; protected set; }

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this as T;
        }
    }
}
