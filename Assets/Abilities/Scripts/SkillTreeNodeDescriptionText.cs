using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeNodeDescriptionText : MonoBehaviour {

    public SkillTree skillTree;

    public SkillTreeNode nodeUnderMouse;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Text>().text = "";
        nodeUnderMouse = getNodeUnderMouse();
        if (nodeUnderMouse != null) { GetComponent<Text>().text = nodeUnderMouse.nodeDescription; }
    }

    public SkillTreeNode getNodeUnderMouse()
    {
        if (skillTree.skillTreePanel.GetComponent<UIMouseListener>().underMouse) { return null; }
        Vector3 mousePosition = Input.mousePosition;
        List<Transform> nodelist = new List<Transform>();
        foreach (SkillTreeNode stn in skillTree.GetComponentsInChildren<SkillTreeNode>())
        {
            Transform node = stn.transform;
            float width = ((RectTransform)node).rect.width * node.lossyScale.x;
            if (node.position.x <= mousePosition.x &&
                node.position.y >= mousePosition.y &&
                node.position.x + width >= mousePosition.x &&
                node.position.y - width <= mousePosition.y)
            {
                return stn;
            }
        }
        return null;
    }
}
