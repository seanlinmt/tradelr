using System.Text;
using System.Web.Script.Serialization;

namespace tradelr.Models.liquid.filters
{
    public static class InputFilter
    {
        private static string facebook_like_button(models.Shop store)
        {
            return
                string.Format(
                    "<iframe src=\"http://www.facebook.com/plugins/like.php?href={0}&amp;layout=standard&amp;show_faces=false&amp;width=450&amp;action=like&amp;colorscheme=light&amp;height=35\" scrolling=\"no\" frameborder=\"0\" style=\"border:none; overflow:hidden; width:450px; height:35px;\" allowTransparency=\"true\"></iframe>", store.url);
        }

        public static string json(object input)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(input);
        }
    }
}