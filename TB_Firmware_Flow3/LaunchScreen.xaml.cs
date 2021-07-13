using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TB_Firmware_Flow3.Manager;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.UI.Core.Preview;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace TB_Firmware_Flow3
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LaunchScreen : Page
    {
        Manager.XboxOneGip xbg;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.GetType().Equals(typeof(XboxOneGip)))
            {
                xbg = (XboxOneGip)e.Parameter;
            }
            else if (e.Parameter.GetType().Equals(typeof(string)))
            {
                ;
            }
            NavigationCacheMode = NavigationCacheMode.Enabled;

        }


        DispatcherTimer timerMonitorPlug = new DispatcherTimer();
        DispatcherTimer timerMonitorPlug1 = new DispatcherTimer();
        DispatcherTimer timerMonitorPlug2 = new DispatcherTimer();

        public static string sowfwareVersion { get; } = "V1.0.2";
        public LaunchScreen()
        {
            this.InitializeComponent();

            timerMonitorPlug.Interval = new TimeSpan(0, 0, 0, 0, 20);　　//6秒后进入圆圈旋转的界面
            timerMonitorPlug.Tick += onTimerMonitorPlug;
            timerMonitorPlug.Start();

            timerMonitorPlug1.Interval = new TimeSpan(0, 0, 0, 0, 8000);　　//6秒后进入圆圈旋转的界面
            timerMonitorPlug1.Tick += onTimerMonitorPlug1;
            timerMonitorPlug1.Start();

            //timerMonitorPlug2.Interval = new TimeSpan(0, 0, 0, 0, 4300);　　//6秒后进入圆圈旋转的界面
            //timerMonitorPlug2.Tick += onTimerMonitorPlug2;
            //timerMonitorPlug2.Start();

            xbg = new XboxOneGip();
            xbg.InitxboxGip();
            XboxOneGip.IsConnected += IsConnect;


            Windows.UI.Core.Preview.SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += MainPage_CloseRequested;

        }
        private async void MainPage_CloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            // 让用户无法关闭
            e.Handled = true;
            Frame.Navigate(typeof(Exit), xbg, new SuppressNavigationTransitionInfo());

        }
        void IsConnect(bool bl)
        {
            xbg.ConnceNotice = true;
        }

        //private void Text_Animate(object sender, RoutedEventArgs e)
        //{
        //    DoubleAnimation opacityAnimation = new DoubleAnimation();
        //    opacityAnimation.From = 0;
        //    opacityAnimation.To = 1;
        //    opacityAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 1500));

        //    Storyboard.SetTarget(opacityAnimation, (TextBlock)sender);
        //    Storyboard.SetTargetProperty(opacityAnimation, "Opacity");

        //    Storyboard sb = new Storyboard();
        //    sb.Children.Add(opacityAnimation);
        //    sb.Begin();

        //}

        void onTimerMonitorPlug(object sender, object e)
        {
            _mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/TB_Logo_Glitch_2.mp4"));
            var mediaPlayer = _mediaPlayerElement.MediaPlayer;
            mediaPlayer.Play();
            timerMonitorPlug.Stop();
          
        }

        void onTimerMonitorPlug1(object sender, object e)
        {
            Frame.Navigate(typeof(StartPage), xbg, new SuppressNavigationTransitionInfo());
            timerMonitorPlug1.Stop();
            // Frame.Navigate(typeof(StartPage), null, new SuppressNavigationTransitionInfo());
            // Window.Current.Content = rootFrame;
        }

        void onTimerMonitorPlug2(object sender, object e)
        {
           
        }
    }
}
