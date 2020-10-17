using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;

[Serializable]
public class CharacterStat
{
    public float BaseValue;

    public virtual float Value
    {
        // le get recalcule la valeur si elle a été mofifiée où si la valeur de base a été modifiées
        get
        {
            if (isDirty || BaseValue != lastBaseValue)
            {
                lastBaseValue = BaseValue;
                _value = CalculateFinalValue();
                isDirty = false;
            }
            return _value;
        }
    }

    protected bool isDirty = true; // permet de savoir si la valeur a été modifiée
    protected float _value;
    protected float lastBaseValue = float.MinValue;

    protected readonly List<StatModifier> statsModifiers; // liste originelle des modificateurs
    public readonly ReadOnlyCollection<StatModifier> StatsModifiers; // version protégée de la liste

    public CharacterStat()
    {
        statsModifiers = new List<StatModifier>();
        StatsModifiers = statsModifiers.AsReadOnly();
    }
    public CharacterStat(float baseValue) : this ()
    {
        BaseValue = baseValue;
    }

    // ajoute un modificateur à la liste statsModifiers et trie cette dernière
    public virtual void AddModifier(StatModifier mod)
    {
        isDirty = true;
        statsModifiers.Add(mod);
        statsModifiers.Sort(CompareModifierOrder);
    }

    // méthode de comparaison des modificateurs
    protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if(a.Order < b.Order)
            return -1;
        else if(a.Order > b.Order)
            return 1;
        else
            return 0;
    }

    // enlève un modificateur précis
    public virtual bool RemoveModifier(StatModifier mod)
    {
        if (statsModifiers.Remove(mod))
        {
            isDirty = true;
            return true;
        }
        return false;
    }

    // enlève tous les modificateurs de stats provenant d'une certaine source
    public virtual bool RemoveAllModifiersFromSource(object source)
    {
        bool didRemove = false; ;
        for(int i = statsModifiers.Count -1 ; i >= 0; i--)
        {
            if(statsModifiers[i].Source == source)
            {
                isDirty = true;
                didRemove = true;
                statsModifiers.RemoveAt(i);
            }
        }
        return didRemove;
    }

    // calcule la valeur de la stat en fonction de tous les modificateurs
    protected virtual float CalculateFinalValue()
    {
        float finalValue = BaseValue;
        float sumPercentAdd = 0;

        for (int i = 0; i< statsModifiers.Count; i++)
        {
            StatModifier mod = statsModifiers[i];

            if(mod.Type == StatModType.Flat)
            {
                finalValue += mod.Value;
            }
            else if(mod.Type == StatModType.PercentAdd)
            {
                sumPercentAdd += mod.Value;

                if(i + 1 >= statsModifiers.Count || statsModifiers[i +1].Type != StatModType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd;
                    sumPercentAdd = 0;
                }
            }
            else if(mod.Type == StatModType.PercentMult)
            {
                finalValue *= 1 + mod.Value;
            }
        }

        return (float)Math.Round(finalValue, 4);
    }

    // enlève tous les modificateurs présents
   public void RemoveAllModifiers()
   {
        for (int i = statsModifiers.Count - 1; i >= 0; i--)
        {
            isDirty = true;
            statsModifiers.RemoveAt(i);
        }
   }

    // permet de mettre la valeur de la CharacterStat à un chiffre précis
   public void SetValue(float v)
   {
        this.RemoveAllModifiers();
        this.AddModifier(new StatModifier(-(BaseValue - v), StatModType.Flat));
   }

    // la valeur de la CharacterStat revient à sa BaseValue
    public void ResetValue()
    {
        this.RemoveAllModifiers();
        this.AddModifier(new StatModifier(BaseValue - Value, StatModType.Flat));
    }
}
