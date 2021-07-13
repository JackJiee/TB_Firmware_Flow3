using D4XGaming.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TB_Firmware_Flow3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        int Disable = 0;  //为1的时候禁用

        XboxOneGip xbg;

        public static bool[] typeControoler { get; set; } = new bool[16];
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
        DispatcherTimer timerMonitorController = new DispatcherTimer();

       // public static event EventHandler<MainPage> IsConnected;
        public MainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            timerMonitorController.Interval = new TimeSpan(0, 0, 0, 0, 100);  //
            timerMonitorController.Tick += ontimerMonitorController;//监测

            //XboxOneGip.IsConnected += IsConnect;

            setVerion_3.Text = LaunchScreen.sowfwareVersion;
            timerMonitorController.Start();
            //Disable = 1;
            if (Disable == 1)
            {
                Vel_IMG.Source = new BitmapImage(new Uri("ms-appx:///Assets/VEL_Disable.png"));
                Button1.IsEnabled = false;
                Recon_IMG.Source = new BitmapImage(new Uri("ms-appx:///Assets/Recon_Disable.png"));
                Button2.IsEnabled = false;
                Recon_White_IMG.Source = new BitmapImage(new Uri("ms-appx:///Assets/Recon_White_Disable.png"));
                Button3.IsEnabled = false;
            }
            //Button1.Visibility = Visibility.Collapsed;
            //Button3.Visibility = Visibility.Collapsed;
            //Button2.Visibility = Visibility.Collapsed;
            //Left_Arrow.Visibility = Visibility.Collapsed;
            //Right_Arrow.Visibility = Visibility.Collapsed;




            Window.Current.CoreWindow.Activated += (sender, args) =>
            {
                if (args.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
                {
                    System.Diagnostics.Debug.WriteLine("************失去焦点********************* ");
                    xbg.SetError(true);
                }
            };

            for (int i = 0; i < 16; i++)
                typeControoler[i] = false;


            //   Button1.IsEnabled = true;
        }
        void  IsConnect(bool bl)
        {
            Debug.WriteLine("IsConnect....");
           // timerMonitorController.Start();
        }
        void ontimerMonitorController(object sender, object e)
        {
            if (!xbg.ConnceNotice) return;
            xbg.ConnceNotice = false;
            D4XDevice[] device = xbg.GetD4XDevices();//判断设备句柄，分析设备是否存在
            int[] data = new int[16];
           // timerMonitorController.Stop();
            for (int i = 0; i < 16; i++)
            {
               
                if (device[i] != null)
                {
                   
                    switch (i)
                    {
                        case 0:
                            if(device[0].FirmwareVersionInfo.Build >= 103 )
                            {
                                Vel_IMG.Source = new BitmapImage(new Uri("ms-appx:///Assets/VEL_Disable.png"));
                                Button1.IsEnabled = false;
                            }
                            else
                            {
                                Vel_IMG.Source = new BitmapImage(new Uri("ms-appx:///Assets/Velocityone.png"));
                                Button1.IsEnabled = true;
                            }
                            data[i] = 1;
                            break;
                        case 1:
                            if(!xbg.http.IsClearFileLoadFinished(1) || device[1].FirmwareVersionInfo.Build >= xbg.http.GetClearControllerVersion(1))
                            {
                                Recon_White_IMG.Source = new BitmapImage(new Uri("ms-appx:///Assets/Recon_White_Disable.png"));
                                Button3.IsEnabled = false;
                            }
                            else
                            {
                                Recon_White_IMG.Source = new BitmapImage(new Uri("ms-appx:///Assets/Recon_White.png"));
                                Button3.IsEnabled = true;
                            }
                            data[i] = 1;
                            break;
                        case 2:
                            if (!xbg.http.IsClearFileLoadFinished(2) || device[2].FirmwareVersionInfo.Build >= xbg.http.GetClearControllerVersion(2))
                            {
                                Recon_IMG.Source = new BitmapImage(new Uri("ms-appx:///Assets/Recon_Disable.png"));
                                Button2.IsEnabled = false;
                            }
                            else
                            {
                                Recon_IMG.Source = new BitmapImage(new Uri("ms-appx:///Assets/Recon.png"));
                                Button2.IsEnabled = true;
                            }
                            data[i] = 1;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    data[i] = 0;
                }
                //判断前一次升级是否失败，哪个手柄
                if (typeControoler[i]&&device[3]!=null)
                    data[i] = 1;
            }
            State(data);
           // timerMonitorController.Stop();
        }

        private void Grid_Loading(FrameworkElement sender, object args)
        {
            Button1.Visibility = Visibility.Collapsed;
            Button3.Visibility = Visibility.Collapsed;
            Button2.Visibility = Visibility.Collapsed;
            Left_Arrow.Visibility = Visibility.Collapsed;
            Right_Arrow.Visibility = Visibility.Collapsed;
            xbg.ConnceNotice = true;
            timerMonitorController.Start();
        }

        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Vel_IMG.Source = Vel_IMG_Back.Source;
        }

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Vel_IMG.Source = Vel_IMG_Back_01.Source;
        }

        private void Button_PointerEntered_1(object sender, PointerRoutedEventArgs e)
        {
            Recon_IMG.Source = Recon_IMG_Back.Source;
        }

        private void Button_PointerExited_1(object sender, PointerRoutedEventArgs e)
        {
            Recon_IMG.Source = Recon_IMG_Back_01.Source;
        }

        private void Button_PointerEntered_2(object sender, PointerRoutedEventArgs e)
        {
            Recon_White_IMG.Source = Recon_White_IMG_Back.Source;
        }

        private void Button_PointerExited_2(object sender, PointerRoutedEventArgs e)
        {
            Recon_White_IMG.Source = Recon_White_IMG_Back_01.Source;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 16; i++)
            {
                typeControoler[i] = false;
            }
            typeControoler[0] = true;
            timerMonitorController.Stop();
            Frame.Navigate(typeof(Velocity), xbg, new SuppressNavigationTransitionInfo());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 16; i++)
            {
                typeControoler[i] = false;
            }
            typeControoler[2] = true;
            timerMonitorController.Stop();
            Frame.Navigate(typeof(Recon),xbg, new SuppressNavigationTransitionInfo());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 16; i++)
            {
                typeControoler[i] = false;
            }
            typeControoler[1] = true;
            timerMonitorController.Stop();
            Frame.Navigate(typeof(Recon_White), xbg, new SuppressNavigationTransitionInfo());
        }

        int BTN_State = 1;  //1:状态1,2:状态2;
        int BTN_Right = 0;
        int BTN_Left = 1;

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            BTN_Left++;
            if (BTN_Left == 1)
            {
                Translation3.Begin();
                Translation4.Begin();
                Translation5.Begin();
                Vel_IMG.Opacity = 1;
                Recon_White_IMG.Opacity = 0.5;
                BTN_Right = 0;
            }
            if (BTN_Left > 1)
            {
                return;
            }
            Button2.XYFocusLeft = Button1;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            BTN_Right++;
            BTN_State = 2;
            if (BTN_State == 2 && BTN_Right == 1)
            {
                Translation.Begin();
                Translation1.Begin();
                Translation2.Begin();
                Recon_White_IMG.Opacity = 1;
                Vel_IMG.Opacity = 0.5;
                BTN_Left = 0;
            }

            if (BTN_State == 2 && BTN_Right > 1)
            {
                return;
            }
            Button2.XYFocusLeft = Left_Arrow;
        }

        private void Button_PointerEntered_3(object sender, PointerRoutedEventArgs e)
        {
            Left_Arrow_IMG.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/ArrowLeft_Back.png"));
        }

        private void Button_PointerExited_3(object sender, PointerRoutedEventArgs e)
        {
            Left_Arrow_IMG.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/ArrowLeft.png"));
        }

        private void Button_PointerEntered_4(object sender, PointerRoutedEventArgs e)
        {
            Right_Arrow_IMG.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/ArrowRight_Back.png"));
        }

        private void Button_PointerExited_4(object sender, PointerRoutedEventArgs e)
        {
            Right_Arrow_IMG.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/ArrowRight.png"));
        }

        private void Button1_GotFocus(object sender, RoutedEventArgs e)
        {
            Vel_IMG.Source = Vel_IMG_Back.Source;
            BTN_Rec1.Visibility = Visibility.Visible;
        }

        private void Button1_LostFocus(object sender, RoutedEventArgs e)
        {
            Vel_IMG.Source = Vel_IMG_Back_01.Source;
            BTN_Rec1.Visibility = Visibility.Collapsed;
        }

        private void Button2_GotFocus(object sender, RoutedEventArgs e)
        {
            Recon_IMG.Source = Recon_IMG_Back.Source;
            BTN_Rec2.Visibility = Visibility.Visible;
        }

        private void Button2_LostFocus(object sender, RoutedEventArgs e)
        {
            Recon_IMG.Source = Recon_IMG_Back_01.Source;
            BTN_Rec2.Visibility = Visibility.Collapsed;
        }

        private void Button3_GotFocus(object sender, RoutedEventArgs e)
        {
            Recon_White_IMG.Source = Recon_White_IMG_Back.Source;
            BTN_Rec3.Visibility = Visibility.Visible;
        }

        private void Button3_LostFocus(object sender, RoutedEventArgs e)
        {
            Recon_White_IMG.Source = Recon_White_IMG_Back_01.Source;
            BTN_Rec3.Visibility = Visibility.Collapsed;
        }

        private void Left_Arrow_GotFocus(object sender, RoutedEventArgs e)
        {
            Left_Arrow_IMG.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/ArrowLeft_Back.png"));
        }

        private void Left_Arrow_LostFocus(object sender, RoutedEventArgs e)
        {
            Left_Arrow_IMG.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/ArrowLeft.png"));
        }

        private void Right_Arrow_GotFocus(object sender, RoutedEventArgs e)
        {
            Right_Arrow_IMG.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/ArrowRight_Back.png"));
        }

        private void Right_Arrow_LostFocus(object sender, RoutedEventArgs e)
        {
            Right_Arrow_IMG.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/ArrowRight.png"));
        }

        
        private void State(int[]data)
        {
            int device1 = data[0];
            int device2 = data[2];
            int device3 = data[1];
          
            if (device1 == 1 && device2== 0 && device3 ==0)
            {
                Canvas.SetLeft(Button1, 850);
                Button2.Visibility = Visibility.Collapsed;
                Button3.Visibility = Visibility.Collapsed;
                Left_Arrow.Visibility = Visibility.Collapsed;
                Right_Arrow.Visibility = Visibility.Collapsed;
                Button1.Visibility = Visibility.Visible;
            }
            if (device1 == 0 && device2 == 1 && device3 == 0)
            {
                Canvas.SetLeft(Button2, 850);
                Button1.Visibility = Visibility.Collapsed;
                Button3.Visibility = Visibility.Collapsed;
                Left_Arrow.Visibility = Visibility.Collapsed;
                Right_Arrow.Visibility = Visibility.Collapsed;
                Button2.Visibility = Visibility.Visible;
            }
            if (device1 == 0 && device2 == 0 && device3 == 1)
            {
                Canvas.SetLeft(Button3, 850);
                Button1.Visibility = Visibility.Collapsed;
                Button2.Visibility = Visibility.Collapsed;
                Left_Arrow.Visibility = Visibility.Collapsed;
                Right_Arrow.Visibility = Visibility.Collapsed;
                Button3.Visibility = Visibility.Visible;
            }
            if (device1 == 1 && device2 == 1 && device3 == 0)
            {
                Canvas.SetLeft(Button1, 400);
                Canvas.SetLeft(Button2, 1375);
                Button3.Visibility = Visibility.Collapsed;
                Left_Arrow.Visibility = Visibility.Collapsed;
                Right_Arrow.Visibility = Visibility.Collapsed;
                Button1.Visibility = Visibility.Visible;
                Button2.Visibility = Visibility.Visible;
            }
            if (device1 == 1 && device2 == 0 && device3 == 1)
            {
                Canvas.SetLeft(Button1, 400);
                Canvas.SetLeft(Button3, 1375);
                Button2.Visibility = Visibility.Collapsed;
                Left_Arrow.Visibility = Visibility.Collapsed;
                Right_Arrow.Visibility = Visibility.Collapsed;
                Button1.Visibility = Visibility.Visible;
                Button3.Visibility = Visibility.Visible;
            }
            if (device1 == 0 && device2 == 1 && device3 == 1)
            {
                Canvas.SetLeft(Button2, 400);
                Canvas.SetLeft(Button3, 1375);
                Button1.Visibility = Visibility.Collapsed;
                Left_Arrow.Visibility = Visibility.Collapsed;
                Right_Arrow.Visibility = Visibility.Collapsed;
                Button2.Visibility = Visibility.Visible;
                Button3.Visibility = Visibility.Visible;
            }
            if (device1 == 1 && device2 == 1 && device3 == 1)
            {
                Canvas.SetLeft(Button1, 400);
                Canvas.SetLeft(Button2, 1375);
                Canvas.SetLeft(Button3, 2350);
                Left_Arrow.Visibility = Visibility.Collapsed;
                Right_Arrow.Visibility = Visibility.Collapsed;
                Button1.Visibility = Visibility.Visible;
                Button2.Visibility = Visibility.Visible;
                Button3.Visibility = Visibility.Visible;
            }
            if (!(device1 == 1 && device2 == 1 && device3 == 1))
            {
                Recon_White_IMG.Opacity = 1;
            }
        }

        private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key==Windows.System.VirtualKey.Escape)
            {
                Frame.Navigate(typeof(Exit), xbg, new SuppressNavigationTransitionInfo());
            }
        }

        private void Grid_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            //if (e.Key == Windows.System.VirtualKey.Escape)
            //{
            //    Frame.Navigate(typeof(Exit), xbg, new SuppressNavigationTransitionInfo());
            //}
        }

        private void Grid_KeyDown_1(object sender, KeyRoutedEventArgs e)
        {
            //if (e.Key == Windows.System.VirtualKey.Escape)
            //{
            //    Frame.Navigate(typeof(Exit), xbg, new SuppressNavigationTransitionInfo());
            //}
        }

        private void Main1_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            //if (e.Key == Windows.System.VirtualKey.Escape)
            //{
            //    Frame.Navigate(typeof(Exit), xbg, new SuppressNavigationTransitionInfo());
            //}
        }
    }
}
