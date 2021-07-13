using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using XboxGC500.Model;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace XboxGC500.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UpgradePage : Page
    {
        private XboxOneGip xbg;
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

        public UpgradePage()
        {
            this.InitializeComponent();
            xbg = new XboxOneGip();
            xbg.InitxboxGip();

            if(UpdateStatic==0)
            {
                UPJM.Visibility = Visibility.Visible;
                UPGO.Visibility = Visibility.Collapsed;
                UPGood.Visibility = Visibility.Collapsed;
                UpBack.Focus(FocusState.Programmatic);
            }
            else if(UpdateStatic==1)
            {
                UPJM.Visibility = Visibility.Collapsed;
                UPGO.Visibility = Visibility.Collapsed;
                UPGood.Visibility = Visibility.Visible;
                UpBack.Focus(FocusState.Programmatic);
            }
            else if(UpdateStatic==2)
            {
                UPJM.Visibility = Visibility.Collapsed;
                UPGO.Visibility = Visibility.Collapsed;
                UPFailed.Visibility = Visibility.Visible;
                UpBK.Focus(FocusState.Programmatic);
            }

        }
        int progressValue;
        private void UpYes_Click(object sender, RoutedEventArgs e)
        {
            xbg.StateUpgrade();
            UPJM.Visibility = Visibility.Collapsed;
            UPGO.Visibility = Visibility.Visible;
            progressValue = xbg.GetProgressValue();
            progressBar1.Value = progressValue;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Tick += timer_Tick;
            timer.Start();
            xbg.IsShowConnect = true;
        }
        public static int UpdateStatic { get; set; } = 0;
        void timer_Tick(object sender, object e)
        {
            //  progressBar1.Value = xbg.GetProgressValue();
            // Debug.Write("\nxbg.GetControllerState():"+xbg.GetControllerState().ToString());return;
            progressValue = xbg.GetProgressValue();
            progressBar1.Value = progressValue;
            if (xbg != null)
            {
                UpBack.Focus(FocusState.Programmatic);
            }
            if (xbg.GetProgressValue() >= 1000 && xbg.GetFinishedState() && !xbg.UpgradePlug)
            {
                Thread.Sleep(4000);
                {
                    UpdateStatic = 1;
                    xbg.IsShowConnect = false;
                    UPGO.Visibility = Visibility.Collapsed;
                    UPGood.Visibility = Visibility.Visible;
                    UpBack.Focus(FocusState.Programmatic);
                    
                }
                (sender as DispatcherTimer).Tick -= timer_Tick;
                (sender as DispatcherTimer).Stop();
            }
            Debug.WriteLine("\nprogressValue:"+progressValue.ToString()+ xbg.GetErrorState().ToString());
            if (xbg.GetErrorState())
            {
                (sender as DispatcherTimer).Tick -= timer_Tick;
                (sender as DispatcherTimer).Stop();
                UpdateStatic = 2;
                Thread.Sleep(1000);
                UPGO.Visibility = Visibility.Collapsed;
                UPFailed.Visibility = Visibility.Visible;

                UpBK.Focus(FocusState.Programmatic);
            }


            // Debug.WriteLine("xbg.GetFinishedState():"+xbg.GetFinishedState().ToString()); 
        }

        private void UpYes_GotFocus(object sender, RoutedEventArgs e)
        {
          //  UpYes.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 29, 149, 255));
         //   UpNo.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
           // UY.Visibility = Visibility.Visible;
        }

        private void UpYes_LostFocus(object sender, RoutedEventArgs e)
        {
           // UY.Visibility = Visibility.Collapsed;
        }

        private void UpNo_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(View.FrontPage), xbg, new SuppressNavigationTransitionInfo());
            xbg.IsShowConnect = false;
        }

        private void UpNo_GotFocus(object sender, RoutedEventArgs e)
        {
           // UpNo.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 29, 149, 255));
           // UpYes.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
           // UN.Visibility = Visibility.Visible;
        }

        private void UpNo_LostFocus(object sender, RoutedEventArgs e)
        {
          //  UN.Visibility = Visibility.Collapsed;
        }

        private void UpBack_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatic = 0;
            xbg.IsShowConnect = false;
            Frame.Navigate(typeof(View.FrontPage), xbg, new SuppressNavigationTransitionInfo());
        }

        private void UpBack_GotFocus(object sender, RoutedEventArgs e)
        {
           // UpBack.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 29, 149, 255));
        }

        private void UpTry_Click(object sender, RoutedEventArgs e)
        {
            UPFailed.Visibility = Visibility.Collapsed;
            UPJM.Visibility = Visibility.Visible;
        }

        private void UpTry_GotFocus(object sender, RoutedEventArgs e)
        {
          //  UpTry.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 29, 149, 255));
           // UpBK.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
        }

        private void UpBK_GotFocus(object sender, RoutedEventArgs e)
        {
           // UpBK.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 29, 149, 255));
           // UpTry.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
        }

        private void UpBK_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatic = 0;
            xbg.IsShowConnect = false;
            Frame.Navigate(typeof(View.FrontPage), xbg, new SuppressNavigationTransitionInfo());
        }


        private void Update_Loaded(object sender, RoutedEventArgs e)
        {
          //  UpYes.Focus(FocusState.Programmatic);

        }

        private void UPGood_LostFocus(object sender, RoutedEventArgs e)
        {
          //  UpBack.Focus(FocusState.Programmatic);
        }

        private void UPFailed_LostFocus(object sender, RoutedEventArgs e)
        {
           // UpTry.Focus(FocusState.Programmatic);
        }

        private void Update_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape)
            {
                e.Handled = true;
                ///Update.Visibility = Visibility.Visible;
            }
        }

        private void UpYes_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
          //  UpYes.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 29, 149, 255));
           // UpNo.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
           // UY.Visibility = Visibility.Visible;
        }

        private void UpYes_PointerExited(object sender, PointerRoutedEventArgs e)
        {
           // UY.Visibility = Visibility.Collapsed;
        }

        private void UpNo_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
         //   UpNo.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 29, 149, 255));
           // UpYes.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
           // UN.Visibility = Visibility.Visible;
        }

        private void UpNo_PointerExited(object sender, PointerRoutedEventArgs e)
        {
           // UN.Visibility = Visibility.Collapsed;
        }

        private void UpBack_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
          //  UpBack.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 29, 149, 255));
        }

        private void UpBack_PointerExited(object sender, PointerRoutedEventArgs e)
        {
          //  UpBack.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 29, 149, 255));
        }

        private void UpTry_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
           // UpTry.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 29, 149, 255));
          //  UpBK.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
        }

        private void UpTry_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            
        }

        private void UpTry_LostFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}
