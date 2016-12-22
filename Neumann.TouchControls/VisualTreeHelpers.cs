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
        /// Searches for child elements of the given type.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <param name="referenceVisual">Parent element.</param>
        /// <param name="name">Name of the element to find.</param>
        /// <returns>Element of given type and name, or null.</returns>
        public static T FindChild<T>(this FrameworkElement referenceVisual, string name) where T : DependencyObject
        {
            var element = referenceVisual as DependencyObject;
            var result = default(T);

            if (element != null)
            {
                var count = VisualTreeHelper.GetChildrenCount(element);
                for (int i = 0; i < count; i++)
                {
                    var childElement = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                    if (childElement != null &&
                        childElement.GetType().Equals(typeof(T)) &&
                        childElement.Name == name)
                    {
                        result = childElement as T;
                        break;
                    }

                    result = childElement.FindChild<T>(name);
                    if (result != null)
                    {
                        break;
                    }
                }
            }
            return result;
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
