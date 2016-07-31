namespace Simple.Wpf.DataGrid.Extensions
{
    using System.Windows;
    using System.Windows.Media;

    public static class DependencyObjectExtensions
    {
        public static T FindAncestor<T>(this DependencyObject current) where T : DependencyObject
        {
            current = VisualTreeHelper.GetParent(current);

            while (current != null)
            {
                var ancestor = current as T;
                if (ancestor != null)
                {
                    return ancestor;
                }
                current = VisualTreeHelper.GetParent(current);
            };

            return null;
        }

        public static T FindAncestor<T>(this DependencyObject current, T lookupItem) where T : DependencyObject
        {
            while (current != null)
            {
                var ancestor = current as T;
                if (ancestor != null && Equals(ancestor, lookupItem))
                {
                    return ancestor;
                }
                current = VisualTreeHelper.GetParent(current);
            };

            return null;
        }

        public static T FindAncestor<T>(this DependencyObject current, string parentName) where T : DependencyObject
        {
            while (current != null)
            {
                if (!string.IsNullOrEmpty(parentName))
                {
                    var frameworkElement = current as FrameworkElement;
                    if (current is T && frameworkElement != null && frameworkElement.Name == parentName)
                    {
                        return (T)current;
                    }
                }
                else if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            };

            return null;

        }

        public static T FindDescendant<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null)
            {
                return null;
            }

            T foundChild = null;

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var childType = child as T;
                if (childType == null)
                {
                    foundChild = FindDescendant<T>(child, childName);

                    if (foundChild != null)
                    {
                        break;
                    }
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }

                    foundChild = FindDescendant<T>(child, childName);

                    if (foundChild != null)
                    {
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static T FindDescendant<T>(this DependencyObject parent) where T : DependencyObject
        {
            if (parent == null)
            {
                return null;
            }

            T foundChild = null;

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var childType = child as T;
                if (childType == null)
                {
                    foundChild = FindDescendant<T>(child);
                    if (foundChild != null)
                    {
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }
            return foundChild;
        }
    }
}