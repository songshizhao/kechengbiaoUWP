using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using 课程表UWP.data;
using 课程表UWP.mywebservice;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace 课程表UWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AsyncPage : Page
    {
        int currentPage = 1;

        public AsyncPage()
        {
            this.InitializeComponent();
            this.Loaded += AsyncPage_Loaded;

            //listView.DataContext = data;
            FindAll(currentPage, currentPage+5);

        }

        private void AsyncPage_Loaded(object sender, RoutedEventArgs e)
        {
            //存储设置，记录上传过的课程表名称和ID
            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
            object titletxt;
            if (root.Values.TryGetValue("title", out titletxt))
            {
                classtitleinput.Text = titletxt.ToString();
            }
            object uploadhistory;
            if (root.Values.TryGetValue("historyupload", out uploadhistory))
            {
                historyupload.Text = uploadhistory.ToString();
            }

            //FileOpenPicker picker = new FileOpenPicker();
            //picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            //picker.ViewMode = PickerViewMode.List;  //设置文件的现实方式，这里选择的是图标
            //picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary; //设置打开时的默认路径，这里选择的是图片库
            //picker.FileTypeFilter.Add(".srt");                       //添加可选择的文件类型，这个必须要设置
            //StorageFile file = await picker.PickSingleFileAsync();    //只能选择一个文件
        }

        //上传课表
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            byte[] filebyte;
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync("class");
            if (file != null)
            {
                IBuffer buffer = null;
                using (IRandomAccessStream streamIn = await file.OpenReadAsync())
                {
                    buffer = WindowsRuntimeBuffer.Create((int)streamIn.Size);

                    await streamIn.ReadAsync(buffer, buffer.Capacity, InputStreamOptions.None);

                    filebyte = buffer.ToArray();



                    string classtitle = classtitleinput.Text;
                    if (classtitle == "")
                    {
                        var dialog = new MessageDialog("您输入的课表名称为空，请给你上传的课表起个名字吧~", "消息提示");
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        mywebservice.kechengbiaoSoapClient client = new mywebservice.kechengbiaoSoapClient();

                        int xc = client.uploadclassAsync(classtitle, filebyte).Result.uploadclassResult;

                        if (xc == 0)
                        {
                            var dialog = new MessageDialog("您输入的课表名称在服务器已经存在了，请换一个名字存储吧~", "消息提示");
                            await dialog.ShowAsync();
                        }
                        else
                        {
                            string message = "您输入的课表上传成功,课表名为" + classtitle + ",课表的数字ID为" + xc.ToString() + "。";

                            var dialog = new MessageDialog(message, "消息提示");
                            await dialog.ShowAsync();


                            //存储设置，记录上传过的课程表名称和ID


                            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;

                            object uploadhistory;
                            string txt;
                            if (root.Values.TryGetValue("historyupload", out uploadhistory))
                            {
                                txt = uploadhistory.ToString() + "课表名称：" + classtitle + ",课表ID：" + xc.ToString() + "。";
                                root.Values["historyupload"] = txt;
                            }
                            else
                            {
                                txt = "课表名称：" + classtitle + ",课表ID：" + xc.ToString() + "。";

                                root.Values["historyupload"] = txt;
                            }
                            historyupload.Text = txt;
                        }
                    }

                }
            }
        }

        //根据标题下载课表
        private async void btn_searchtitle_Click(object sender, RoutedEventArgs e)
        {
            string classtitle = searchtitleinput.Text;
            if (classtitle == "")
            {
                var dialog = new MessageDialog("标题似乎为空...", "消息提示");
                await dialog.ShowAsync();
                return;
            }
            else
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                try
                {
                    mywebservice.kechengbiaoSoapClient client = new mywebservice.kechengbiaoSoapClient();
                    int size = await client.GetDocumentLenByNameAsync(classtitle);

                    if (size == 0)
                    {
                        var dialog = new MessageDialog("[找不到输入课表文件，size=" + size + "]", "消息提示");
                        await dialog.ShowAsync();
                        return;
                    }

                    byte[] filebyte = new byte[size];// = new byte[size];
                    Debug.WriteLine(size);

                    mywebservice.DownloadClassByNameResponse x = await client.DownloadClassByNameAsync(classtitle);
                    filebyte = x.DownloadClassByNameResult;

                    if (filebyte != null)
                    {
                        StorageFile file = await folder.CreateFileAsync("class", CreationCollisionOption.ReplaceExisting);
                        IBuffer buffer = filebyte.AsBuffer();
                        using (IRandomAccessStream writestream = await file.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None))
                        {
                            await writestream.WriteAsync(buffer);
                            var dialog = new MessageDialog("success：D[课表已经成功载入了，快去看看吧~]", "消息提示");
                            await dialog.ShowAsync();
                            return;
                        }
                    }
                    else
                    {
                        var dialog = new MessageDialog("error：<[找不到输入title的课表，请确认课表title]", "消息提示");
                        await dialog.ShowAsync();
                    }

                }
                catch (Exception)
                {
                    var dialog = new MessageDialog("error：D[课表id不是数字]", "消息提示");
                    await dialog.ShowAsync();
                }

            }

        }

        private async void btn_searchid_Click(object sender, RoutedEventArgs e)
        {
            int classid;
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            try
            {

                try
                {
                    classid = Convert.ToInt32(searchidinput.Text);
                }
                catch (Exception)
                {

                    var dialog = new MessageDialog("输入的似乎不是数字...", "消息提示");
                    await dialog.ShowAsync();
                    return;
                }

                if (classid <= 0)
                {
                    var dialog = new MessageDialog("要输入正整数哇...", "消息提示");
                    await dialog.ShowAsync();
                    return;
                }


                kechengbiaoSoapClient client = new mywebservice.kechengbiaoSoapClient();
                int size = await client.GetDocumentLenByIdAsync(classid);

                if (size == 0)
                {
                    var dialog = new MessageDialog("[找不到输入id的课表，请确认课表id]", "消息提示");
                    await dialog.ShowAsync();
                    return;
                }

                byte[] filebyte = new byte[size];
                Debug.WriteLine(size);

                mywebservice.DownloadClassByIdResponse x = await client.DownloadClassByIdAsync(classid);
                filebyte = x.DownloadClassByIdResult;

                if (filebyte != null)
                {

                    StorageFile file = await folder.CreateFileAsync("class", CreationCollisionOption.ReplaceExisting);
                    IBuffer buffer = filebyte.AsBuffer();
                    using (IRandomAccessStream writestream = await file.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None))
                    {

                        await writestream.WriteAsync(buffer);
                        var dialog = new MessageDialog("[课表已经成功载入了，快去看看吧~]", "消息提示");
                        await dialog.ShowAsync();
                        return;

                    }



                }
                else
                {
                    var dialog = new MessageDialog("[找不到输入id的课表，请确认课表id]", "消息提示");
                    await dialog.ShowAsync();
                }

            }
            catch (Exception)
            {
                var dialog = new MessageDialog("[课表id不是数字]", "消息提示");
                await dialog.ShowAsync();
            }

        }

        ObservableCollection<ClassIndex> data = new ObservableCollection<ClassIndex>();
        private async void FindAll(int m,int n)
        {
            data.Clear();
            try
            {
                kechengbiaoSoapClient client = new kechengbiaoSoapClient();
                ArrayOfXElement tables = await client.SelectMNAsync(m,n);
               
                foreach (XElement el in tables.Nodes.Descendants("Table"))
                {
                    ClassIndex table = new ClassIndex();
                    foreach (XElement ell in el.Nodes())
                    {
                        switch (Convert.ToString(ell.Name))
                        {
                            case "ID":
                                table.IndexId = Convert.ToInt32(ell.Value);
                                break;

                            case "classtitle":
                                table.IndexTitle = ell.Value;
                                break;


                            default:

                                break;
                        }
                    }
                    data.Add(table);
                }
                listView.DataContext = data;
            }
            catch (Exception)
            {
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int classid;
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            try
            {
                try
                {
                    classid = Convert.ToInt32(btn.Tag.ToString());
                }
                catch (Exception)
                {

                    var dialog = new MessageDialog("输入的似乎不是数字...", "消息提示");
                    await dialog.ShowAsync();
                    return;
                }

                if (classid <= 0)
                {
                    var dialog = new MessageDialog("要输入正整数哇...", "消息提示");
                    await dialog.ShowAsync();
                    return;
                }


                kechengbiaoSoapClient client = new kechengbiaoSoapClient();
                int size = await client.GetDocumentLenByIdAsync(classid);

                if (size == 0)
                {
                    var dialog = new MessageDialog("[找不到输入id的课表，请确认课表id]", "消息提示");
                    await dialog.ShowAsync();
                    return;
                }

                byte[] filebyte = new byte[size];
                Debug.WriteLine(size);

                DownloadClassByIdResponse x = await client.DownloadClassByIdAsync(classid);
                filebyte = x.DownloadClassByIdResult;

                if (filebyte != null)
                {

                    StorageFile file = await folder.CreateFileAsync("class", CreationCollisionOption.ReplaceExisting);
                    IBuffer buffer = filebyte.AsBuffer();
                    using (IRandomAccessStream writestream = await file.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None))
                    {

                        await writestream.WriteAsync(buffer);
                        var dialog = new MessageDialog("[课表已经成功载入了，快去看看吧~]", "消息提示");
                        await dialog.ShowAsync();
                        return;

                    }

                }
                else
                {
                    var dialog = new MessageDialog("[找不到输入id的课表，请确认课表id]", "消息提示");
                    await dialog.ShowAsync();
                }
            }
            catch (Exception)
            {
                var dialog = new MessageDialog("[课表id不是数字]", "消息提示");
                await dialog.ShowAsync();
            }
        }

        private void NextPageButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            currentPage += 5;
            FindAll(currentPage, currentPage + 5);
        }

        private void PreviousPageButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            currentPage -= 5;
            FindAll(currentPage, currentPage + 5);
        }
    }

}
            






