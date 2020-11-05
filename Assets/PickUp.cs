using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public Transform destination;
    public bool attached;
    public Material rootObjectColour;
    public Material cMaterial;
    public Material beforeMaterial;
    public Material pickedUpObject;

    [Header("Trajectory")]
    private float trajectoryVertDist = 0.25f;
    private LineRenderer line;
    [SerializeField]
    private Transform lineIntersect;

    [SerializeField]
    private float maxCurveLength = 5;

    private void Start()
    {
        lineIntersect.transform.parent = null;
        lineIntersect.gameObject.SetActive(false);
        cMaterial = GetComponent<Renderer>().material;
        line = GetComponent<LineRenderer>();
    }

    public void PickUpObject(Transform dest)
    {
        destination = dest;

        if(GetComponent<FixedJoint>())
        {
            Destroy(GetComponent<FixedJoint>());
        }

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Collider>().enabled = false;

        foreach(Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            if(rb.CompareTag("Box"))
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.GetComponent<Collider>().enabled = false;
            }
        }

        transform.position = dest.position;

        if(transform.parent != null && transform.parent.parent.CompareTag("Box"))
        {
            transform.parent.parent.GetComponent<MeshRenderer>().material = transform.parent.parent.GetComponent<PickUp>().cMaterial;
            GetComponent<MeshRenderer>().material = rootObjectColour;
            beforeMaterial = rootObjectColour;
        }

        GetComponent<MeshRenderer>().material = pickedUpObject;

        transform.parent = dest;
    }

    public void AttachObject(Transform attachPoint)
    {
        destination = attachPoint;

        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        transform.parent = attachPoint;
        transform.position = attachPoint.position;
        transform.rotation = attachPoint.rotation;

        FixedJoint fj = gameObject.AddComponent<FixedJoint>();
        fj.connectedBody = attachPoint.GetComponentInParent<Rigidbody>();
        fj.enableCollision = true;

        bool setRoot = false;
        foreach(Transform t in transform.root.GetComponentsInChildren<Transform>())
        {
            if (t.CompareTag("Box"))
            {
                if(!setRoot)
                {
                    t.GetComponent<MeshRenderer>().material = rootObjectColour;
                    setRoot = true;
                }
                else
                {
                    t.GetComponent<MeshRenderer>().material = cMaterial;
                }
            }
        }
    }

    public void DropObject()
    {
        if (GetComponent<FixedJoint>())
        {
            Destroy(GetComponent<FixedJoint>());
        }

        destination = null;

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            if (rb.CompareTag("Box"))
            {
                rb.isKinematic = false;
                rb.GetComponent<Collider>().enabled = true;
            }
        }

        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().enabled = true;
        if (beforeMaterial != null)
            GetComponent<MeshRenderer>().material = beforeMaterial;
        else
            GetComponent<MeshRenderer>().material = cMaterial;

        transform.parent = null;
    }

    public void ThrowObject(Vector3 forward)
    {
        DropObject();
        ClearTrajectory();
        GetComponent<Rigidbody>().AddForce(forward, ForceMode.VelocityChange);
    }

    public void DrawTrajectory(Vector3 startPos, Vector3 cVelocity)
    {
        lineIntersect.gameObject.SetActive(true);
        var curvePoints = new List<Vector3>();
        curvePoints.Add(startPos);

        var currentPosition = startPos;
        var currentVelocity = cVelocity;

        RaycastHit hit;
        Ray ray = new Ray(currentPosition, currentVelocity.normalized);

        while (!Physics.Raycast(ray, out hit, trajectoryVertDist) && Vector3.Distance(startPos, currentPosition) < maxCurveLength)
        {
            var t = trajectoryVertDist / currentVelocity.magnitude;

            currentVelocity = currentVelocity + t * Physics.gravity;

            currentPosition = currentPosition + t * currentVelocity;

            curvePoints.Add(currentPosition);

            ray = new Ray(currentPosition, currentVelocity.normalized);

            lineIntersect.transform.position = currentPosition;

            if (hit.transform)
            {
                curvePoints.Add(hit.point);
            }

            line.positionCount = curvePoints.Count;
            line.SetPositions(curvePoints.ToArray());
        }
    }

    private void ClearTrajectory()
    {
        lineIntersect.gameObject.SetActive(false);
        line.positionCount = 0;
    }
}
