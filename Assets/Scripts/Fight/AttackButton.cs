using UnityEngine;
using UnityEngine.UI;

public class AttackButton : MonoBehaviour
{
    private Skill skill;
    private GameObject Enemy;
    private FightManager FightManager;
    private int IdEnemy;

    private void Awake()
    {
        FightManager = GameObject.FindGameObjectWithTag("FightManager").GetComponent<FightManager>();
        skill = new Skill("auto-attack", FightManager.GetPlayer().GetComponent<Stats>().Atk.Value, 0, Range.SINGLE);
    }

    public void setSkill(Skill skill)
    {
        this.skill = skill;
    }

    public void SetEnemy(GameObject e)
    {
        Enemy = e;
        transform.GetChild(0).GetComponent<Text>().text = Enemy.GetComponent<Infos>().MobName;
    }

    public GameObject GetEnemy()
    {
        return Enemy;
    }

    public void SetIdEnemy(int i)
    {
        IdEnemy = i;
    }

    public int GetIdEnemy()
    {
        return IdEnemy;
    }

    private void Update()
    {
        if (!FightManager.GetEnemies().Contains(Enemy))
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    public void AttackEnemy()
    {
        // Debug.Log(skill.getName());
        FightManager.PlayerAttacks(Enemy, skill);
    }
}
