<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Theme
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="settings_tabs">
        <ul class="hidden">
            <li><a href="#themes">themes</a></li>
            <li><a href="#editor">theme editor</a></li>
            <li><a href="#settings">theme settings</a></li>
            <li><a href="#mobile_editor"><img src="/Content/img/icons/mobile.png" /> mobile theme editor</a></li>
            <li><a href="#mobile_settings"><img src="/Content/img/icons/mobile.png" /> mobile theme settings</a></li>
        </ul>
        <div id="themes" class="hidden">
            <% Html.RenderAction("List"); %>
        </div>
        <div id="editor" class="hidden">
            <% Html.RenderAction("editor", new{ ismobile = false }); %>
        </div>
        <div id="settings" class="hidden">
            <% Html.RenderAction("settings", new { ismobile = false }); %>
        </div>
        <div id="mobile_editor" class="hidden">
            <% Html.RenderAction("editor", new{ ismobile = true }); %>
        </div>
        <div id="mobile_settings" class="hidden">
            <% Html.RenderAction("settings", new { ismobile = true }); %>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
<link rel="stylesheet" media="screen" type="text/css" href="/Content/css/colorpicker/colorpicker.css" />
<style type="text/css">
.CodeMirror-line-numbers {
    background-color: #EEEEEE;
    color: #AAAAAA;
    font-family: monospace;
    font-size: 12px;
    padding-right: 0.3em;
    padding-top: 0.4em;
    text-align: right;
    width: 2.2em;
}

