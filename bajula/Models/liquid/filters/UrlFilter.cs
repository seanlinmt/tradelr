using System;
using System.Web;
using tradelr.Common.Library.Imaging;
using tradelr.Library.Constants;
using tradelr.Models.liquid.models;

namespace tradelr.Models.liquid.filters
{
    public static class UrlFilter
    {
        public static string asset_url(string input, string asset_url)
        {
            return String.Format("{0}/assets/{1}", asset_url, input);
        }

        public static string img_tag(string input, string alt = "")
        {
            return String.Format("<img src='{0}' alt='{1}' />", input, alt ?? "");
        }

        public static string link_to(string input, string url)
        {
            return string.Format("<a title='{0}' href='{1}'>{0}</a>", input, url);
        }

        public static string link_to_tag(TitleUrl input, TitleUrl tag, string tag_current_handle)
        {
            return string.Format("<a href='/collections/{0}/{1}'>{2}</a>", tag_current_handle, tag.url, tag.title);
        }

        public static string link_to_remove_tag(TitleUrl input, TitleUrl tag, string tag_current_handle)
        {
            return string.Format("<a href='/collections/{0}/{1}'>{2}</a>", tag_current_handle, tag.url, tag.title);
        }

        public static string link_to_add_tag(TitleUrl input, TitleUrl tag, string tag_current_handle)
        {
            return string.Format("<a href='/collections/{0}/{1}'>{2}</a>", tag_current_handle, tag.url, tag.title);
        }

        public static string link_to_theme(string input, string theme)
        {
            return string.Format("<a href='/?theme={0}'>{1}</a>", theme, input);
        }

        public static string product_img_url(string input, string size)
        {
            if (String.IsNullOrEmpty(input))
            {
                return GeneralConstants.PHOTO_NO_THUMBNAIL_MEDIUM;
            }

            Imgsize sz;
            if (!Enum.TryParse(size, true, out sz))
            {
                return input;
            }

            return Img.by_size(input, sz);
        }

        public static string script_tag(string input, string theme_version)
        {
            return String.Format("<script type='text/javascript' src='{0}?v={1}'></script>", input, theme_version);
        }

        public static string stylesheet_tag(string input, string theme_version)
        {
            return String.Format("<link media='all' type='text/css' rel='stylesheet' href='{0}?v={1}'>", input, theme_version);
        }

        public static string tradelr_asset_url(string input)
        {
            return String.Concat("/Scripts/store/", input);
        }

        public static string within(string input, dynamic collection)
        {
            // ignore if input already has collections
            if (input.IndexOf("collections", StringComparison.OrdinalIgnoreCase) != -1)
            {
                return input;
            }
            if (collection == null)
            {
                return input;
            }
            var type = collection.GetType();
            if (type == typeof(Collection))
            {
                if (collection.handle == "all")
                {
                    return input;
                }
                
                return string.Format("/collections/{0}/{1}", collection.handle, input);
            }

            return input;
        }
    }
}