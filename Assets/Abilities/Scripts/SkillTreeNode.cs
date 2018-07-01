using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;
using Sirenix.OdinInspector;

[RequireComponent(typeof(SkillnodeTooltip))]
public class SkillTreeNode : MonoBehaviour {

    public static List<SkillTreeNode> all = new List<SkillTreeNode>();

    [System.Serializable]
    public class TextOrTMP {
        
        public Text text = null;
        public TextMeshProUGUI tmp = null;


        public bool hasText()
        {
            if (text != null || tmp != null) { return true; }
            return false;
        }

        public string getText()
        {
            if (tmp) { return tmp.text; }
            else if (text) { return text.text; }
            else return "";
        }

        public void setText(string _text)
        {
            if (tmp) { tmp.text = _text; }
            if (text) { text.text = _text; }
        }

        public void setColour(Color colour)
        {
            if (tmp) { tmp.color = colour; }
            if (text) { text.color = colour; }
        }

    }


    [System.Serializable]
    public struct RequirementFromNode
    {
        public SkillTreeNode node;
        public int requirement;
    }

    public enum nodeRequirementType
    {
        node, axis
    }
    
    public Tree tree;
    public int pointsAllocated = 0;
    public int maxPoints = 1;
    public nodeRequirementType nodeType = nodeRequirementType.node;
    public string nodeID = "";
    public string nodeName = "";
    [TextArea][FormerlySerializedAs("description")]
    public string nodeDescription = "";
	public string loreText = "";


    Image icon = null;
	Image frame = null;
	Image flashingFrame = null;

    int baseSize = 50;
    int size = 50;

    public GameObject connectionPrefab = null;
    public List<RequirementFromNode> requirements = new List<RequirementFromNode>();
    public List<RequirementFromNode> necessaryRequirements = new List<RequirementFromNode>();
    // nodes are placed in this list when a connection to them is initialised
    [HideInInspector]
    public List<SkillTreeNode> nodesAlreadyConnectedTo = new List<SkillTreeNode>();

    public List<Axis> axisRequirements = new List<Axis>();
    public Transform axisCentre = null;
    public TextOrTMP pointsText = null;

