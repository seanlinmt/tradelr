using System.Text;
using System.Text.RegularExpressions;
using tradelr.Library;
using tradelr.Models.liquid.models;

namespace tradelr.Models.liquid.filters
{
    public static class TextFilter
    {
        public static string default_pagination(Pagination input)
        {
            var html = new StringBuilder();
            if (input.previous != null)
            {
                html.AppendFormat("<span class='prev'><a href='{0}'>{1}</a></span>", input.previous.url, input.previous.title);
            }

            foreach (var part in input.parts)
            {
                if (part.is_link)
                {
                    html.AppendFormat("<span class='page'><a href='{0}'>{1}</a></span>", part.url, part.title);
                }
                else if (part.title == input.current_page.ToString())
                {
                    html.AppendFormat("<span class='page current'>{0}</span>", part.title);
                }
                else
                {
                    html.AppendFormat("<span class='deco'>{0}</span>", part.title);
                }
            }

            if (input.next != null)
            {
                html.AppendFormat("<span class='next'><a href='{0}'>{1}</a></span>", input.next.url, input.next.title);
            }

            return html.ToString();
        }

        public static string handle(string input)
        {
            return handleize(input);
        }

        public static string handleize(string input)
        {
            return input.ToPerma();
        }

        public static string highlight(string input, string textToHighlight)
        {
            return Regex.Replace(input, @"(" + textToHighlight + ")", "<strong class=highlight>$1</strong>", RegexOptions.IgnoreCase);
        }

        public static string paragraphs(string input)
        {
            return input.ToHtmlParagraph();
        }

        public static string newline_to_br(string input)
        {
            return input.ToHtmlBreak();
        }

        public static string pluralize(long input, string single, string plural)
        {
            if (input == 1)
            {
                return single;
            }

            return plural;
        }
    }

}