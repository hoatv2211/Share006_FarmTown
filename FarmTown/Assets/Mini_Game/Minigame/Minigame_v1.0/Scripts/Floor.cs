using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Floor : MonoBehaviour
{
    private Vector3 rootPos;
    public List<Enemy> enemies = new List<Enemy>();

    public FloorWhere floorWhere;
    public GameObject select;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<IPreviousPosition>()?.SetPreviousPosition(rootPos);
        GameObject[] listFoor = GameObject.FindGameObjectsWithTag("Floor");

        for (int i = 0; i < listFoor.Length; i++)
        {
            if (listFoor[i].GetComponent<Floor>().enemies.Count <= 0)
            {
                Player.instance.SetCanDestroyFloor(listFoor[i]);
            }
        }

        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = gameObject.transform;
            select.SetActive(true);
            if(Player.instance.canAttack)
            {
                select.SetActive(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            select.SetActive(false);
        }
    }

    private void Start()
    {
        if (select != null)
            select.GetComponent<SpriteRenderer>().sortingLayerName = "Canvas";
        StartCoroutine(Delay100ms());
    }

    IEnumerator Delay100ms()
    {
        yield return new WaitForSeconds(0.1f);
        rootPos = enemies[0].GetCheckpoint();
    }

    public void DestroyEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            GameManagerMiniGame.instance.totalEnemy--;
            if (enemies.Count > 0)
            {
                rootPos = enemies[0].GetCheckpoint();
            }
            else if (GameManagerMiniGame.instance.totalEnemy <= 0)
            {
                GameManagerMiniGame.instance.Win();
            }
        }
    }

    private void FixedUpdate()
    {
        RaycastHit2D hitMainha = Physics2D.Raycast(transform.position + new Vector3(0, 1.5f, 0), Vector2.up);
        if (hitMainha.collider != null)
        {
            if (hitMainha.collider.name.Equals("main_mai"))
            {
                floorWhere = FloorWhere.Top;
            }
            else
            {
                floorWhere = FloorWhere.Middle;
            }
        }
    }
}

public enum FloorWhere
{
    Top,
    Middle,
}
