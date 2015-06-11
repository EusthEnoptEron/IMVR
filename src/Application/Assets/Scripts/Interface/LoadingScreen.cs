using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using Gestures;

public class LoadingScreen : Singleton<LoadingScreen> {
    
    private static readonly string kPrecacheFontGlyphsString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_+=~`[]{}|\\:;\"'<>,.?/ ";
     
    [System.Serializable]
    public struct CacheFont
    {
        public Font theFont;
        public int size;
        public FontStyle style;
    };

    private Image _fill;
    private CanvasGroup _shutter;
    private int _okCount = 0;

    
    public GameObject shimCamera;
    
    /// <summary>
    /// Fonts to preload.
    /// </summary>
    public CacheFont[] fonts;

	// Use this for initialization
	void Awake () {
        _fill = transform.FindRecursively("Fill").GetComponent<Image>();
        _shutter = transform.GetComponentInChildren<CanvasGroup>();

	}

    IEnumerator Start()
    {

        // Precache fonts, taken from http://answers.unity3d.com/questions/733570/massive-lag-due-to-fontcachefontfortext-please-hel.html
        if (fonts != null)
        {
            foreach (var font in fonts)
            {
                StartCoroutine(PrecacheFontGlyphs(
                    font.theFont,
                    font.size,
                    font.style,
                    kPrecacheFontGlyphsString
                ));
            }
        }

        ModeController.Instance.Mode = GameObject.FindObjectOfType<BrowseMode>();


        // Load artist imagery
        ResourceManager.DB.Artists.ForEach(artist =>
        {

            var ticket = artist.Pictures.FirstOrDefault()
                    ?? artist.Albums.Select(album => album.Atlas).FirstOrDefault();

            if (ticket != null)
                ImageAtlas.LoadSprite(ticket);
        });


        yield return new WaitForSeconds(2);
        Debug.Log("Disable");
        ModeController.Instance.Mode = null;
    }


    // Precache the font glyphs for the given font data.
    // Intended to run asynchronously inside of a coroutine.
    IEnumerator PrecacheFontGlyphs(Font theFont, int fontSize, FontStyle style, string glyphs)
    {
        for (int index = 0; (index < glyphs.Length); ++index)
        {
            theFont.RequestCharactersInTexture(
                glyphs[index].ToString(),
                fontSize, style);
            yield return null;
        }

        yield break;
    }

	
	// Update is called once per frame
	void Update () {
        //Debug.Log(ImageAtlas.Progress);
        _fill.fillAmount = ImageAtlas.Progress;

        if (!ImageAtlas.IsLoading) _okCount++;
        else _okCount = 0;

        if (_okCount > 5)
        {
            _okCount = 0;
            enabled = false;
        }
	}

    void OnEnable()
    {

        HandProvider.Instance.gameObject.SetActive(false);

        OVRManager.display.timeWarp = false;

        //shimCamera.SetActive(true);
        _shutter.alpha = 1;
        foreach (var camera in shimCamera.GetComponentsInChildren<Camera>())
        {
            camera.clearFlags = CameraClearFlags.SolidColor;
        }
    }

    void OnDisable()
    {
        HandProvider.Instance.gameObject.SetActive(true);

        OVRManager.capiHmd.RecenterPose();
       
        _shutter.DOFade(0, 1).OnComplete(delegate
        {
            foreach (var camera in shimCamera.GetComponentsInChildren<Camera>())
            {
                camera.clearFlags = CameraClearFlags.Nothing;
            }
        });
    }

}
