using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AbilityTooltip))]
public class AbilityBarIcon : MonoBehaviour {

    public static List<AbilityBarIcon> all = new List<AbilityBarIcon>();

	[HideInInspector] public GameObject player;
    Image icon;

    public int abilityNumber = 1;
	private AbilityTooltip tooltipAbility;
    public Ability ability = null;

    public Image cooldownBar = null;
    public bool cooldownBarActive = true;
    UIBarWithMaterial outOfMana = null;

    BaseMana playerMana = null;
    ChargeManager playerCharges = null;

    void Awake()
    {
        all.Add(this);
    }

    void OnDestroy()
    {
        all.Remove(this);
        PlayerFinder.playerChangedEvent -= ChangePlayer;
    }

	// Use this for initialization
	void Start () {
        player = PlayerFinder.getPlayer();
        PlayerFinder.playerChangedEvent += ChangePlayer;
		if (player != null) {
			playerMana = player.GetComponent<BaseMana> ();
			playerCharges = player.GetComponent<ChargeManager> ();
		}
		tooltipAbility = GetComponent<AbilityTooltip> ();
        foreach (UIBarWithMaterial bar in GetComponentsInChildren<UIBarWithMaterial>())
        {
            //if (bar.name == "CooldownBar") { bar.currentFill = bar.maxFill; }
            //else 
            if (bar.name == "OutOfMana") { outOfMana = bar; }
        }
        foreach (Image im in GetComponentsInChildren<Image>())
        {
            if (im.name == "Sprite") { icon = im; }
            if (im.name == "CooldownBar") { cooldownBar = im; }
        }
	}

    public void ChangePlayer(GameObject newPlayer)
    {
        player = newPlayer;
		if (player != null) {
			playerMana = player.GetComponent<BaseMana> ();
			playerCharges = player.GetComponent<ChargeManager> ();
		}
    }
	
	// Update is called once per frame
	void Update () { //TODO make this only trigger when the ability is changed
        if (!player ) { return; }
        
        AbilityList abilityList = player.GetComponent<AbilityList>();
        if (abilityList.abilities.Count < abilityNumber) { return; }

        if (ability != abilityList.abilities[abilityNumber - 1])
        {
            // update the ability
            ability = abilityList.abilities[abilityNumber - 1];
            // update the tooltip
			tooltipAbility.SetAbility(abilityList.abilities[abilityNumber - 1]);
            // update the icon
            icon.sprite = abilityList.abilities[abilityNumber - 1].abilitySprite;
            // activate or deactivate the cooldown bar
            //activateCooldownBarIfNecessary();
            // update the number of charges in the charge display
            //chargeDisplay.updateMaxCharges((int) playerCharges.maxCharges[abilityNumber - 1]);
            // inform the skill saving manager
            save();
        }
        
        if (!playerMana.hasEnoughManaToUse(ability))
        {
            if (!outOfMana.gameObject.activeSelf)
            {
                outOfMana.gameObject.SetActive(true);
            }
        }
        else { outOfMana.gameObject.SetActive(false); }

        //if (cooldownBar.gameObject.activeSelf)
        //{
        //    cooldownBar.currentFill = 1 - playerCharges.getChargesFloat(ability);
        //    if (cooldownBar.currentFill < 0) { cooldownBar.currentFill = 0; }
        //}

        if (cooldownBarActive && cooldownBar && !cooldownBar.gameObject.activeSelf)
        {
            cooldownBar.gameObject.SetActive(true);
        }

        if (!cooldownBarActive && cooldownBar && cooldownBar.gameObject.activeSelf)
        {
            cooldownBar.gameObject.SetActive(false);
        }

    }

    public void save()
    {
		if (ability != null) {
			SkillSavingManager.selectionChange (abilityNumber, ability.playerAbilityID);
		}
    }

    public void clicked()
    {
        UIBase.instance.openSkills();
        AbilityPanelManager.instance.abilityClicked = abilityNumber;
    }

    public void activateCooldownBarIfNecessary()
    {
        if (player.GetComponent<ChargeManager>().hasCooldown(ability))
        {
            cooldownBar.gameObject.SetActive(true);
        }
        else
        {
            cooldownBar.gameObject.SetActive(false);
        }
    }

    public void activateCooldownBar()
    {
        cooldownBarActive = true;
    }

    public void deactivateCooldownBar()
    {
        cooldownBarActive = false;
    }

}


