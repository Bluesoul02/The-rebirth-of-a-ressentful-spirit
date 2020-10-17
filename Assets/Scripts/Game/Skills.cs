using UnityEngine;

public class Skills : MonoBehaviour
{
    private int NbSkills = 0;
    private int NbSpells = 0;
    private Skill[] SkillsEnemy;
    private Skill[] SpellsEnemy;

    public string[] NameSkills;
    public float[] DamageSkills;
    public int[] CostSkills;
    public Type[] TypeSkills;
    public Range[] RangeSkills;

    private void Awake()
    {
        SkillsEnemy = new Skill[NameSkills.Length];
        SpellsEnemy = new Skill[NameSkills.Length];

        for (int i = 0; i < NameSkills.Length; i++)
        {
            if (TypeSkills[i] == Type.MAGIC)
            {
                SpellsEnemy[NbSpells] = new Skill(NameSkills[i], DamageSkills[i], CostSkills[i], TypeSkills[i], RangeSkills[i]);
                NbSpells++;
            }
            else
            {
                SkillsEnemy[NbSkills] = new Skill(NameSkills[i], DamageSkills[i], CostSkills[i], TypeSkills[i], RangeSkills[i]);
                NbSkills++;
            }
        }
    }

    public Skill[] GetSkills()
    {
        return SkillsEnemy;
    }

    public Skill[] GetSpells()
    {
        return SpellsEnemy;
    }

    public int LowestCostSpell()
    {
        int ToReturn = SpellsEnemy[0].getCost();
        if(NbSpells > 1)
        {
            for (int i = 0; i < NbSpells; i++)
            {
                if (SpellsEnemy[i] != null)
                    if (SpellsEnemy[i].getCost() < ToReturn)
                        ToReturn = SpellsEnemy[i].getCost();
            }
        }
        return ToReturn;
    }

    public int LowestCostSkill()
    {
        int ToReturn = SkillsEnemy[0].getCost();
        if (NbSkills > 1)
        {
            for (int i = 0; i < NbSkills; i++)
            {
                Debug.Log(SkillsEnemy[i].getName());
                if (SkillsEnemy[i] != null)
                    if (SkillsEnemy[i].getCost() < ToReturn)
                        ToReturn = SkillsEnemy[i].getCost();
            }
        }
        return ToReturn;
    }
}
