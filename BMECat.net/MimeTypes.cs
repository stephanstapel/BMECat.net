﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace BMECat.net
{
    public enum MimeTypes
    {
        Unknown = 0,
        ImageJpeg,
        ImageTiff,
        ImageGif,
        ApplicationPdf,
        TextHtml,
        Url,
        VideoUrl
    }


    internal static class MimeTypesExtensions
    {
        private static Dictionary<string, MimeTypes> _mimeTypes = new Dictionary<string, MimeTypes>()
        {
            { "image/jpeg", MimeTypes.ImageJpeg },
            { "image/gif", MimeTypes.ImageGif },
            { "image/tif", MimeTypes.ImageTiff },
            { "image/tiff", MimeTypes.ImageTiff },
            { "application/pdf", MimeTypes.ApplicationPdf },
            { "text/html", MimeTypes.TextHtml },
            { "video/url", MimeTypes.VideoUrl },
            { "url", MimeTypes.Url },
        };

        public static MimeTypes FromString(this MimeTypes _, string s)
        {
            s = s.ToLower().Trim();
            if (_mimeTypes.ContainsKey(s))
            {
                return _mimeTypes[s];
            }
            else
            {
                Console.WriteLine($"Unknown mime type {s}");

                return MimeTypes.Unknown;
            }
        } // !FromString()


        public static string EnumToString(this MimeTypes c)
        {
            foreach (KeyValuePair<string, MimeTypes> kv in _mimeTypes)
            {
                if (kv.Value == c)
                {
                    return kv.Key;
                }
            }

            return null;
        } // !ToString()
    }
}