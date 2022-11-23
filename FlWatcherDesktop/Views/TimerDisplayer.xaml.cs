using System;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FlWatcherDesktop.Views;
/// <summary>
/// Interaction logic for TimerDisplayer.xaml
/// </summary>
public partial class TimerDisplayer : UserControl
{


    public TimerDisplayer()
    {
        InitializeComponent();

        Loaded += TimerDisplayer_Loaded;
    }

    private void TimerDisplayer_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        Storyboard sb = (Storyboard)line.FindResource("spin");
        sb.Begin();
    }
}
