using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatFromAbilityUse
{
    public TaggedStatsHolder.TaggableStat stat = null;
    public List<Tags.AbilityTags> requiredTags = new List<Tags.AbilityTags>();
    public float duration = 0f;
    public string name = string.Empty;
    public WeaponRequirementType weaponRequirement = WeaponRequirementType.NoRequirement;

    public StatFromAbilityUse(TaggedStatsHolder.TaggableStat _stat, List<Tags.AbilityTags> _requiredTags, float _duration,
        string _name, WeaponRequirementType _weaponRequirement = WeaponRequirementType.NoRequirement)
    {
        stat = _stat;
        requiredTags = _requiredTags;
        duration = _duration;
        name = _name;
        weaponRequirement = _weaponRequirement;
    }
}
