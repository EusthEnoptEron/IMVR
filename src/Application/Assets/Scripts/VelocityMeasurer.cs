using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class VelocityMeasurer
{
    public float Interval = 0.3f;

    struct Entry
    {
        public float Time;
        public Vector3 Position;
    }

    private List<Entry> entries = new List<Entry>();

    public void AddPosition(Vector3 position)
    {

        entries.Add(new Entry
        {
            Time = Time.time,
            Position = position
        });
    }

    public Vector3 GetVelocity()
    {
        if (entries.Count < 2) return Vector3.zero;

        var e2 = entries.Last();
        var e1 = entries[entries.Count - 2];
        // Calculate
        return (e2.Position - e1.Position) / (e2.Time - e1.Time);
        //return GetDifference() / Interval;
    }

    public Vector3 GetDifference()
    {
        entries.RemoveAll(e => e.Time < Time.time - Interval);
        return (entries.LastOrDefault().Position - entries.FirstOrDefault().Position);
    }

    public IEnumerable<Vector3> Entries
    {
        get
        {
            return entries.Select(e => e.Position);
        }
    }
}

public class Accumulator<T>
{
    public float Interval = 0.3f;

    struct Entry
    {
        public float Time;
        public T Value;
    }
    private List<Entry> entries = new List<Entry>();

    public Accumulator() { }
    public Accumulator(float interval)
    {
        Interval = interval;
    }

    public void Add(T sample)
    {
        entries.Add(new Entry
        {
            Time = Time.time,
            Value = sample
        });
    }

    public T[] Values
    {
        get
        {
            Clean();
            return entries.Select(e =>e.Value).ToArray();
        }
    }

    private void Clean()
    {
        entries.RemoveAll(e => e.Time < Time.time - Interval);
    }

    internal void Clear()
    {
        entries.Clear();
    }
}