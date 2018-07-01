using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSavingManager : MonoBehaviour {

    public static SkillSavingManager instance = null;

    // Use this for initialization
    void Awake () {
        instance = this;
	}

    public static void selectionChange(int abilityNumber1to5, string newPlayerAbilityID)
    {
        CharacterDataTracker tracker = PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>();
        if (tracker.charData.abilityBar == null) {
            tracker.charData.abilityBar = new List<string>();
        }
        while (tracker.charData.abilityBar.Count < abilityNumber1to5)
        {
            tracker.charData.abilityBar.Add("");
        }
        tracker.charData.abilityBar[abilityNumber1to5 - 1] = newPlayerAbilityID;
        
        if (tracker.charData.characterName != "")
        {
            tracker.SaveCharacterData();
        }
    }

    public static void loadAbilityBar()
    {
        CharacterDataTracker tracker = PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>();
        AbilityList abilityList = PlayerFinder.getPlayer().GetComponent<AbilityList>();
        List<string> savedAbilities = tracker.charData.abilityBar;
        for (int i = 0; i < savedAbilities.Count; i++)
        {
            Ability ability = AbilityIDList.getPlayerAbility(savedAbilities[i]);
            if (ability == null)
            {
                abilityList.abilities[i] = AbilityIDList.getAbility(AbilityID.nullAbility);
            }
            else
            {
                abilityList.abilities[i] = ability;
            }
        }
    }


    public void loadSkillTrees(List<CharacterData.SavedSkillTree> trees)
    {
        foreach (CharacterData.SavedSkillTree tree in trees)
        {
            loadAbility(tree.treeID, tree.slotNumber, tree.abilityXP, tree.unspentPoints, tree.nodesTaken);
        }
    }

    public void loadCharacterTree(CharacterData.SavedCharacterTree tree)
    {
        loadTree(tree.treeID, tree.unspentPoints, tree.unspentAxisPoints, tree.nodesTaken);
    }

    public void loadAbility(string playerAbilityID, int slotNumber, float abilityXP, int unspentPoints, List<StringIntPair> nodesTaken)
    {
        UIBase uiBase = UIBase.instance;
        uiBase.openSkills();
        // specialised in the ability
        SpecialisedAbilityList specialisedAbilityList = PlayerFinder.getPlayer().GetComponent<SpecialisedAbilityList>();
        specialisedAbilityList.Specialise(AbilityIDList.getPlayerAbility(playerAbilityID), slotNumber, false);
        // set the ability xp
        specialisedAbilityList.abilityXP[slotNumber] = abilityXP;
        // set the ability level
        specialisedAbilityList.abilityLevels[slotNumber] = SpecialisedAbilityManager.getAbilityLevel(abilityXP);

        // update the tree
        loadTree(playerAbilityID, unspentPoints, 0, nodesTaken);

        uiBase.closeSkills();
    }

    public void loadTree(string treeID, int unspentPoints, int unspentAxisPoints, List<StringIntPair> nodesTaken)
    {
        // open the relevant parts of the ui
        UIBase uiBase = UIBase.instance;
        uiBase.openSkills();
        uiBase.openPassiveTree(true);
        // get a list of the saved node IDs for iterating through later
        List<string> savedNodeIDs = new List<string>();
        Dictionary<string, int> nodeIDsToPositionInNodesTaken = new Dictionary<string, int>();
        for (int i = 0; i < nodesTaken.Count; i++)
        {
            savedNodeIDs.Add(nodesTaken[i].key);
            nodeIDsToPositionInNodesTaken.Add(nodesTaken[i].key, i);
        }
        // find the correct tree
        foreach (Tree tree in FindObjectsOfType<Tree>())
        {
            if (tree.treeID == treeID)
            {
                tree.unspentPoints = unspentPoints;
                tree.unspentAxisPoints = unspentAxisPoints;
                SkillTree skillTree = tree as SkillTree;
                if (skillTree) { skillTree.open(); }
                foreach (SkillTreeNode node in tree.nodeList)
                {
                    if (savedNodeIDs.Contains(node.nodeID))
                    {
                        node.pointsAllocated = nodesTaken[nodeIDsToPositionInNodesTaken[node.nodeID]].value;
                        node.updateText();
                    }
                }
                tree.updateMutator();
            }
        }
        uiBase.closeSkillTrees();
        uiBase.closeSkills();
        uiBase.closePassiveTree();
    }

    public void saveNodeData(SkillTreeNode node)
    {
        Tree tree = node.tree;
        SkillTree skillTree = tree as SkillTree;
        CharacterData data = PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>().charData;
        // if it's a skill tree find the correct saved skill tree and update it
        if (skillTree)
        {
            List<CharacterData.SavedSkillTree> skillTrees = data.savedSkillTrees;
            foreach (CharacterData.SavedSkillTree sst in skillTrees)
            {
                if (sst.treeID == tree.treeID)
                {
                    sst.unspentPoints = tree.unspentPoints;
                    // look for the node in the dictionary
                    foreach (StringIntPair pair in sst.nodesTaken)
                    {
                        if (pair.key == node.nodeID)
                        {
                            pair.value = node.pointsAllocated;
                            return;
                        }
                    }
                    // if the node has not been found in the dictionary add it
                    sst.nodesTaken.Add(new StringIntPair(node.nodeID, node.pointsAllocated));
                    return;
                }
            }
        }
        // if it's not a skill tree update the character tree
        else
        {
            // initialise the character tree data if necessary
            if (data.savedCharacterTree == null)
            {
                data.savedCharacterTree = new CharacterData.SavedCharacterTree(tree.treeID, tree.unspentPoints, tree.unspentAxisPoints);
            }
            else
            {
                data.savedCharacterTree.treeID = tree.treeID;
                data.savedCharacterTree.unspentPoints = tree.unspentPoints;
                data.savedCharacterTree.unspentAxisPoints = tree.unspentAxisPoints;
            }
            // look for the node in the list
            foreach (StringIntPair pair in data.savedCharacterTree.nodesTaken)
            {
                if (pair.key == node.nodeID)
                {
                    pair.value = node.pointsAllocated;
                    return;
                }
            }
            // if the node has not been found in the dictionary add it
            data.savedCharacterTree.nodesTaken.Add(new StringIntPair(node.nodeID, node.pointsAllocated));
            return;
        }
    }

    public void saveUnspentPoints(Tree tree)
    {
        SkillTree skillTree = tree as SkillTree;
        CharacterData data = PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>().charData;
        // if it's a skill tree find the correct saved skill tree and update it
        if (skillTree)
        {
            List<CharacterData.SavedSkillTree> skillTrees = data.savedSkillTrees;
            foreach (CharacterData.SavedSkillTree sst in skillTrees)
            {
                if (sst.treeID == tree.treeID)
                {
                    sst.unspentPoints = tree.unspentPoints;
                }
            }
        }
        else
        {
            // initialise the character tree data if necessary
            if (data.savedCharacterTree == null)
            {
                data.savedCharacterTree = new CharacterData.SavedCharacterTree(tree.treeID, tree.unspentPoints, tree.unspentAxisPoints);
            }
            else
            {
                data.savedCharacterTree.treeID = tree.treeID;
                data.savedCharacterTree.unspentPoints = tree.unspentPoints;
                data.savedCharacterTree.unspentAxisPoints = tree.unspentAxisPoints;
            }
        }
    }

    public void despecialise(string oldPlayerAbilityID)
    {
        CharacterData data = PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>().charData;
        if (oldPlayerAbilityID != "")
        {
            CharacterData.SavedSkillTree treeToRemove = null;
            foreach (CharacterData.SavedSkillTree tree in data.savedSkillTrees)
            {
                if (tree.treeID == oldPlayerAbilityID)
                {
                    treeToRemove = tree;
                }
            }
            if (treeToRemove == null) { Debug.LogError("Failed to remove saved tree with id " + oldPlayerAbilityID); }
            else
            {
                data.savedSkillTrees.Remove(treeToRemove);
            }
        }
    }

    public void changeSpecialisation(string newPlayerAbilityID, int slotNumber, string oldPlayerAbilityID = "")
    {
        CharacterData data = PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>().charData;
        // remove the old tree if an old PlayerAbilityID is given
        if (oldPlayerAbilityID != "")
        {
            CharacterData.SavedSkillTree treeToRemove = null;
            foreach (CharacterData.SavedSkillTree tree in data.savedSkillTrees)
            {
                if (tree.treeID == oldPlayerAbilityID)
                {
                    treeToRemove = tree;
                }
            }
            if (treeToRemove == null) { Debug.LogError("Failed to remove saved tree with id " + oldPlayerAbilityID); }
            else {
                data.savedSkillTrees.Remove(treeToRemove);
            }
        }
        // save the new ability
        data.savedSkillTrees.Add(new CharacterData.SavedSkillTree(newPlayerAbilityID, slotNumber, 0, 1));
    }

    public void saveAbilityXP(SpecialisedAbilityList sal)
    {
        CharacterData data = PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>().charData;
        for (int i = 0; i < sal.abilities.Count; i++)
        {
            foreach (CharacterData.SavedSkillTree tree in data.savedSkillTrees)
            {
                if (tree.treeID == sal.abilities[i].playerAbilityID)
                {
                    tree.abilityXP = (int) sal.abilityXP[i];
                }
            }
        }
        PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>().SaveCharacterData();
    }

    public void pushToNodePogression(string nodeID)
    {
        CharacterData data = PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>().charData;
        data.characterTreeNodeProgression.Add(nodeID);
        PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>().SaveCharacterData();
    }

    public void PopNodeProgression(string nodeID = "")
    {
        CharacterData data = PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>().charData;
        if (data.characterTreeNodeProgression.Count > 0)
        {
            if (nodeID != "")
            {
                if (data.characterTreeNodeProgression[data.characterTreeNodeProgression.Count - 1] != nodeID)
                {
                    return;
                }
            }

            data.characterTreeNodeProgression.RemoveAt(data.characterTreeNodeProgression.Count - 1);
        }
        PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>().SaveCharacterData();
    }

    public void removeNodeFromPogression(string nodeID)
    {
        CharacterData data = PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>().charData;
        data.characterTreeNodeProgression.RemoveAll(x => x == nodeID);
        PlayerFinder.getPlayer().GetComponent<CharacterDataTracker>().SaveCharacterData();
    }
}
