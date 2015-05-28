using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IMVR.Commons;
using Foundation;
using System.Linq;
using System.Text;
using System.Globalization;

public class ArtistOverView : View {
    private ListGroupedCircleLayout layout;
    private CylinderInteractor interactor;
    protected override void Awake()
    {
        base.Awake();

        layout = gameObject.AddComponent<ListGroupedCircleLayout>();
        interactor = layout.gameObject.AddComponent<CylinderRaycaster>()
                           .gameObject.AddComponent<CylinderInteractor>();

        layout.height = 1;
        layout.radius = 0.5f;
    }

    IEnumerator Start()
    {
        //yield return null;

        //Debug.Log(db.Artists.Count);
        var groups = ResourceManager.DB.Artists.Take(20).GroupBy(o =>
        {
            char firstLetter = RemoveDiacritics( o.Name.Substring(0, 1).ToUpper() )[0];
            
            if (firstLetter < 'A' || firstLetter > 'Z') firstLetter = '#';
           
            return firstLetter.ToString();
        })
        .ToDictionary(g => g.Key, g => g.Select(artist =>
        {
            var tile = new GameObject().AddComponent<ArtistTile>();
            tile.SetArtist(artist);
            return tile.gameObject;
        }).ToList());

        for (char c = 'A'; c <= 'Z'; c++)
        {
            if (!groups.ContainsKey(c.ToString())) groups.Add(c.ToString(), new List<GameObject>());
        }

        layout.Items = groups;
        yield return null;

    }

    private string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

}