    // Use this for initialization
    void Awake () {
		if (tree == null)
        {
            // go through parents trying to find a skill tree
            tree = findTreeInParents(transform.parent);
            tree.nodeList.Add(this);
        }
        // find the points text
        if (pointsText == null || (pointsText.text == null && pointsText.tmp == null))
        {
            pointsText = new TextOrTMP();
            pointsText.tmp = GetComponentInChildren<TextMeshProUGUI>();
            if (pointsText.tmp == null) { pointsText.text = GetComponentInChildren<Text>(); }
        }
        updateText();
        updateSize();
        // find the icon
        foreach (Image image in GetComponentsInChildren<Image>(true))
        {
            if (image.name == "Icon")
            {
                icon = image;
            }
            else if (image.name == "Border")
            {
                frame = image;
            }
			else if (image.name == "FlashingBorder")
			{
				flashingFrame = image;
			}
        }
        if (icon == null) { icon = GetComponent<Image>(); }
        all.Add(this);
        // initialise connections
        foreach (RequirementFromNode req in requirements)
        {
            if (!req.node.nodesAlreadyConnectedTo.Contains(this))
            {
                nodesAlreadyConnectedTo.Add(req.node);
                GameObject connectionObject = Instantiate(connectionPrefab, transform.parent);
                SkillTreeConnection newConnection = connectionObject.GetComponent<SkillTreeConnection>();
                newConnection.startNode = this;
                newConnection.endNode = req.node;
                newConnection.calculateRequirement();
                RectTransform connectionTransform = newConnection.transform as RectTransform;
                // stretch from this node to the other node
                if (connectionTransform){
                    Vector2 centre = getCentre();
                    connectionTransform.position = centre;
                    Vector3 lossyScale = transform.lossyScale;
                    connectionTransform.sizeDelta = new Vector2(connectionTransform.rect.width, 
                        Vector2.Distance(new Vector2(centre.x / lossyScale.x, centre.y / lossyScale.y), new Vector2(req.node.getCentre().x / lossyScale.x, req.node.getCentre().y / lossyScale.y)));
                    // rotate
                    float newZ = Vector2.Angle(req.node.transform.position - transform.position, new Vector2(0,1));
                    Vector3 cross = Vector3.Cross(req.node.transform.position - transform.position, new Vector2(0, 1));
                    if (cross.z > 0) { newZ = 180 - newZ; }
                    else { newZ = 180 + newZ; }
                    if (newZ != 0)
                    {
                        connectionTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, newZ));
                    }

                    newConnection.updateNumberRotationAndPosition();
                    connectionTransform.SetAsFirstSibling();
                }
            }
        }

    }

    public Vector2 getCentre()
    {
        return new Vector2(transform.position.x, transform.position.y);

        //RectTransform rectT = transform as RectTransform;
        //if (rectT)
        //{
        //    return new Vector2(transform.position.x + rectT.rect.width / 2, transform.position.y + rectT.rect.height / 2);
        //}

        //return new Vector2(0, 0); 
    }

    void OnDestroy()
    {
        all.Remove(this);
    }

    Tree findTreeInParents(Transform transform)
    {
        if (transform.GetComponent<Tree>())
        {
            return transform.GetComponent<Tree>();
        }
        else
        {
            if (transform.parent == null)
            {
                return null;
            }
            else
            {
                return findTreeInParents(transform.parent);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        // make the image darker if it is not taken
        if (nodeType != nodeRequirementType.axis && tree)
        {
            if (taken())
            {
				icon.color = Color.white;
				if (frame) { frame.color = Color.white; }
				if (flashingFrame) { flashingFrame.color = new Color(0f, 0f, 0f, 0f); }
            }
            else if (requirementsMet())
            {
				icon.color = new Color(0.7f, 0.7f, 0.7f);
                SkillTree skillTree = tree as SkillTree;
				if (tree.unspentPoints > 0 && (!skillTree || skillTree.specialised)) {
					if (frame) {
						frame.color = new Color(0f, 0f, 0f, 0f);
					}
					if (flashingFrame) {
						flashingFrame.color = Color.white;
					}
				} else {
					if (frame) {
						frame.color = new Color(0.7f, 0.7f, 0.7f);
					}
					if (flashingFrame) {
						flashingFrame.color = new Color(0f, 0f, 0f, 0f);
					}

				}
            }
            else
            {
                icon.color = new Color(0.25f, 0.25f, 0.25f);
                if (frame) {
                    if (tree && (tree as CharacterTree != null))
                    {
						frame.color = new Color(0.1f, 0.1f, 0.1f, 1f);
						flashingFrame.color = new Color(0f, 0f, 0f, 0f);
                    }
                    else
                    {
						frame.color = new Color(0.25f, 0.25f, 0.25f, 1f);
						flashingFrame.color = new Color(0f, 0f, 0f, 0f);
                    }
                }
            }
        }
        // make the text gold depending on the points taken
        if (GetComponentInChildren<Text>())
        {
            if (maxPoints == pointsAllocated)
            {
                pointsText.setColour(new Color(0.72f, 0.53f, 0.23f));
            }
            else if (pointsAllocated > 0)
            {
                pointsText.setColour(new Color(0.95f, 0.77f, 0.53f));
            }
            else
            {
                pointsText.setColour(new Color(1f, 1f, 1f));
            }
        }
	}

    public void Clicked()
    {
        // return if the player has not specialised in this ability
        SkillTree skillTree = tree as SkillTree;
        if (skillTree)
        {
            if (!PlayerFinder.getPlayer().GetComponent<SpecialisedAbilityList>().abilities.Contains(skillTree.ability)) { return; }
        }

        // check if there is a spare point and this is not at max points
        if ((nodeType == nodeRequirementType.axis && tree.unspentAxisPoints > 0) || (nodeType == nodeRequirementType.node && tree.unspentPoints > 0))
        {
            if (pointsAllocated < maxPoints || maxPoints < 0)
            {
                // if the requirements are met then allocate the point
                if (requirementsMet())
                {
                    pointsAllocated++;
                    if (nodeType == nodeRequirementType.node) { tree.unspentPoints--; }
                    if (nodeType == nodeRequirementType.axis) { tree.unspentAxisPoints--; }
                    tree.updateMutator();
                    updateText();
                    // play a sound
                    UISounds.playSound(UISounds.UISoundLabel.AllocateNode);
                    // update mana costs on tooltips
                    SkillTree st = tree as SkillTree;
                    if (st && st.ability)
                    {
                        AbilityTooltip.updateManaCosts(st.ability);
                    }
                    // update the node progression (for respeccing)
                    if (tree as CharacterTree)
                    {
                        SkillSavingManager.instance.pushToNodePogression(nodeID);
                    }
                }
            }
        }

        // save
        SkillSavingManager.instance.saveNodeData(this);
        PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>().SaveCharacterData();
    }

    public bool requirementsMet()
    {
        // check node requirements
        bool nodeRequirementsMet = false;
        if (requirements.Count == 0 && necessaryRequirements.Count == 0) { nodeRequirementsMet = true; }
        else
        {
            // check if a necessary requirement is met
            foreach (RequirementFromNode requirement in necessaryRequirements)
            {
                if (requirement.node.pointsAllocated >= requirement.requirement)
                {
                    nodeRequirementsMet = true;
                }
            }
            // check if a regular requirement is met
            foreach (RequirementFromNode requirement in requirements)
            {
                if (requirement.node.pointsAllocated >= requirement.requirement)
                {
                    nodeRequirementsMet = true;
                }
            }
            // check if a necessary requirement is not met
            foreach (RequirementFromNode requirement in necessaryRequirements)
            {
                if (requirement.node.pointsAllocated < requirement.requirement)
                {
                    nodeRequirementsMet = false;
                }
            }
        }

        // axis requirements
        bool axisRequirementsMet = false;
        if (axisRequirements == null || axisRequirements.Count == 0) { axisRequirementsMet = true; }
        else
        {
            if (axisRequirements.Count != 2) { Debug.LogError("Axis requirement must be of size 0 or 2, but " + name + " has an axis requirement of size " + axisRequirements.Count); return false; }
            Vector3 a1p = axisRequirements[0].transform.position;
            Vector3 a2p = axisRequirements[1].transform.position;
            float centreSign = Mathf.Sign((axisCentre.position.x - a1p.x) * (a2p.y - a1p.y) - (axisCentre.position.y - a1p.y) * (a2p.x - a1p.x));

            List<Vector3> points = new List<Vector3>();
            
            points.Add(transform.position);
            Vector3 lossyScale = transform.lossyScale;
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            Rect rect = (transform as RectTransform).rect;
            points.Add(pos - new Vector2((rect.width / 2) * lossyScale.x, 0));
            points.Add(pos + new Vector2((rect.width / 2) * lossyScale.x, 0));
            points.Add(pos - new Vector2(0, (rect.height / 2) * lossyScale.y));
            points.Add(pos + new Vector2(0, (rect.height / 2) * lossyScale.y));
            points.Add(pos - 0.7f * (new Vector2((rect.width / 2) * lossyScale.x, (rect.height / 2) * lossyScale.y)));
            points.Add(pos + 0.7f * (new Vector2((rect.width / 2) * lossyScale.x, (rect.height / 2) * lossyScale.y)));
            points.Add(pos - 0.7f * (new Vector2(-(rect.width / 2) * lossyScale.x, (rect.height / 2) * lossyScale.y)));
            points.Add(pos + 0.7f * (new Vector2(-(rect.width / 2) * lossyScale.x, (rect.height / 2) * lossyScale.y)));

            foreach (Vector2 point in points) {
                if (centreSign == Mathf.Sign((point.x - a1p.x) * (a2p.y - a1p.y) - (point.y - a1p.y) * (a2p.x - a1p.x)))
                {
                    axisRequirementsMet = true;
                }
            }
            //d = (x−x1)(y2−y1)−(y−y1)(x2−x1)
        }

        // return true if both types of requirement are met
        return (nodeRequirementsMet && axisRequirementsMet);
    }

    public bool taken()
    {
        if (pointsAllocated > 0 || maxPoints == 0)
        {
            return true;
        }
        return false;
    }

    public void updateText()
    {
        string text = "" + pointsAllocated.ToString();
        if (maxPoints > 0)
        {
			text += "/" + maxPoints.ToString();
        }

		if (nodeType == nodeRequirementType.axis) {
			pointsText.tmp.GetComponent<PassiveBonusRef> ().passiveTextScript.UpdateText (pointsAllocated);
		}
        pointsText.setText(text);
    }

    public void updateSize()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(size, size);
        rect.localPosition = new Vector3(rect.localPosition.x - (size - baseSize) / 2, rect.localPosition.y + (size - baseSize) / 2, 0);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
        foreach (RequirementFromNode req in requirements)
        {
            if (req.node != null)
            {
                Gizmos.DrawLine(getCentre(), req.node.getCentre());
            }
        }
    }

    public void respec(bool updateMutator = true)
    {
        if (pointsAllocated > 0)
        {
            if (nodeType == nodeRequirementType.node) { tree.unspentPoints += pointsAllocated; }
            else if (nodeType == nodeRequirementType.axis) { tree.unspentAxisPoints += pointsAllocated; }
            pointsAllocated = 0;
            if (updateMutator) { tree.updateMutator(); }
            updateText();
            // save
            if (tree as CharacterTree)
            {
                SkillSavingManager.instance.removeNodeFromPogression(nodeID);
            }
            SkillSavingManager.instance.saveNodeData(this);
            PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>().SaveCharacterData();
        }
    }

    public void respecOnePoint(bool updateMutator = true)
    {
        if (pointsAllocated > 0)
        {
            if (nodeType == nodeRequirementType.node) { tree.unspentPoints += 1; }
            else if (nodeType == nodeRequirementType.axis) { tree.unspentAxisPoints += 1; }
            pointsAllocated--;
            if (updateMutator) { tree.updateMutator(); }
            updateText();
            // save
            if (tree as CharacterTree)
            {
                SkillSavingManager.instance.PopNodeProgression(nodeID);
            }
            SkillSavingManager.instance.saveNodeData(this);
            PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>().SaveCharacterData();
        }
    }
}