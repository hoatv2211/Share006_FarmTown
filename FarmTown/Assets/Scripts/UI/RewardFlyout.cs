using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RewardFlyout : MonoBehaviour
{
    private bool isRun;
    private bool PointedCoin;
    private int indexArrayCoin;
    private int numberPointsCoin = 15;
    private float speed = 0.5f;
    private Vector3[] positionCoin;
    private Transform pointerCoin;
    public int numberCoin;
    public int id;
    [SerializeField] GameObject Coin;        //doi tuong bay
    [SerializeField] Text NumberCoinText;   //so coin
    [SerializeField] Transform pointerRight;//vi tri luon xuong
    [SerializeField] Sprite exp, coin, diamond;
    void Start()
    {

        if (id == 1)
            pointerCoin = GameBootstrap.Instance.InventoryRewardTarget;
        if (id == 0)
            pointerCoin = GameBootstrap.Instance.ProduceRewardTarget;
        if (id == 2)
        {
            transform.GetChild(0).GetComponent<Image>().sprite = exp;
            pointerCoin = GameBootstrap.Instance.ExperienceRewardTarget;
        }
        if (id == 3)
        {
            transform.GetChild(0).GetComponent<Image>().sprite = coin;
            pointerCoin = GameBootstrap.Instance.GoldRewardTarget;
        }
        if (id == 4)
        {
            transform.GetChild(0).GetComponent<Image>().sprite = diamond;
            pointerCoin = GameBootstrap.Instance.DiamondRewardTarget;
        }
        transform.GetChild(0).GetComponent<Image>().SetNativeSize();
        positionCoin = new Vector3[numberPointsCoin];
        NumberCoinText.text = "+" + numberCoin;
        DrawQuadraticCurveItem();
        StartCoroutine(waitRun());
    }
    void DrawQuadraticCurveItem()
    {
        for (int i = 1; i < numberPointsCoin + 1; i++)
        {
            float t = i / (float)numberPointsCoin;
            positionCoin[i - 1] = CalculateQuadraticBezierPoint(t, transform.position, pointerRight.position, pointerCoin.position);
        }
    }
    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        p.z = 0;
        return p;
    }
    IEnumerator upSpeed()
    {
        yield return new WaitForSeconds(0.3f);
        speed += 2f;
        StartCoroutine(upSpeed());
    }
    IEnumerator waitRun()
    {

        yield return new WaitForSeconds(1.5f);
        gameObject.GetComponent<Animator>().enabled = false;
        isRun = true;
        StartCoroutine(upSpeed());
        yield return new WaitForSeconds(0.3f);
        Coin.SetActive(true);
    }

    void ItemFly()
    {
        DrawQuadraticCurveItem();
        if (Vector3.Distance(Coin.transform.position, positionCoin[indexArrayCoin]) < 0.1f)
        {
            // pointerCoin.gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            if (indexArrayCoin < positionCoin.Length - 1) indexArrayCoin = indexArrayCoin + 1;
        }
        if (Vector3.Distance(Coin.transform.position, positionCoin[numberPointsCoin - 1]) < 0.1f)
        {
            if (PointedCoin == false)
            {
        // Fly to target and grant experience.
                if (id == 2)
                {
                    GameBootstrap.Instance.AddExperience(numberCoin);
                }
                if (id == 2 || id == 3 || id == 4)
                    pointerCoin.gameObject.GetComponent<Animator>().Play("experienceProgress", -1, 0);
                if (id == 0 || id == 1 )
                    pointerCoin.gameObject.GetComponent<Animator>().Play("test", -1, 0);
                Coin.SetActive(false);
                PointedCoin = true;
            }
        }
        Coin.transform.position = Vector3.MoveTowards(Coin.transform.position, positionCoin[indexArrayCoin], Time.deltaTime * speed);
    }

    void Update()
    {
        if (isRun == true)
        {
            ItemFly();
            if (PointedCoin == true) Destroy(gameObject);
        }
    }
}
