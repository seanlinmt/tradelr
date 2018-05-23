using System.CodeDom;
using System.IO;
using System.Text;
using Microsoft.CSharp;

namespace tradelr.Libraries.Helpers
{
    public static class StringHelper
    {
        public static string WrapText(this string textToWrap, int maxChars, string separator)
        {
            if (textToWrap.Length < maxChars)
            {
                return textToWrap;
            }
            string[] words = textToWrap.Split(new char[] { ' ' });
            var sentence = new StringBuilder("");
            var phrase = new StringBuilder("");
            foreach (string word in words)
            {
                if (phrase.Length == 0 || phrase.Length + word.Length + 1 > maxChars)
                {
                    if (sentence.Length > 0)
                    {
                        sentence.Append(separator);
                    }
                    sentence.Append(phrase);
                    phrase = new StringBuilder(word);
                }
                else
                {
                    phrase.Append(" ");
                    phrase.Append(word);
                }
            }
            if (sentence.Length > 0)
            {
                sentence.Append(separator);
            }
            sentence.Append(phrase);
            return sentence.ToString();
        }

        public static string ToLiteral(string input)
        {
            var writer = new StringWriter();
            var provider = new CSharpCodeProvider();
            provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
            return writer.GetStringBuilder().ToString();
        }


    }
}
