<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.shipping.ShippingProfile>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<%@ Import Namespace="tradelr.Models.shipping" %>
<% if (!Model.IsPermanent)
   {%>
<div class="fr">
    <a id="deleteShippingProfile" href="#" class="icon_del smaller">delete shipping profile</a>
</div>
<% } %>
<div class="clear">
</div>
<div class="section_header">
    Shipping Type
</div>
<div class="form_group">
    <ul>
        <li class="w200px" title="fixed costs per destination">
            <input id="flatrateshipping" type="radio" name="shippingType" value="<%= ShippingProfileType.FLATRATE %>"
                <%=Model.type == ShippingProfileType.FLATRATE ? "checked='checked'" : ""%> />
                <label for="flatrateshipping">
                    flat rate shipping</label></li>
        <li title="calculated costs dependant on weight and/or pricing">
            <input id="calculatedshipping" type="radio" name="shippingType" value="<%= ShippingProfileType.NONE %>"
                <%=Model.type == ShippingProfileType.SHIPWIRE || Model.type == ShippingProfileType.CALCULATED ? "checked='checked'" : ""%> />
                <label for="calculatedshipping">
                    calculated shipping</label></li></ul>
    <div class="clear">
    </div>
</div>
<div id="flatrate" class="hidden">
    <div class="section_header">
        Flat Rate Shipping</div>
    <div class="form_group">
        <p class="icon_help">
            Specify shipping costs by destination country</p>
        <select name="country" id="country">
            <option value="">Please select a country</option>
            <option value="2">Afghanistan</option>
            <option value="3">Albania</option>
            <option value="4">Algeria</option>
            <option value="239">American Samoa</option>
            <option value="5">Andorra</option>
            <option value="6">Angola</option>
            <option value="224">Anguilla</option>
            <option value="7">Antigua and Barbuda</option>
            <option value="8">Argentina</option>
            <option value="9">Armenia</option>
            <option value="249">Aruba</option>
            <option value="10">Australia</option>
            <option value="11">Austria</option>
            <option value="12">Azerbaijan</option>
            <option value="13">Bahamas</option>
            <option value="14">Bahrain</option>
            <option value="15">Bangladesh</option>
            <option value="16">Barbados</option>
            <option value="17">Belarus</option>
            <option value="18">Belgium</option>
            <option value="19">Belize</option>
            <option value="20">Benin</option>
            <option value="225">Bermuda</option>
            <option value="21">Bhutan</option>
            <option value="22">Bolivia</option>
            <option value="23">Bosnia and Herzegovina</option>
            <option value="24">Botswana</option>
            <option value="217">Bouvet Island</option>
            <option value="25">Brazil</option>
            <option value="226">British Indian Ocean Territory</option>
            <option value="228">British Virgin Islands</option>
            <option value="26">Brunei</option>
            <option value="27">Bulgaria</option>
            <option value="28">Burkina Faso</option>
            <option value="29">Burundi</option>
            <option value="30">Cambodia</option>
            <option value="31">Cameroon</option>
            <option value="32">Canada</option>
            <option value="33">Cape Verde</option>
            <option value="229">Cayman Islands</option>
            <option value="34">Central African Republic</option>
            <option value="35">Chad</option>
            <option value="36">Chile</option>
            <option value="37">China</option>
            <option value="203">Christmas Island</option>
            <option value="204">Cocos (Keeling) Islands</option>
            <option value="38">Colombia</option>
            <option value="39">Comoros</option>
            <option value="41">Congo, Republic of</option>
            <option value="218">Cook Islands</option>
            <option value="42">Costa Rica</option>
            <option value="44">Croatia</option>
            <option value="45">Cuba</option>
            <option value="46">Cyprus</option>
            <option value="47">Czech Republic</option>
            <option value="48">Denmark</option>
            <option value="49">Djibouti</option>
            <option value="50">Dominica</option>
            <option value="51">Dominican Republic</option>
            <option value="52">Ecuador</option>
            <option value="53">Egypt</option>
            <option value="54">El Salvador</option>
            <option value="55">Equatorial Guinea</option>
            <option value="56">Eritrea</option>
            <option value="57">Estonia</option>
            <option value="58">Ethiopia</option>
            <option value="230">Falkland Islands (Malvinas)</option>
            <option value="261">Faroe Islands</option>
            <option value="59">Fiji</option>
            <option value="60">Finland</option>
            <option value="61">France</option>
            <option value="244">French Guiana</option>
            <option value="209">French Polynesia</option>
            <option value="215">French Southern Territories</option>
            <option value="62">Gabon</option>
            <option value="63">Gambia</option>
            <option value="64">Georgia</option>
            <option value="65">Germany</option>
            <option value="66">Ghana</option>
            <option value="231">Gibraltar</option>
            <option value="67">Greece</option>
            <option value="243">Greenland</option>
            <option value="68">Grenada</option>
            <option value="245">Guadeloupe</option>
            <option value="240">Guam</option>
            <option value="69">Guatemala</option>
            <option value="70">Guinea</option>
            <option value="71">Guinea-Bissau</option>
            <option value="72">Guyana</option>
            <option value="73">Haiti</option>
            <option value="206">Heard Island and McDonald Islands</option>
            <option value="189">Holy See (Vatican City State)</option>
            <option value="74">Honduras</option>
            <option value="241">Hong Kong</option>
            <option value="75">Hungary</option>
            <option value="76">Iceland</option>
            <option value="77">India</option>
            <option value="78">Indonesia</option>
            <option value="79">Iran</option>
            <option value="80">Iraq</option>
            <option value="81">Ireland</option>
            <option value="222">Isle of Man</option>
            <option value="82">Israel</option>
            <option value="83">Italy</option>
            <option value="43">Ivory Coast</option>
            <option value="84">Jamaica</option>
            <option value="85">Japan</option>
            <option value="86">Jordan</option>
            <option value="87">Kazakhstan</option>
            <option value="88">Kenya</option>
            <option value="89">Kiribati</option>
            <option value="255">Kosovo</option>
            <option value="92">Kuwait</option>
            <option value="93">Kyrgyzstan</option>
            <option value="94">Laos</option>
            <option value="95">Latvia</option>
            <option value="96">Lebanon</option>
            <option value="97">Lesotho</option>
            <option value="98">Liberia</option>
            <option value="99">Libya</option>
            <option value="100">Liechtenstein</option>
            <option value="101">Lithuania</option>
            <option value="102">Luxembourg</option>
            <option value="242">Macao</option>
            <option value="103">Macedonia</option>
            <option value="104">Madagascar</option>
            <option value="105">Malawi</option>
            <option value="106">Malaysia</option>
            <option value="107">Maldives</option>
            <option value="108">Mali</option>
            <option value="109">Malta</option>
            <option value="110">Marshall Islands</option>
            <option value="246">Martinique</option>
            <option value="111">Mauritania</option>
            <option value="112">Mauritius</option>
            <option value="210">Mayotte</option>
            <option value="113">Mexico</option>
            <option value="114">Micronesia, Federated States of</option>
            <option value="115">Moldova</option>
            <option value="116">Monaco</option>
            <option value="117">Mongolia</option>
            <option value="118">Montenegro</option>
            <option value="232">Montserrat</option>
            <option value="119">Morocco</option>
            <option value="120">Mozambique</option>
            <option value="121">Myanmar (Burma)</option>
            <option value="122">Namibia</option>
            <option value="123">Nauru</option>
            <option value="124">Nepal</option>
            <option value="125">Netherlands</option>
            <option value="250">Netherlands Antilles</option>
            <option value="208">New Caledonia</option>
            <option value="126">New Zealand</option>
            <option value="127">Nicaragua</option>
            <option value="128">Niger</option>
            <option value="129">Nigeria</option>
            <option value="219">Niue</option>
            <option value="207">Norfolk Island</option>
            <option value="237">Northern Mariana Islands</option>
            <option value="90">North Korea</option>
            <option value="130">Norway</option>
            <option value="131">Oman</option>
            <option value="132">Pakistan</option>
            <option value="133">Palau</option>
            <option value="256">Palestinian Territory, Occupied</option>
            <option value="134">Panama</option>
            <option value="135">Papua New Guinea</option>
            <option value="136">Paraguay</option>
            <option value="137">Peru</option>
            <option value="138">Philippines</option>
            <option value="139">Poland</option>
            <option value="140">Portugal</option>
            <option value="238">Puerto Rico</option>
            <option value="141">Qatar</option>
            <option value="142">Romania</option>
            <option value="143">Russia</option>
            <option value="144">Rwanda</option>
            <option value="234">Saint Helena</option>
            <option value="145">Saint Kitts and Nevis</option>
            <option value="146">Saint Lucia</option>
            <option value="212">Saint Martin (French part)</option>
            <option value="213">Saint Pierre and Miquelon</option>
            <option value="147">Saint Vincent and the Grenadines</option>
            <option value="148">Samoa</option>
            <option value="149">San Marino</option>
            <option value="150">Sao Tome and Principe</option>
            <option value="151">Saudi Arabia</option>
            <option value="152">Senegal</option>
            <option value="153">Serbia</option>
            <option value="154">Seychelles</option>
            <option value="155">Sierra Leone</option>
            <option value="156">Singapore</option>
            <option value="157">Slovakia</option>
            <option value="158">Slovenia</option>
            <option value="159">Solomon Islands</option>
            <option value="160">Somalia</option>
            <option value="161">South Africa</option>
            <option value="235">South Georgia and the South Sandwich Islands</option>
            <option value="91">South Korea</option>
            <option value="162">Spain</option>
            <option value="163">Sri Lanka</option>
            <option value="164">Sudan</option>
            <option value="165">Suriname</option>
            <option value="251">Svalbard and Jan Mayen</option>
            <option value="166">Swaziland</option>
            <option value="167">Sweden</option>
            <option value="168">Switzerland</option>
            <option value="169">Syria</option>
            <option value="196">Taiwan</option>
            <option value="170">Tajikistan</option>
            <option value="171">Tanzania</option>
            <option value="172">Thailand</option>
            <option value="173">Timor-Leste</option>
            <option value="174">Togo</option>
            <option value="220">Tokelau</option>
            <option value="175">Tonga</option>
            <option value="176">Trinidad</option>
            <option value="177">Tunisia</option>
            <option value="178">Turkey</option>
            <option value="179">Turkmenistan</option>
            <option value="236">Turks and Caicos Islands</option>
            <option value="180">Tuvalu</option>
            <option value="181">Uganda</option>
            <option value="182">Ukraine</option>
            <option value="183">United Arab Emirates</option>
            <option value="184">United Kingdom</option>
            <option value="185">United States</option>
            <option value="262">United States Minor Outlying Islands</option>
            <option value="186">Uruguay</option>
            <option value="263">U.S. Virgin Islands</option>
            <option value="187">Uzbekistan</option>
            <option value="188">Vanuatu</option>
            <option value="190">Venezuela</option>
            <option value="191">Vietnam</option>
            <option value="214">Wallis and Futuna</option>
            <option value="264">Western Sahara</option>
            <option value="192">Yemen</option>
            <option value="265">Zaire (Democratic Republic of Congo)</option>
            <option value="193">Zambia</option>
            <option value="194">Zimbabwe</option>
        </select>
        <button id="buttonAddCountry" class="small" type="button">
            add destination</button>
        <table id="flatrateTable">
            <thead>
                <tr>
                    <td>
                    </td>
                    <td class="bold">
                        Shipping Destination
                    </td>
                    <td class="bold">
                        Shipping Cost (<%= Model.currency.code %>)
                    </td>
                </tr>
            </thead>
            <tbody>
                <% foreach (var rule in Model.flatrateRules)
                   {%>
                <tr id="<%= rule.id %>">
                    <td class="w50px al am">
                        <span class="hover_del"></span>
                    </td>
                    <td>
                        <%= rule.country %>
                        <%= Html.Hidden("shipping_destination", rule.countryid) %>
                    </td>
                    <td>
                        <%= Html.TextBox("shipping_cost", rule.cost)%>
                    </td>
                </tr>
                <%
                   }%>
                <tr>
                    <td colspan="3">
                        <span class="tip">The following shipping cost will be applied to any destination not
                            specified above</span>
                    </td>
                </tr>
                <tr>
                    <td class="w50px al am">
                        <%= Html.CheckBox("applyEverywhereElseCost", Model.applyEverywhereElseCost) %>
                    </td>
                    <td>
                        Everywhere Else
                        <input type="hidden" value="" name="shipping_destination" />
                    </td>
                    <td>
                        <input type="text" value="<%= Model.everywhereElseCost %>" name="shipping_cost" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</div>
