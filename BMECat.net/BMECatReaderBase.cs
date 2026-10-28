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
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace BMECat.net
{
    internal class BMECatReaderBase
    {        
        protected static QuantityCode _convertQuantityCode(string input, BMECatExtensions extensions)
        {
            if (input == null)
            {
                return new QuantityCode();
            }
            if ((extensions != null) && (extensions.QuantityCodeConverter != null))
            {
                return new QuantityCode() { ClearText = input, Code = extensions.QuantityCodeConverter.Convert(input) };
            }
            else
            {
                return new QuantityCode() { ClearText = input, Code = default(QuantityCodes).FromString(input) };
            }
        } // !_convertQuantityCode()


        /// <summary>
        /// Loads the document into memory.
        ///
        /// Documents without xmlns attribute get the given root element, so that all xpath expressions
        /// can use the bmecat namespace prefix. Large documents are buffered in a temporary file.
        /// </summary>
        protected static XmlDocument _LoadDocument(Stream inputStream, string rootElementReplacement, BMECatExtensions extensions)
        {
            // make sure that the root element contains xmlns elements to indicate which namespace to use
            string tempPath = "";
            Stream convertedStream = new MemoryStream();
            byte[] firstPartOfBuffer = new byte[1024];
            int bytesRead = StreamUtils.FillBuffer(inputStream, firstPartOfBuffer);
            // ISO-8859-1 maps every byte to exactly one character and back. This keeps all bytes besides the
            // rewritten root element untouched, no matter which encoding the document uses and whether
            // the buffer ends in the middle of a multi byte character
            System.Text.Encoding byteEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            string firstPartString = byteEncoding.GetString(firstPartOfBuffer, 0, bytesRead);

            if (!firstPartString.Contains("xmlns="))
            {
                string pattern = "<BMECAT.*?>";
                string outputString = Regex.Replace(firstPartString, pattern, rootElementReplacement);
                byte[] outputBuffer = byteEncoding.GetBytes(outputString);
                convertedStream.Write(outputBuffer, 0, outputBuffer.Length);
            }
            else
            {
                inputStream.Position = 0;
            }

            if (inputStream.Length > BMECatConstants.LargeFileSizeLimit)
            {
                tempPath = System.IO.Path.GetTempFileName();
                convertedStream.Position = 0;
                using (FileStream fs = System.IO.File.OpenWrite(tempPath))
                {
                    convertedStream.CopyTo(fs);
                    inputStream.CopyTo(fs);
                }

                convertedStream.Dispose();
                convertedStream = System.IO.File.OpenRead(tempPath);
            }
            else
            {
                inputStream.CopyTo(convertedStream);
                convertedStream.Position = 0;
            }

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(convertedStream);
            }
            finally
            {
                // the document has been read into memory, hence the temporary resources can be
                // released right away, no matter whether loading succeeded or failed
                convertedStream.Dispose();
                _deleteTemporaryFile(tempPath, extensions);
            }

            return doc;
        } // !_LoadDocument()


        /// <summary>
        /// Creates a namespace manager that maps the bmecat prefix to the namespace of the root element
        /// </summary>
        protected static XmlNamespaceManager _CreateNamespaceManager(XmlDocument doc)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);

            string xmlnsURL = XmlUtils.AttributeText(doc.DocumentElement, "xmlns");
            if (!String.IsNullOrEmpty(xmlnsURL))
            {
                nsmgr.AddNamespace("bmecat", xmlnsURL);
            }

            return nsmgr;
        } // !_CreateNamespaceManager()


        private static void _deleteTemporaryFile(string tempPath, BMECatExtensions extensions)
        {
            if (String.IsNullOrEmpty(tempPath))
            {
                return;
            }

            try
            {
                System.IO.File.Delete(tempPath);
            }
            catch (Exception ex)
            {
                if ((extensions != null) && (extensions.LogService != null))
                {
                    extensions.LogService.Error($"Could not delete temporary BMECat file {tempPath}. Error message: {ex.ToString()}");
                }
            }
        } // !_deleteTemporaryFile()


        protected static Party _ReadPartyAddress(XmlNode addressNode, XmlNamespaceManager nsmgr)
        {
            Party p = new Party()
            {
                Name2 = XmlUtils.nodeAsString(addressNode, "./bmecat:NAME2", nsmgr),
                Name3 = XmlUtils.nodeAsString(addressNode, "./bmecat:NAME3", nsmgr),
                Department = XmlUtils.nodeAsString(addressNode, "./bmecat:DEPARTMENT", nsmgr),
                ContactName = XmlUtils.nodeAsString(addressNode, "./bmecat:CONTACT", nsmgr),
                Street = XmlUtils.nodeAsString(addressNode, "./bmecat:STREET", nsmgr),
                Zip = XmlUtils.nodeAsString(addressNode, "./bmecat:ZIP", nsmgr),
                BoxNo = XmlUtils.nodeAsString(addressNode, "./bmecat:BOXNO", nsmgr),
                ZipBox = XmlUtils.nodeAsString(addressNode, "./bmecat:ZIPBOX", nsmgr),
                City = XmlUtils.nodeAsString(addressNode, "./bmecat:CITY", nsmgr),
                State = XmlUtils.nodeAsString(addressNode, "./bmecat:STATE", nsmgr),
                Country = XmlUtils.nodeAsString(addressNode, "./bmecat:COUNTRY", nsmgr),
                VATID = XmlUtils.nodeAsString(addressNode, "./bmecat:VAT_ID", nsmgr),
                Phone = XmlUtils.nodeAsString(addressNode, "./bmecat:PHONE", nsmgr), // @todo phone is typed
                Fax = XmlUtils.nodeAsString(addressNode, "./bmecat:FAX", nsmgr) // @todo fax is typed
            };

            foreach (XmlNode contactDetailNode in XmlUtils.SelectNodes(addressNode, "./bmecat:CONTACT_DETAILS", nsmgr))
            {
                p.ContactDetails.Add(new ContactDetails()
                {
                    Id = XmlUtils.nodeAsString(contactDetailNode, "./bmecat:CONTACT_ID", nsmgr),
                    Name = XmlUtils.nodeAsString(contactDetailNode, "./bmecat:CONTACT_NAME", nsmgr),
                    FirstName = XmlUtils.nodeAsString(contactDetailNode, "./bmecat:FIRST_NAME", nsmgr),
                    Title = XmlUtils.nodeAsString(contactDetailNode, "./bmecat:TITLE", nsmgr),
                    AcademicTitle = XmlUtils.nodeAsString(contactDetailNode, "./bmecat:ACADEMIC_TITLE", nsmgr),
                    ContactRole = XmlUtils.nodeAsString(contactDetailNode, "./bmecat:CONTACT_ROLE", nsmgr), // @todo role is typed
                    ContactDescription = XmlUtils.nodeAsString(contactDetailNode, "./bmecat:CONTACT_DESCR", nsmgr),
                    Phone = XmlUtils.nodesAsStrings(contactDetailNode, "./bmecat:PHONE", nsmgr), // @todo phone is typed
                    Fax = XmlUtils.nodesAsStrings(contactDetailNode, "./bmecat:FAX", nsmgr), // @todo fax is typed
                    URL = XmlUtils.nodeAsString(contactDetailNode, "./bmecat:URL", nsmgr),
                    Emails = XmlUtils.nodeAsString(contactDetailNode, "./bmecat:EMAILS", nsmgr) // @todo find out if this is a list in reality 
                });
            }

            return p;
        } // !_ReadPartyAddress()
    }
}