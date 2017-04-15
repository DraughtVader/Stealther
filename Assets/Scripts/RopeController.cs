using System.Collections.Generic;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    [SerializeField]
    protected RopeNode ropeNodePrefab;

    private List<RopeNode> ropeNodes;
    private float ropeNodeDistance;
    private bool isAttached;
    private AnchoredJoint2D attachedBody;

    public bool IsAttached
    {
        get {return isAttached; }
    }

    public Transform FirstRopeNode
    {
        get { return ropeNodes[0].transform; }
    }

    public void AttachRope(Vector2 position, AnchoredJoint2D newBody)
    {
        ropeNodes = new List<RopeNode>();
        ropeNodeDistance = ropeNodePrefab.GetComponent<DistanceJoint2D>().distance;
        attachedBody = newBody;
        var current = (Vector2)transform.position;
        var direction = (position - current).normalized;
        float distance = Vector2.Distance(position, current);
        
        int numberRequired = (int)((distance*0.75f) / ropeNodeDistance);
        for (int i = 0; i < numberRequired; i++)
        {
            var ropeNode = Instantiate(ropeNodePrefab, transform.position, Quaternion.identity) as RopeNode;
            ropeNode.name = "RopeNode" + i;
            ropeNode.RopeController = this;
            var joint = ropeNode.GetComponent<AnchoredJoint2D>();
            ropeNode.transform.SetParent(transform);
            ropeNodes.Add(ropeNode);
        }
        attachedBody.connectedBody = ropeNodes[0].Rigidbody2D;
        for (int i = 0; i < ropeNodes.Count - 1; i++)
        {
            ropeNodes[i].AnchoredJoint2D.connectedBody = ropeNodes[i + 1].Rigidbody2D;
            ropeNodes[i].transform.position = current + direction * (distance * (i / (float)ropeNodes.Count));
        }

        var last = ropeNodes[ropeNodes.Count - 1];
        last.transform.position = position;
        last.AnchoredJoint2D.connectedAnchor = position;
        last.enabled = true;

        isAttached = true;
    }

    public void CutRope(RopeNode ropeNode)
    {
        int index = ropeNodes.IndexOf(ropeNode);

        if (ropeNodes.Count <= 8 || index > ropeNodes.Count - 8) //remaining rope is too short, end it
        {
            var last = ropeNodes[ropeNodes.Count - 1];
            last.AnchoredJoint2D.enabled = false;
        }
        else
        {
            ropeNodes.RemoveRange(0, index);
        }
    }

    public void DetachRope()
    {
        var last = ropeNodes[ropeNodes.Count - 1];
        isAttached = false;
        attachedBody.enabled = false;
    }

    protected void Update()
    {
        if (!isAttached)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (ropeNodes.Count < 8)
            {
                return;
            }
            var last = ropeNodes[ropeNodes.Count - 1];
            var secondlast = ropeNodes[ropeNodes.Count - 2];
            secondlast.AnchoredJoint2D.connectedAnchor = last.AnchoredJoint2D.connectedAnchor;
            secondlast.AnchoredJoint2D.connectedBody = null;
            ropeNodes.Remove(last);
            Destroy(last.gameObject);

            for (int i = 0; i < ropeNodes.Count-2; i++)
            {
                var force = ropeNodes[i].transform.up * 1.75f;
                ropeNodes[i].GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (ropeNodes.Count > 30)
            {
                return;
            }
            var first = ropeNodes[0];
            var newFirst = Instantiate(ropeNodePrefab, transform.position, Quaternion.identity);
            newFirst.RopeController = this;
            newFirst.name = "RopeNode";
            var ropeNode = newFirst;
            newFirst.transform.SetParent(transform);
            newFirst.transform.SetAsFirstSibling();

            ropeNode.AnchoredJoint2D.connectedBody = first.Rigidbody2D;
            attachedBody.connectedBody = ropeNode.Rigidbody2D;
            ropeNodes.Insert(0, ropeNode);
        }
    }

    private Vector3[] GetNodePositions()
    {
        Vector3[] positions = new Vector3[ropeNodes.Count];
        for (int i = 0; i < ropeNodes.Count; i++)
        {
            positions[i] = ropeNodes[i].transform.position;
        }
        return positions;
    }
}
