using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//电子邮件puhalskijsemen@gmail.com
//源码网站 开vpn全局模式打开 http://web3incubators.com/
//电报https://t.me/gamecode999
//网页客服 http://web3incubators.com/kefu.html
using UnityEngine.EventSystems;
public class MagicTreeRewardIcon : MonoBehaviour {
    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject(0))
        {
            transform.parent.GetComponent<MagicCoinTree>().Harvest();
        }
    }
}
