using System.IO;
using System.Text;
using DotLiquid;
using tradelr.Areas.dashboard.Models.theme;
using tradelr.DBML;
using tradelr.Library.Constants;
using tradelr.Models.liquid.filters;

namespace tradelr.Models.liquid
{
    public class LiquidTemplate : LiquidTemplateBase
    {
        private StringBuilder head_content { get; set; }
        private Template layout_template { get; set; }
        
        private Hash registers { get; set; }

        public LiquidTemplate(MASTERsubdomain sd, bool isMobile) : base(sd, isMobile)
        {
            registers = new Hash();
            head_content = new StringBuilder();
        }

        public void AddHeaderContent(string content)
        {
            head_content.Append(content);
        }

        public void AddRegisters(string name, object obj)
        {
            registers.Add(name, obj);
        }

        private string ParseTemplate(string templatename)
        {
            return ReadTemplateFile(string.Format("{0}/{1}/{2}", GeneralConstants.APP_ROOT_DIR, themepath, templatename));
        }

        public void InitLayoutTemplate(string templatename)
        {
            // reads theme.liquid template
            var parsedstring = ParseTemplate(templatename);
            layout_template = CreateTemplate(parsedstring);
        }

        public new void InitContentTemplate(string templatename)
        {
            var parsedstring = ParseTemplate(templatename);
            content_template = CreateTemplate(parsedstring);
        }

        public string Render()
        {
            var p = new RenderParameters()
            {
                LocalVariables = parameters,
                Values = values,
                Filters = new[]
                                          {
                                              typeof (TextFilter), 
                                              typeof(InputFilter), 
                                              typeof(UrlFilter),
                                              typeof(MoneyFilter)
                                          },
                Registers = registers
            };

            AddParameters("content_for_header", head_content.ToString());

            // render content first
            AddParameters("content_for_layout", content_template.Render(p));

            return layout_template.Render(p);
        }
    }
}