using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using 课程表UWP.mywebservice;

namespace 课程表UWP.data
{
    class WebServiceHelper
    {



        /// <summary>     
        /// XElement转换为XmlElement     
        /// </summary>     
        public static XmlElement ToXmlElement(XElement xElement)
        {
            if (xElement == null) return null;

            XmlElement xmlElement = null;
            XmlReader xmlReader = null;
            try
            {
                xmlReader = xElement.CreateReader();
                var doc = new XmlDocument();
                xmlElement = doc.ReadNode(xElement.CreateReader()) as XmlElement;
            }
            catch
            {
            }
            finally
            {
                if (xmlReader != null) xmlReader.Dispose();
            }

            return xmlElement;
        }

        /// <summary>     
        /// XmlElement转换为XElement     
        /// </summary>     
        public static XElement ToXElement(XmlElement xmlElement)
        {
            if (xmlElement == null) return null;

            XElement xElement = null;
            try
            {
                var doc = new XmlDocument();
                doc.AppendChild(doc.ImportNode(xmlElement, true));
                xElement = XElement.Parse(doc.InnerXml);
            }
            catch { }

            return xElement;
        }





    }



}
