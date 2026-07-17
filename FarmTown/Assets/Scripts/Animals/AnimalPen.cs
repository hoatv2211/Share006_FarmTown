using System.Collections;
using System.Collections.Generic;
using FarmTown.Save;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
public class AnimalPen : MonoBehaviour
{

    [FormerlySerializedAs("id")] public int animalTypeId;
    [FormerlySerializedAs("idThucAn")] public int feedCropId;
    [FormerlySerializedAs("stt")] public int instanceIndex;
    Coroutine moveCoroutine;
    Vector2 pointerStart, pointerEnd;
    int moveState;
    StableInstanceId stableIdentity;
    string StableId => stableIdentity != null ? stableIdentity.StableId : AnimalPersistence.DefaultPenStableId(animalTypeId, instanceIndex);
    string LegacyPenName => AnimalPersistence.DefaultLegacyPenName(animalTypeId, instanceIndex);
    void Start()
    {
        stableIdentity = GetComponent<StableInstanceId>();
        UpdateSortingOrder();
        if (AnimalPersistence.IsPenFlipped(StableId, LegacyPenName))
            transform.GetChild(0).localScale = new Vector3(transform.GetChild(0).localScale.x * -1, transform.GetChild(0).localScale.y);
        var occupancy = AnimalPersistence.GetPenOccupancy(StableId, LegacyPenName);
        if (occupancy >= 0)
        {
            for (int i = 0; i <= occupancy; i++)
            {
                transform.GetChild(i + 1).gameObject.SetActive(true);
            }
        }
    }
    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (PlayerPrefs.GetInt("dangdichuyen") == 0)
            {
                if (moveCoroutine != null)
                    StopCoroutine(moveCoroutine);
                moveCoroutine = StartCoroutine(BeginMoveAfterHold());
            }
        }
    }
    IEnumerator BeginMoveAfterHold()
    {
        pointerStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        yield return new WaitForSeconds(.8f);
        pointerEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Vector2.Distance(pointerStart, pointerEnd) < .2f)
        {
            moveState = 1;
            MovementDialog.instance.ShowMovementIndicator(transform.position);
            yield return new WaitForSeconds(.3f);
            MovementDialog.instance.originalPosition = transform.position;
            MainCamera.Instance.lockCam();
            moveState = 2;
            gameObject.AddComponent<PlacementCollisionChecker>();
            gameObject.AddComponent<Rigidbody2D>();
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            gameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
            MainCamera.Instance.unLockCam();
            MovementDialog.instance.Open();
            MovementDialog.instance.confirmButton.onClick.AddListener(ConfirmMove);
            MovementDialog.instance.cancelButton.onClick.AddListener(CancelMove);
            MovementDialog.instance.rotateButton.onClick.AddListener(Rotate);
            PlayerPrefs.SetInt("dangdichuyen", 1);
        }


    }
    private void OnMouseDrag()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {

            if (moveState == 2)
            {
                Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 posCam = new Vector2(cursorPosition.x, cursorPosition.y);
                Vector3 target = new Vector3(posCam.x, posCam.y, 0);
                target = new Vector3(((int)(posCam.x / 0.62f)) * 0.62f, ((int)(posCam.y / 0.32f)) * 0.32f, 0);
                transform.position = target;
            }
        }
    }
    private void OnMouseUp()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);
            if (moveState == 1)
                moveState = 0;
            if (moveState == 2)
            {


            }
        }
    }
    public void ConfirmMove()
    {

        MovementDialog.instance.newPosition = transform.position;
        if (gameObject.GetComponent<PlacementCollisionChecker>().hasCollision == true)
            transform.position = MovementDialog.instance.originalPosition;
        else
        {
            AnimalPersistence.SetPenPosition(StableId, LegacyPenName, transform.position);
        }
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        Destroy(gameObject.GetComponent<PlacementCollisionChecker>());
        gameObject.GetComponent<PolygonCollider2D>().isTrigger = false;
        moveState = 0;
        UpdateSortingOrder();
        MainCamera.Instance.unLockCam();
        MovementDialog.instance.Close();
        PlayerPrefs.SetInt("dangdichuyen", 0);
    }
    public void CancelMove()
    {
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        Destroy(gameObject.GetComponent<PlacementCollisionChecker>());
        gameObject.GetComponent<PolygonCollider2D>().isTrigger = false;
        MainCamera.Instance.unLockCam();
        transform.position = MovementDialog.instance.originalPosition;
        MovementDialog.instance.Close();
        moveState = 0;
        PlayerPrefs.SetInt("dangdichuyen", 0);
    }
    public void Rotate()
    {
        transform.GetChild(0).localScale = new Vector3(transform.GetChild(0).localScale.x * -1, transform.GetChild(0).localScale.y);
        AnimalPersistence.SetPenFlipped(StableId, LegacyPenName,
            !AnimalPersistence.IsPenFlipped(StableId, LegacyPenName));
    }
    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (transform.GetChild(1).GetComponent<FarmAnimal>().CurrentState == "doi" || transform.GetChild(2).GetComponent<FarmAnimal>().CurrentState == "doi" || transform.GetChild(3).GetComponent<FarmAnimal>().CurrentState == "doi")
                AnimalFeedingPanel.instance.open(transform.position, feedCropId);
            if (PlayerPrefs.GetInt("huongdan") == 22)
            {
                TutorialController.instance.pointerHand.SetActive(false);
                TutorialController.instance.animalFeedDragHand.SetActive(true);
                TutorialController.instance.ShowWorldMessage("Kéo icon lúa để cho ăn", "Drag the rice icon to feed it");
            }
            for (int i = 1; i < 4; i++)
            {
                if (transform.GetChild(i).GetComponent<FarmAnimal>().CurrentState == "thuhoach")
                {
                    transform.GetChild(i).GetComponent<FarmAnimal>().Harvest();
                }
            }
        }
    }

    public void PlaceAnimal()
    {
        var occupancy = AnimalPersistence.GetPenOccupancy(StableId, LegacyPenName);
        if (occupancy < 0)
        {
            AnimalPersistence.SetPenOccupancy(StableId, LegacyPenName, 0);
            transform.GetChild(1).gameObject.SetActive(true);
            AnimalPersistence.SetAnimalQuantity(animalTypeId, AnimalPersistence.GetAnimalQuantity(animalTypeId) + 1);
            AnimalShopList.instance.RefreshQuantity(animalTypeId);
            GameBootstrap.Instance.AddGold(-AnimalPersistence.GetAnimalPrice(animalTypeId));
            if (PlayerPrefs.HasKey("huongdan"))
            {
                PlayerPrefs.SetInt("huongdan", PlayerPrefs.GetInt("huongdan") + 1);

            }
        }
        else
        {
            if (occupancy < 2)
            {
                occupancy += 1;
                AnimalPersistence.SetPenOccupancy(StableId, LegacyPenName, occupancy);
                for (int i = 0; i <= occupancy; i++)
                {
                    transform.GetChild(i + 1).gameObject.SetActive(true);

                }
                AnimalPersistence.SetAnimalQuantity(animalTypeId, AnimalPersistence.GetAnimalQuantity(animalTypeId) + 1);
                AnimalShopList.instance.RefreshQuantity(animalTypeId);
                GameBootstrap.Instance.AddGold(-AnimalPersistence.GetAnimalPrice(animalTypeId));
                if (PlayerPrefs.HasKey("huongdan"))
                {
                    PlayerPrefs.SetInt("huongdan", PlayerPrefs.GetInt("huongdan") + 1);
                    if (PlayerPrefs.GetInt("huongdan") == 22)
                    {
                        //PlayerPrefs.DeleteKey("huongdan");
                        TutorialController.instance.shopDragHand.SetActive(false);
                        DialogShop.instance.close();
                        TutorialController.instance.pointerHand.SetActive(true);
                        TutorialController.instance.MovePointerHand(transform.position);
                        TutorialController.instance.ShowWorldMessage("Nhấn vào chuồng gà", "Click the chicken coop");
                    }
                }
            }
            else
            {
                NotificationController.Instance.ShowWorldNotification("Đã đủ vật nuôi!", "The breeding cage is full", transform.position);
            }
        }
    }
    public void UpdateSortingOrder()
    {
        Vector2 target = transform.position;
        transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - target.y) * 100f);
        transform.GetChild(1).GetComponent<MeshRenderer>().sortingOrder = Mathf.RoundToInt((13 - target.y) * 100f) + 1;
        transform.GetChild(2).GetComponent<MeshRenderer>().sortingOrder = Mathf.RoundToInt((13 - target.y) * 100f) + 1;
        transform.GetChild(3).GetComponent<MeshRenderer>().sortingOrder = Mathf.RoundToInt((13 - target.y) * 100f) + 1;
        if (animalTypeId != 0)
            transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt((13 - target.y) * 100f) + 2;
    }
}
