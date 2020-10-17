using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public string inputFront;
    public string inputBack;
    public string inputLeft;
    public string inputRight;
    public string inputInteract;
    private bool moving = false;

    public GameObject GameManager;
    public Animator Animator;

    public float runSpeed;
    public float walkSpeed;
    private GameObject loot;
    private GameObject dialBox;
    private bool talking;
    private string[] currentText;
    private bool triggerEntered;
    private DialText dialTxt;

    private GameObject container;
    private GameObject dial;
    private int currentDial;
    private int longDial;
    private Animation animations;
    private GameObject inventaire;

    // Start is called before the first frame update
    void Start()
    {
        // on la chargera dans la collision si on utilise des presets de loot
        dialBox = Resources.Load<GameObject>("DialBox");
        animations = gameObject.GetComponent<Animation>();
        inventaire = GameObject.FindGameObjectWithTag("Inventaire");
    }

    // Update is called once per frame
    void Update()
    {
        moving = false;
        if (enabled && !talking)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKey(inputFront))
                {
                    transform.Translate(0, runSpeed * Time.deltaTime, 0);
                    Animator.SetFloat("Speed", 1.1f);
                    moving = true;
                }

                if (Input.GetKey(inputBack))
                {
                    transform.Translate(0, -runSpeed * Time.deltaTime, 0);
                    Animator.SetFloat("Speed", 1.1f);
                    moving = true;
                }

                if (Input.GetKey(inputLeft))
                {
                    transform.Translate(-runSpeed * Time.deltaTime, 0, 0);
                    Animator.SetFloat("Speed", 1.1f);
                    moving = true;
                }

                if (Input.GetKey(inputRight))
                {
                    transform.Translate(runSpeed * Time.deltaTime, 0, 0);
                    Animator.SetFloat("Speed", 1.1f);
                    moving = true;
                }
            }
            else
            {
                if (Input.GetKey(inputFront))
                {
                    transform.Translate(0, walkSpeed * Time.deltaTime, 0);
                    Animator.SetFloat("Speed", 0.4f);
                    moving = true;

                }

                if (Input.GetKey(inputBack))
                {
                    transform.Translate(0, -walkSpeed * Time.deltaTime, 0);
                    Animator.SetFloat("Speed", 0.4f);
                    moving = true;
                }

                if (Input.GetKey(inputLeft))
                {
                    transform.Translate(-walkSpeed * Time.deltaTime, 0, 0);
                    if (!gameObject.GetComponent<SpriteRenderer>().flipX)
                        gameObject.GetComponent<SpriteRenderer>().flipX = true;
                    Animator.SetFloat("Speed", 0.4f);
                    moving = true;
                }

                if (Input.GetKey(inputRight))
                {
                    transform.Translate(walkSpeed * Time.deltaTime, 0, 0);
                    if (gameObject.GetComponent<SpriteRenderer>().flipX)
                        gameObject.GetComponent<SpriteRenderer>().flipX = false;
                    Animator.SetFloat("Speed", 0.4f);
                    moving = true;
                }
            }

            if (!moving)
            {
                Animator.SetFloat("Speed", 0);
            }
        }

        // si le joueur appuie sur E après être entré en collision avec un PNJ
        if (Input.GetKeyDown(inputInteract) && dial == null && triggerEntered)
        {
            Debug.Log("PNJ Start");
            talking = true;
            gameObject.GetComponent<PlayerController>().enabled = false;
            GameObject canvas = GameObject.Find("Dialogue");
            dial = Instantiate(dialBox, new Vector3(canvas.transform.position.x, canvas.transform.position.y / 3), Quaternion.identity, canvas.transform);
            if (dialTxt.twoDial && dialTxt.hasTalked())
            {
                currentText = dialTxt.secondText;
            }
            else
            {
                currentText = dialTxt.firstText;
            }
            dial.GetComponentInChildren<Text>().text = currentText[currentDial];
            longDial = currentText.Length;
            currentDial++;
        }
        // durant le dialogue
        else if (dial != null && triggerEntered)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.LeftControl))
            {
                // si on a atteint la dernière ligne de dialogue
                if (longDial == currentDial)
                {
                    Debug.Log("PNJ End");
                    talking = false;
                    gameObject.GetComponent<PlayerController>().enabled = true;
                    longDial = 0;
                    currentDial = 0;

                    // active l'event lié au dialogue s'il y en a
                    if (dialTxt.eventAfterDial != null && !dialTxt.hasTalked())
                    {
                        dialTxt.eventAfterDial.SetActive(!dialTxt.eventAfterDial.activeInHierarchy);
                    }
                    // indique que le joueur à déjà parlé à ce PNJ
                    dialTxt.Talked();

                    currentText = null;
                    Destroy(dial);
                }
                else
                {
                    Debug.Log("PNJ");
                    dial.GetComponentInChildren<Text>().text = currentText[currentDial];
                    currentDial++;
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject Infos;
        // si le joueur intéragit avec un coffre non ouvert
        if (collision.CompareTag("Container") && Input.GetKeyDown(inputInteract) && (container == null || !container.activeInHierarchy))
        {
            Debug.Log("Loot Start");

            container = collision.GetComponent<Lootable>().LootInstance;
            container.SetActive(true);

            gameObject.GetComponent<PlayerController>().enabled = false;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            inventaire.GetComponent<Canvas>().enabled = true;
        }
        // suite à l'ouverture du coffre permet au joueur de le fermer
        else if (container != null && container.activeInHierarchy && Input.GetKeyDown(inputInteract))
        {
            Debug.Log("Loot End");
            gameObject.GetComponent<PlayerController>().enabled = true;
            inventaire.GetComponent<Canvas>().enabled = false;
            container.SetActive(false);
        }
        // si le joueur intéragir avec un portail ou une porte
        if (collision.CompareTag("Teleport") && Input.GetKeyDown(inputInteract))
        {
            if (!GameObject.FindGameObjectWithTag("Infos"))
            {
                // l'objet infos contiendra les infos sur le joueur
                Infos = new GameObject("Infos");
                Infos.AddComponent<Stats>();
                Infos.AddComponent<InfosPlayer>();
                Infos.tag = "Infos";

                Infos.GetComponent<Stats>().CopyStatsFrom(gameObject);
                Infos.GetComponent<InfosPlayer>().CopyInfosFrom(gameObject);
                Infos.GetComponent<InfosPlayer>().SetZone(collision.GetComponent<Teleporter>().nextScene);
                DontDestroyOnLoad(Infos);
            }

            GameManager.GetComponent<MainManager>().SaveSettings();
            GameManager.GetComponent<MainManager>().SaveInventory();
            SceneManager.LoadScene(collision.GetComponent<Teleporter>().nextScene);
        }
        // si le joueur intéragit avec l'auberge
        if (collision.transform.name == "Inn" && Input.GetKeyDown(inputInteract))
        {
            Debug.Log("Inn");
            gameObject.GetComponent<Stats>().Mana.ResetValue();
            gameObject.GetComponent<Stats>().Health.ResetValue();

            Debug.Log(gameObject.GetComponent<Stats>().Health.Value);
            Debug.Log(gameObject.GetComponent<Stats>().Mana.Value);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // initialisation du dialgue
        if (collision.CompareTag("PNJ"))
        {
            triggerEntered = true;
            dialTxt = collision.GetComponent<DialText>();
            Debug.Log("trigger entered");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (container != null)
        {
            // au cas où si le joueur ferme l'inventaire avec I au lieu de E suite à l'ouverture d'un coffre
            if (container.activeInHierarchy)
            {
                Debug.Log("trigger exit loot");
                gameObject.GetComponent<PlayerController>().enabled = true;
                inventaire.GetComponent<Canvas>().enabled = false;
                container.SetActive(false);
                // Destroy(container);
            }
        }
        // reset dialogue
        if (collision.CompareTag("PNJ"))
        {
            triggerEntered = false;
            currentText = null;
            dialTxt = null;
            Destroy(dial);
            Debug.Log("trigger exit PNJ");
        }
    }
}
