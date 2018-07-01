using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssenceManager : MonoBehaviour
{
    [System.Serializable]
    public class EssenceHolder
    {
        public EssenceType essenceType = EssenceType.None;
        public List<Essence> essences = new List<Essence>();

		public EssenceHolder(EssenceType type){
			essenceType = type;
		}
    }

    public List<EssenceHolder> essences = new List<EssenceHolder>();

    public void addEssence(Essence essence)
    {
        cleanLists();
        foreach (EssenceHolder holder in essences)
        {
            if (holder.essenceType == essence.essenceType)
            {
                holder.essences.Add(essence);
                return;
            }
        }
        if (essence)
        {
            EssenceHolder newHolder = new EssenceHolder(essence.essenceType);
            essences.Add(newHolder);
            newHolder.essences.Add(essence);
        }
    }

    public int getEssenceCount(EssenceType _type)
    {
        cleanLists();
        
        foreach (EssenceHolder holder in essences)
        {
            if (holder.essenceType == _type)
            {
                return holder.essences.Count;
            }
        }

        return 0;
    }

    public void removeEssences(EssenceType _type, int count)
    {
        cleanLists();
        foreach (EssenceHolder holder in essences)
        {
            if (holder.essenceType == _type)
            {
                int removed = 0;
                while (holder.essences.Count > 0 && removed < count)
                {
                    removed++;
                    holder.essences[0].destroy();
                    holder.essences.RemoveAt(0);
                }

                return;
            }
        }
    }

    public void cleanLists()
    {
        foreach (EssenceHolder held in essences)
        {
            held.essences.RemoveAll(x => x == null);
        }
    }

}

public enum EssenceType
{
    None, Void
}