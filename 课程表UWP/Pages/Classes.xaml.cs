using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using 课程表UWP.data;

namespace 课程表UWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Classes : Page
    {
        ObservableCollection<Course> Monday = new ObservableCollection<Course>();
        ObservableCollection<Course> Tuesday = new ObservableCollection<Course>();
        ObservableCollection<Course> Wednesday = new ObservableCollection<Course>();
        ObservableCollection<Course> Thursday = new ObservableCollection<Course>();
        ObservableCollection<Course> Friday = new ObservableCollection<Course>();
        ObservableCollection<Course> Saturday = new ObservableCollection<Course>();
        ObservableCollection<Course> Sunday = new ObservableCollection<Course>();

        public Classes()
        {
            this.InitializeComponent();

            initialize();//初始化
        }



  


        //数据读取初始化
        async void initialize()
        {



            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile XmlFileInApp, XmlFileInLocal;
            //如果本地文件夹内不存在xml记录，复制安装包内xml过去
            if (await folder.TryGetItemAsync("class") == null)
            {

                //获取安装包内xml
                XmlFileInApp = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///data\class.xml"));
                Stream XmlReader = await XmlFileInApp.OpenStreamForReadAsync();
                //读取xml到内存
                XmlDocument xml = new XmlDocument();
                xml.Load(XmlReader);

                //默认开始有8结课
                foreach (var element in xml.DocumentElement)
                {
                    XmlElement each_element = (XmlElement)element;
                    XmlNodeList nodelist = each_element.ChildNodes;

                    for (int i = nodelist.Count - 1; i >= 8; i--)
                    {
                        XmlNode n = nodelist[i];
                        each_element.RemoveChild(n);

                    }
                }
                //在本地新建xml文件
                XmlFileInLocal = await folder.CreateFileAsync("class", CreationCollisionOption.ReplaceExisting);
                //保存xml文件到local
                Stream XmlWriter = await XmlFileInLocal.OpenStreamForWriteAsync();
                xml.Save(XmlWriter);
                XmlWriter.Dispose();
            }
            //读xml文件
            XmlFileInLocal = await folder.GetFileAsync("class");
            Stream XmlReader1 = await XmlFileInLocal.OpenStreamForReadAsync();
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.Load(XmlReader1);

            foreach (var element in XmlDoc.DocumentElement)
            {
                XmlElement each_element = (XmlElement)element;
                //Debug.WriteLine(each_element.Name);

                foreach (var InnerElement in each_element)
                {
                    XmlElement each_InnerElement = (XmlElement)InnerElement;
                    //Debug.WriteLine(each_InnerElement.Name);//Debug.WriteLine(each_InnerElement.GetAttribute("duration"));
                    Course acourse = new Course();
                    acourse.class_name_property = each_InnerElement.GetAttribute("name");
                    acourse.class_index_property[0] = each_element.Name;
                    acourse.class_index_property[1] = each_InnerElement.GetAttribute("index");
                    acourse.class_duration_property = each_InnerElement.GetAttribute("duration");
                    acourse.class_startTime = each_InnerElement.GetAttribute("startTime");
                    acourse.class_endTime = each_InnerElement.GetAttribute("endTime");
                    acourse.class_room_property = each_InnerElement.GetAttribute("room");
                    acourse.class_score_property = each_InnerElement.GetAttribute("score");
                    acourse.class_teacher_property = each_InnerElement.GetAttribute("teacher");
                    acourse.class_type_property = Convert.ToInt32(each_InnerElement.GetAttribute("type"));

                    switch (acourse.class_type_property)
                    {
                        case 0:
                            acourse.class_tag_property = "必修";
                            break;
                        case 1:
                            acourse.class_tag_property = "选修";
                            break;
                        case 2:
                            acourse.class_tag_property = "旁听";
                            break;
                        case 3:
                            acourse.class_tag_property = "专业课";
                            break;
                        case 4:
                            acourse.class_tag_property = "非学分绩";
                            break;
                        case 5:
                            acourse.class_tag_property = "学分绩";
                            break;
                    }




                    switch (each_InnerElement.GetAttribute("weeklimit"))
                    {
                        case "1":

                            acourse.class_weeklimit_property = "单";
                            break;
                        case "0":

                            acourse.class_weeklimit_property = "";
                            break;

                        case "2":

                            acourse.class_weeklimit_property = "双";
                            break;



                    }



                    acourse.class_finished_property = each_InnerElement.GetAttribute("finished");
                    //Debug.WriteLine(acourse.class_index_property);
                    switch (each_element.Name)
                    {
                        case "Monday":
                            Monday.Add(acourse);
                            break;
                        case "Tuesday":
                            Tuesday.Add(acourse);
                            break;
                        case "Wednesday":
                            Wednesday.Add(acourse);
                            break;
                        case "Thursday":
                            Thursday.Add(acourse);
                            break;
                        case "Friday":
                            Friday.Add(acourse);
                            break;
                        case "Saturday":
                            Saturday.Add(acourse);
                            break;
                        case "Sunday":
                            Sunday.Add(acourse);
                            break;
                    }


                }

                day1.DataContext = Monday;
                day2.DataContext = Tuesday;
                day3.DataContext = Wednesday;
                day4.DataContext = Thursday;
                day5.DataContext = Friday;
                day6.DataContext = Saturday;
                day7.DataContext = Sunday;
            }
        }

        private void edit_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine("sd");
            var edit_button = (Grid)sender;
            Frame.Navigate(typeof(EditPage), edit_button.Tag);

        }

        private async void delete_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string[] index = (string[])btn.Tag;

            var day = index[0];
            var classindex = index[1];

            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile XmlFileInLocal;

            //读xml文件数据
            XmlFileInLocal = await folder.GetFileAsync("class");
            Stream XmlReader = await XmlFileInLocal.OpenStreamForReadAsync();
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.Load(XmlReader);
            XmlReader.Dispose();

            foreach (var element in XmlDoc.DocumentElement)
            {
                XmlElement each_element = (XmlElement)element;
                if (each_element.Name == day)
                {
                    foreach (var InnerElement in each_element)
                    {
                        XmlElement each_InnerElement = (XmlElement)InnerElement;
                        if (each_InnerElement.GetAttribute("index") == classindex)
                        {
                            each_InnerElement.SetAttribute("finished", "1");
                        }

                    }
                }
            }

            ////保存xml到local
            StorageFile xml = await folder.CreateFileAsync("class", CreationCollisionOption.ReplaceExisting);
            Stream fileWriter = await xml.OpenStreamForWriteAsync();
            XmlDoc.Save(fileWriter);
            fileWriter.Dispose();


            Monday.Clear(); Tuesday.Clear(); Thursday.Clear(); Wednesday.Clear(); Friday.Clear(); Saturday.Clear(); Sunday.Clear();
            initialize();



        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            findday();
        }


        public void findday()
        {
            switch (DateTime.Now.DayOfWeek.ToString())
            {
                case "Monday":
                    MainHub.ScrollToSection(day1);
                    break;
                case "Tuesday":
                    MainHub.ScrollToSection(day2);
                    break;
                case "Wednesday":
                    MainHub.ScrollToSection(day3);
                    break;
                case "Thursday":
                    MainHub.ScrollToSection(day4);
                    break;
                case "Friday":
                    MainHub.ScrollToSection(day5);
                    break;
                case "Saturday":
                    MainHub.ScrollToSection(day6);
                    break;
                case "Sunday":
                    MainHub.ScrollToSection(day7);
                    break;
            }
        }




    }
}
