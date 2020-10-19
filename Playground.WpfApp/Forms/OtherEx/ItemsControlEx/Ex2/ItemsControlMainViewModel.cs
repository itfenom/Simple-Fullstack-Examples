using System.Collections.ObjectModel;
using System.Windows.Input;
using Playground.WpfApp.Mvvm;

namespace Playground.WpfApp.Forms.OtherEx.ItemsControlEx.Ex2
{
    public class ItemsControlMainViewModel : PropertyChangedBase
    {
        private ObservableCollection<IconInfo> _sourceIconInfos;

        public ObservableCollection<IconInfo> SourceIconInfos
        {
            get => _sourceIconInfos;
            set => SetPropertyValue(ref _sourceIconInfos, value);
        }

        private string _prompt;

        public string Prompt
        {
            get => _prompt;
            set => SetPropertyValue(ref _prompt, value);
        }

        public ICommand LoadCommand { get; set; }

        public override string Title => "ItemsControl using UserControl";

        public ItemsControlMainViewModel()
        {
            LoadCommand = new DelegateCommand(() => OnLoadIcons());
            ClickHandlerCommand = new DelegateCommand<string>(msg => OnIconClicked(msg), p => true);
        }

        private void OnLoadIcons()
        {
            SourceIconInfos = new ObservableCollection<IconInfo>
            {
                new IconInfo
                {
                    Label = "3D Shape 01",
                    ResourcePath ="/Playground.WpfApp;component/Images/3D-Shape-01.png",
                    Command = ClickHandlerCommand
                },
                new IconInfo
                {
                    Label = "3D Shape 02",
                    ResourcePath ="/Playground.WpfApp;component/Images/3D-Shape-02.png",
                    Command = ClickHandlerCommand
                },
                new IconInfo
                {
                    Label = "Astrologer",
                    ResourcePath ="/Playground.WpfApp;component/Images/Astrologer.png",
                    Command = ClickHandlerCommand
                },
                new IconInfo
                {
                    Label = "Balance",
                    ResourcePath ="/Playground.WpfApp;component/Images/Balance-02.png",
                    Command = ClickHandlerCommand
                },
                new IconInfo
                {
                    Label = "Candle",
                    ResourcePath ="/Playground.WpfApp;component/Images/Candle.png",
                    Command = ClickHandlerCommand
                },
                new IconInfo
                {
                    Label = "Clapper-Board",
                    ResourcePath ="/Playground.WpfApp;component/Images/Clapper-Board-02.png",
                    Command = ClickHandlerCommand
                },
                new IconInfo
                {
                    Label = "Container",
                    ResourcePath ="/Playground.WpfApp;component/Images/Container.png",
                    Command = ClickHandlerCommand
                },
                new IconInfo
                {
                    Label = "Cooler",
                    ResourcePath ="/Playground.WpfApp;component/Images/Cooler-01.png",
                    Command = ClickHandlerCommand
                },
                new IconInfo
                {
                    Label = "Crown-King",
                    ResourcePath ="/Playground.WpfApp;component/Images/Crown-King.png",
                    Command = ClickHandlerCommand
                },
                new IconInfo
                {
                    Label = "Digital Signage",
                    ResourcePath ="/Playground.WpfApp;component/Images/DigitalSignage.png",
                    Command = ClickHandlerCommand
                },
            };
        }

        public DelegateCommand<string> ClickHandlerCommand { get; set; }

        private void OnIconClicked(string msg)
        {
            Prompt = msg + " Clicked.";
        }
    }
}