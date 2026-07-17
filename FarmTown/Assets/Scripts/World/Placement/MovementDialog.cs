using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
public class MovementDialog : MonoBehaviour
{
    public static MovementDialog instance;
    [FormerlySerializedAs("btnQuay")] public Button rotateButton;
    [FormerlySerializedAs("btnYes")] public Button confirmButton;
    [FormerlySerializedAs("btnNo")] public Button cancelButton;
    private Animator animator;
    [FormerlySerializedAs("muiTen"), SerializeField] private GameObject movementIndicatorPrefab;
    [FormerlySerializedAs("posOld")] public Vector3 originalPosition;
    [FormerlySerializedAs("posNew")] public Vector3 newPosition;
    void Start()
    {
        PlayerPrefs.SetInt("dangdichuyen", 0);
        instance = this;
        animator = GetComponent<Animator>();
    }
    public void Open()
    {
        
        MainCamera.Instance.lockCam();
        animator.enabled = true;
        animator.Play("dialogLiem", -1, 0);
    }
    public void Close()
    {
        animator.enabled = false;
        transform.localScale = Vector3.zero;
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        rotateButton.onClick.RemoveAllListeners();
    }
    
    public void ShowMovementIndicator(Vector2 position)
    {
        GameObject indicator = Instantiate(movementIndicatorPrefab, position, Quaternion.identity);
        Destroy(indicator, .3f);
    }
    
}
