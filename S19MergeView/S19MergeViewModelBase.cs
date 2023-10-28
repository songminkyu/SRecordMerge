using CommunityToolkit.Mvvm.Input;
using S19Merge.BindServices;
using S19Merge.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace S19Merge.ViewModel
{
    public class S19MergeViewModelBase : BindableBase
    {
        protected List<string>? allAddresses { get; set; }
        public S19MergeViewModelBase() { }

        public ObservableCollection<SRecord>? SRecords
        {
            get { return GetValue<ObservableCollection<SRecord>>(); }
            set { SetValue(value); }
        }

        public RelayCommand<object>? SRecordLoadedCommand
        {
            get { return GetValue<RelayCommand<object>>(); }
            set { SetValue(value); }
        }
        public RelayCommand<object>? SRecordExportCommand
        {
            get { return GetValue<RelayCommand<object>>(); }
            set { SetValue(value); }
        }
    }
}
