using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Neumann.TouchControls
{
    public static class VisualTreeHelpers
    {
        /// <summary>
        /// Searches for child elements of the given type.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <param name="referenceVisual">Parent element.</param>
        /// <returns>First element of given type, or null.</returns>
        public static T GetVisualChild<T>(this DependencyObject referenceVisual) where T : DependencyObject
        {
            DependencyObject child = null;
            for (Int32 i = 0; i < VisualTreeHelper.GetChildrenCount(referenceVisual); i++)
            {
                child = VisualTreeHelper.GetChild(referenceVisual, i) as DependencyObject;
                if (child != null && (child.GetType() == typeof(T)))
                {
                    break;
                }
                else if (child != null)
                {
                    child = GetVisualChild<T>(child);
                    if (child != null && (child.GetType() == typeof(T)))
                    {
                        break;
                    }
                }
            }
            return child as T;
        }

        /// <summary>
        /// Searches for a parent element of the given type.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <param name="element">Child element.</param>
        /// <returns>Element of given type, or null.</returns>
        public static T GetParent<T>(this DependencyObject element) where T : DependencyObject
        {
            if (element != null)
            {
                if (typeof(T).Equals(element.GetType()))
                {
                    return (T)element;
                }
                else
                {
                    var parent = VisualTreeHelper.GetParent(element);
                    var result = GetParent<T>(parent);
                    if (result != null)
                    {
                        return (T)result;
                    }
                }
            }
            return default(T);
        }
    }
}
