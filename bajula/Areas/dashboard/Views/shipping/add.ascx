<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.shipping.viewmodel.ShippingRuleViewModel>" %>
<%@ Import Namespace="tradelr.Areas.dashboard.Models.shipping" %>
<%@ Import Namespace="tradelr.Models.shipping" %>
<form id="shippingAddForm" action="/dashboard/shipping/create" method="post">
<div class="form_entry">
    <div class="form_label">
        <label for="name">
            Shipping service name</label><span class="tip">Examples: Free Shipping, International, Ground </span>
    </div>
    <%= Html.TextBox("name")%>
</div>
<div class="fl mr10">
    <div class="form_entry" style="width: 180px;">
        <div class="form_label">
            <label for="country">
                Country</label>
        </div>
        <select name="country" id="country">
            <option value="">Everywhere Else</option>
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
    </div>
</div>
<div class="fl">
    <div class="form_entry">
        <div class="form_label">
            <label>
                State</label>
        </div>
        <select id="states_canadian" name="states_canadian" class="hidden states">
            <option value="">Others</option>
            <option value="ON">Ontario</option>
            <option value="QC">Quebec</option>
            <option value="NS">Nova Scotia</option>
            <option value="NB">New Brunswick</option>
            <option value="MB">Manitoba</option>
            <option value="BC">British Columbia</option>
            <option value="PE">Prince Edward Island</option>
            <option value="SK">Saskatchewan</option>
            <option value="AB">Alberta</option>
            <option value="NL">Newfoundland and Labrador</option>
        </select>
        <select id="states_us" name="states_us" class="hidden states">
            <option value="">Others</option>
            <option value="AL">Alabama</option>
            <option value="AK">Alaska</option>
            <option value="AZ">Arizona</option>
            <option value="AR">Arkansas</option>
            <option value="CA">California</option>
            <option value="CO">Colorado</option>
            <option value="CT">Connecticut</option>
            <option value="DE">Delaware</option>
            <option value="FL">Florida</option>
            <option value="GA">Georgia</option>
            <option value="HI">Hawaii</option>
            <option value="ID">Idaho</option>
            <option value="IL">Illonois</option>
            <option value="IN">Indiana</option>
            <option value="IA">Iowa</option>
            <option value="KS">Kansas</option>
            <option value="KY">Kentucky</option>
            <option value="LA">Louisiana</option>
            <option value="ME">Maine</option>
            <option value="MD">Maryland</option>
            <option value="MA">Massachusetts</option>
            <option value="MI">Michigan</option>
            <option value="MN">Minnesota</option>
            <option value="MS">Mississippi</option>
            <option value="MO">Missouri</option>
            <option value="MT">Montana</option>
            <option value="NE">Nebraska</option>
            <option value="NV">Nevada</option>
            <option value="NH">New Hamsphire</option>
            <option value="NJ">New Jersey</option>
            <option value="NM">New Mexico</option>
            <option value="NY">New York</option>
            <option value="NC">North Carolina</option>
            <option value="ND">North Dakota</option>
            <option value="OH">Ohio</option>
            <option value="OK">Oklahoma</option>
            <option value="OR">Oregon</option>
            <option value="PA">Pennsylvania</option>
            <option value="RI">Rhode Island</option>
            <option value="SC">South Carolina</option>
            <option value="SD">South Dakota</option>
            <option value="TN">Tennessee</option>
            <option value="TX">Texas</option>
            <option value="UT">Utah</option>
            <option value="VT">Vermont</option>
            <option value="VA">Virginia</option>
            <option value="WA">Washington</option>
            <option value="WV">West Virginia</option>
            <option value="WI">Wisconsin</option>
            <option value="WY">Wyoming</option>
        </select>
        <input type="text" id="states_other" name="states_other" class="states" />
    </div>
</div>
<div class="clear">
</div>
<div class="form_entry">
    <div class="form_label">
        <label>
            Type of rule</label>
    </div>
    <ul>
        <li>
            <input type="radio" id="weight_rule" name="rule_type" value="weight" <%= Model.ruleType == RuleType.WEIGHT?"checked='checked'":"" %>  /><label for="weight_rule">weight-based
                rule</label><span class="smaller"><a id="usemetric" href="#" class="pr4 pl4 r4">metric(kg)</a> | <a id="useimperial" href="#" class="pr4 pl4 r4">imperial(lb)</a></span>
        </li>
        <li>
            <input type="radio" id="price_rule" name="rule_type" value="price" <%= Model.ruleType == RuleType.PRICE?"checked='checked'":"" %> /><label for="price_rule">price-based
                rule</label></li>
    </ul>
</div>
<div class="form_entry">
    <div class="form_label">
        <label for="matchvalue">
            Effective value, <span id="effectivelabel">lb</span></label>
            <span class="tip">Apply rule to orders equal and above this value</span>
    </div>
    <%= Html.TextBox("matchvalue", Model.matchValue)%>
