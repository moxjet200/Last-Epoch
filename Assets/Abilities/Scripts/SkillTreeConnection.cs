using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeConnection : MonoBehaviour {

    public SkillTreeNode startNode;
    public SkillTreeNode endNode;
    int requirement = 0;

	// Use this for initialization
	public void OnTreeOpen () {
        // get start and end nodes
        Rect rect = GetComponent<RectTransform>().rect;
        if (startNode == null || endNode == null)
        {
            if (rect.width > rect.height)
            {
                startNode = getNearestNode(new Vector3(transform.position.x, transform.position.y, transform.position.z));
                endNode = getNearestNode(new Vector3(transform.position.x + 75, transform.position.y, transform.position.z));
            }
            else
            {
                startNode = getNearestNode(new Vector3(transform.position.x, transform.position.y - 60, transform.position.z));
                endNode = getNearestNode(new Vector3(transform.position.x, transform.position.y + 35, transform.position.z));
            }
        }
        // set the requirement number based on one node's requirement of the other
        calculateRequirement();
    }

    public SkillTreeNode getNearestNode(Vector3 position)
    {
        SkillTreeNode nearestNode = SkillTreeNode.all[0];
        float nearestDistance = float.MaxValue;
        float nodeDistance = 0f;
        foreach (SkillTreeNode node in SkillTreeNode.all)
        {
            if (node.transform.parent == transform.parent.parent)
            {
                nodeDistance = Vector3.Distance(node.transform.position, position);
                if (nodeDistance < nearestDistance)
                {
                    nearestNode = node;
                    nearestDistance = nodeDistance;
                }
            }
        }
        return nearestNode;
    }

    public void calculateRequirement()
    {
        // set the requirement number based on one node's requirement of the other
        requirement = 0;
        foreach (SkillTreeNode.RequirementFromNode req in startNode.requirements)
        {
            if (req.node == endNode)
            {
                requirement = req.requirement;
            }
        }
        foreach (SkillTreeNode.RequirementFromNode req in endNode.requirements)
        {
            if (req.node == startNode)
            {
                requirement = req.requirement;
            }
        }
        // change the number
        foreach (Transform child in transform)
        {
            if (child.name == "hRailNumber" || child.name == "vRailNumber")
            {
                if (requirement == 0)
                {
                    child.gameObject.SetActive(false);
                }
                else
                {
                    child.GetComponentInChildren<Text>().text = getRomanNumeral(requirement);
                }
            }
        }
    }

    public void updateNumberRotationAndPosition()
    {
        foreach (Transform child in transform)
        {
            if (child.name == "hRailNumber" || child.name == "vRailNumber")
            {
                child.transform.parent = startNode.transform.parent;
                child.transform.position = (startNode.transform.position + endNode.transform.position) / 2;
                child.transform.parent = transform;
                child.transform.eulerAngles = new Vector3(0,0,0);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (startNode && endNode)
        {
            if (startNode.taken() && endNode.taken())
            {
                GetComponent<Image>().color = new Color(0.99f, 0.71f, 0.38f);
            }
            else if (startNode.requirementsMet() && endNode.requirementsMet() && (startNode.pointsAllocated>= requirement || endNode.pointsAllocated >= requirement))
            {
                GetComponent<Image>().color = new Color(0.49f, 0.28f, 0.14f);
            }
            else
            {
                GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);
            }
        }
	}

    // returns a roman numeral for a given number
    public string getRomanNumeral(int number)
    {
        if (number == 1) { return "I"; }
        if (number == 2) { return "II"; }
        if (number == 3) { return "III"; }
        if (number == 4) { return "IV"; }
        if (number == 5) { return "V"; }
        if (number == 6) { return "VI"; }
        if (number == 7) { return "VII"; }
        if (number == 8) { return "VIII"; }
        if (number == 9) { return "IX"; }
        if (number == 10) { return "X"; }
        if (number == 11) { return "XI"; }
        if (number == 12) { return "XII"; }
        if (number == 13) { return "XIII"; }
        if (number == 14) { return "XIV"; }
        if (number == 15) { return "XV"; }
        if (number == 16) { return "XVI"; }
        if (number == 17) { return "XVII"; }
        if (number == 18) { return "XVIII"; }
        if (number == 19) { return "XIX"; }
        if (number == 20) { return "XX"; }
        else { return "0"; }
    }


}
