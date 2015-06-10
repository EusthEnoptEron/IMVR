using UnityEngine;
using System.Collections;

public abstract class Theme
{
    public abstract Color BaseColor { get; }
    public abstract Color NormalColor { get; }
    public abstract Color HighlightedColor { get; }
    public abstract Color ActivatedColor { get; }
    public abstract Color SpecialColor { get; }


    private static Theme _current = new BaseTheme();
    public static event System.EventHandler Change = delegate { };

    public static Theme Current
    {
        get
        {
            return _current;
        }
        set
        {
            _current = value;
                
            Change(_current, new System.EventArgs());
        }
    }
}


public class BaseTheme : Theme
{

    public override Color BaseColor
    {
        get { return Color.white; }
    }

    public override Color NormalColor
    {
        get { return Color.white; }
    }

    public override Color HighlightedColor
    {
        get { return Color.white; }

    }

    public override Color ActivatedColor
    {
        get { return Color.white; }
    }

    public override Color SpecialColor
    {
        get { return Color.white; }

    }
}

public class GreenTheme : Theme
{

    public override Color BaseColor
    {
        get { return Color.green; }
    }

    public override Color NormalColor
    {
        get { return Color.white; }
    }

    public override Color HighlightedColor
    {
        get { return new Color(.75f, .75f, .75f); }
    }

    public override Color ActivatedColor
    {
        get { return new Color(.03f, .78f, .46f); }
    }

    public override Color SpecialColor
    {
        get { return new Color(1f, .06f, .06f); }
    }
}

public class BlueTheme : Theme
{

    public override Color BaseColor
    {
        get { return Color.blue; }
    }

    public override Color NormalColor
    {
        get { return Color.white; }
    }

    public override Color HighlightedColor
    {
        get { return new Color(.75f, .75f, .75f); }
    }

    public override Color ActivatedColor
    {
        get { return new Color(.03f, .46f, .78f); }
    }

    public override Color SpecialColor
    {
        get { return new Color(1f, .06f, .06f); }
    }
}