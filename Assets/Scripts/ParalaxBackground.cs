using UnityEngine;

public class ParalaxBackground : MonoBehaviour
{
    [SerializeField]
    private Vector2 paralax;
    private float length;
    private SpriteRenderer sr;
    [SerializeField]
    private Transform referenceCamTransform;
    private Vector2 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        length = sr.sprite.bounds.size.x;
        lastPosition = referenceCamTransform.position;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 delta = lastPosition - (Vector2)referenceCamTransform.position;
        transform.position += new Vector3(delta.x * paralax.x, 0);
        lastPosition = referenceCamTransform.position;

        Vector2 distance = referenceCamTransform.position - transform.position;

        if (Mathf.Abs(distance.x) >= length)
        {
            float offsetFromCameraX = distance.x % length;
            transform.position = new Vector3(referenceCamTransform.position.x + offsetFromCameraX, transform.position.y, transform.position.z);
        }
    }
}
