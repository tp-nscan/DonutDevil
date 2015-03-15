using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WpfUtils
{
    public static class ObservableCollectionEx
    {
        public static void SelectedItem<T>(this ObservableCollection<T> collection, T selectedItem)
            where T : class, ISelectable
        {
            foreach (var item in collection)
            {
                item.IsSelected = item.Equals(selectedItem);
            }
        }

        public static void AddMany<T>(this ObservableCollection<T> observableCollection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                observableCollection.Add(item);
            }
        }
    }

    public interface ISelectable
    {
        bool IsSelected { get; set; }
    }
}
