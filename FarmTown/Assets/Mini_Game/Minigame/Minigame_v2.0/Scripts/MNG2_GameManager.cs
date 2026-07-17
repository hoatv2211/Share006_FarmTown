using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MNG2_GameManager : MonoBehaviour
{
    public static MNG2_GameManager instance;

    [SerializeField] GameObject win, lost;
    [SerializeField] GameObject[] levels;
    [SerializeField] int levelToTest;
    [SerializeField] GameObject phaohoa;

    [SerializeField] Text textLevel;

    private int keyCount;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Init();
        //MNG2_SoundManager.instance.PlaySound(TypeSound.Background, 1, true);
    }

    private void Init()
    {
        keyCount = 0;
        win.SetActive(false);
        lost.SetActive(false);
        GameObject newLevel;
        if (levelToTest == 0)
        {
            newLevel = Instantiate(levels[PlayerPrefs.GetInt("LevelOfMinigame2")], Vector3.zero, Quaternion.identity);
        }
        else
        {
            newLevel = Instantiate(levels[levelToTest - 1], Vector3.zero, Quaternion.identity);
        }
        newLevel.transform.parent = transform;
        newLevel.name = "Level " + PlayerPrefs.GetInt("LevelOfMinigame2");
        textLevel.text = "Level " + PlayerPrefs.GetInt("LevelOfMinigame2");
    }

    public void Win()
    {
        if (PlayerPrefs.GetInt("LevelOfMinigame2") < levels.Length - 1)
        {
            PlayerPrefs.SetInt("LevelOfMinigame2", PlayerPrefs.GetInt("LevelOfMinigame2") + 1);
        }
        else
        {
            PlayerPrefs.SetInt("LevelOfMinigame2", 0);
        }
        StartCoroutine(DelayWin());
    }

    IEnumerator DelayWin()
    {
        GameObject newPhaoHoa = Instantiate(phaohoa, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(2);
        Destroy(newPhaoHoa);
        MNG2_Player.instance.Stop();
        win.SetActive(true);
    }

    public void Lost()
    {
        StartCoroutine(DelayThua());
    }

    IEnumerator DelayThua()
    {
        MNG2_Player.instance.effectChet.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        if(MNG2_Player.instance.gameObject != null)
            Destroy(MNG2_Player.instance.gameObject);
        yield return new WaitForSeconds(1.5f);
        lost.SetActive(true);
    }

    

    public void ButtonHandle()
    {
        Destroy(transform.GetChild(0).gameObject);
        Init();
    }

    public void ButtonNext()
    {

    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.N))
        {
            ButtonHandle();
        }
    }
}
