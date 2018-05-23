using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Svg;

namespace tradelr.Libraries.ActionResults
{
    public class SvgToPngActionResult : ActionResult
    {
        readonly Bitmap image;
        public SvgToPngActionResult(Bitmap image)
        {
            this.image = image;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "image/png";
            using (var ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                ms.WriteTo(context.HttpContext.Response.OutputStream);
            }
        }
    }
}