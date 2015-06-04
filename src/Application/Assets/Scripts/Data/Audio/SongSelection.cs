using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using IMVR.Commons;
using System.Collections.Generic;
using Foundation;

/// <summary>
/// Central class for song selection!
/// </summary>
public class SongSelection {

    /// <summary>
    /// An min and max inclusive range.
    /// </summary>
    public struct Range
    {
        private float _min;
        private float _max;

        public float Min { get { return _min; } }
        public float Max { get { return _max; } }

        public Range(float min, float max)
        {
            _min = Mathf.Min(min, max);
            _max = Mathf.Max(min, max);
        }

        public bool Contains(float value)
        {
            return value >= _min && value <= _max;
        }
        public bool IsFull
        {
            get
            {
                return _min == 0 && _max == 1;
            }
        }
    }


    public event EventHandler SelectionChanged = delegate { };
    
    private List<Song> _allSongs;
    private List<Song> _filteredSongs;
    private List<Song> _filteredSongsBackBuffer;

    private bool _omitValueLess = true;
    
    private Dictionary<MetaGroup, Range> _criteria;
    private bool _dirty;

    private float _maxTempo = 0;

    public SongSelection()
    {
        _allSongs = ResourceManager.DB.Songs;
        _filteredSongs = _allSongs;
        _criteria = GetCriteria().ToDictionary(
            group => group,
            group => new Range(0, 1)
        );

        _maxTempo = _allSongs.Select(s => s.Tempo).Max();

        RequestRecalculate();
    }

    /// <summary>
    /// Resets the current selection.
    /// </summary>
    public void Reset()
    {
        _criteria = GetCriteria().ToDictionary(
            group => group,
            group => new Range(0, 1)
        );

        RaiseSelectionChanged();
    }

    public void ChangeCriterion(MetaGroup group, float min, float max)
    {
        _criteria[group] = new Range(min, max);

        RequestRecalculate();
    }

    public Range GetCriterion(MetaGroup group)
    {
        return _criteria[group];
    }

    /// <summary>
    /// Returns a list of criteria that can be used in this selection.
    /// </summary>
    /// <returns></returns>
    public MetaGroup[] GetCriteria()
    {
        return Enum.GetValues(typeof(MetaGroup)).Cast<MetaGroup>().ToArray();
    }


    /// <summary>
    /// Gets the set of currently selected songs.
    /// </summary>
    public IEnumerable<Song> Songs
    {
        get
        {
            return _filteredSongs;
        }
    }

    public bool OmitValueLess
    {
        get { return _omitValueLess; }
        set
        {
            if (value != _omitValueLess)
            {
                _omitValueLess = value;
                RequestRecalculate();
            }
        }
    }


    /// <summary>
    /// Helps filtering songs by value
    /// </summary>
    /// <param name="song"></param>
    /// <returns></returns>
    private bool SongIsValid(Song song)
    {
        foreach (var criterion in _criteria)
        {
            if (criterion.Key.HasValue(song))
            {
                float value = criterion.Key.GetValue(song).Value;
                
                // Normalze if tempo.
                if (criterion.Key == MetaGroup.Tempo)
                    value /= _maxTempo;

                if (!criterion.Value.Contains(value))
                {
                    //Task.RunOnMain(delegate { Debug.Log(criterion.Key); });
                    return false;
                }
            }
            else if (OmitValueLess && !criterion.Value.IsFull)
            {
                //Task.RunOnMain(delegate { Debug.Log(criterion.Key); });

                return false;
            }
        }
        return true;
    }

    private bool _recalculateRequested = false;
    private void RequestRecalculate()
    {
        if (!_recalculateRequested)
        {
            Task.Run(delegate
            {
                _filteredSongsBackBuffer = _allSongs.Where(SongIsValid).ToList();

                // Swap
                var temp = _filteredSongs;
                _filteredSongs = _filteredSongsBackBuffer;
                _filteredSongsBackBuffer = temp;

                _recalculateRequested = false;
                Task.RunOnMain(RaiseSelectionChanged);
            });

            
        }
    }

    private void RaiseSelectionChanged()
    {
        _dirty = true;
        SelectionChanged(this, new EventArgs());
    }

}
