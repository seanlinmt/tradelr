<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Login.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    Setup Your Account
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="banner">
        <div class="content">
            <h1>
                Setup your account</h1>
        </div>
    </div>
    <form autocomplete="off" id="setupForm" action="<%= Url.Action("setup","register") %>" method="post">
    <div class="container_center">
        <h3 class="headingSetup">
            Take a minute to complete the following and start using your account</h3>
        <div class="section_header">
            Set Your Timezone
        </div>
        <div class="profile_others form_group">
            <div class="form_entry">
                <div class="form_label">
                    <label for="timezone" class="required">
                        Choose Your Timezone</label>
                </div>
                <select class="w450px" name="timezone" id="timezone">
                    <option value="">None</option>
                    <option value="Dateline Standard Time">(UTC-12:00) International Date Line West</option>
                    <option value="UTC-11">(UTC-11:00) Coordinated Universal Time-11</option>
                    <option value="Samoa Standard Time">(UTC-11:00) Samoa</option>
                    <option value="Hawaiian Standard Time">(UTC-10:00) Hawaii</option>
                    <option value="Alaskan Standard Time">(UTC-09:00) Alaska</option>
                    <option value="Pacific Standard Time (Mexico)">(UTC-08:00) Baja California</option>
                    <option value="Pacific Standard Time">(UTC-08:00) Pacific Time (US &amp; Canada)</option>
                    <option value="US Mountain Standard Time">(UTC-07:00) Arizona</option>
                    <option value="Mountain Standard Time (Mexico)">(UTC-07:00) Chihuahua, La Paz, Mazatlan</option>
                    <option value="Mountain Standard Time">(UTC-07:00) Mountain Time (US &amp; Canada)</option>
                    <option value="Central America Standard Time">(UTC-06:00) Central America</option>
                    <option value="Central Standard Time">(UTC-06:00) Central Time (US &amp; Canada)</option>
                    <option value="Central Standard Time (Mexico)">(UTC-06:00) Guadalajara, Mexico City,
                        Monterrey</option>
                    <option value="Canada Central Standard Time">(UTC-06:00) Saskatchewan</option>
                    <option value="SA Pacific Standard Time">(UTC-05:00) Bogota, Lima, Quito</option>
                    <option value="Eastern Standard Time" selected="selected">(UTC-05:00) Eastern Time (US
                        &amp; Canada)</option>
                    <option value="US Eastern Standard Time">(UTC-05:00) Indiana (East)</option>
                    <option value="Venezuela Standard Time">(UTC-04:30) Caracas</option>
                    <option value="Paraguay Standard Time">(UTC-04:00) Asuncion</option>
                    <option value="Atlantic Standard Time">(UTC-04:00) Atlantic Time (Canada)</option>
                    <option value="Central Brazilian Standard Time">(UTC-04:00) Cuiaba</option>
                    <option value="SA Western Standard Time">(UTC-04:00) Georgetown, La Paz, Manaus, San
                        Juan</option>
                    <option value="Pacific SA Standard Time">(UTC-04:00) Santiago</option>
                    <option value="Newfoundland Standard Time">(UTC-03:30) Newfoundland</option>
                    <option value="E. South America Standard Time">(UTC-03:00) Brasilia</option>
                    <option value="Argentina Standard Time">(UTC-03:00) Buenos Aires</option>
                    <option value="SA Eastern Standard Time">(UTC-03:00) Cayenne, Fortaleza</option>
                    <option value="Greenland Standard Time">(UTC-03:00) Greenland</option>
                    <option value="Montevideo Standard Time">(UTC-03:00) Montevideo</option>
                    <option value="UTC-02">(UTC-02:00) Coordinated Universal Time-02</option>
                    <option value="Mid-Atlantic Standard Time">(UTC-02:00) Mid-Atlantic</option>
                    <option value="Azores Standard Time">(UTC-01:00) Azores</option>
                    <option value="Cape Verde Standard Time">(UTC-01:00) Cape Verde Is.</option>
                    <option value="Morocco Standard Time">(UTC) Casablanca</option>
                    <option value="UTC">(UTC) Coordinated Universal Time</option>
                    <option value="GMT Standard Time">(UTC) Dublin, Edinburgh, Lisbon, London</option>
                    <option value="Greenwich Standard Time">(UTC) Monrovia, Reykjavik</option>
                    <option value="W. Europe Standard Time">(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm,
                        Vienna</option>
                    <option value="Central Europe Standard Time">(UTC+01:00) Belgrade, Bratislava, Budapest,
                        Ljubljana, Prague</option>
                    <option value="Romance Standard Time">(UTC+01:00) Brussels, Copenhagen, Madrid, Paris</option>
                    <option value="Central European Standard Time">(UTC+01:00) Sarajevo, Skopje, Warsaw,
                        Zagreb</option>
                    <option value="W. Central Africa Standard Time">(UTC+01:00) West Central Africa</option>
                    <option value="Jordan Standard Time">(UTC+02:00) Amman</option>
                    <option value="GTB Standard Time">(UTC+02:00) Athens, Bucharest, Istanbul</option>
                    <option value="Middle East Standard Time">(UTC+02:00) Beirut</option>
                    <option value="Egypt Standard Time">(UTC+02:00) Cairo</option>
                    <option value="Syria Standard Time">(UTC+02:00) Damascus</option>
                    <option value="South Africa Standard Time">(UTC+02:00) Harare, Pretoria</option>
                    <option value="FLE Standard Time">(UTC+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn,
                        Vilnius</option>
                    <option value="Israel Standard Time">(UTC+02:00) Jerusalem</option>
                    <option value="E. Europe Standard Time">(UTC+02:00) Minsk</option>
                    <option value="Namibia Standard Time">(UTC+02:00) Windhoek</option>
                    <option value="Arabic Standard Time">(UTC+03:00) Baghdad</option>
                    <option value="Arab Standard Time">(UTC+03:00) Kuwait, Riyadh</option>
                    <option value="Russian Standard Time">(UTC+03:00) Moscow, St. Petersburg, Volgograd</option>
                    <option value="E. Africa Standard Time">(UTC+03:00) Nairobi</option>
                    <option value="Iran Standard Time">(UTC+03:30) Tehran</option>
                    <option value="Arabian Standard Time">(UTC+04:00) Abu Dhabi, Muscat</option>
                    <option value="Azerbaijan Standard Time">(UTC+04:00) Baku</option>
                    <option value="Mauritius Standard Time">(UTC+04:00) Port Louis</option>
                    <option value="Georgian Standard Time">(UTC+04:00) Tbilisi</option>
                    <option value="Caucasus Standard Time">(UTC+04:00) Yerevan</option>
                    <option value="Afghanistan Standard Time">(UTC+04:30) Kabul</option>
                    <option value="Ekaterinburg Standard Time">(UTC+05:00) Ekaterinburg</option>
                    <option value="Pakistan Standard Time">(UTC+05:00) Islamabad, Karachi</option>
                    <option value="West Asia Standard Time">(UTC+05:00) Tashkent</option>
                    <option value="India Standard Time">(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi</option>
                    <option value="Sri Lanka Standard Time">(UTC+05:30) Sri Jayawardenepura</option>
                    <option value="Nepal Standard Time">(UTC+05:45) Kathmandu</option>
                    <option value="Central Asia Standard Time">(UTC+06:00) Astana</option>
                    <option value="Bangladesh Standard Time">(UTC+06:00) Dhaka</option>
                    <option value="N. Central Asia Standard Time">(UTC+06:00) Novosibirsk</option>
                    <option value="Myanmar Standard Time">(UTC+06:30) Yangon (Rangoon)</option>
                    <option value="SE Asia Standard Time">(UTC+07:00) Bangkok, Hanoi, Jakarta</option>
                    <option value="North Asia Standard Time">(UTC+07:00) Krasnoyarsk</option>
                    <option value="China Standard Time">(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi</option>
                    <option value="North Asia East Standard Time">(UTC+08:00) Irkutsk</option>
                    <option value="Singapore Standard Time">(UTC+08:00) Kuala Lumpur, Singapore</option>
                    <option value="W. Australia Standard Time">(UTC+08:00) Perth</option>
                    <option value="Taipei Standard Time">(UTC+08:00) Taipei</option>
                    <option value="Ulaanbaatar Standard Time">(UTC+08:00) Ulaanbaatar</option>
                    <option value="Tokyo Standard Time">(UTC+09:00) Osaka, Sapporo, Tokyo</option>
                    <option value="Korea Standard Time">(UTC+09:00) Seoul</option>
                    <option value="Yakutsk Standard Time">(UTC+09:00) Yakutsk</option>
                    <option value="Cen. Australia Standard Time">(UTC+09:30) Adelaide</option>
                    <option value="AUS Central Standard Time">(UTC+09:30) Darwin</option>
                    <option value="E. Australia Standard Time">(UTC+10:00) Brisbane</option>
                    <option value="AUS Eastern Standard Time">(UTC+10:00) Canberra, Melbourne, Sydney</option>
                    <option value="West Pacific Standard Time">(UTC+10:00) Guam, Port Moresby</option>
                    <option value="Tasmania Standard Time">(UTC+10:00) Hobart</option>
                    <option value="Vladivostok Standard Time">(UTC+10:00) Vladivostok</option>
                    <option value="Central Pacific Standard Time">(UTC+11:00) Magadan, Solomon Is., New
                        Caledonia</option>
                    <option value="New Zealand Standard Time">(UTC+12:00) Auckland, Wellington</option>
                    <option value="UTC+12">(UTC+12:00) Coordinated Universal Time+12</option>
                    <option value="Fiji Standard Time">(UTC+12:00) Fiji</option>
                    <option value="Kamchatka Standard Time">(UTC+12:00) Petropavlovsk-Kamchatsky - Old</option>
                    <option value="Tonga Standard Time">(UTC+13:00) Nuku'alofa</option>
                </select>
            </div>
        </div>
        <div class="section_header">
            Set Your Currency
        </div>
        <div class="profile_others form_group">
            <div class="form_entry">
                <div class="form_label">
                    <label for="currency" class="required">
                        Currency</label></div>
                <select class="w350px" name="currency" id="currency">
                    <option value="">None</option>
                    <option value="395">ANG Netherlands Antillean gulden</option>
                    <option value="6">ARS Argentine peso</option>
                    <option value="319">AUD Australian dollar</option>
                    <option value="318">AWG Aruban florin</option>
                    <option value="329">BAM Bosnia and Herzegovina konvertibilna marka</option>
                    <option value="322">BBD Barbadian dollar</option>
                    <option value="1">BHD Bahraini dinar</option>
                    <option value="333">BIF Burundi franc</option>
                    <option value="326">BMD Bermudian dollar</option>
                    <option value="332">BND Brunei dollar</option>
                    <option value="328">BOB Bolivian boliviano</option>
                    <option value="331">BRL Brazilian real</option>
                    <option value="321">BSD Bahamian dollar</option>
                    <option value="327">BTN Bhutanese ngultrum</option>
                    <option value="330">BWP Botswana pula</option>
                    <option value="323">BYR Belarusian ruble</option>
                    <option value="324">BZD Belize dollar</option>
                    <option value="335">CAD Canadian dollar</option>
                    <option value="341">CDF Congolese franc</option>
                    <option value="423">CHF Swiss franc</option>
                    <option value="338">CLP Chilean peso</option>
                    <option value="339">CNY Chinese renminbi</option>
                    <option value="340">COP Colombian peso</option>
                    <option value="342">CRC Costa Rican colon</option>
                    <option value="344">CUC Cuban peso</option>
                    <option value="336">CVE Cape Verdean escudo</option>
                    <option value="345">CZK Czech koruna</option>
                    <option value="347">DJF Djiboutian franc</option>
                    <option value="346">DKK Danish krone</option>
                    <option value="348">DOP Dominican peso</option>
                    <option value="352">EEK Estonian kroon</option>
                    <option value="349">EGP Egyptian pound</option>
                    <option value="351">ERN Eritrean nakfa</option>
                    <option value="353">ETB Ethiopian birr</option>
                    <option value="320">EUR European euro</option>
                    <option value="355">FJD Fijian dollar</option>
                    <option value="354">FKP Falkland Islands pound</option>
                    <option value="431">GBP British pound</option>
                    <option value="358">GIP Gibraltar pound</option>
                    <option value="357">GMD Gambian dalasi</option>
                    <option value="360">GNF Guinean franc</option>
                    <option value="350">GQE Central African CFA franc</option>
                    <option value="359">GTQ Guatemalan quetzal</option>
                    <option value="361">GYD Guyanese dollar</option>
                    <option value="364">HKD Hong Kong dollar</option>
                    <option value="363">HNL Honduran lempira</option>
                    <option value="343">HRK Croatian kuna</option>
                    <option value="362">HTG Haitian gourde</option>
                    <option value="365">HUF Hungarian forint</option>
                    <option value="368">IDR Indonesian rupiah</option>
                    <option value="441">ILS Israeli New Sheqel</option>
                    <option value="367">INR Indian rupee</option>
                    <option value="366">ISK Icelandic króna</option>
                    <option value="370">JMD Jamaican dollar</option>
                    <option value="2">JOD Jordanian dinar</option>
                    <option value="371">JPY Japanese yen</option>
                    <option value="373">KES Kenyan shilling</option>
                    <option value="3">KMF Comoro Franc</option>
                    <option value="374">KPW North Korean won</option>
                    <option value="375">KRW South Korean won</option>
                    <option value="4">KWD Kuwaiti Dinar</option>
                    <option value="337">KYD Cayman Islands dollar</option>
                    <option value="372">KZT Kazakhstani tenge</option>
                    <option value="376">LAK Lao kip</option>
                    <option value="418">LKR Sri Lankan rupee</option>
                    <option value="379">LRD Liberian dollar</option>
                    <option value="378">LSL Lesotho loti</option>
                    <option value="381">LTL Lithuanian litas</option>
                    <option value="377">LVL Latvian lats</option>
                    <option value="380">LYD Libyan dinar</option>
                    <option value="383">MGA Malagasy ariary</option>
                    <option value="392">MMK Myanma kyat</option>
                    <option value="390">MNT Mongolian tugrik</option>
                    <option value="382">MOP Macanese pataca</option>
                    <option value="387">MRO Mauritanian ouguiya</option>
                    <option value="388">MUR Mauritian rupee</option>
                    <option value="386">MVR Maldivian rufiyaa</option>
                    <option value="384">MWK Malawian kwacha</option>
                    <option value="389">MXN Mexican peso</option>
                    <option value="385">MYR Malaysian ringgit</option>
                    <option value="391">MZM Mozambican metical</option>
                    <option value="393">NAD Namibian dollar</option>
                    <option value="398">NGN Nigerian naira</option>
                    <option value="397">NIO Nicaraguan córdoba</option>
                    <option value="399">NOK Norwegian krone</option>
                    <option value="394">NPR Nepalese rupee</option>
                    <option value="396">NZD New Zealand dollar</option>
                    <option value="5">OMR Rial Omani</option>
                    <option value="401">PAB Panamanian balboa</option>
                    <option value="403">PEN Peruvian nuevo sol</option>
                    <option value="402">PGK Papua New Guinean kina</option>
                    <option value="404">PHP Philippine peso</option>
                    <option value="400">PKR Pakistani rupee</option>
                    <option value="7">PYG Paraguayan Guarani</option>
                    <option value="405">QAR Qatari riyal</option>
                    <option value="406">RON Romanian leu</option>
                    <option value="411">RSD Serbian dinar</option>
                    <option value="407">RUB Russian ruble</option>
                    <option value="408">RWF Rwandan franc</option>
                    <option value="410">SAR Saudi riyal</option>
                    <option value="415">SBD Solomon Islands dollar</option>
                    <option value="412">SCR Seychellois rupee</option>
                    <option value="422">SEK Swedish krona</option>
                    <option value="414">SGD Singapore dollar</option>
                    <option value="419">SHP Saint Helena pound</option>
                    <option value="413">SLL Sierra Leonean leone</option>
                    <option value="416">SOS Somali shilling</option>
                    <option value="420">SRD Surinamese dollar</option>
                    <option value="409">STD São Tomé and Príncipe dobra</option>
                    <option value="421">SZL Swazi lilangeni</option>
                    <option value="425">THB Thai baht</option>
                    <option value="429">TMT Turkmen manat</option>
                    <option value="427">TND Tunisian dinar</option>
                    <option value="428">TRY Turkish new lira</option>
                    <option value="426">TTD Trinidad and Tobago dollar</option>
                    <option value="424">TWD New Taiwan dollar</option>
                    <option value="430">UGX Ugandan shilling</option>
                    <option value="432">USD United States dollar</option>
                    <option value="433">UYU Uruguayan peso</option>
                    <option value="435">VEB Venezuelan bolivar</option>
                    <option value="436">VND Vietnamese dong</option>
                    <option value="434">VUV Vanuatu vatu</option>
                    <option value="437">WST Samoan tala</option>
                    <option value="334">XAF Central African CFA franc</option>
                    <option value="317">XCD East Caribbean dollar</option>
                    <option value="369">XDR Special Drawing Rights</option>
                    <option value="325">XOF West African CFA franc</option>
                    <option value="356">XPF CFP franc</option>
                    <option value="417">ZAR South African rand</option>
                    <option value="438">ZMK Zambian kwacha</option>
                    <option value="439">ZWR Zimbabwean dollar</option>
                </select>
            </div>
        </div>
        <div class="section_header">
            Enter Organization Information</div>
        <table>
            <tr>
                <td>
                    <div id="profile_address" class="form_group">
                        <div class="form_entry">
                            <div class="form_label">
                                <label for="organisation" class="required">
                                    Business Name</label>
                            </div>
                            <%= Html.TextBox("organisation","", new Dictionary<string, object>{{"style","width:300px"}})%>
                        </div>
                        <% Html.RenderPartial("~/Views/contacts/userLocation.ascx", "#setupForm"); %>
                        <div class="form_entry" style="width: 377px">
                            <div class="form_label">
                                <label for="address" class="required">
                                    Street Address</label>
                            </div>
                            <%= Html.TextArea("address","", new Dictionary<string, object>{{"style","width:100%"}})%>
                        </div>
                        <div>
                            <div class="fl">
                                <div class="form_entry w200px">
                                    <div class="form_label">
                                        <label for="city" class="required">
                                            City</label>
                                    </div>
                                    <%= Html.TextBox("city")%>
                                    <%= Html.Hidden("citySelected")%>
                                </div>
                            </div>
                            <div class="fl pl10">
                                <div class="form_entry w200px">
                                    <div class="form_label">
                                        <label for="postcode" class="required">
                                            Postal/Zip Code</label>
                                    </div>
                                    <%= Html.TextBox("postcode")%>
                                </div>
                            </div>
                            <div class="clear">
                            </div>
                        </div>
                        <div class="form_entry">
                            <div class="form_label">
                                <label for="phone">
                                    Phone number</label></div>
                            <%= Html.TextBox("phone")%>
                        </div>
                    </div>
                </td>
                <td>
                    <div id="address_help" class="bg_blue help_message">
                        Your organization info will <strong>appear in the invoices and purchase orders</strong>
                        that you create.
                        <br />
                        <div class="profile_invoice">
                            <img alt="" src="/Content/img/invoice.gif" /></div>
                    </div>
                </td>
            </tr>
        </table>
        <div class="section_header">
            Terms of Service and Privacy Policy</div>
        <div class="profile_others form_group">
            <p>
                Before you complete your setup, please review our <a href="/terms" target="_blank">terms
                    of service</a> and <a href="/privacy" target="_blank">privacy policy</a>.</p>
            <div class="mt10 relative">
                <label for="tos" class="pl4">
                    Yes, I agree to the terms of service and privacy policy</label>
                <input class="fl mt6" type="checkbox" name="tos" id="tos" />
            </div>
        </div>
        <div class="buttonRow">
            <button id="buttonStart" type="button" class="green large ajax">
                Start Now!</button>
        </div>
    </div>
    </form>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
    <%= Html.RegisterViewJS() %>
</asp:Content>