.CodeMirror-line-numbers div { line-height: 16px;}
.CodeMirror-wrapping { border: 1px solid #eee;}
</style>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="AdditionalJS" runat="server">
<script type="text/javascript" src="/Scripts/codemirror/codemirror.js"></script>
<script type="text/javascript">
var LiquidSettingsClass = function(context, settings) {
    var theme_settings = settings;
    
    // set theme settings value
    var setThemeValues = function(preset) {
        var settings = { };
        if (theme_settings.presets != null &&
            theme_settings.presets != undefined) {
            if (preset != undefined && preset != '') {
                settings = theme_settings.presets[preset];
            }
            else {
                settings = theme_settings.presets[theme_settings.current];
            }
        }
        else {
            settings = theme_settings.current;
        }

        for (var k in settings) {
            var el = $("#theme-settings *[name='" + k + "']", context);
            if (el.length != 0) {

                if (el.attr('type') == 'checkbox') {
                    el.val(settings[k]);
                    el.attr('checked', settings[k]);
                    $("<input/>").attr({
                            name: k,
                            type: 'hidden',
                            value: 'false'
                        }).insertAfter(el);
                }
                else if (el.hasClass('color')) {
                    el.val(settings[k]);
                    el.trigger('change');
                }
                else if (el.attr('type') == 'radio') {
                    el.filter("[value=" + settings[k] + "]").attr("checked", "checked");
                }
                else {
                    el.val(settings[k]);
                }
            }
        }
    };

    $('#theme-settings #preset', context).bind('change', function() {
        var selected = $(this).val();
        setThemeValues(selected);
        $('#theme-settings .color', context).trigger('change');
    });
    
    $('#ThemeSettingsForm', context).submit(function() {
        $('input[type=checkbox]', this).each(function() {
            if ($(this).is(':checked')) {
                $(this).val('true');
            }
            else {
                $(this).val('false');
            }
        });
    
        // check that there is a specified
        if ($('#save_preset', context).is(":checked") && 
            $('#save_preset_existing', context).val() == "new" &&
            $('#save_preset_custom', context).val() == '') 
        {
            $.jGrowl('Please specify a preset name');
            $('#save_preset_custom', context).focus();
            return false;
        }
        return true;
    });
    
    // insert section header
    $('#theme-settings fieldset > legend', context).each(function() {
        var header = $.trim($(this).text());
        if (header == '') {
            return true;
        }
        
        var parent = $(this).parents('fieldset');
        var section = $('<div/>').addClass('section_header collapsed').html(header);
        $(parent).before(section).hide();
        $(this).remove();
        return true;
    });
    
    // color picker
    $('#theme-settings .color', context).each(function() {
        var selector = $("<div/>").attr('class', 'color-selector').css('background-color', $(this).val());
        $(this).after(selector);
    });

    $('#theme-settings .color', context).bind('change', function() {
        var val = $(this).val();
        $(this).next().css('background-color', val);
    });

    $('#theme-settings #save_preset', context).bind('change', function() {
        $('#theme-settings #save_preset_div', context).toggle();
    });

    // collapsible headings
    $('#theme-settings .section_header', context).bind('click', function() {
        $(this).next().toggle();
        $(this).toggleClass('collapsed');
    });

    $('#theme-settings .color', context).ColorPicker({
            onSubmit: function(hsb, hex, rgb, el) {
                $(el).val('#' + hex);
                $(el).ColorPickerHide();
                $(el).trigger('change');
            },
            onBeforeShow: function() {
                $(this).ColorPickerSetColor(this.value);
            }
        });
    
    setThemeValues();
    inputSelectors_init('#theme-settings', context);
    init_autogrow('#theme-settings', context);
    $('#theme-settings .section_header:first', context).trigger('click');
};
</script>
<script type="text/javascript">
    var LiquidEditorClass = function (context, isMobile) {
        var changes = new Hashtable();
        var codemirror;
        var path;
        var init = function (p, code) {
            $('.theme_editor_content', context).html(code);
            $.scrollTo('.theme_editor_content', context);
            path = p;
            codemirror = CodeMirror.fromTextArea($("#file_content", context)[0], {
                height: "1000px",
                parserfile: getParser(p),
                stylesheet: ["/Content/css/codemirror/xmlcolors.css?v2",
                             "/Content/css/codemirror/liquidcolors.css?v2",
                             "/Content/css/codemirror/csscolors.css?v4",
                             "/Content/css/codemirror/jscolors.css?v3"],
                path: "/Scripts/codemirror/",
                textWrapping: false,
                lineNumbers: true,
                onChange: function () {
                    $('#file_changed', context).change();
                    changes.put(path, escape(codemirror.getCode()));
                }
            });
        };

        var getParser = function (filename) {
            var extIndex = filename.lastIndexOf('.');
            var ext = "";
            if (extIndex != -1) {
                ext = filename.substring(extIndex + 1).toLowerCase();
            }

            switch (ext) {
                case "css":
                    return "parsers/parsecss.js";
                case "js":
                    return "parsers/parsejavascript.js";
                case "html":
                    return "parsers/parsehtmlmixed.js";
                case "xml":
                    return "parsers/parsexml.js";
                case "liquid":
                default:
                    return "parsers/parseliquid.js";
            }
        };

        ///////////////// START first time run ////////////////////////////
        $('.theme_file_delete', context).live('click', function () {
            var ok = window.confirm("Are you sure? This will delete the current loaded file.");
            if (!ok) {
                return false;
            }
            var url = $(this).attr("rel");
            $.post("/dashboard/themes/deletefile", { path: url, ismobile: isMobile }, function (json_result) {
                if (json_result.success) {
                    $('.theme_editor_content', context).html('');
                    $(".theme_editor_files a[rel='" + url + "']", context).parent().slideUp(function () {
                        $(this).remove();
                    });
                }
                $.jGrowl(json_result.message);
            });
            return false;
        });

        $(".file > a", context).live("click", function () {
            var supportedExtensions = ["js", "css", "liquid", "json", "html", "htm"];
            var url = $(this).attr("rel");
            var ext = url.toLowerCase().split(".").pop();
            if (supportedExtensions.indexOf(ext) == -1 && !tradelr.util.is_image(url)) {
                $.jGrowl('This file type is unsupported by the theme editor.');
                $.scrollTo('.theme_editor_content');
                return false;
            }

            $('.theme_editor_content', context).showLoadingBlock();
            $.post('<%= Url.Action("filecontent", "themes") %>', { path: url, ismobile: isMobile }, function (result) {
                if (tradelr.util.is_image(url)) {
                    $('.theme_editor_content', context).html(result);
                }
                else {
                    init(url, result);
                }
            });

            return false;
        });

        $('.buttonEditorSave', context).live('click', function () {
            var keys = changes.keys();
            var values = changes.values();
            $(this).buttonDisable();
            $.ajax({
                type: "POST",
                url: '<%= Url.Action("save","themes") %>',
                dataType: "json",
                contentType: "application/json",
                data: $.toJSON({ names: keys, contents: values, ismobile: isMobile }),
                success: function (json_result) {
                    if (json_result.success) {
                        changes.clear();
                    }
                    $.jGrowl(json_result.message);
                }
            });
        });

        $('.new_template_button', context).click(function () {
            var name = $('.new_template_name', context).val();
            var type = $('.new_template_select', context).val();
            if (name == '') {
                $.jGrowl('Please enter a name');
                return false;
            }

            var ok = window.confirm("Are you sure? This will overwrite any existing file with the same name.");
            if (!ok) {
                return false;
            }

            $.post('<%= Url.Action("addtemplate","themes") %>', { name: name, type: type, ismobile: isMobile }, function (result) {
                if (result == "") {
                    $.jGrowl('Failed to add new template');
                    return false;
                }
                var filename = [type, ".", name, ".liquid"].join("");
                // append to list
                $(".theme_templates", context).prepend("<li class='file ext_liquid'><a href='#' rel='/templates/" + filename + "'>" + filename + "</a></li>");

                // display content
                var url = "/templates/" + filename;
                init(url, result);
                $('.new_template', context).hide();
                $('.new_template_reveal', context).show();
                return false;
            });
            return false;
        });

        $('.new_snippet_button', context).click(function () {
            var name = $('.new_snippet_name', context).val();
            if (name == '') {
                $.jGrowl('Please enter a name');
                return false;
            }

            var ok = window.confirm("Are you sure? This will overwrite any existing file with the same name.");
            if (!ok) {
                return false;
            }

            $.post('<%= Url.Action("addsnippet","themes") %>', { name: name, ismobile: isMobile }, function (result) {
                if (result == "") {
                    $.jGrowl('Failed to add new snippet');
                    return false;
                }
                var filename = [name, ".liquid"].join("");
                // append to list
                $(".theme_snippets", context).prepend("<li class='file ext_liquid'><a href='#' rel='/snippets/" + filename + "'>" + filename + "</a></li>");

                // display content
                var url = "/snippets/" + filename;
                init(url, result);
                $('.new_snippet', context).hide();
                $('.new_snippet_reveal', context).show();
                return false;
            });
            return false;
        });

        var uploaderTheme = new qq.FileUploader({
            element: $('.buttonUploadTheme', context)[0],
            action: '<%= Url.Action("import","themes") %>',
            params: { ismobile: isMobile },
            allowedExtensions: ['zip'],
            onSubmit: function (id, filename) {
            },
            onComplete: function (id, filename, json_data) {
                if (json_data.success) {
                    window.location.reload();
                }
                $.jGrowl(json_data.message);
            }
        });

        var uploaderAsset = new qq.FileUploader({
            element: $('.buttonUploadAsset', context)[0],
            action: '<%= Url.Action("addasset","themes") %>',
            params: { ismobile: isMobile },
            onSubmit: function (id, filename) {
            },
            onComplete: function (id, filename, json_data) {
                if (json_data.success) {
                    // append to list
                    var ext = filename.split(".").pop();
                    $(".theme_assets").prepend("<li class='file ext_" + ext + "'><a href='#' rel='/assets/" + filename + "'>" + filename + "</a></li>");

                    // display content
                    $('.new_asset', context).hide();
                    $('.new_asset_reveal', context).show();
                }
                $.jGrowl(json_data.message);
            }
        });

        $('.qq-upload-button', '.buttonUploadAsset').addClass('qq-upload-button-small').css("width","110px");

        $('.new_template_name', context).alphanumeric();
        ///////////////// END first time run ////////////////////////////
    };  /// end liquideditorclass
    
    $(document).ready(function () {
        $('#settings_tabs').tabs();
        $('#navstore').addClass('navselected');
    });
</script>
</asp:Content>
