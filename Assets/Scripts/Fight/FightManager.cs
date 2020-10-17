using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class FightManager : MonoBehaviour
{
    private GameObject player;
    private GameObject Infos;
    private FightLog fightLog;
    private GameObject enemy;
    private int nbEnemies;
    private bool won;
    private bool lost;
    private List<GameObject> enemies;

    private List<GameObject> fightingOrder;
    private GameObject currentFighter;
    private int idCurrentFighter = 0;

    public GameObject MainPanel;
    public GameObject AttackPanel;
    public GameObject PauseMenu;

    private void Awake()
    {
        Debug.Log("début");

        // on rend le curseur visible pour plus d'ergonomie
        Cursor.visible = true;

        // infos sert à récupérer des informations sur le joueur (stats et ennemi rencontré surtout)
        Infos = GameObject.FindGameObjectWithTag("Infos");

        // on a une prefab pour le joueur donc on instancie un nouveau joueur et on repositionne du sprite dans la scène
        player = Instantiate((GameObject)Resources.Load("Player/" + Infos.GetComponent<InfosPlayer>().GetZone() + "/Player"));
        if (player.transform.childCount > 0)
            Destroy(player.transform.GetChild(0).gameObject); // ici supression de la camera attachée au joueur
        player.transform.position = GameObject.Find("SpawnPlayer").transform.position;
        if (Infos.GetComponent<InfosPlayer>().GetZone() == "Scene2")
        {
            player.GetComponent<Transform>().localScale = new Vector3(13, 13, 1);
        }

        // on copie les infos dans les components de Player, on modifiera donc les valeurs dans Player et pas dans infos pour plus de logique
        player.GetComponent<Stats>().CopyStatsFrom(Infos);
        player.GetComponent<InfosPlayer>().CopyInfosFrom(Infos);
        Destroy(player.GetComponent<PlayerController>()); // on empêche le joueur de pouvoir se déplacer en combat
        Debug.Log("joueur prêt");

        enabled = true;
        won = false;
        lost = false;
        fightLog = GameObject.Find("FightLog").transform.GetChild(0).GetComponent<FightLog>(); // initialisation du FigtLog qui sert à afficher des messages en combats

        Debug.Log("ennemi rencontré : " + Infos.GetComponent<InfosPlayer>().GetNameEnemyEncountred());

        // récupération des infos sur l'ennemi rencontré
        string enemyZone = player.GetComponent<InfosPlayer>().GetZone();

        Vector3 initialEnemyPosition = GameObject.Find("SpawnEnemy").transform.position; // variable utilisée pour faire apparaître les ennemis à des positions adéquates

        int randomEnemies = Random.Range(0, 100); // nombre aléatoire d'ennemis à combattre, 25% de chance d'avoir 1 ou 3 ennemis, 50% d'avoir 2 ennemis
        bool renameEnemies;
        if (randomEnemies <= 25)
        {
            nbEnemies = 1;
            renameEnemies = false;
        }
        else if (randomEnemies <= 50)
        {
            nbEnemies = 3;
            renameEnemies = true;
        }
        else
        {
            nbEnemies = 2;
            renameEnemies = true;
        }
        enemies = new List<GameObject>(nbEnemies);

        // initialisation du tableau fightingOrder qui sera utilisé pour le combat
        fightingOrder = new List<GameObject>(nbEnemies + 1)
        {
            player
        };

        object[] enemiesResources = Resources.LoadAll("Enemies/" + enemyZone); // chargement des ressources correspondant aux ennemis de la zone du combat
        for (int i = 1; i < nbEnemies + 1; i++) // création des ennemis pour le combat
        {
            GameObject instance = Instantiate((GameObject)enemiesResources[Random.Range(0, enemiesResources.Length - 1)]); // les ennemis sont des prefabs donc on fait une nouvelle instance pour chaque ennemi

            instance.transform.position = new Vector2(initialEnemyPosition.x, initialEnemyPosition.y - (5 * (i - 1))); // postionnement qui utilise la position du premier ennemi

            if (renameEnemies)
                instance.GetComponent<Infos>().MobName += " " + i; // quand il y a plusieurs ennemis, on les renomme afin que le joueur puisse les différencier

            // on stock l'ennemi dans des tableaux
            fightingOrder.Add(instance);
            enemies.Add(instance);
            instance.AddComponent<UIStat>(); // on ajoute un affichage des points de vie pour chaque ennemi
        }
        Debug.Log(nbEnemies + " ennemis prêts");

        // tri des personnages en fontion de leur vitesse
        fightingOrder.Sort(CompareSpeed);
        Debug.Log("tri fait");

        // lancement de la boucle de combat
        Debug.Log("lancement de la boucle de combat");
        currentFighter = fightingOrder[0]; // current fighter est initialisé au premier élément de fightingOrder afin de définir un premier combattant
        // affichage stat du joueur
        player.AddComponent<UIStat>();
    }

    // méthode appelée quand un tour se finit, elle gère aussi quelques effets de début de tour
    public void GoToNextTurn()
    {
        // quand un tour se finit, le prochain combattant est choisi dans la liste fightingOrder
        if (idCurrentFighter == fightingOrder.Count - 1)
            idCurrentFighter = 0;
        else
            idCurrentFighter++;

        currentFighter = fightingOrder[idCurrentFighter];

        // si le currentFighter est le joueur alors on réactive la panneau principal et on lui rajoute des TP
        // sinon on désactive les panneaux inutiles pendant les tours des ennemis
        if (currentFighter == player)
        {
            MainPanel.SetActive(true);
            MainPanel.transform.GetChild(1).gameObject.SetActive(true);
            addTP(player);
        }
        else
        {
            MainPanel.SetActive(false);
            AttackPanel.SetActive(false);
        }

        // si le currentFighter a choisi de se défendre au dernier tour alors on lui retire les boosts de défense
        Stats StatsCurrentFighter = currentFighter.GetComponent<Stats>();
        if (StatsCurrentFighter.Defense.Value > StatsCurrentFighter.Defense.BaseValue)
        {
            currentFighter.GetComponent<Stats>().Defense.RemoveAllModifiersFromSource("DefendAction");
        }
    }

    // fonction de comparaison des combattants en fonction de leur vitesse
    private int CompareSpeed(GameObject a, GameObject b)
    {
        if (a.GetComponent<Stats>().Speed.Value > b.GetComponent<Stats>().Speed.Value)
            return -1;
        else if (a.GetComponent<Stats>().Speed.Value < b.GetComponent<Stats>().Speed.Value)
            return 1;
        else
            return 0;
    }

    // fonction utilisée pour toute attaque qui ne cible qu'un personnage
    public void Attack(Skill skill, GameObject Target)
    {
        // en fonction du type de compétences (magique ou physique) on modifie les calculs de dégâts et quelle ressource sera utilisée (Mana ou TP)
        // les calculs prennent en compte la défense ou la résistance magique de la cible
        float damage;
        if (skill.getType() == Type.SKILL)
        {
            // calcul des dégâts en fonction de l'attaque du currentFighter et de la défense de l'ennemi
            damage = (skill.getDamage() + currentFighter.GetComponent<Stats>().Atk.Value) - Target.GetComponent<Stats>().Defense.Value;

            // on retire des TP au currentFighter
            currentFighter.GetComponent<Stats>().TP.AddModifier(new StatModifier(-skill.getCost(), StatModType.Flat, "fight"));
        }
        else
        {
            // calcul des dégâts en fonction de l'affinité magique du currentFighter et de la défense magique de l'ennemi
            damage = (skill.getDamage() + currentFighter.GetComponent<Stats>().Atk.Value) - Target.GetComponent<Stats>().MR.Value;

            // on retire du Mana au currentFighter
            currentFighter.GetComponent<Stats>().Mana.AddModifier(new StatModifier(-skill.getCost(), StatModType.Flat, "fight"));
        }

        // si les dégâts sont négatifs alors on les remonte à 0 sinon l'attaque "rendra" des points de vie à la cible
        if (damage < 0)
            damage = 0;

        // ici on affiche les messages dans le journal de combat
        if (Target.tag != "Player")
        {
            fightLog.attackLog("Player", Target.GetComponent<Infos>().MobName, skill.getName(), damage.ToString());
        }
        else
        {
            fightLog.attackLog(currentFighter.GetComponent<Infos>().MobName, "Player", skill.getName(), damage.ToString());
            addTP(player);
        }

        // enfin on enlève les points de vie à la cible
        Target.GetComponent<Stats>().Health.AddModifier(new StatModifier(-damage, StatModType.Flat, "fight"));

        // quand une attaque est faite, on passe automatiquement au tour suivant
        GoToNextTurn();
    }

    // fonction utilisée quand le joueur choisit d'utiliser une compétence à ciblage unique
    public void PlayerAttacks(GameObject enemy, Skill skill)
    {
        // le joueur ne peut pas attaquer s'il n'a pas assez de la ressource nécessaire
        if ((player.GetComponent<Stats>().TP.Value >= skill.getCost() & skill.getType() == Type.SKILL) | (player.GetComponent<Stats>().Mana.Value >= skill.getCost() & skill.getType() == Type.MAGIC))
        {
            Attack(skill, enemy);
        }
    }

    // fonction utilisée quand le joueur choisit d'utiliser une compétence à ciblage multiple
    public void PlayerAttacksAll(Skill skill)
    {
        // on répéte le mécanisme de PlayerAttacks mais sur chaque ennemi
        // cette fonction empêche de puiser plusieurs fois dans la ressource Mana ou TP
        GameObject enemy;
        float damage;

        if (skill.getType() == Type.MAGIC & player.GetComponent<Stats>().Mana.Value > skill.getCost())
        {
            for (int i = 0; i < nbEnemies; i++)
            {
                enemy = enemies[i];
                damage = (skill.getDamage() + player.GetComponent<Stats>().AP.Value) - enemy.GetComponent<Stats>().MR.Value;
                if (damage < 0)
                {
                    damage = 0;
                }
                enemy.GetComponent<Stats>().Health.AddModifier(new StatModifier(-damage, StatModType.Flat, "fight"));
                player.GetComponent<Stats>().Mana.AddModifier(new StatModifier(-skill.getCost(), StatModType.Flat, "fight"));
                fightLog.attackLog("Player", enemy.GetComponent<Infos>().MobName, skill.getName(), damage.ToString());
            }

            // quand une attaque est faite, on passe automatiquement au tour suivant
            GoToNextTurn();
        }
        else if (player.GetComponent<Stats>().TP.Value > skill.getCost() & skill.getType() == Type.SKILL)
        {
            for (int i = 0; i < nbEnemies; i++)
            {
                enemy = enemies[i];
                damage = (skill.getDamage() + player.GetComponent<Stats>().Atk.Value) - enemy.GetComponent<Stats>().Defense.Value;
                if (damage < 0)
                {
                    damage = 0;
                }
                enemy.GetComponent<Stats>().Health.AddModifier(new StatModifier(-damage, StatModType.Flat, "fight"));
                player.GetComponent<Stats>().TP.AddModifier(new StatModifier(-skill.getCost(), StatModType.Flat, "fight"));
                fightLog.attackLog("Player", enemy.GetComponent<Infos>().MobName, skill.getName(), damage.ToString());
            }

            // quand une attaque est faite, on passe automatiquement au tour suivant
            GoToNextTurn();
        }
    }

    // fonction qui fait attaquer les ennemis de façon "intelligente"
    public void EnemyTurn()
    {
        /* 
            on vérifie si l'ennemi peut lancer une attaque magique ou physique
            s'il peut lancer les deux, un random décide du type d'attaque qui sera utilisée
            sinon il lancera le type de sort qu'il pourra
            s'il n'a ni Mana ni TP et qu'il a plus d'1/5 de ses points de vie, alors il utilise une attaque de base
            s'il a moins d'1/5 alors il se défend
        */

        int random;
        if (currentFighter.GetComponent<Skills>().LowestCostSpell() < currentFighter.GetComponent<Stats>().Mana.Value)
        {
            if (currentFighter.GetComponent<Skills>().LowestCostSkill() < currentFighter.GetComponent<Stats>().TP.Value)
            {
                random = Random.Range(0, 1);
                if (random == 0)
                {
                    ThrowRandomSkillToPlayer("magic");
                }
                else
                {
                    ThrowRandomSkillToPlayer("skill");
                }
            }
            else
            {
                ThrowRandomSkillToPlayer("magic");
            }
        }
        else
        {
            if (currentFighter.GetComponent<Skills>().LowestCostSkill() < currentFighter.GetComponent<Stats>().TP.Value)
            {
                ThrowRandomSkillToPlayer("skill");
            }
            else
            {
                if (currentFighter.GetComponent<Stats>().Health.Value > currentFighter.GetComponent<Stats>().Health.BaseValue / 5)
                    AutoAttack(player);
                else
                    DefendAction();
            }

        }
    }

    // fonction qui permet de faire une attaque de base mentionnée plus haut
    public void AutoAttack(GameObject Target)
    {
        // une attaque de base ne coûte pas de TP et ne fait pas de dégâts supplémentaires
        Attack(new Skill("auto-attack", 0, 0), Target);
    }

    // fonction qui ajoute des TP lors que plusieurs actions
    public void addTP(GameObject Target)
    {
        Target.GetComponent<Stats>().TP.AddModifier(new StatModifier(Random.Range(2, 8), StatModType.Flat, "fight"));
    }

    // fonction qui "choisit" une compétence à utiliser sur le joueur, le type est décidé dans la méthode EnemyTurn
    public void ThrowRandomSkillToPlayer(string type)
    {
        // une compétence doit forcément être chosie afin de passer au tour suivant, les boucles while sont là pour forcer le choix
        bool chosen = false;
        Skill RandomSkill;
        if (type == "magic")
        {
            while (!chosen)
            {
                RandomSkill = currentFighter.GetComponent<Skills>().GetSpells()[Random.Range(0, currentFighter.GetComponent<Skills>().GetSpells().Length - 1)];

                // on ne valide l'attaque que si l'ennemi a les ressources nécessaires
                if (RandomSkill.getCost() <= currentFighter.GetComponent<Stats>().Mana.Value)
                {
                    chosen = true;
                    Attack(RandomSkill, player);
                }
            }
        }
        else if (type == "skill")
        {
            while (!chosen)
            {
                RandomSkill = currentFighter.GetComponent<Skills>().GetSkills()[Random.Range(0, currentFighter.GetComponent<Skills>().GetSkills().Length - 1)];

                // on ne valide l'attaque que si l'ennemi a les ressources nécessaires
                if (RandomSkill.getCost() <= currentFighter.GetComponent<Stats>().TP.Value)
                {
                    chosen = true;
                    Attack(RandomSkill, player);
                }
            }
        }
    }

    // fonction appelée quand un combattant choisit de se défendre
    public void DefendAction()
    {
        // défendre signifie augmenter sa défense et sa défense magique jusqu'au prochain tour
        currentFighter.GetComponent<Stats>().Defense.AddModifier(new StatModifier(1, StatModType.PercentAdd, "DefendAction"));
        currentFighter.GetComponent<Stats>().MR.AddModifier(new StatModifier(1, StatModType.PercentAdd, "DefendAction"));

        // affichage de l'action dans le journal pour avoir un historique des infos
        if (currentFighter.tag != "Player")
        {
            fightLog.defenseLog(currentFighter.GetComponent<Infos>().MobName);
        }
        else
        {
            fightLog.defenseLog("Player");
        }
        GoToNextTurn();
    }

    // la méthode Update est une méthode d'Unity qui est appelée à chaque frame (rafraîchissement de l'écran)
    private void Update()
    {
        // on vérifie si une des conditions de fin de combat est validée
        if (winCondition())
        {
            leaveFight(true);
        }
        else if (loseCondition())
        {
            leaveFight(false);
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            PauseMenu.SetActive(!PauseMenu.activeInHierarchy);
        }

        if (enabled)
        {
            // ici on vérifie pour chaque ennemi s'il a encore des points de vie
            for (int i = 0; i < nbEnemies; i++)
            {
                if (enemies[i].GetComponent<Stats>().Health.Value <= 0)
                {
                    // si l'ennemi n'a plus de point de vie alors on le retire du combat
                    GameObject enemyToKill = enemies[i];

                    // on met un message dans le journal
                    fightLog.deathLog(enemyToKill.GetComponent<Infos>().MobName);

                    // on enlève l'ennemi des listes enemies et fightingOrder
                    enemies.RemoveAt(i);
                    fightingOrder.RemoveAt(fightingOrder.FindIndex(x => x == enemyToKill));

                    // cette ligne enlève l'affichage des points de vie 
                    enemyToKill.GetComponent<UIStat>().dead();

                    // Destroy permet de supprimer l'objet de la scène
                    Destroy(enemyToKill);

                    // on décrémente nbEnemies afin d'éviter de sortir de la liste enemies
                    nbEnemies--;
                    break;
                }
            }

            // ici un if qui empêche un ennemi qui vient de mourir d'attaquer
            if (nbEnemies > 0)
            {
                if (currentFighter != player)
                    if (!fightingOrder.Contains(currentFighter))
                        currentFighter = fightingOrder[idCurrentFighter];
                    else
                        EnemyTurn();
            }
        }
    }

    // focntion appelée si le joueur gagne ou perd le combat
    public void leaveFight(bool win)
    {
        if (win)
        {
            won = true;

            // on rajoute le nom de l'ennemi rencontré à la liste des ennemis tués par le joueur
            player.GetComponent<InfosPlayer>().EnemiesKilled.Add(Infos.GetComponent<InfosPlayer>().GetNameEnemyEncountred());

            // on dit que le joueur doit réapparaître car il a gagné (différence avec la réapparition suite à une défaite ou une fuite)
            player.GetComponent<InfosPlayer>().SetWinningRespawning(true);

            // enfin on affiche un panneau de victoire
            GameObject.Find("Victory").transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            lost = true;

            // on dit que le joueur doit réapparaître car il a perdu (différence avec la réapparition suite à une victoire ou une fuite)
            player.GetComponent<InfosPlayer>().SetLoosingRespawning(true);

            // enfin on affiche un panneau de défaite
            GameObject.Find("Defeat").transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    // fonction appelée à la toute fin d'un combat
    public void quit()
    {
        string ZoneToLoad = player.GetComponent<InfosPlayer>().GetZone();

        // quand le joueur choisit de fuir, la fonction quit est directement appelée donc le joueur n'est ni gagnant ni perdant
        if (!player.GetComponent<InfosPlayer>().IsLoosingRespawning() & !player.GetComponent<InfosPlayer>().IsWinningRespawning())
        {
            fightLog.escapeLog(true);

            // on dit que le joueur doit réapparaître car il a fuit (différence avec la réapparition suite à une victoire ou une défaite)
            player.GetComponent<InfosPlayer>().SetEscapeRespawning(true);
        }

        // on copie les infos et les stats du joueur dans Infos que l'on garde dans le changement de scène
        Infos.GetComponent<Stats>().CopyStatsFrom(player);
        Infos.GetComponent<InfosPlayer>().CopyInfosFrom(player);

        // l'inventaire (drag&drop) est conservé à travers les scènes
        GameObject dragDrop = GameObject.Find("Drag&Drop");
        dragDrop.GetComponent<Canvas>().enabled = false;
        dragDrop.transform.GetChild(0).gameObject.SetActive(true);
        dragDrop.GetComponent<Inventory>().enabled = true;
        DontDestroyOnLoad(dragDrop);

        // on charge la scène d'oùu vient le joueur
        SceneManager.LoadScene(ZoneToLoad);
    }

    private bool winCondition()
    {
        // si plus d'ennemis alors gagné
        if (nbEnemies == 0 & !won)
        {
            enabled = false;
            return true;
        }
        return false;
    }

    private bool loseCondition()
    {
        // si player n'a plus d'HP alors perdu
        if (player.GetComponent<Stats>().Health.Value <= 0 & !lost)
        {
            enabled = false;
            return true;
        }
        return false;
    }

    public int GetNbEnemies()
    {
        return nbEnemies;
    }

    public List<GameObject> GetEnemies()
    {
        return enemies;
    }

    public GameObject GetCurrentFighter()
    {
        return currentFighter;
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public void escapeFailed()
    {
        fightLog.escapeLog(false);
    }
}
