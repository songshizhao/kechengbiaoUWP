using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace 课程表UWP.Pages
{

    public sealed partial class SetPage : Page
    {


        public SetPage()
        {
            this.InitializeComponent();


            //查找设值
            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
            object classtitle;
            if (root.Values.TryGetValue("title", out classtitle))
            {
              
                titleinput.Text = classtitle.ToString();
            }


        }


   

        private void btn_yes_Tapped(object sender, TappedRoutedEventArgs e)
        {

            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
            root.Values["title"] = titleinput.Text;

            cd1.Hide();
        }

        private void btn_cancel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            cd1.Hide();
        }


        private async void editTitle_Click(object sender, RoutedEventArgs e)
        {
            await cd1.ShowAsync();
        }








        //清除所有课程信息
        private async void Delete_all_Toggled(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile XmlFileInLocal;

            //读xml文件数据
            XmlFileInLocal = await folder.GetFileAsync("class");
            Stream XmlReader = await XmlFileInLocal.OpenStreamForReadAsync();
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.Load(XmlReader);
            XmlReader.Dispose();
            if (Delete_all.IsOn == true)
            {
                foreach (var element in XmlDoc.DocumentElement)
                {
                    XmlElement each_element = (XmlElement)element;
                    foreach (var InnerElement in each_element)
                    {
                        XmlElement each_InnerElement = (XmlElement)InnerElement;
                        each_InnerElement.SetAttribute("finished", "1");
                    }
                }
            }
            else
            {
                foreach (var element in XmlDoc.DocumentElement)
                {
                    XmlElement each_element = (XmlElement)element;
                    foreach (var InnerElement in each_element)
                    {
                        XmlElement each_InnerElement = (XmlElement)InnerElement;
                        each_InnerElement.SetAttribute("finished", "0");
                    }
                }
            }
            ////保存xml到local
            StorageFile xml = await folder.CreateFileAsync("class", CreationCollisionOption.ReplaceExisting);
            Stream fileWriter = await xml.OpenStreamForWriteAsync();
            XmlDoc.Save(fileWriter);
            fileWriter.Dispose();


            //显示消息框
            await new MessageDialog("清除完成").ShowAsync();
            Delete_all.IsOn = true;
            Frame.Navigate(typeof(MainPage));

        }


        //设置每天的课节数
        private async void Button_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            int number;
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            //获取安装包内xml
            StorageFile XmlFileInApp = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///data\class.xml"));
            Stream XmlReader = await XmlFileInApp.OpenStreamForReadAsync();
            //读取xml到内存
            XmlDocument xml = new XmlDocument();
            xml.Load(XmlReader);


            foreach (var element in xml.DocumentElement)
            {
                XmlElement each_element = (XmlElement)element;
                XmlNodeList nodelist = each_element.ChildNodes;
                number = cmbbox.SelectedIndex + 1;
                for (int i = nodelist.Count - 1; i >= number; i--)
                {
                    XmlNode n = nodelist[i];
                    each_element.RemoveChild(n);

                }
            }
 
                //在本地新建xml文件
                StorageFile XmlFileInLocal = await folder.CreateFileAsync("class", CreationCollisionOption.ReplaceExisting);
                //保存xml文件到local
                Stream XmlWriter = await XmlFileInLocal.OpenStreamForWriteAsync();
                xml.Save(XmlWriter);
                XmlWriter.Dispose();

                Frame.Navigate(typeof(MainPage));
          }

 
    }

}

