using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
public class NotificationController : MonoBehaviour {
    public static NotificationController Instance { get; private set; }
    [FormerlySerializedAs("tb"), SerializeField] private GameObject notificationPrefab;

	void Awake () {
        Instance = this;
	}
    public void ShowWorldNotification(string vietnameseText, string englishText, Vector2 position)
    {
        GameObject notification = Instantiate(notificationPrefab, position, Quaternion.identity);
        if (Application.systemLanguage == SystemLanguage.Vietnamese)
            notification.transform.GetChild(0).GetComponent<Text>().text = vietnameseText;
        else
            notification.transform.GetChild(0).GetComponent<Text>().text = englishText;
        Destroy(notification, 1f);
    }
    public void ShowCanvasNotification(string vietnameseText, string englishText)
    {
        GameObject notification = Instantiate(notificationPrefab, transform.position, Quaternion.identity);
        notification.GetComponent<Canvas>().sortingOrder = 120;
        if (Application.systemLanguage == SystemLanguage.Vietnamese)
            notification.transform.GetChild(0).GetComponent<Text>().text = vietnameseText;
        else
            notification.transform.GetChild(0).GetComponent<Text>().text = englishText;
        Destroy(notification, 1f);
    }
}
