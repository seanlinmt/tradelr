using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DotLiquid;
using DotLiquid.Util;
using tradelr.Library;
using tradelr.Models.liquid.models;

namespace tradelr.Models.liquid.extend
{
    public class PaginateBlock : Block
    {
        private static readonly Regex Syntax = R.B(R.Q(@"({0})\s*(by\s*(\d+))?"), Liquid.QuotedFragment);
        private string collection_name;
        private int page_size;
        private Dictionary<string, string> _attributes;

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            NodeList = NodeList ?? new List<object>();
            Match syntaxMatch = Syntax.Match(markup);
            if (syntaxMatch.Success)
            {
                collection_name = syntaxMatch.Groups[1].Value;
                if (!string.IsNullOrEmpty(syntaxMatch.Groups[2].Value))
                {
                    page_size = int.Parse(syntaxMatch.Groups[3].Value);
                }
                else
                {
                    page_size = 20;
                }
                _attributes = new Dictionary<string, string>(Template.NamingConvention.StringComparer);
                _attributes.Add("window_size", "3");
                R.Scan(markup, Liquid.TagAttributes, (key, value) => _attributes[key] = value);
            }
            else
            {
                throw new SyntaxErrorException("Syntax Error in tag 'paginate' - Valid syntax: paginate [collection] by number");
            }
            
            base.Initialize(tagName, markup, tokens);
        }

        public override void Render(Context context, TextWriter result)
        {
            var currentUrl = context.Registers["current_url"] as Uri;
            context.Stack(() =>
                              {
                                  var currentPage = 1;
                                  if (context.Registers.ContainsKey("current_page"))
                                  {
                                      currentPage = Convert.ToInt32(context.Registers["current_page"]);
                                  }

                                  var pagination = new Pagination
                                                       {
                                                           page_size = page_size,
                                                           current_page = currentPage,
                                                           current_offset = page_size*(currentPage - 1)
                                                       };

                                  context["paginate"] = pagination;
                                  context.SaveScoped("limit", pagination.page_size, collection_name);
                                  context.SaveScoped("offset", pagination.current_offset, collection_name);

                                  var collection = context[collection_name] as IEnumerable<object>;
                                  if (collection != null)
                                  {
                                      var collectionSize = collection.Count();
                                      int pageCount = (int)(Math.Ceiling(collectionSize / (float)page_size) + 1);

                                      pagination.items = collectionSize;
                                      pagination.pages = pageCount - 1;

                                      if (currentPage > 1)
                                      {
                                          pagination.previous = link(currentUrl, "&laquo; Previous", currentPage - 1);
                                      }
                                      if ((currentPage + 1) < pageCount)
                                      {
                                          pagination.next = link(currentUrl, "Next &raquo;", currentPage + 1);
                                      }
                                      pagination.parts = new List<TitleUrl>();

                                      var hellip_break = false;

                                      if (pageCount > 2)
                                      {
                                          for (int i = 1; i < pageCount; i++)
                                          {
                                              if (currentPage == i)
                                              {
                                                  pagination.parts.Add(nolink(i.ToString()));
                                              }
                                              else if (i == 1 || i == (pageCount - 1))
                                              {
                                                  pagination.parts.Add(link(currentUrl, i.ToString(), i));
                                              }
                                              else if (i <= currentPage - int.Parse(_attributes["window_size"]) ||
                                                       i >= currentPage + int.Parse(_attributes["window_size"]))
                                              {
                                                  if (hellip_break)
                                                  {
                                                      continue;
                                                  }
                                                  pagination.parts.Add(nolink("&hellip;"));
                                                  hellip_break = true;
                                                  continue;
                                              }
                                              else
                                              {
                                                  pagination.parts.Add(link(currentUrl, i.ToString(), i));
                                              }
                                              hellip_break = false;
                                          }
                                      }
                                  }
                                  RenderAll(NodeList, context, result);
                              });
        }

        private TitleUrl nolink(string title)
        {
            return new TitleUrl() {title = title, is_link = false};
        }

        private TitleUrl link(Uri current_url, string title, int page)
        {
            var queryParams = HttpUtility.ParseQueryString(current_url.Query);
            queryParams["page"] = page.ToString();

            return new TitleUrl()
                       {
                           title = title,
                           url = string.Concat(current_url.LocalPath, queryParams.ToQueryString(false)),
                           is_link = true
                       };
        }
    }

}