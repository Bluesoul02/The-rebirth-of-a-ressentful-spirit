public class Skill
{
    private string name;
    private float damage;
    private Type type;
    private Range range;
    private int cost;

    public Skill(string name, float damage, int cost, Type type, Range range)
    {
        this.name = name;
        this.damage = damage;
        this.cost = cost;
        this.type = type;
        this.range = range;
    }

    public Skill(string name, float damage, int cost) : this(name, damage, cost, Type.SKILL, Range.SINGLE) { }

    public Skill(string name, float damage, int cost, Range range) : this(name, damage, cost, Type.SKILL, range) { }

    public Skill(string name, float damage, int cost, Type type) : this(name, damage, cost, type, Range.SINGLE) { }

    public Type getType()
    {
        return type;
    }

    public string getName()
    {
        return name;
    }

    public float getDamage()
    {
        return damage;
    }

    public int getCost()
    {
        return cost;
    }

    public Range getRange()
    {
        return range;
    }
}