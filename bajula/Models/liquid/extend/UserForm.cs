using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DotLiquid;
using DotLiquid.Exceptions;
using DotLiquid.Util;
using tradelr.Models.liquid.models.Blog;
using tradelr.Models.liquid.models.Form;

namespace tradelr.Models.liquid.extend
{
    public class UserForm : Block
    {
        private static readonly Regex Syntax = R.B(R.Q(@"({0}+)"), Liquid.VariableSignature);
        private string _variableName;

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            Match syntaxMatch = Syntax.Match(markup);
            if (syntaxMatch.Success)
            {
                _variableName = syntaxMatch.Groups[1].Value;
            }
            else
            {
                throw new SyntaxException("Syntax Error in 'form' - Valid syntax: form [article]");
            }

            base.Initialize(tagName, markup, tokens);
        }

        public override void Render(Context context, TextWriter result)
        {
            var article = context[_variableName] as Article;
            if (article != null)
            {
                RenderCommentForm(context, result, article);
            }
            else
            {
                // render normal form
                RenderNormalForm(context, result, _variableName);
            }
        }

        private void RenderCommentForm(Context context, TextWriter result, Article article)
        {
            result.WriteLine("<form id=\"article-{0}-comment-form\" class=\"comment-form\" method=\"post\" action=\"{1}\">", article.id, article.comment_post_url);
            context.Stack(() =>
            {
                context["form"] = Hash.FromAnonymousObject(new
                {
                    posted_successfully = context["comment.posted_successfully"],
                    errors = context["comment.errors"],
                    author = context["comment.author"],
                    email = context["comment.email"],
                    body = context["comment.body"]
                });
                RenderAll(NodeList, context, result);
            });
            result.WriteLine("</form>");
        }

        private void RenderNormalForm(Context context, TextWriter result, string formtype)
        {
            result.WriteLine("<form class=\"{0}-form\" method=\"post\" action=\"/contact\">", formtype);
            result.WriteLine("<input type=\"hidden\" value=\"{0}\" name=\"form_type\">", formtype);
            
            context.Stack(() =>
            {
                context["form"] = Hash.FromAnonymousObject(new
                {
                    errors = context["posted_contact_form.errors"],
                    posted_successfully = context["posted_contact_form.posted_successfully"]
                });

                context["form.errors.messages"] = context["posted_contact_form.errors.messages"];

                RenderAll(NodeList, context, result);
            });
                                  
            result.WriteLine("</form>");
        }

    }
}