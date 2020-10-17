public enum StatModType
{
    Flat = 100,
    PercentAdd = 200,
    PercentMult = 300,
}

// les StatsModifier sont utilisés en combat lors d'attaques, de défenses et d'utilisations d'objets
public class StatModifier
{
    public readonly float Value; // Valeur de la modification à effectuer

    public readonly StatModType Type; // Type du StatModifier est-ce un pourcentage ou non ?

    public readonly int Order; // on peut aussi appliquer un ordre aux modificateurs pour qu'ils soient calculés dans les premiers ou non

    public readonly object Source; // enfin on peut leur donner une source que l'on peut utiliser avec la méthode RemoveAllModifiersFromSource de la classe CharacterStat

    // il y a différents constructeurs selon les besoins que l'on a 
    public StatModifier(float value, StatModType type, int order, object source)
    {
        Value = value;
        Type = type;
        Order = order;
        Source = source;
    }

    public StatModifier(float value, StatModType type) : this (value, type, (int)type, null) { }

    public StatModifier(float value, StatModType type, int order) : this(value, type, order, null) { }

    public StatModifier(float value, StatModType type, object source) : this(value, type, (int)type, source) { }
}
