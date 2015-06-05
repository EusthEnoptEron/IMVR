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
    private GameObject _prefLabel;
    
    protected override void Awake()
    {
        base.Awake();

        Selection = new SongSelection();

        gameObject.AddComponent<CanvasGroup>();

        // BUILD CYLINDER
        cylinder = new GameObject().AddComponent<CylinderLayout>();
        cylinder.transform.SetParent(transform, false);
        cylinder.radius = 0.5f;
        cylinder.height = 1f;
        cylinder.scale = 1;
        cylinder.Resize(8, 2);

        cylinder.autoLayout = false;

        _prefLabel = Resources.Load<GameObject>("Prefabs/UI/pref_Label");
    }

    protected void Start()
    {


        // --- INIT COUNTER ---
        var counter = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/pref_SelectorCounter"));
        _counterText = counter.transform.FindRecursively("Counter").GetComponent<Text>();
        counter.GetComponentInChildren<Button>().onClick.AddListener(StartListening);
            

        cylinder.SetTile(0, 1, counter);
        // ---

        // --- INIT SLIDERS ---
        var criteria = Selection.GetCriteria();
        cylinder.SetTile(cylinder.xSegments - 1, 1, InitSliders(0, criteria.Length / 2));
        cylinder.SetTile(1, 1, InitSliders(criteria.Length / 2));

        // ---

        // --- INIT CHART ---
        var chart = GameObject.Instantiate<GameObject>(
            Resources.Load<GameObject>("Prefabs/Objects/pref_SongChart")    
        ).GetComponent<SongMetaChart>();

        chart.transform.localPosition = Vector3.zero;
        chart.SetSongs(Selection.Songs);

        var indicator = chart.GetComponent<SelectionVisualizer>();
        indicator.selection = Selection;

        cylinder.SetTile(0, 0, chart.gameObject);
        // ---

        Selection.SelectionChanged += Selection_SelectionChanged;
        
        FinishInitialization();
    }

    private void StartListening()
    {
        var listenView = Navigator.Navigate<ListenView>();
        listenView.selection = Selection;
    }



    void Selection_SelectionChanged(object sender, System.EventArgs e)
    {
        _counterText.text = Selection.Songs.Count().ToString("G", CultureInfo.CurrentUICulture);
    }

    /// <summary>
    /// Inits the slider being used by the selector.
    /// </summary>
    /// <returns></returns>
    private GameObject InitSliders(int offset, int length = -1)
    {
        GameObject container = new GameObject("Slider Container");
        GameObject sliderPrefab = Resources.Load<GameObject>("Prefabs/Objects/pref_Slider");

        float sliderHeight = 0.4f;
        var criteria = Selection.GetCriteria();

        if (length < 0) length = criteria.Length;

        Debug.LogFormat("From {0} to {1}", offset, offset + length);


        for (int i = 0; (i + offset) < criteria.Length && i < length; i++)
        {
            var criterion = criteria[i + offset];
            var slider = Instantiate<GameObject>(sliderPrefab).GetComponent<CylinderSlider>();
            var label = Instantiate<GameObject>(_prefLabel).GetComponent<Text>();

            slider.onValueChanged.AddListener(
                delegate
                {
                    //Debug.Log("UPDATE" + criterion);
                    Selection.ChangeCriterion(criterion, slider.MinValue, slider.MaxValue);
                }
            );

            slider.MinValue = Selection.GetCriterion(criterion).Min;
            slider.MaxValue = Selection.GetCriterion(criterion).Max;

            slider.transform.localPosition += Vector3.down * sliderHeight * i;
            label.transform.localPosition += Vector3.down * sliderHeight * i + Vector3.up * (sliderHeight / 2f);

            slider.transform.SetParent(container.transform, false);
            label.transform.SetParent(container.transform, false);

            label.text = criterion.ToString();
        }

        return container;
    }

}
