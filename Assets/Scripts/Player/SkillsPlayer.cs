using UnityEngine;

public class SkillsPlayer : MonoBehaviour
{
    private Skill[] PlayerSkills;
    private Skill[] PlayerSpells;
    private int nbSpells;
    private int nbSkills;

    private void Awake()
    {
        PlayerSkills = new Skill[100];
        this.nbSkills = 0;
        this.PlayerSkills[this.nbSkills] = new Skill("Charge", 2, 10);
        this.nbSkills += 1;
        this.PlayerSkills[this.nbSkills] = new Skill("Charge+", 4, 25);
        this.nbSkills += 1;
        PlayerSpells = new Skill[100];
        this.nbSpells = 0;
        this.PlayerSpells[this.nbSpells] = new Skill("Fire Ball", 3, 5, Type.MAGIC);
        this.nbSpells += 1;
        this.PlayerSpells[this.nbSpells] = new Skill("Fire Storm", 10, 50, Type.MAGIC, Range.ALL);
        this.nbSpells += 1;
        // print(this.PlayerSkills[this.nbSkills - 1].getName());
    }

    public Skill[] getPlayerSkills()
    {
        return PlayerSkills;
    }

    public int getNbSkills()
    {
        return nbSkills;
    }

    public Skill[] getPlayerSpells()
    {
        return PlayerSpells;
    }

    public int getNbSpells()
    {
        return nbSpells;
    }
}
