using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Gestures;

public static class RingMenuBuilder {

    private static GameObject _itemPrefab;
    private static GameObject ItemPrefab
    {
        get
        {
            if (_itemPrefab == null)
                _itemPrefab = Resources.Load<GameObject>("Prefabs/UI/pref_RingMenuItem");
            return _itemPrefab;
        }
    }

    private static GameObject _submenuPrefab;
    private static GameObject SubmenuPrefab
    {
        get
        {
            if (_submenuPrefab == null)
                _submenuPrefab = Resources.Load<GameObject>("Prefabs/UI/pref_RingSubMenuItem");
            return _submenuPrefab;
        }
    }


    public static RingMenuItem CreateItem(FingerType finger, string text, IRingMenu parent)
    {
        return CreateItem(finger, text, null, parent);
	}

    public static RingMenuItem CreateItem(FingerType finger, string text, Sprite icon, IRingMenu parent)
    {
        var item = GameObject.Instantiate<GameObject>(ItemPrefab);

        Initialize(item, finger, text, icon, parent);

        return item.GetComponent<RingMenuItem>();
    }

    public static RingSubMenu CreateMenu(FingerType finger, string text, Sprite icon, Sprite sprite, IRingMenu parent)
    {
        var item = GameObject.Instantiate<GameObject>(SubmenuPrefab);

        Initialize(item, finger, text, icon, parent);
        var menu = item.GetComponent<RingSubMenu>();
        menu.Thumbnail = sprite;
        menu.InfoText = text;

        return item.GetComponent<RingSubMenu>();
    }

    public static RingSubMenu CreateMenu(FingerType finger, string text, Sprite sprite, IRingMenu parent)
    {
        return CreateMenu(finger, text, null, sprite, parent);
    }

    private static void Initialize(GameObject item, FingerType finger, string text, Sprite icon, IRingMenu parent)
    {
        // Update names
        item.name = text;

        var menuItem = item.GetComponent<RingMenuItem>();
        menuItem.fingerType = finger;
        menuItem.text.text = text;

        var img = menuItem.ui.GetComponent<Image>();
        
        img.sprite = icon;
        if (icon == null) img.color = Color.clear;

        // Set parent
        item.transform.SetParent(parent.ItemNode.transform);
    }

}
