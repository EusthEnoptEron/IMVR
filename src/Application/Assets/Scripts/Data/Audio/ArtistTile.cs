using UnityEngine;
using System.Collections;
using IMVR.Commons;
using UnityEngine.UI;
using VirtualHands.Data.Image;
using System.Linq;

public class ArtistTile : Tile {
    private RectTransform rectTransform;
    private UnityEngine.UI.Image m_image;
    private Text m_text;

    protected override void Awake()
    {
        //m_image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = Vector2.one * Tile.PIXELS_PER_UNIT;

        var component = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/PictureWithText"));
        //m_image = new GameObject().AddComponent<Image>();
        m_image = component.GetComponentInChildren<UnityEngine.UI.Image>();
        m_text = component.GetComponentInChildren<Text>();

        // Create tile
        component.transform.SetParent(transform, false);
        //component.transform.localPosition = Vector3.zero;
        //component.transform.localRotation = Quaternion.identity;
        //component.transform.localScale = Vector3.one;
        //m_image.sprite = maskSprite;
    }

    public void SetArtist(Artist artist)
    {
        var firstAlbum = artist.Albums.FirstOrDefault(album => album.Atlas != null);
        if (firstAlbum != null)
        {
            //Debug.LogFormat("{0}: ({1}) {2}", artist.Name, firstAlbum.Atlas.Position, firstAlbum.Atlas.Atlas.Path);
            m_image.sprite = ImageAtlas.LoadSprite(firstAlbum.Atlas);
        }

        //m_image = ImageAtlas.LoadSprite(artist.)
        m_text.text = artist.Name;
    }

    protected override void OnDestroy()
    {
        //throw new System.NotImplementedException();
    }
}
