using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    public string promptMessage = "Pick up Item";
    private Rigidbody rb;
    private Collider coll;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }

    public void OnPickedUp(Transform holdPosition)
    {
        rb.isKinematic = true;
        coll.enabled = false;

        transform.SetParent(holdPosition);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void OnDropped()
    {
        transform.SetParent(null);

        rb.isKinematic = false;
        coll.enabled = true;
    }
}