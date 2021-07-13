using D4XGaming.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TB_Firmware_Flow3.Manager;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace TB_Firmware_Flow3
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Fail : Page
    {
        XboxOneGip xbg;

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

        public Fail()
        {
            this.InitializeComponent();
        }

        private void BTN_Upgrade_Click(object sender, RoutedEventArgs e)
        {
            D4XDevice[] device = xbg.GetD4XDevices();
            if (MainPage.typeControoler[0]&&device[3]!=null)
                Frame.Navigate(typeof(Velocity), xbg, new SuppressNavigationTransitionInfo());
            else if (MainPage.typeControoler[1] && device[3] != null)
                Frame.Navigate(typeof(Recon_White), xbg, new SuppressNavigationTransitionInfo()); 
            else if (MainPage.typeControoler[2] && device[3] != null)
                Frame.Navigate(typeof(Recon), xbg, new SuppressNavigationTransitionInfo());
            else
            {
                xbg.ConnceNotice = true;
                Frame.Navigate(typeof(MainPage), xbg, new SuppressNavigationTransitionInfo());
            }
            xbg.SetError(false);
        }

        private void Grid_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape)
            {
                e.Handled = true;
            }
        }

        private void BTN_Upgrade_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Light.Visibility = Visibility.Visible;
        }

        private void BTN_Upgrade_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Light.Visibility = Visibility.Collapsed;
        }

        private void BTN_Upgrade_GotFocus(object sender, RoutedEventArgs e)
        {
            Light.Visibility = Visibility.Visible;
        }

        private void BTN_Upgrade_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {

        }

        private void BTN_Upgrade_LostFocus(object sender, RoutedEventArgs e)
        {
            Light.Visibility = Visibility.Collapsed;
        }
    }
}