</div>
<div class="form_entry">
    <div class="form_label">
        <label for="cost">
            Shipping cost, <%= Model.currency %></label>
    </div>
    <%= Html.TextBox("cost", Model.cost)%>
</div>
<div class="pt5">
    <button class="green" type="button" id="buttonSave">
        save</button>
    <button type="button" id="buttonCancel">
        close</button>
</div>
<%= Html.Hidden("metric", Model.isMetric) %>
<%= Html.Hidden("ruleid", Model.id) %>
<%= Html.Hidden("profileid", Model.profileid) %>
<span id="currency" class="hidden"><%= Model.currency %></span>
<span id="countryval" class="hidden"><%= Model.countryid  %></span>
<span id="stateval" class="hidden"><%= Model.state %></span>
</form>
<script type="text/javascript">

    function init_location(value) {
        var selected;
        if (value == undefined || value == '') {
            selected = $('#country', "#shippingAddForm").val();
        }
        else {
            selected = value;
        }
        $('.states', "#shippingAddForm").hide();
        switch (selected) {
            case "185":
                $('#states_us', "#shippingAddForm").show().val($('#stateval', "#shippingAddForm").text());
                break;
            case "32":
                $('#states_canadian', "#shippingAddForm").show().val($('#stateval', "#shippingAddForm").text());
                break;
            default:
                $('#states_other', "#shippingAddForm").show().val($('#stateval', "#shippingAddForm").text());
                break;
        }
    }

    $(document).ready(function () {
        // disable fields if it's not in edit mode
        if ($('#ruleid', '#shippingAddForm').val() != '') {
            $('#country,#states_other,#states_us,#states_canadian,#weight_rule,#price_rule', '#shippingAddForm').attr('disabled', true);
            $('#shippingAddForm').attr('action', '/dashboard/shipping/update');
        }

        $('#cost,#matchvalue').numeric({ allow: '.' });
        if ($('#metric', '#shippingAddForm').val() == 'True') {
            $('#effectivelabel', '#shippingAddForm').html('kg');
            $('#usemetric').addClass('selected');
        }
        else {
            $('#effectivelabel', '#shippingAddForm').html('lb');
            $('#useimperial').addClass('selected');
        }

        $('#country', "#shippingAddForm").bind('change', function () {
            init_location();
        });
        var countryval = $('#countryval', "#shippingAddForm").text();
        $('#country', "#shippingAddForm").val(countryval);
        init_location(countryval);

        // prettify
        inputSelectors_init();
    });

    $('#price_rule', '#shippingAddForm').click(function () {
        $('#effectivelabel', '#shippingAddForm').html($('#currency', '#shippingAddForm').text());
    });

    $('#weight_rule', '#shippingAddForm').click(function () {
        if ($('#metric', '#shippingAddForm').val() == 'True') {
            $('#effectivelabel', '#shippingAddForm').html('kg');
        }
        else {
            $('#effectivelabel', '#shippingAddForm').html('lb');
        }
    });

    $('#usemetric', '#shippingAddForm').click(function () {
        if ($('#price_rule', '#shippingAddForm').is(':checked')) {
            $('#weight_rule', '#shippingAddForm').attr('checked', true);
        }

        if ($('#metric', '#shippingAddForm').val() == 'True') {
            return false;
        }

        $('#usemetric').addClass('selected');
        $('#useimperial').removeClass('selected');

        $('#matchvalue', '#shippingAddForm').val(tradelr.util.convertweight($('#matchvalue', '#shippingAddForm').val(), true));
        $('#effectivelabel', '#shippingAddForm').html('kg');
        $('#metric', '#shippingAddForm').val('True');
        $.post('/settings/metric/1');
        return false;
    });

    $('#useimperial', '#shippingAddForm').click(function () {
        if ($('#price_rule', '#shippingAddForm').is(':checked')) {
            $('#weight_rule', '#shippingAddForm').attr('checked', true);
        }

        if ($('#metric', '#shippingAddForm').val() == 'False') {
            return false;
        }

        $('#usemetric').removeClass('selected');
        $('#useimperial').addClass('selected');

        $('#matchvalue', '#shippingAddForm').val(tradelr.util.convertweight($('#matchvalue', '#shippingAddForm').val(), false));
        $('#effectivelabel', '#shippingAddForm').html('lb');
        $('#metric', '#shippingAddForm').val('False');
        $.post('/settings/metric/0');
        return false;
    });

    $('#buttonSave', '#shippingAddForm').click(function () {
        $(this).buttonDisable();
        $('#shippingAddForm').trigger('submit');
    });

    $('#buttonCancel', '#shippingAddForm').click(function () {
        dialogBox_close();
        return false;
    });

    $('#shippingAddForm').submit(function () {
        var serialized = $(this).serialize();
        var action = $(this).attr("action");

        var ok = $('#shippingAddForm').validate({
            invalidHandler: function (form, validator) {
                $(validator.invalidElements()[0]).focus();
            },
            focusInvalid: false,
            rules: {
                name: {
                    required: true
                },
                country: {
                    required: false
                },
                matchvalue: {
                    required: true
                },
                cost: {
                    required: true
                }
            }
        }).form();
        if (!ok) {
            $('#buttonSave', '#shippingAddForm').buttonEnable();
            return false;
        }
        // post form
        $.ajax({
            type: "POST",
            url: action,
            data: serialized,
            dataType: "json",
            success: function (json_data) {
                if (json_data.success) {
                    var data = json_data.data;

                    if ($('#ruleid', '#shippingAddForm').val() != '') {
                        var numberPattern = /[\d\.]+/g;
                        var id = $('#ruleid', '#shippingAddForm').val();
                        /////////// UPDATE
                        var row = $('#' + id, '#shipping_countries');
                        var namecell = $(row).find('td:eq(1)');
                        var matchvalcell = $(row).find('td:eq(2)');
                        var costcell = $(row).find('td:eq(3)');

                        var newname = $('#name', '#shippingAddForm').val();
                        var newmatchvalue = $(matchvalcell).text().replace(numberPattern, $('#matchvalue', '#shippingAddForm').val());
                        var newcost = $(costcell).text().replace(numberPattern, $('#cost', '#shippingAddForm').val());

                        $(namecell).text(newname);
                        $(matchvalcell).text(newmatchvalue);
                        $(costcell).text(newcost);
                        $.jGrowl('Rule successfully updated');
                        dialogBox_close();
                    }
                    else {
                        var renderRow = function (id, name, matchValue, cost) {
                            return ["<tr id='" + id + "'>",
                                "<td class='w50px al'>",
                                "<span class='hover_edit'></span>",
                                "<span class='hover_del'></span>",
                                "</td>",
                                "<td class='al'>",
                                name,
                                "</td>",
                                "<td class='w200px'>",
                                matchValue,
                                "</td>",
                                "<td class='w200px'>",
                                cost,
                                "</td>",
                                "</tr>"];
                        };

                        /////////// CREATE
                        if (!$('#shipping_header').is(':visible')) {
                            $('#shipping_header').fadeIn();
                        }
                        // if state is null look for country entry
                        // check if that country already exist... 
                        var countryentry = $(".shipping_country_name:contains('" + data.country + "')");

                        if (data.state == '') {

                            if (countryentry.length != 0) {
                                // if exist then add to table
                                var entry = renderRow(data.id, data.name, data.matchValue, data.cost).join('');
                                countryentry.next().append(entry);
                            }
                            else {
                                // if not then create and add to table
                                var head = ["<li>",
                                    "<span class='shipping_country_name bold'>",
                                    data.country,
                                    "</span>",
                                    "<table>"];
                                var row = renderRow(data.id, data.name, data.matchValue, data.cost);
                                var foot = ["</table>", "<ul class='shipping_states'>", "</ul>", "</li>"];
                                var entry = head.concat(row).concat(foot).join('');

                                $('#shipping_countries').append(entry);
                            }
                        }
                        else {
                            // handle state
                            // is there already a country entry?
                            if (countryentry.length == 0) {
                                // add new country entry
                                var head = ["<li>",
                                    "<span class='shipping_country_name bold'>",
                                    data.country,
                                    "</span>",
                                    "<table>",
                                    "</table>",
                                    "<ul class='shipping_states'>",
                                    "<li><span class='shipping_state_name smaller bold'>",
                                    data.state,
                                    "</span>",
                                    "<table>"];
                                var row = renderRow(data.id, data.name, data.matchValue, data.cost);
                                var foot = ["</table>", "</li>", "</ul>", "</li>"];
                                var entry = head.concat(row).concat(foot).join('');

                                $('#shipping_countries').append(entry);
                            }
                            else {
                                // is there an existing state entry?
                                var stateentry = $(countryentry).siblings('.shipping_states').find(".shipping_state_name:contains('" + data.state + "')");
                                if (stateentry.length == 0) {
                                    // no state entry
                                    var head = ["<li>",
                                        "<span class='shipping_state_name smaller bold'>",
                                        data.state,
                                        "</span>",
                                        "<table>"];
                                    var row = renderRow(data.id, data.name, data.matchValue, data.cost);
                                    var foot = ["</table>", "</li>"];
                                    var entry = head.concat(row).concat(foot).join('');

                                    $(countryentry).siblings('.shipping_states').append(entry);
                                }
                                else {
                                    // append to existing state entry
                                    var entry = renderRow(data.id, data.name, data.matchValue, data.cost).join('');
                                    stateentry.next().append(entry);
                                }
                            }
                        }
                    }
                }
                else {
                    $.jGrowl(json_data.message);
                }
                $('#buttonSave', '#shippingAddForm').buttonEnable();
                return false;
            }
        });
        return false;
    });
</script>