<div id="calculated" class="hidden">
    <div class="section_header">
        Calculated Shipping</div>
    <div class="form_group">
        <ul>
            <li>
                <input id="calculated_manual" type="radio" name="calculated_ship" value="<%= ShippingProfileType.CALCULATED %>"
                    <%=Model.type == ShippingProfileType.CALCULATED ? "checked='checked'" : ""%> /><label for="calculated_manual">
                        use shipping rules - specify shipping costs by total weight or total price of an
                        order</label>
            </li>
            <li>
                <input id="calculated_shipwire" type="radio" name="calculated_ship" value="<%=ShippingProfileType.SHIPWIRE%>"
                    <%=Model.type == ShippingProfileType.SHIPWIRE ? "checked='checked'" : ""%> <%= Model.shipwireEnabled?"":"disabled='disabled'" %> />
                <label for="calculated_shipwire">
                    use <a class="font_shipwire" href="http://www.shipwire.com/pp/o.php?id=5191" target="_blank">
                        Shipwire</a>
                    <% if (!Model.shipwireEnabled)
                       {%>
                    (<a href="/dashboard/networks">connect with Shipwire</a>)
                    <%}%>
                </label>
            </li>
        </ul>
        <div class="clear">
        </div>
    </div>
    <div id="manual" class="hidden">
        <a id="shipping_destination_add" class="icon_add" href="#">add shipping rule</a>
        <div id="shipping_header" class="hidden smaller font_darkgrey bold">
            <div class="fr ar" style="width: 170px">
                shipping cost,
                <%= Model.currency.symbol %></div>
            <div class="ar fr w200px" title="Apply shipping cost when order is equal to or above this value">
                effective value</div>
            <div class="clear">
            </div>
        </div>
        <ul id="shipping_countries">
            <%
                foreach (var countrygroup in Model.shippingGroups)
                {%>
            <li><span class="shipping_country_name bold">
                <%= countrygroup.name %></span>
                <table>
                    <% foreach (var rule in countrygroup.rules)
                       { %>
                    <tr id="<%= rule.id %>">
                        <td class="w50px al">
                            <span class="hover_edit"></span><span class="hover_del"></span>
                        </td>
                        <td class="al">
                            <%= rule.name %>
                        </td>
                        <td class="w200px">
                            <%= rule.matchValue %>
                        </td>
                        <td class="w200px">
                            <%= rule.currency %><%= rule.cost %>
                        </td>
                    </tr>
                    <% }%>
                </table>
                <ul class="shipping_states">
                    <% foreach (var stategroup in countrygroup.subgroup)
                       {%>
                    <li><span class="shipping_state_name smaller bold">
                        <%= stategroup.name %></span>
                        <table>
                            <% foreach (var rule in stategroup.rules)
                               { %>
                            <tr id="<%= rule.id %>">
                                <td class="w50px al">
                                    <span class="hover_edit"></span><span class="hover_del"></span>
                                </td>
                                <td class="al">
                                    <%= rule.name %>
                                </td>
                                <td class="w200px">
                                    <%= rule.matchValue %>
                                </td>
                                <td class="w200px">
                                    <%= rule.currency %><%= rule.cost %>
                                </td>
                            </tr>
                            <% }%>
                        </table>
                    </li>
                    <%} %>
                </ul>
            </li>
            <%
                }%>
        </ul>
    </div>
