using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using TB_Firmware_Flow3.Manager;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TB_Firmware_Flow3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartPage : Page
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

       // public delegate void MyDelegate1();
       // public static event EventHandler<StartPage> IsConnected;
        //EventHandler<XboxOneGip>
        public StartPage()
        {
            
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            timerMonitorPlug.Interval = new TimeSpan(0, 0, 0, 0, 100);　　//6秒后进入圆圈旋转的界面
            timerMonitorPlug.Tick += onTimerMonitorPlug;
            timerMonitorPlug.Start();
            //xbg = new XboxOneGip();
            //xbg.InitxboxGip();
             timerMonitorPlug.Start();

            setVerion_1.Text = LaunchScreen.sowfwareVersion;
            setVerion_2.Text = LaunchScreen.sowfwareVersion;

            DispatcherTimer MonitorVersion = new DispatcherTimer();
            MonitorVersion.Interval = new TimeSpan(0, 0, 0, 0, 500);
            MonitorVersion.Tick += OnMonitorVersion;
            MonitorVersion.Start();

        }
        //监控线上版本与本地版本的对比
        void OnMonitorVersion(object sender, object e)
        {
            bool[] connectType = xbg.GetconnectType();
            if (connectType[(int)ControllerType.ReconBlack])
            {
                loadFinisedIndex = 2;
                if (xbg.http.IsLoadVersionFinished((int)ControllerType.ReconBlack))
                    UpdateExecute((int)ControllerType.ReconBlack);
            }
            if (connectType[(int)ControllerType.ReconWhite])
            {
                loadFinisedIndex = 1;
                if (xbg.http.IsLoadVersionFinished((int)ControllerType.ReconWhite))
                    UpdateExecute((int)ControllerType.ReconWhite);
            }
        }
        bool[] loadFile = new bool[16];//下载标志
        bool[] loadFileFinshed = new bool[16];//下载完成标准
        int  loadFinisedIndex = 0;
        void UpdateExecute(int controllertype)
        {
            uint[] connectType = xbg.GetVersion();
            if (xbg.IsUpdateMode() || connectType[controllertype] < xbg.http.GetClearControllerVersion(controllertype))
            {
                if (xbg.http.IsClearFileLoadFinished(controllertype)&& !loadFileFinshed[controllertype])
                {
                    xbg.SetUpdateData(controllertype, xbg.http.loadByteArray[controllertype]);
                    loadFileFinshed[controllertype] = true;
                    xbg.ConnceNotice = true;
                }
                else if (!loadFile[controllertype])//每个类型执行一次
                {
                    xbg.http.UpgradeClearController(controllertype);
                    loadFile[controllertype] = true;
                }
            }
            
        }

        enum ControllerType { VelocityOneFlight = 0, ReconWhite, ReconBlack, BootLoaderMode }//枚举所有类型

        int timeCount = 0;
        void onTimerMonitorPlug(object sender, object e)
        {
            if (xbg.GetUpgradeState()||xbg.GetFinishedState()) return;

            if (!xbg.Connected())
            {
                if (timeCount > 0)
                {
                    //loadFinised = false;
                    Front.Visibility = Visibility.Visible;
                    Back.Visibility = Visibility.Collapsed;
                    Frame.Navigate(typeof(StartPage), xbg, new SuppressNavigationTransitionInfo());
                }
                timeCount = 0;
                return; 
            }
            
            if (timeCount == 0)
            {
                Front.Visibility = Visibility.Collapsed;
                Back.Visibility = Visibility.Visible;
                Text_Animate(Checking, null);
            }
           // Debug.WriteLine("jjjjjjjjjjjj");

            if (loadFileFinshed[loadFinisedIndex] && timeCount != 0x1000)   //3秒后导航到主界面，要几秒可以在Text_Animate内的定时器修改.
            {
                timeCount = 0x1000;//每次跳转执行一次
                Frame.Navigate(typeof(MainPage), xbg, new SuppressNavigationTransitionInfo());
              
            }
            else if(timeCount>20)
                timeCount = 0x1000;


        }

       


        private void Image_AnimateImageopened(object sender, RoutedEventArgs e)
        {
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = 0;
            opacityAnimation.To = 1;
            opacityAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 1000));

            Storyboard.SetTarget(opacityAnimation, (Image)sender);
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");

            Storyboard sb = new Storyboard();
            sb.Children.Add(opacityAnimation);
            sb.Begin();
        }

        private void Text_Animate(object sender, RoutedEventArgs e)
        {
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = 1;
            opacityAnimation.To = 0;
            opacityAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 3000));

            Storyboard.SetTarget(opacityAnimation, (TextBlock)sender);
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");

            Storyboard sb = new Storyboard();
            sb.Children.Add(opacityAnimation);
            sb.Begin();
          
        }

        private void Image_AnimateImageopened1(object sender, RoutedEventArgs e)
        {
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = 0;
            opacityAnimation.To = 1;
            opacityAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));

            Storyboard.SetTarget(opacityAnimation, (Image)sender);
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");

            Storyboard sb = new Storyboard();
            sb.Children.Add(opacityAnimation);
            sb.Begin();
        }
    }
}
