using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using IMVR.Commons;

public class Playlist
{

    #region Properties and Events
    public event EventHandler Change;
    public event EventHandler IndexChange;

    /// <summary>
    /// Gets or sets whether or not the playlist should be cyclic (repeat itself)
    /// </summary>
    public bool Cyclic = false;
    private List<Song> playlist;

    /// <summary>
    /// Gets the current index, where -1 means that the playlist hasn't been initiated yet.
    /// </summary>
    public int Index { get; private set; }

    #endregion

    public Playlist()
    {
        playlist = new List<Song>();
        Index = -1;
    }

    public void Add(Song song)
    {
        playlist.Add(song);
        RaiseChange();
    }

    public void Add(IEnumerable<Song> songs)
    {
        playlist.AddRange(songs);
        RaiseChange();
    }

    public void Override(Song song)
    {
        Clear();
        Add(song);
    }

    public bool IsEmpty
    {
        get
        {
            return Count == 0;
        }
    }

    public void Clear()
    {
        playlist.Clear();
        Index = -1;
        RaiseChange();
    }

    public bool Move(int steps)
    {
        int newIndex;
        if (Cyclic)
            newIndex = ((Index + steps) + Count * 5) % Count;
        else
            newIndex = Mathf.Clamp(Index + steps, 0, Count - 1);


        if (newIndex != Index)
        {
            Index = newIndex;
            RaiseIndexChange();
            return true;
        }
        else return false;
    }

    public bool MoveBackward()
    {
        return Move(-1);
    }
    public bool MoveForward()
    {
        return Move(1);
    }

    public Song Current
    {
        get
        {
            if (Index == -1) return null;
            return playlist[Index];
        }
    }

    public int Count
    {
        get
        {
            return playlist.Count;
        }
    }

    public IList<Song> Songs
    {
        get
        {
            return playlist;
        }
    }


    #region Private Members
    private void RaiseChange()
    {
        var evt = Change;
        if (evt != null)
        {
            evt(this, new EventArgs());
        }
    }


    private void RaiseIndexChange()
    {
        var evt = IndexChange;
        if (evt != null)
        {
            evt(this, new EventArgs());
        }
    }

    #endregion
}