</div>
<%= Html.Hidden("shippingProfileID", Model.id) %>
<script type="text/javascript">
    $('#deleteShippingProfile').die().live('click', function () {
        var ok = window.confirm('Are you sure you want to delete this shipping profile?');
        if (!ok) {
            return false;
        }
        var profileid = $('#shippingProfileID').val();
        $.post('/dashboard/shipping/deleteprofile/' + profileid, function (json_result) {
            if (json_result.success) {
                $('#shipping_profile').html('');
                $('#shippingprofile').val('');
                $('option[value=' + profileid + ']', '#shippingprofile').remove();
            }
            else {
                $.jGrowl(json_result.message);
            }
        }, 'json');
        return false;
    });

    $('.hover_del', '#flatrateTable').die().live('click', function () {
        // delete if rule id not null otherwise just remove entry
        var row = $(this).parents('tr');
        var id = row.attr('id');
        if (id == undefined || id == '') {
            $(row).remove();
        }
        else {
            $.post('/dashboard/shipping/delete/' + id, { profileid: $('#shippingProfileID').val() }, function (json_result) {
                if (json_result.success) {
                    $(row).slideUp('fast', function () {
                        $(this).remove();
                    });
                }
                else {
                    $.jGrowl(json_result.message);
                }
            }, 'json');
        }
    });

    $('#buttonAddCountry').die().live('click', function () {
        var selectedCountry = $('#country', '#flatrate').val();
        if (selectedCountry == '') {
            $.jGrowl('Please select a destination first');
            return false;
        }

        // check if entry already exist
        var entries = $('td > input[type="hidden"]', '#flatrateTable');
        var matchFound = false;
        $.each(entries, function () {
            if ($(this).val() == selectedCountry) {
                matchFound = true;
                return false;
            }
        });

        if (matchFound) {
            $.jGrowl('Destination has already been added');
            return false;
        }

        var selectedCountryName = $('#country > option[value=' + selectedCountry + ']', '#flatrate').text();
        var html = ['<tr>',
                    '<td class="w50px al am"><span class="hover_del"></span></td>',
                    '<td>', selectedCountryName, '<input name="shipping_destination" id="shipping_destination" type="hidden" value="' + selectedCountry + '" /></td>',
                    '<td><input type="text" name="shipping_cost" id="shipping_cost" /></td>',
                    '</tr>'];
        $('#flatrateTable > tbody').prepend(html.join(''));
        inputSelectors_init('#flatrateTable > tbody');
        $('input[type=text]', '#flatrateTable').numeric({ allow: '.' });
        return false;
    });
    $(document).ready(function () {
        if ($('#shipping_countries').children().length != 0) {
            $('#shipping_header').fadeIn();
        }

        inputSelectors_init();
        $('input[type=text]', '#flatrateTable').numeric({ allow: '.' });
    });
</script>
