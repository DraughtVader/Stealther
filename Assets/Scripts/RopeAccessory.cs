using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnchoredJoint2D))]
public class RopeAccessory : MonoBehaviour
{
    [SerializeField]
    protected GameObject nodePrefab;

    [SerializeField]
    protected int nodeCount;

    protected DistanceJoint2D distanceJoint;
    protected List<DistanceJoint2D> nodes = new List<DistanceJoint2D>();

    private void Start()
    {
        distanceJoint = GetComponent<DistanceJoint2D>();
        nodes.Add(distanceJoint);
        NinjaController ninja = GetComponentInParent<NinjaController>();
        distanceJoint.connectedBody = ninja.Rigidbody;

        for (int i = 0; i < nodeCount; i++)
        {
            var node = Instantiate(nodePrefab, transform.position, Quaternion.identity);
            node.transform.SetParent(transform);
            var joint = node.GetComponent<DistanceJoint2D>();
            joint.connectedBody = nodes[i].GetComponent<Rigidbody2D>();
            nodes.Add(joint);
        }
    }
}
