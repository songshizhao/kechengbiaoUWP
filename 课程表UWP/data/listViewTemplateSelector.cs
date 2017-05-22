using System.IO;
using System.Xml;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using System.Diagnostics;

namespace 课程表UWP.data
{
    public sealed class ListViewTemplateSelector : DataTemplateSelector
    {
        public DataTemplate _DataTemplate01;        
        public DataTemplate DataTemplate01
        {
            get
            {
                return _DataTemplate01;
            }

            set
            {
                if (value != _DataTemplate01)
                {
                    _DataTemplate01 = value;
                }
            }
        }
        public DataTemplate _DataTemplate02;
        public DataTemplate DataTemplate02
        {
            get
            {
                return _DataTemplate02;
            }

            set
            {
                if (value != _DataTemplate02)
                {
                    _DataTemplate02 = value;
                }
            }
        }


        public DataTemplate _DataTemplate03;
        public DataTemplate DataTemplate03
        {
            get
            {
                return _DataTemplate03;
            }

            set
            {
                if (value != _DataTemplate03)
                {
                    _DataTemplate03 = value;
                }
            }
        }


        public DataTemplate _DataTemplate04;
        public DataTemplate DataTemplate04
        {
            get
            {
                return _DataTemplate04;
            }

            set
            {
                if (value != _DataTemplate04)
                {
                    _DataTemplate04 = value;
                }
            }
        }




        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {


            ListView list = ItemsControl.ItemsControlFromItemContainer(container) as ListView;

            //int index = list.IndexFromContainer(container);

            //Debug.WriteLine(item.GetType().ToString());
           // ----------------------------------------------------------------------------
            Course D = (Course)item;

            if (D.class_finished_property == "1")//结课或者无课
            {
                return DataTemplate02;
            }

            //else if (D.class_weeklimit_property == 1)//单周
            //{
            //    return DataTemplate03;
            //}

            //else if (D.class_weeklimit_property == 2)//双周
            //{
            //    return DataTemplate04;
            //}
            else
            {
                return DataTemplate01;
            }

            //return DataTemplate01;
        }



        















        }
    }

