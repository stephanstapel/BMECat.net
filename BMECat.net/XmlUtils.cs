/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 * 
 *   http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace BMECat.net
{
    public class XmlUtils
    {
        private static readonly string[] _GermanDateFormats = { "dd.MM.yyyy", "dd.MM.yyyy HH:mm", "dd.MM.yyyy HH:mm:ss" };


        /// <summary>
        ///  reads a certain attribute value
        /// </summary>
        public static string AttributeText(XmlNode node, string attributeName, string defaultText = "")
        {
            try
            {
                XmlAttribute attrib = node.Attributes[attributeName];
                if (attrib != null)
                {
                    return attrib.Value;
                }
            }
            catch
            {
            }

            return defaultText;
        } // !AttributeText()


        public static bool nodeAsBool(XmlNode node, string xpath, XmlNamespaceManager nsmgr = null, bool defaultValue = true)
        {
            if (node == null)
            {
                return defaultValue;
            }

            string value = nodeAsString(node, xpath, nsmgr);
            if (value == "")
            {
                return defaultValue;
            }
            else
            {
                if ((value.Trim().ToLowerInvariant() == "true") || (value.Trim() == "1"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        } // !nodeAsBool()


        public static string nodeAsString(XmlNode node, string xpath, XmlNamespaceManager nsmgr = null, string defaultValue = "")
        {
            if (node == null)
            {
                return defaultValue;
            }            

            try
            {
                XmlNode _node = node.SelectSingleNode(xpath, nsmgr);
                if (_node == null)
                {
                    return defaultValue;
                }
                else
                {
                    return _node.InnerText;
                }
            }
            catch (XPathException)
            {
                return defaultValue;
            }
        } // nodeAsString()


        public static List<string> nodesAsStrings(XmlNode node, string xpath, XmlNamespaceManager nsmgr = null, List<string> defaultValue = null)
        {
            if (node == null)
            {
                return defaultValue;
            }

            try
            {
                List<string> retval = new List<string>();
                foreach(XmlNode _node in node.SelectNodes(xpath, nsmgr))
                {
                    retval.Add(_node.InnerText);
                }
                return retval;
            }
            catch (XPathException)
            {
                return defaultValue;
            }
        } // nodesAsStrings()


        public static int? nodeAsInt(XmlNode node, string xpath, XmlNamespaceManager nsmgr = null, int? defaultValue = null)
        {
            if (node == null)
            {
                return defaultValue;
            }

            string temp = nodeAsString(node, xpath, nsmgr, "");
            int retval;
            if (Int32.TryParse(temp, NumberStyles.Integer, CultureInfo.InvariantCulture, out retval))
            {
                return retval;
            }
            else
            {
                return defaultValue;
            }
        } // !nodeAsInt()


        public static decimal? nodeAsDecimal(XmlNode node, string xpath, XmlNamespaceManager nsmgr = null, decimal? defaultValue = null)
        {
            if (node == null)
            {
                return defaultValue;
            }

            string temp = nodeAsString(node, xpath, nsmgr, "");
            decimal retval;

            // xml notation, i.e. dot as decimal separator and no group separators at all.
            // NumberStyles.Any must not be used here since it would interpret the decimal comma
            // of a non conformant, localized document as group separator, e.g. 0,19 as 019
            if (decimal.TryParse(temp, NumberStyles.Float, CultureInfo.InvariantCulture, out retval))
            {
                return retval;
            }

            // tolerate non conformant documents that were written with a decimal comma,
            // e.g. 0,19 instead of 0.19. Values like 1,234 are ambiguous (0.19 style decimal comma
            // or english group separator) and are hence rejected
            string[] parts = temp.Split(',');
            if ((parts.Length == 2) && (parts[1].Length != 3) && !temp.Contains("."))
            {
                if (decimal.TryParse(temp.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out retval))
                {
                    return retval;
                }
            }

            return defaultValue;
        } // !nodeAsDecimal()


        public static DateTime? nodeAsDateTime(XmlNode node, string xpath, XmlNamespaceManager nsmgr = null, DateTime? defaultValue = null)
        {
            if (node == null)
            {
                return defaultValue;
            }

            string temp = nodeAsString(node, xpath, nsmgr, "");
            DateTime retval;
            // invariant culture: gregorian calendar, independent from the machine the code runs on.
            // The current culture would e.g. read 2004-08-20 as buddhist year 2004 in th-TH
            CultureInfo provider = CultureInfo.InvariantCulture;

            // tolerate non conformant documents that were written with german date notation, e.g. 01.02.2004.
            // This has to be checked first since the lenient parsing below would read it month first, i.e. as january 2nd
            if (DateTime.TryParseExact(temp, _GermanDateFormats, provider, DateTimeStyles.None, out retval))
            {
                return retval;
            }

            if (DateTime.TryParseExact(temp, "yyyyMMdd", provider, DateTimeStyles.None, out retval))
            {
                return retval;
            }

            // xml notation, e.g. 2004-08-20, 2004-08-20T14:59:54 or 2004-08-20T14:59:54+02:00
            if (DateTime.TryParse(temp, provider, DateTimeStyles.None, out retval))
            {
                return retval;
            }

            return defaultValue;
        } // !nodeAsDateTime()



        public static XmlNodeList SelectNodes(XmlNode node, string xpath, XmlNamespaceManager nsmgr = null)
        {
            return node.SelectNodes(xpath, nsmgr);
        } // !SelectNodes()


        public static XmlNode SelectSingleNode(XmlNode node, string xpath, XmlNamespaceManager nsmgr = null)
        {
            return node.SelectSingleNode(xpath, nsmgr);
        } // !SelectNodes()
    }
}
