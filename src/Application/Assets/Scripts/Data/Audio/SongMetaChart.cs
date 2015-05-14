using UnityEngine;
using System.Collections;
using IMVR.Commons;
using System.Collections.Generic;
using System;
using System.Linq;
using Kender.uGUI;

[RequireComponent(typeof(PointChart))]
public class SongMetaChart : MonoBehaviour {
    private PointChart pointChart;
    private IEnumerable<Song> data;

    public MetaGroup xAxis = MetaGroup.Energy;
    public MetaGroup yAxis = MetaGroup.Danceability;
    public MetaGroup zAxis = MetaGroup.Speechiness;

    public Canvas canvas;
    public GameObject comboboxPrefab;

    private ComboBox[] comboboxes = new ComboBox[3];

	// Use this for initialization
	void Awake () {
        pointChart = GetComponent<PointChart>();

        CreateCombobox(0);
        CreateCombobox(1);
        CreateCombobox(2);
	}

    IEnumerator Start()
    {
        yield return null;

        InitCombobox(0);
        InitCombobox(1);
        InitCombobox(2);

        // Debug
        SetSongs(ResourceManager.DB.Songs);
    }

    private void CreateCombobox(int axis)
    {
        comboboxes[axis] = GameObject.Instantiate<GameObject>(comboboxPrefab).GetComponent<ComboBox>();
        comboboxes[axis].transform.SetParent(canvas.transform, false);
    }

    private void InitCombobox(int axis)
    {
        var combobox = comboboxes[axis];
        switch (axis)
        {
            case 0: combobox.transform.localPosition = (pointChart.pivotOffset + Vector3.right) * 1200; break;
            case 1: combobox.transform.localPosition = (pointChart.pivotOffset + Vector3.up) * 1200; break;
            case 2: combobox.transform.localPosition = (pointChart.pivotOffset + Vector3.forward) * 1200; break;
            default: Debug.LogError("Invalid axis"); break;
        }

        combobox.transform.localScale = Vector3.one * 2;

        combobox.ClearItems();
        combobox.AddItems(new string[]{ "Select axis" }.Concat(Enum.GetNames(typeof(MetaGroup))).ToArray());

        combobox.SelectedIndex = (int)(axis == 0 ? xAxis : (axis == 1 ? yAxis : zAxis)) + 1;

        combobox.OnSelectionChanged += delegate(int item)
        {
            ChangeAxis(axis, combobox.Items[item].Caption);
        };

    }


	
	// Update is called once per frame
	void Update () {
	
	}

    public void ChangeAxis(int axis, string value)
    {
        switch (axis)
        {
            case 0:
                xAxis = (MetaGroup)Enum.Parse(typeof(MetaGroup), value, false);
                break;
            case 1:
                yAxis = (MetaGroup)Enum.Parse(typeof(MetaGroup), value, false);
                break;
            case 2:
                zAxis = (MetaGroup)Enum.Parse(typeof(MetaGroup), value, false);
                break;
            default:
                Debug.LogError("Invalid axis");
                break;
        }

        Refresh();
    }

    private void Refresh()
    {
        // Set points
        pointChart.points = data.Select(song => new Vector3(
            xAxis.HasValue(song) ? xAxis.GetValue(song).Value : 0f,
            yAxis.HasValue(song) ? yAxis.GetValue(song).Value : 0f,
            zAxis.HasValue(song) ? zAxis.GetValue(song).Value : 0f
        )).ToArray();

        pointChart.UpdatePoints();
    }

    public void SetSongs(IEnumerable<Song> songs)
    {
        data = songs;

        Refresh();
    }
}
