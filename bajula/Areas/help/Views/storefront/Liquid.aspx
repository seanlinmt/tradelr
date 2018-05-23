<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/NotLoggedIn.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="banner">
        <div class="content">
            <h1>
                Help</h1>
        </div>
    </div>
    <div class="banner_main">
        <div class="content pt20">
            <div id="help_nav" class="fl">
                <% Html.RenderPartial("~/Areas/help/Views/help_navigation.ascx"); %>
            </div>
            <div id="help_content_top" class="ml200">
            </div>
            <div id="help_content" class="ml200">
                <h1>
                    Using Liquid</h1>
                <p>
                    There are two types of liquid markup: Output and Tag.</p>
                <div class="code_block">
                    <p>
                        Output is surrounded by</p>
                    <p>
                        {{ two curly brackets }}
                    </p>
                    <p>
                        Tags are surrounded by</p>
                    <p>
                        {% a curly bracket and a percent %}
                    </p>
                </div>
                <h2>
                    Output</h2>
                <p>
                    Output blocks will be replaced with the data which they reference. For example,
                    if you want to display the title of the current product, you would use <span class="code_span">
                        {{ product.title }}</span>.</p>
                <h3>
                    Filters</h3>
                <p>
                    Filters are special keywords you can apply to Outputs to process each block before
                    they are displayed. For example, You can also specify multiple filters so that each
                    filter will consequitively process output from a previous filter. For example</p>
                <div class="code_block">
                    <p>
                        To show only the first 20 characters on the product description</p>
                    <p>
                        {{ product.description | truncate: 20 }}</p>
                    <p>
                        To remove any HTML tags from the product description and then truncate the result</p>
                    <p>
                        {{ product.description | strip_html | truncate: 20 }}</p>
                </div>
                <p>
                    <a href="/help/reference#filters">Click here for a list of supported filters</a></p>
                <h2>
                    Tags</h2>
                <p>
                    Tags are used for the logic in your template. A start tag and an end tag is used
                    to enclose content that is to be processed. Think of them like HTML tags but with
                    super abilities.
                </p>
                <div class="code_block">
                    <p>
                        For example, to get a list of names of featured products</p>
                    {% for prod in products.featured %}
                    <br />
                    <span class="tab">{{ prod.name }}</span><br />
                    {% endfor %}
                </div>
                <p>
                    The following sections describes the different types of tags supported by Tradelr</p>
                <h3>
                    Comment</h3>
                <p>
                    Comment is the simplest tag. It just swallows content.</p>
                <div class="code_block">
                    <p>
                        We made 1 million dollars {% comment %} in losses {% endcomment %} this year</p>
                </div>
                <p>
                    The above will display</p>
                <div class="code_result">
                    <p>
                        We made 1 million dollars this year</p>
                </div>
                <h3>
                    If / Else / Unless
                </h3>
                <p>
                    If / Else should be well known from any imaginable programming language. Liquid
                    allows you to write simple expressions in the if or unless (and optionally, elsif
                    and else) clause:
                </p>
                <div class="code_block">
                    {% if user %}<br />
                    <span class="tab">Hello {{ user.name }}</span><br />
                    {% endif %}<br />
                    <br />
                    {% if user.name == 'sean' %}<br />
                    <span class="tab">Hello sean</span><br />
                    {% elsif user.name == 'bob' %}<br />
                    <span class="tab">Hello bob</span><br />
                    {% endif %}<br />
                    <br />
                    {% if user.name == 'sean' or user.name == 'bob' %}<br />
                    <span class="tab">Hello sean or bob</span><br />
                    {% endif %}<br />
                    <br />
                    {% if user.name == 'bob' and user.age > 45 %}<br />
                    <span class="tab">Hello old bob</span><br />
                    {% endif %}<br />
                    <br />
                    {% if user.name != 'sean' %}<br />
                    <span class="tab">Hello not sean</span><br />
                    {% endif %}<br />
                    <br />
                    # Same as above<br />
                    {% unless user.name == 'sean' %}<br />
                    <span class="tab">Hello not sean</span><br />
                    {% endunless %}<br />
                    <br />
                    # Check if the user has a credit card<br />
                    {% if user.creditcard != null %}<br />
                    <span class="tab">poor sob</span><br />
                    {% endif %}<br />
                    <br />
                    # Same as above<br />
                    {% if user.creditcard %}<br />
                    <span class="tab">poor sob</span><br />
                    {% endif %}<br />
                    <br />
                    # Check for an empty array<br />
                    {% if user.payments == empty %}<br />
                    <span class="tab">you never paid !</span><br />
                    {% endif %}<br />
                    <br />
                    {% if user.age > 18 %}<br />
                    <span class="tab">Login here</span><br />
                    {% else %}<br />
                    <span class="tab">Sorry, you are too young</span><br />
                    {% endif %}<br />
                    <br />
                    # array = 1,2,3<br />
                    {% if array contains 2 %}<br />
                    <span class="tab">array includes 2</span><br />
                    {% endif %}<br />
                    <br />
                    # string = 'hello world'<br />
                    {% if string contains 'hello' %}<br />
                    <span class="tab">string includes 'hello'</span><br />
                    {% endif %}<br />
                </div>
                <h3>
                    Case</h3>
                <p>
                    If you need more conditions, you can use the case statement:</p>
                <div class="code_block">
                    {% case condition %}<br />
                    <span class="tab"></span>{% when 1 %}<br />
                    <span class="tab"></span><span class="tab"></span>hit 1<br />
                    <span class="tab"></span>{% when 2 or 3 %}<br />
                    <span class="tab"></span><span class="tab"></span>hit 2 or 3<br />
                    <span class="tab"></span>{% else %}<br />
                    <span class="tab"></span><span class="tab"></span>... else ...<br />
                    {% endcase %}<br />
                </div>
                <p>
                    Example:</p>
                <div class="code_block">
                    {% case template %}<br />
                    <span class="tab"></span>{% when 'label' %}<br />
                    <span class="tab"></span><span class="tab"></span>{{ label.title }}<br />
                    <span class="tab"></span>{% when 'product' %}<br />
                    <span class="tab"></span><span class="tab"></span>{{ product.title }}<br />
                    <span class="tab"></span>{% else %}<br />
                    <span class="tab"></span><span class="tab"></span>{{page_title}}<br />
                    {% endcase %}<br />
                </div>
                <h3>
                    Cycle</h3>
                <p>
                    Often you have to alternate between different colors or similar tasks. Liquid has
                    built-in support for such operations, using the cycle tag.</p>
                <div class="code_block">
                    {% cycle 'one', 'two', 'three' %}<br />
                    {% cycle 'one', 'two', 'three' %}<br />
                    {% cycle 'one', 'two', 'three' %}<br />
                    {% cycle 'one', 'two', 'three' %}<br />
                </div>
                <p>
                    will result in</p>
                <div class="code_result">
                    one<br />
                    two<br />
                    three<br />
                    one<br />
                </div>
                <p>
                    If no name is supplied for the cycle group, then it's assumed that multiple calls
                    with the same parameters are one group.</p>
                <p>
                    If you want to have total control over cycle groups, you can optionally specify
                    the name of the group. This can even be a variable.</p>
                <div class="code_block">
                    {% cycle 'group 1': 'one', 'two', 'three' %}<br />
                    {% cycle 'group 1': 'one', 'two', 'three' %}<br />
                    {% cycle 'group 2': 'one', 'two', 'three' %}<br />
                    {% cycle 'group 2': 'one', 'two', 'three' %}<br />
                </div>
                <p>
                    will result in</p>
                <div class="code_result">
                    one<br />
                    two<br />
                    one<br />
                    two<br />
                </div>
                <h3>
                    For loops</h3>
                <p>
                    Liquid allows for loops over collections:</p>
                <div class="code_block">
                    {% for item in array %} <span class="tab"></span>{{ item }} {% endfor %}
                </div>
                <p>
                    During every for loop, the following helper variables are available for extra styling
                    needs:</p>
                <div class="code_block">
                    forloop.length # => length of the entire for loop<br />
                    forloop.index # => index of the current iteration<br />
                    forloop.index0 # => index of the current iteration (zero based)<br />
                    forloop.rindex # => how many items are still left?<br />
                    forloop.rindex0 # => how many items are still left? (zero based)<br />
                    forloop.first # => is this the first iteration?<br />
                    forloop.last # => is this the last iternation?<br />
                </div>
                <p>
                    There are several attributes you can use to influence which items you receive in
                    your loop</p>
                <p>
                    <span class="code_span">limit:int</span> lets you restrict how many items you get.
                    <span class="code_span">offset:int</span> lets you start the collection with the
                    nth item.</p>
                <div class="code_block">
                    # array = [1,2,3,4,5,6]<br />
                    <br />
                    {% for item in array limit:2 offset:2 %}<br />
                    <span class="tab"></span>{{ item }}<br />
                    {% endfor %}<br />
                    <br />
                    # results in 3,4
                </div>
                <p>
                    Reversing the loop</p>
                <div class="code_block">
                    {% for item in collection reversed %}
                    <br />
                    <span class="tab"></span>{{item}}<br />
                    {% endfor %}<br />
                </div>
                <p>
                    Instead of looping over an existing collection, you can define a range of numbers
                    to loop through. The range can be defined by both literal and variable numbers:</p>
                <div class="code_block">
                    # if item.quantity is 4...<br />
                    <br />
                    {% for i in (1..item.quantity) %}<br />
                    <span class="tab"></span>{{ i }}<br />
                    {% endfor %}<br />
                    <br />
                    # results in 1,2,3,4<br />
                </div>
                <h3>
                    Tables</h3>
                You can create table rows and cells using Liquid. The following will create a table
                of 12 rows and 3 columns
                <div class="code_block">
                    &lt;table&gt;<br />
                    {% tablerow item in items cols: 3 limit: 12 %}<br />
                    <span class="tab"></span>{{ item.variable }}<br />
                    {% endtablerow %}<br />
                    &lt;/table&gt;
                </div>
                <p>
                    The following fields are also available</p>
                <div class="code_block">
                    tablerowloop.length # => length of the entire for loop<br />
                    tablerowloop.index # => index of the current iteration
                    <br />
                    tablerowloop.index0 # => index of the current iteration (zero based)
                    <br />
                    tablerowloop.rindex # => how many items are still left?<br />
                    tablerowloop.rindex0 # => how many items are still left? (zero based)<br />
                    tablerowloop.first # => is this the first iteration?<br />
                    tablerowloop.last # => is this the last iteration?
                    <br />
                    tablerowloop.col # => index of column in the current row<br />
                    tablerowloop.col0 # => index of column in the current row (zero based)<br />
                    tablerowloop.col_first # => is this the first column in the row?<br />
                    tablerowloop.col_last # => is this the last column in the row?<br />
                </div>
                <p>
                    Using the above fields, you can determine if a table cell is the first or last column.</p>
                <div class="code_block">
                    {% tablerow item in items cols: 3 %}<br />
                    <span class="tab"></span>{% if tablerowloop.col_first %}<br />
                    <span class="tab"></span><span class="tab"></span>First column: {{ item.variable
                    }}<br />
                    <span class="tab"></span>{% else %}<br />
                    <span class="tab"></span><span class="tab"></span>Different column: {{ item.variable
                    }}<br />
                    <span class="tab"></span>{% endif %}<br />
                    {% endtablerow %}<br />
                </div>
                <h3>
                    Variable Assignment</h3>
                <p>
                    You can store data in your own variables, to be used in output or other tags as
                    desired. The simplest way to create a variable is with the assign tag, which has
                    a pretty straight forward syntax:
                </p>
                <div class="code_block">
                    <p>
                        {% assign name = 'freestyle' %}</p>
                    {% for t in collections.tags %}<br />
                    <span class="tab"></span>{% if t == name %}<br />
                    <span class="tab"></span><span class="tab"></span>&lt;p&gt;Freestyle!&lt;/p&gt;<br />
                    <span class="tab"></span>{% endif %}<br />
                    {% endfor %}<br />
                </div>
                <p>
                    Another way of doing this would be to assign <span class="code_span">true</span>
                    or <span class="code_span">false</span> values to the variable:</p>
                <div class="code_block">
                    {% assign freestyle = false %}<br />
                    <br />
                    {% for t in collections.tags %}<br />
                    <span class="tab"></span>{% if t == 'freestyle' %}<br />
                    <span class="tab"></span><span class="tab"></span>{% assign freestyle = true %}<br />
                    <span class="tab"></span>{% endif %}<br />
                    {% endfor %}<br />
                    <br />
                    {% if freestyle %}<br />
                    <span class="tab"></span>&lt;p&gt;Freestyle!&lt;/p&gt;<br />
                    {% endif %}<br />
                </div>
                <p>
                    If you want to combine a number of strings into a single string and save it to a
                    variable, you can do that with the capture tag. This tag is a block which "captures"
                    whatever is rendered inside it, then assigns the captured value to the given variable
                    instead of rendering it to the screen.
                </p>
                <div class="code_block">
                    {% capture attribute_name %}<br />
                    <span class="tab"></span>{{ item.title | handleize }}-{{ i }}-color<br />
                    {% endcapture %}<br />
                    <br />
                    &lt;label for="{{ attribute_name }}"&gt;Color:&lt;/label&gt;<br />
                    &lt;select name="attributes[{{ attribute_name }}]" id="{{ attribute_name }}"&gt;<br />
                    <span class="tab"></span>&lt;option value="red"&gt;Red&lt;/option&gt;<br />
                    <span class="tab"></span>&lt;option value="green"&gt;Green&lt;/option&gt;<br />
                    <span class="tab"></span>&lt;option value="blue"&gt;Blue&lt;/option&gt;<br />
                    &lt;/select&gt;<br />
                </div>
                <h3>
                    Form</h3>
                <p>
                    The Form tag renders a form. This is used to allow customers to post comments on
                    blogs or contact form.</p>
                <h3>
                    Include</h3>
                <p>
                    The Include tag allows you to insert a snippet into the current layout. A snippet
                    is a .liquid file in your snippets folder.</p>
                <div class="code_block">
                    <p>
                        The following will insert the snippet foo.liquid in your theme's snippets folder</p>
                    {% include 'foo' %}
                </div>
                <p>
                    You can also assign variables to be used within the snippet. For example, the following
                    is the content of the file color.liquid in your snippets folder</p>
                <div class="code_block">
                    color: '{{ color }}'<br />
                    shape: '{{ shape }}'<br />
                </div>
                <p>
                    In your theme.liquid layout, you assign values to be used in color.liquid as follows</p>
                <div class="code_block">
                    {% assign shape = 'circle' %}<br />
                    {% include 'color' %}<br />
                    {% include 'color' with 'red' %}<br />
                    {% include 'color' with 'blue' %}<br />
                    <br />
                    {% assign shape = 'square' %}<br />
                    {% include 'color' with 'red' %}<br />
                </div>
                <p>
                    This will give the following result</p>
                <div class="code_result">
                    color: ''<br />
                    shape: 'circle'<br />
                    color: 'red'<br />
                    shape: 'circle'<br />
                    color: 'blue'<br />
                    shape: 'circle'<br />
                    <br />
                    color: 'red'<br />
                    shape: 'square'<br />
                </div>
                <h3>
                    Pagination</h3>
                <p>
                    The Paginate tag allows you to page a collection of items by a maximum number of
                    items per page. For example, the following will only show the url of 10 search results
                    per page. The <span class="code_span">default_pagination</span> filter will display
                    navigation for the paginated pages.
                </p>
                <div class="code_block">
                    {% paginate search.results by 10 %}<br />
                    <span class="tab"></span>{% for item in search.results %}<br />
                    <span class="tab"></span><span class="tab"></span>{{ item.url }}<br />
                    <span class="tab"></span>{% endfor %}<br />
                    &lt;div id="paginate"&gt;<br />
                    <span class="tab"></span>{{ paginate | default_pagination }}<br />
                    &lt;/div&gt;<br />
                    {% endpaginate %}<br />
                </div>
                <p>
                    The following fields are available</p>
                <div class="code_block">
                    paginate.page_size # => The number of items per page<br />
                    paginate.current_page # => The current page number<br />
                    paginate.current_offset # => How many pages away we are from the first page<br />
                    paginate.pages # => Total number of pages<br />
                    paginate.items # => Total amount of items in this collection<br />
                    paginate.previous # => Exists if there is a previous page.<br />
                    paginate.previous.title # => Title of the link<br />
                    paginate.previous.url # => URL of the link
                    <br />
                    paginate.next # => Exists if there is another page.<br />
                    paginate.next.title # => Link title<br />
                    paginate.next.url # => Link URL
                    <br />
                    <br />
                    paginate.parts # => List of all pages for this pagination which consists of the
                    following:<br />
                    <span class="tab"></span>part.is_link # => Is this part a link?<br />
                    <span class="tab"></span>part.title # => Link Title<br />
                    <span class="tab"></span>part.url # => Link URL
                    <br />
                </div>
                <div class="mt50 ar">
                Next: <a href="/help/storefront/variable">Liquid Variables: Accessing your store content</a>
                </div>
            </div>
            <div id="help_content_bottom" class="ml200">
            </div>
            <div class="clear"></div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    Help - Using Liquid
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
</asp:Content>
