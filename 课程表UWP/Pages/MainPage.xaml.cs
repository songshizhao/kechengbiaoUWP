using System;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using 课程表UWP.Pages;

namespace 课程表UWP
{

    public sealed partial class MainPage : Page
    {



        public MainPage()
        {
            this.InitializeComponent();

            //ApplicationView.PreferredLaunchViewSize = new Size(460, 720);
            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            //ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(450, 540));

            this.Loaded += MainPage_Loaded;

           
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //查找设值
            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
            object classtitle;
            if (root.Values.TryGetValue("title", out classtitle))
            {
                titleblock.Text = classtitle.ToString();

            }
        }



        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

            
            mainFrame.Navigate(typeof(Classes));

            //查找设值
            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
            object classtitle;
            if (root.Values.TryGetValue("title", out classtitle))
            {
                titleblock.Text = classtitle.ToString();
               
            }

            //设置右上角时间提示
            switch (DateTime.Now.DayOfWeek.ToString())
            {
                case "Monday":
                    dayofweek.Text = "星期一" + DateTime.Now.Month.ToString() + "月" + DateTime.Now.Day.ToString() + "日";
                    break;
                case "Tuesday":
                    dayofweek.Text = "星期二" + DateTime.Now.Month.ToString() + "月" + DateTime.Now.Day.ToString() + "日";
                    break;
                case "Wednesday":
                    dayofweek.Text = "星期三" + DateTime.Now.Month.ToString() + "月" + DateTime.Now.Day.ToString() + "日";
                    break;
                case "Thursday":
                    dayofweek.Text = "星期四" + DateTime.Now.Month.ToString() + "月" + DateTime.Now.Day.ToString() + "日";
                    break;
                case "Friday":
                    dayofweek.Text = "星期五" + DateTime.Now.Month.ToString() + "月" + DateTime.Now.Day.ToString() + "日";
                    break;
                case "Saturday":
                    dayofweek.Text = "星期六" + DateTime.Now.Month.ToString() + "月" + DateTime.Now.Day.ToString() + "日";
                    break;
                case "Sunday":
                    dayofweek.Text = "星期天" + DateTime.Now.Month.ToString() + "月" + DateTime.Now.Day.ToString() + "日";
                    break;

            }



        }


    
        //呼出汉堡菜单
        private void menu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            mainSplitView.IsPaneOpen = !mainSplitView.IsPaneOpen;
        }
        //汉堡菜单选项
        private void Item_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ListBoxItem item= (ListBoxItem)sender;
            switch (item.Tag.ToString())
            {
                case "我的课表":
                    mainFrame.Navigate(typeof(Classes));
                    break;
                case "课表同步":
                    mainFrame.Navigate(typeof(AsyncPage));
                    break;
                case "课表设置":
                    mainFrame.Navigate(typeof(SetPage));
                    break;
                case "关于作者":
                    mainFrame.Navigate(typeof(AboutPage));
                    break;
                case "更新日志":
                    mainFrame.Navigate(typeof(BlogPage));
                    break;
                default:
                    break;
            }
        }


        ////设置标题
        //public void setTitle(string title)
        //{
        //    titleblock.Text = title;
        //}

        private void AppBar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StackPanel sp = (StackPanel)sender;
            switch (sp.Tag.ToString())
            {
                case "课表":
                    mainFrame.Navigate(typeof(Classes));
                    break;
                case "关于":
                    mainFrame.Navigate(typeof(AboutPage));
                    break;
                case "设置":
                    mainFrame.Navigate(typeof(SetPage));
                    break;
                case "同步":
                    mainFrame.Navigate(typeof(AsyncPage));
                    break;
                case "好评":

                    break;

                default:
                    break;
            }
        }
    }

}








//private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
//{
//    MainHub.Height = Window.Current.Bounds.Height - 65;
//}

//protected override void OnNavigatedTo(NavigationEventArgs e)
//{

//    findday();
//}