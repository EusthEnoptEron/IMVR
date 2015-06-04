using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

public class SelectorView : View {
    private CylinderLayout cylinder;
    public SongSelection Selection;

    private Text _counterText;
    
    protected override void Awake()
    {
        base.Awake();

        Selection = new SongSelection();

        gameObject.AddComponent<CanvasGroup>();

        // BUILD CYLINDER
        cylinder = new GameObject().AddComponent<CylinderLayout>();
        cylinder.transform.SetParent(transform, false);
        cylinder.radius = 0.5f;
        cylinder.height = 1;
        cylinder.scale = 1;
        cylinder.Resize(8, 1);

        cylinder.autoLayout = false;
    }

    protected void Start()
    {


        // --- INIT COUNTER ---
        var counter = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/pref_SelectorCounter"));
        _counterText = counter.transform.FindRecursively("Counter").GetComponent<Text>();

        cylinder.SetTile(0, 0, counter);
        // ---

        // --- INIT SLIDERS ---
        cylinder.SetTile(cylinder.xSegments - 1, 0, InitSliders());
        // ---

        // --- INIT CHART ---
        var chart = GameObject.Instantiate<GameObject>(
            Resources.Load<GameObject>("Prefabs/Objects/pref_SongChart")    
        ).GetComponent<SongMetaChart>();

        chart.transform.localPosition = Vector3.zero;
        chart.SetSongs(Selection.Songs);

        cylinder.SetTile(1, 0, chart.gameObject);
        // ---

        Selection.SelectionChanged += Selection_SelectionChanged;
        
        FinishInitialization();
    }


    void Selection_SelectionChanged(object sender, System.EventArgs e)
    {
        _counterText.text = Selection.Songs.Count().ToString("G", CultureInfo.CurrentUICulture);
    }

    /// <summary>
    /// Inits the slider being used by the selector.
    /// </summary>
    /// <returns></returns>
    private GameObject InitSliders()
    {
        GameObject container = new GameObject("Slider Container");
        GameObject sliderPrefab = Resources.Load<GameObject>("Prefabs/Objects/pref_Slider");

        float sliderHeight = 0.2f;
        int i = 0;
        foreach (var criterion in Selection.GetCriteria())
        {
            var crit = criterion;
            var slider = Instantiate<GameObject>(sliderPrefab).GetComponent<CylinderSlider>();

            slider.onValueChanged.AddListener(
                delegate {
                    //Debug.Log("UPDATE" + criterion);
                    Selection.ChangeCriterion(crit, slider.MinValue, slider.MaxValue);
                }
            );

            slider.MinValue = Selection.GetCriterion(criterion).Min;
            slider.MaxValue = Selection.GetCriterion(criterion).Max;

            slider.transform.localPosition += Vector3.down * sliderHeight * i++;
            slider.transform.SetParent(container.transform, false);
        }

        return container;
    }

}
