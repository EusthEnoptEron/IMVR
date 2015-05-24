using UnityEngine;
using System.Collections;

/// <summary>
/// Helper class for navigating
/// </summary>
public static class Navigator {

    public static void ChangeView(View view, bool pop = true)
    {
        RootPanel.Instance.Controller.ChangeView(view, pop);
    }

    public static T ChangeView<T>() where T : View
    {
        return RootPanel.Instance.Controller.ChangeView<T>();
    }


    /// <summary>
    /// Steps into a view.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Navigate<T>() where T : View
    {
        return RootPanel.Instance.Controller.Navigate<T>();
    }

    /// <summary>
    /// Goes back a view.
    /// </summary>
    /// <returns></returns>
    public static View GoBack()
    {
        return RootPanel.Instance.Controller.GoBack();
    }
}
