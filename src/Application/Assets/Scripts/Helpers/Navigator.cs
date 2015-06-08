using UnityEngine;
using System.Collections;

/// <summary>
/// Helper class for navigating
/// </summary>
public static class Navigator {

    public static void ChangeView(View view, bool pop = true)
    {
        ModeController.Instance.Mode.ChangeView(view, pop);
    }

    public static T ChangeView<T>() where T : View
    {
        return ModeController.Instance.Mode.ChangeView<T>();
    }


    /// <summary>
    /// Steps into a view.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Navigate<T>() where T : View
    {
        return ModeController.Instance.Mode.Navigate<T>();
    }

    /// <summary>
    /// Goes back a view.
    /// </summary>
    /// <returns></returns>
    public static View GoBack()
    {
        return ModeController.Instance.Mode.GoBack();
    }
}
