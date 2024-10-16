using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Transform target = null;
    private Vector3 offset;

    void Start()
    {
        target = null;
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            target = collider.transform;
            offset = target.position - transform.position;
        }
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            target = null;
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            target.transform.position = transform.position + offset;
        }
    }
}
