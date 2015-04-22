using UnityEngine;
using System.Collections;
using IMVR.Commons;
using UnityEngine.UI;
using VirtualHands.Data.Image;
using System.Linq;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ArtistTile : Tile, IPointerClickHandler {
    private RectTransform rectTransform;
    private UnityEngine.UI.Image m_image;
    private Text m_text;
    private Artist m_artist;

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
        name = artist.Name;

        m_artist = artist;
    }

    protected override void OnDestroy()
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var artistView = FlowManager.Instance.PushView<ArtistView>();
        artistView.artist = m_artist;

        Jukebox.Instance.Playlist.Add(m_artist.Albums.SelectMany(a => a.Tracks));
        Jukebox.Instance.Play();
    }

}
