using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability")]
[System.Serializable]
public class Ability : ScriptableObject
{
    public string abilityName = "New Ability";
    public Sprite abilitySprite;
    [Tooltip("This ID is used for saving and loading the ability")]
    public string playerAbilityID = "";
    [Tooltip("The delay between pressing the button to use the ability and the ability object being created")]
    public float useDelay = 0.5f;
    [Tooltip("The delay between pressing the button to use the ability and being able to use the ability again")]
    public float useDuration = 1f;
    public float manaCost = 0f;
    [Tooltip("The prefab containing this ability's component")]
    public GameObject abilityPrefab;
    [Tooltip("The animation instruction for the actor using the ability")]
    public AbilityAnimation animation = AbilityAnimation.Cast;
    [Tooltip("If there is a shared hit detector then events like newEnemyHit will only be called the first time that enemy is hit by any objects from the same use. This can be used to prevent shotgunning and infinite chaining.")]
    public bool sharedHitDetector = false;
    [Tooltip("Decides whether this scales with attackspeed or cast speed")]
    public Tags.Properties speedScaler = Tags.Properties.CastSpeed;
    [Tooltip("Multiplies the animation speed and the speed at which useDelay and useDuration are reached")]
    public float speedMultiplier = 1f;
    [Tooltip("Affects what sort of attack/cast speed affects this ability")]
    public List<Tags.AbilityTags> useTags = new List<Tags.AbilityTags>();
    [Tooltip("Listed alongside use tags on the tooltip, but have no actuall affect. Used for tagging skills as 'minion' without making their cast time scale with minion cast speed.")]
    public List<Tags.AbilityTags> fakeUseTags = new List<Tags.AbilityTags>();
    [FMODUnity.EventRef]
    [Tooltip("This sound is played when an actor uses this ability, not when it is instantiated by another ability.")]
    public string useSound = "";
    [FMODUnity.EventRef]
    [Tooltip("This sound is played when an actor begins to use this ability")]
    public string beginUseSound = "";

    [Header("DelayedCasts")]
    public int delayedCasts = 0;
    public float delayedCastDuration = 0.25f;

    [Header("Charges")]
    [Tooltip("If an ability does not use charges then the max charges should be 0")]
    public float maxCharges = 0f;
    public float chargesGainedPerSecond = 0f;

    [Header("Casting Requirments")]
    [Tooltip("If true, this ability can only be cast at target locations that the caster could walk to")]
    public bool requiresAccessibleTargetLocation = false;
    [Tooltip("If true, this ability will be cast from a cast point if one with the correct animation requirement is present on the caster")]
    public bool useCastPoints = true;
    [Tooltip("Instant cast skills can be used instantly at any time, as long as you are not dead or stunned")]
    public bool instantCastForPlayer = false;
    
    [Header("Channelling")]
    [Tooltip("Is this a channelled ability")]
    public bool channelled = false;
    [Tooltip("Can the target location be changed during the channel")]
    public bool canUpdateTargetWhileChannelling = false;

    [Header("Movement")]
    [Tooltip("If the option is enabled this will cause the player to move into range before using the ability")]
    public bool moveIntoRangeBeforeUse = false;
    [Tooltip("The range at which to stop if the player moves into range before using the ability")]
    public float stopRange = 1f;
    public bool stopRangeAffectedByWeaponRange = false;
    [Space(10)]
    [Tooltip("If this is not not 'None' then the ability will require a movement to be complete before the use is finished")]
    public AbilityMovement movementDuringUse = AbilityMovement.None;
    public float baseMovementDuration = 0f;
    public float movementDurationPerUnitDistance = 0f;
    public bool immuneDuringMovement = false;
    [Tooltip("If this is 1 then then movement will stop 1 unit in front of the target, if it is -1 it will stop 1 unity behind the target")]
    public float landingDistanceFromTarget = 0f;
    [Tooltip("If true then can land further back from the target than it started")]
    public bool canMoveBack = false;
    [Tooltip("If true then the animation speed will be adapted so that the animation will finish at the end of movement if it has a length of 1 second")]
    public bool animationDurationMatchesMovement = false;
    [Tooltip("If true then the animation speed will be adapted so that the animation will finish at the end of movement if it has a length of 1 second")]
    public float baseMovementAnimationLength = 1f;
    [Tooltip("Prevents traversing areas off the navmesh with this movement ability")]
    public bool stopAtEdgeOfNavmesh = false;
    [Tooltip("Keeps resetting height to ground height during movement")]
    public bool stayAtGroundHeight = false;
    [Tooltip("When mana runs out is as if this ability has reached its destination")]
    public bool stopWhenOutOfMana = false;

    [Header("Player Specific Movement Properties")]
    [Tooltip("If true then if a player is using this ability then releasing the key will stop the ability")]
    public bool stopMovementOnKeyRelease = false;
    [Tooltip("If true then the destination changes with mouse movement during movement")]
    public bool changeDestinationWhileMoving = false;

    [Header("AI Instructions")]
    [Tooltip("if true Ai will target itself with this ability")]
    public bool targetsSelfOnly = false;
    [Tooltip("if true Ai will target allies with this ability")]
    public bool targetsAllies = false;
    [Tooltip("if true Ai will not use this ability on entities that have an ability object from this ability as a child")]
    public bool nonStackingAttachable = false;
    [Tooltip("should the Ai only target entities that are alive, only entities that are dead, or either?")]
    public LifeState requiredLifeState = LifeState.Alive;
    [Tooltip("if true Ai will not update the target location right before the ability object is created")]
    public bool doNotUpdateTargetLocation = false;

    [Header("Player Restrictions")]
    public bool limitRangeForPlayers = false;
    public float rangeLimitForPlayers = 0f;
    public bool requireWeaponType = false;
    public List<int> permittedWeaponTypes = new List<int>();
    public bool requiresSheild = false;

    [Header("Tool Tips")]

    [TextArea]
    public string skillLore;


    [TextArea]
    public string description;



    public static Ability getAbility(AbilityID id)
    {
        return AbilityIDList.getAbility(id);
    }

    public static float minimumChannelTime()
    {
        return 0.2f;
    }

}
