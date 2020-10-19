using System.Collections.ObjectModel;
using System.Linq;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.OtherEx.ItemsControlEx.Ex2
{
    public class IconViewModel : PropertyChangedBase
    {
        private ObservableCollection<IconInfo> _iconInfos;

        public IconViewModel()
        {
            AscSortCommand = new DelegateCommand(OnSortAsc);
            DescSortCommand = new DelegateCommand(OnSortDesc);
        }

        public DelegateCommand AscSortCommand { get; set; }
        public DelegateCommand DescSortCommand { get; set; }

        public ObservableCollection<IconInfo> IconInfos
        {
            get => _iconInfos;
            set => SetPropertyValue(ref _iconInfos, value);
        }

        private void OnSortAsc()
        {
            if (IconInfos != null)
            {
                IconInfos = new ObservableCollection<IconInfo>(IconInfos.OrderBy(i => i.Label));
            }
        }

        private void OnSortDesc()
        {
            if (IconInfos != null)
            {
                IconInfos = new ObservableCollection<IconInfo>(IconInfos.OrderByDescending(i => i.Label));
            }
        }
    }
}