using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ForestTreeClickHandler : MonoBehaviour
{

    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            if (PlayerPrefs.GetInt("dangdichuyen") == 0)
            {
                if (transform.parent.GetComponent<TreeSawAction>().isBeingCleared == false)
                {
                    if (!PlayerPrefs.HasKey("huongdan"))
                    {
                        MainCamera.Instance.DisableAll();
                        transform.parent.GetComponent<TreeSawAction>().ShowToolPanel();

                    }
                    else
                    {
                        if (PlayerPrefs.GetInt("huongdan") == 14)
                        {
                            transform.parent.GetComponent<TreeSawAction>().ShowToolPanel();
                            TutorialController.instance.pointerHand.SetActive(false);
                            TutorialController.instance.downwardDragHand.SetActive(true);
                            TutorialController.instance.MoveDownwardDragHand(new Vector2(transform.position.x, transform.position.y + 2f));
                            TutorialController.instance.ShowWorldMessage("Kéo icon cưa xuống gốc", "Pull the saw icon down to the root");
                        }
                    }
                }
            }
        }
    }
}
