using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ghost") || collision.CompareTag("Player"))
        {
            onTrigger?.Invoke();
            Destroy(this);
        }
    }
}
