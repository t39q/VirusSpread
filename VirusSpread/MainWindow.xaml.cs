using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static VirusSpread.Model.PeopleModel;

namespace VirusSpread
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void begin_Click(object sender, RoutedEventArgs e)
        {
            int count = 10;
            int moveDistance = int.Parse(moveSpeed.Text.ToString());
            int infectiousNum = int.Parse(infectious.Text);
            BussnessLogic.VirusSpread.Init();
            GenPeople(BussnessLogic.VirusSpread.peoples);
            Task.Run(() => { PeopleRun(BussnessLogic.VirusSpread.peoples, count); });
            Task.Run(() => { BussnessLogic.VirusSpread.Run(count,moveDistance); });
            Task.Run(() => { BussnessLogic.VirusSpread.Spread(count, infectiousNum); });
        }
        private void PeopleRun(ConcurrentBag<Model.PeopleModel.People> peoples,int count)
        {
            int spreadCount = 0;
            while (true)
            {
                if (spreadCount> count)
                {
                    break;
                }
                map.Dispatcher.Invoke(new Action(() => {
                    map.Children.Clear();
                    int N = 0;
                    foreach (var item in peoples)
                    {
                        Button button = buttonDic["P" + item.ID.ToString()];
                        Canvas.SetLeft(button, item.X);
                        Canvas.SetTop(button, item.Y);
                        if (item.Disease)
                        {
                            button.Background = Brushes.Red;
                            N++;
                        }
                        else
                        {
                            button.Background = Brushes.Green;
                        }
                        map.Children.Add(button);
                    }
                    infectedNum.Content = N.ToString();
                    countLable.Content=spreadCount.ToString();
                }));
                Thread.Sleep(1000);
                spreadCount++;
            }
        }
        ConcurrentDictionary<string, Button> buttonDic;
        private void GenPeople(ConcurrentBag<Model.PeopleModel.People> peoples)
        {
            buttonDic = new ConcurrentDictionary<string, Button>();
            map.Children.Clear();
            foreach (var item in peoples)
            {
                Button button = new Button();
                button.Name = "P" + item.ID.ToString();
                button.Content = item.ID.ToString();
                button.Foreground = Brushes.White;
                button.FontSize = 6;
                button.Width = 15;
                button.Height = 15;
                Canvas.SetLeft(button,item.X);
                Canvas.SetTop(button, item.Y);
                if (item.Disease)
                {
                    button.Background = Brushes.Red;
                }
                else 
                {
                    button.Background = Brushes.Green;
                }
                map.Children.Add(button);
                buttonDic.TryAdd(button.Name,button);
            }
        }
    }
}
