using System.Collections.Generic;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    private static List<RopeController> ropes;

    [SerializeField]
    protected RopeNode ropeNodePrefab;

    [SerializeField]
    protected int minRopeNodes = 8;

    private List<RopeNode> ropeNodes;
    private float ropeNodeDistance;
    private bool isAttached;
    private NinjaController attachedBody;

    public bool IsAttached
    {
        get {return isAttached; }
    }

    public Transform FirstRopeNode
    {
        get { return ropeNodes[0].transform; }
    }

    public Transform LastRopeNode
    {
        get { return ropeNodes[ropeNodes.Count - 1].transform; }
    }

    public static void DestroyAllRopes()
    {
        if (ropes == null)
        {
            return;
        }
        int length = ropes.Count;
        for (int i = 0; i < length; i++)
        {
            Destroy(ropes[i].gameObject);;
        }
        ropes.Clear();
    }

    public void AttachRope(Vector2 position, NinjaController newBody)
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
            ropeNode.transform.SetParent(transform);
            ropeNodes.Add(ropeNode);
        }
        attachedBody.AnchoredJoint2D.connectedBody = ropeNodes[0].Rigidbody2D;
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
        if (index < 0)
        {
            return;
        }

        if (ropeNodes.Count <= minRopeNodes || index > ropeNodes.Count - minRopeNodes) //remaining rope is too short, end it
        {
            var last = ropeNodes[ropeNodes.Count - 1];
            last.AnchoredJoint2D.enabled = false;
        }
        else
        {
            ropeNodes.RemoveRange(0, index);
        }

        if (attachedBody != null)
        {
            DetachRope();
        }
    }

    public void DetachRope()
    {
        if (attachedBody == null)
        {
            return;
        }
        var last = ropeNodes[ropeNodes.Count - 1];
        isAttached = false;
        attachedBody.AnchoredJoint2D.enabled = false;
        attachedBody.RemoveRopeController();
        attachedBody = null;
    }

    public void AttachNinja(NinjaController ninja)
    {
        attachedBody = ninja;
    }

    public RopeNode GetNextRopeNode(RopeNode current, float vertical)
    {
        int index = ropeNodes.IndexOf(current);
        if (index < 0)
        {
            return null;
        }

        if (vertical > 0)
        {
            return index == ropeNodes.Count - 1 ? null : ropeNodes[index + 1];
        }
        else
        {
            return index == 0 ? null : ropeNodes[index - 1];
        }
    }

    public void RopeLengthChange(float vericalAxis)
    {
        if (vericalAxis > 0)
        {
            ShortenRope();
        }
        else
        {
            LengthenRope();
        }
    }

    private void ShortenRope()
    {
        if (ropeNodes.Count < minRopeNodes)
        {
            return;
        }
        var last = ropeNodes[ropeNodes.Count - 1];
        var secondlast = ropeNodes[ropeNodes.Count - 2];
        secondlast.AnchoredJoint2D.connectedAnchor = last.AnchoredJoint2D.connectedAnchor;
        secondlast.AnchoredJoint2D.connectedBody = null;
        ropeNodes.Remove(last);
        Destroy(last.gameObject);
    }

    private void LengthenRope()
    {
        if (ropeNodes.Count > 30 || ropeNodes.Count == 0)
        {
            return;
        }
        var first = ropeNodes[0];
        var newFirst = Instantiate(ropeNodePrefab, first.transform.position, Quaternion.identity);
        newFirst.RopeController = this;
        newFirst.name = "RopeNode";
        var ropeNode = newFirst;
        newFirst.transform.parent = transform;
        newFirst.transform.SetAsFirstSibling();

        ropeNode.AnchoredJoint2D.connectedBody = first.Rigidbody2D;
        attachedBody.AnchoredJoint2D.connectedBody = ropeNode.Rigidbody2D;
        ropeNodes.Insert(0, ropeNode);
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

    private void Start()
    {
        if (ropes == null)
        {
            ropes = new List<RopeController>();
        }
        ropes.Add(this);
    }

    private void OnDestroy()
    {
        ropes.Remove(this);
    }
}
