using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Libraries.Imaging
{
    public static class UtilImage
    {
        /// <summary>
        /// wraps <img></img> tag around imageurl
        /// </summary>
        /// <param name="imgpath"></param>
        /// <returns></returns>
        public static string ToHtmlImage(this string imgpath)
        {
            return string.Concat("<img src='", imgpath, "' alt='' />");
        }

        /// <summary>
        /// wraps <img></img> tag around imageurl
        /// </summary>
        /// <param name="imgpath"></param>
        /// <param name="imageclass"></param>
        /// <returns></returns>
        public static string ToHtmlImage(this string imgpath, string imageclass)
        {
            return string.Concat("<img class='", imageclass, "' src='", imgpath, "' alt='' />");
        }
    }
}
