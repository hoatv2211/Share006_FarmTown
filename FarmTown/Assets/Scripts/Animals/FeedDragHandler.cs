using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FeedDragHandler : MonoBehaviour {

    [FormerlySerializedAs("id")] public int feedCropId;
    private void OnTriggerEnter2D(Collider2D target)
    {
        var feedReceiver = target.gameObject.GetComponent<IFeedReceiver>();
        if (feedReceiver != null)
        {
            feedReceiver.Feed(feedCropId);
        }
    }
}
