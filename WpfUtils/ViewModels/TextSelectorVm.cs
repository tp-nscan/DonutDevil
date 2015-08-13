using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace WpfUtils.ViewModels
{
    public class TextSelectorVm : NotifyPropertyChanged
    {
        public TextSelectorVm(IEnumerable<string> textSelections)
        {
            TextSelections = textSelections.ToList();
        }

        private readonly Subject<string> _textSelectionChanged
            = new Subject<string>();
        public IObservable<string> OnTextSelected
        {
            get { return _textSelectionChanged; }
        }

        #region SelectCommand

        RelayCommand _selectCommand;
        public ICommand SelectCommand
        {
            get
            {
                return _selectCommand ?? (
                    _selectCommand = new RelayCommand(
                        param => DoSelect(),
                        param => CanSelect()
                    ));
            }
        }

        private void DoSelect()
        {
            _textSelectionChanged.OnNext(SelectedText);
        }

        bool CanSelect()
        {
            return ! String.IsNullOrEmpty(SelectedText);
        }

        #endregion // SelectCommand

        public IList<string> TextSelections { get; }

        private string _selectedText;
        public string SelectedText
        {
            get { return _selectedText; }
            set
            {
                _selectedText = value;
                OnPropertyChanged("SelectedText");
            }
        }
    }
}