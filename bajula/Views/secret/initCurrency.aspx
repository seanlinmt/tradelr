<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    initCurrency
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="currencyAddForm" action="/dashboard/addCurrencies" method="post">
   
    <button type="submit">
        Import All</button>
</form>

    <script type="text/javascript">
        $('#currencyAddForm').submit(function() {
            var action = $(this).attr("action");

            // get all info
            var data = [];
            var rows = $('table > tbody').find('tr');
            $.each(rows, function(i, val) {
                var cols = $(this).find('td');
                if (cols.length == 4) {
                    var name = $(cols[1]).text().trim();
                    var code = $(cols[2]).text().trim();
                    var symbol = $(cols[3]).text().trim();
                    if (currency !== "" && code !== "" && symbol !== "") {
                        var currency = new Object();
                        currency.name = name;
                        currency.code = code;
                        currency.symbol = symbol;
                        data.push(currency);
                    }
                }


            });
            var encoded = $.toJSON(data);

            // post form
            $.ajax({
                contentType: "application/json",
                type: "POST",
                url: action,
                dataType: "json",
                data: encoded,
                success: function(json_data) {
                    if (json_data.success) {
                        $.jGrowl("Done");
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                    return false;
                }
            });
            return false;
        });
    </script>

    <table width="100%" cellspacing="1" cellpadding="1" bgcolor="black">
        <thead>
            <tr bgcolor="#aaaaff">
                <th width="200" align="left">
                    Country
                </th>
                <th align="left">
                    Currency
                </th>
                <th width="75" align="center">
                    ISO-4217
                </th>
                <th width="45" align="center">
                    Symbol
                </th>
            </tr>
        </thead>
        <tbody>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">A</font>
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Afghanistan
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Afghan_afghani">Afghan afghani</a>
                </td>
                <td align="center">
                    <tt>AFN</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Albania
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Albanian_lek">Albanian lek</a>
                </td>
                <td align="center">
                    <tt>ALL</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Algeria
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Algerian_dinar">Algerian dinar</a>
                </td>
                <td align="center">
                    <tt>DZD</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    American Samoa
                </td>
                <td align="left" colspan="3">
                    see United States
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Andorra
                </td>
                <td align="left" colspan="3">
                    see Spain and France
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Angola
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Angolan_kwanza">Angolan kwanza</a>
                </td>
                <td align="center">
                    <tt>AOA</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Anguilla
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/East_Caribbean_dollar">East Caribbean dollar</a>
                </td>
                <td align="center">
                    <tt>XCD</tt>
                </td>
                <td align="center">
                    EC$
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Antigua and Barbuda
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/East_Caribbean_dollar">East Caribbean dollar</a>
                </td>
                <td align="center">
                    <tt>XCD</tt>
                </td>
                <td align="center">
                    EC$
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Argentina
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Argentine_peso">Argentine peso</a>
                </td>
                <td align="center">
                    <tt>ARS</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Armenia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Armenian_dram">Armenian dram</a>
                </td>
                <td align="center">
                    <tt>AMD</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Aruba
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Aruban_florin">Aruban florin</a>
                </td>
                <td align="center">
                    <tt>AWG</tt>
                </td>
                <td align="center">
                    ƒ
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Australia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Australian_dollar">Australian dollar</a>
                </td>
                <td align="center">
                    <tt>AUD</tt>
                </td>
                <td align="center">
                    $
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Austria
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/European_euro">European euro</a>
                </td>
                <td align="center">
                    <tt>EUR</tt>
                </td>
                <td align="center">
                    €
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Azerbaijan
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Azerbaijani_manat">Azerbaijani manat</a>
                </td>
                <td align="center">
                    <tt>AZN</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">B</font>
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Bahamas
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Bahamian_dollar">Bahamian dollar</a>
                </td>
                <td align="center">
                    <tt>BSD</tt>
                </td>
                <td align="center">
                    B$
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Bahrain
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Bahraini_dinar">Bahraini dinar</a>
                </td>
                <td align="center">
                    <tt>BHD</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Bangladesh
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Bangladeshi_taka">Bangladeshi taka</a>
                </td>
                <td align="center">
                    <tt>BDT</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Barbados
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Barbadian_dollar">Barbadian dollar</a>
                </td>
                <td align="center">
                    <tt>BBD</tt>
                </td>
                <td align="center">
                    Bds$
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Belarus
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Belarusian_ruble">Belarusian ruble</a>
                </td>
                <td align="center">
                    <tt>BYR</tt>
                </td>
                <td align="center">
                    Br
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Belgium
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/European_euro">European euro</a>
                </td>
                <td align="center">
                    <tt>EUR</tt>
                </td>
                <td align="center">
                    €
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Belize
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Belize_dollar">Belize dollar</a>
                </td>
                <td align="center">
                    <tt>BZD</tt>
                </td>
                <td align="center">
                    BZ$
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Benin
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/West_African_CFA_franc">West African CFA franc</a>
                </td>
                <td align="center">
                    <tt>XOF</tt>
                </td>
                <td align="center">
                    CFA
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Bermuda
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Bermudian_dollar">Bermudian dollar</a>
                </td>
                <td align="center">
                    <tt>BMD</tt>
                </td>
                <td align="center">
                    BD$
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Bhutan
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Bhutanese_ngultrum">Bhutanese ngultrum</a>
                </td>
                <td align="center">
                    <tt>BTN</tt>
                </td>
                <td align="center">
                    Nu.
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Bolivia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Bolivian_boliviano">Bolivian boliviano</a>
                </td>
                <td align="center">
                    <tt>BOB</tt>
                </td>
                <td align="center">
                    Bs.
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Bosnia-Herzegovina
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Bosnia_and_Herzegovina_konvertibilna_marka">Bosnia and Herzegovina konvertibilna marka</a>
                </td>
                <td align="center">
                    <tt>BAM</tt>
                </td>
                <td align="center">
                    KM
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Botswana
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Botswana_pula">Botswana pula</a>
                </td>
                <td align="center">
                    <tt>BWP</tt>
                </td>
                <td align="center">
                    P
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Brazil
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Brazilian_real">Brazilian real</a>
                </td>
                <td align="center">
                    <tt>BRL</tt>
                </td>
                <td align="center">
                    R$
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    British Indian Ocean Territory
                </td>
                <td align="left" colspan="3">
                    see United Kingdom
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Brunei
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Brunei_dollar">Brunei dollar</a>
                </td>
                <td align="center">
                    <tt>BND</tt>
                </td>
                <td align="center">
                    B$
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Bulgaria
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Bulgarian_lev">Bulgarian lev</a>
                </td>
                <td align="center">
                    <tt>BGN</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Burkina Faso
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/West_African_CFA_franc">West African CFA franc</a>
                </td>
                <td align="center">
                    <tt>XOF</tt>
                </td>
                <td align="center">
                    CFA
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Burma
                </td>
                <td align="left" colspan="3">
                    see Myanmar
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Burundi
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Burundi_franc">Burundi franc</a>
                </td>
                <td align="center">
                    <tt>BIF</tt>
                </td>
                <td align="center">
                    FBu
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">C</font>
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Cambodia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Cambodian_riel">Cambodian riel</a>
                </td>
                <td align="center">
                    <tt>KHR</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Cameroon
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Central_African_CFA_franc">Central African CFA
                        franc</a>
                </td>
                <td align="center">
                    <tt>XAF</tt>
                </td>
                <td align="center">
                    CFA
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Canada
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Canadian_dollar">Canadian dollar</a>
                </td>
                <td align="center">
                    <tt>CAD</tt>
                </td>
                <td align="center">
                    $
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Canton and Enderbury Islands
                </td>
                <td align="left" colspan="3">
                    see Kiribati
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Cape Verde
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Cape_Verdean_escudo">Cape Verdean escudo</a>
                </td>
                <td align="center">
                    <tt>CVE</tt>
                </td>
                <td align="center">
                    Esc
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Cayman Islands
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Cayman_Islands_dollar">Cayman Islands dollar</a>
                </td>
                <td align="center">
                    <tt>KYD</tt>
                </td>
                <td align="center">
                    KY$
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Central African Republic
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Central_African_CFA_franc">Central African CFA
                        franc</a>
                </td>
                <td align="center">
                    <tt>XAF</tt>
                </td>
                <td align="center">
                    CFA
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Chad
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Central_African_CFA_franc">Central African CFA
                        franc</a>
                </td>
                <td align="center">
                    <tt>XAF</tt>
                </td>
                <td align="center">
                    CFA
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Chile
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Chilean_peso">Chilean peso</a>
                </td>
                <td align="center">
                    <tt>CLP</tt>
                </td>
                <td align="center">
                    $
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    China
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Chinese_renminbi">Chinese renminbi</a>
                </td>
                <td align="center">
                    <tt>CNY</tt>
                </td>
                <td align="center">
                    ¥
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Christmas Island
                </td>
                <td align="left" colspan="3">
                    see Australia
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Cocos (Keeling) Islands
                </td>
                <td align="left" colspan="3">
                    see Australia
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Colombia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Colombian_peso">Colombian peso</a>
                </td>
                <td align="center">
                    <tt>COP</tt>
                </td>
                <td align="center">
                    Col$
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Comoros
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Comorian_franc">Comorian franc</a>
                </td>
                <td align="center">
                    <tt>KMF</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Congo
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Central_African_CFA_franc">Central African CFA
                        franc</a>
                </td>
                <td align="center">
                    <tt>XAF</tt>
                </td>
                <td align="center">
                    CFA
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Congo, Democratic Republic
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Congolese_franc">Congolese franc</a>
                </td>
                <td align="center">
                    <tt>CDF</tt>
                </td>
                <td align="center">
                    F
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Cook Islands
                </td>
                <td align="left" colspan="3">
                    see New Zealand
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Costa Rica
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Costa_Rican_colon">Costa Rican colon</a>
                </td>
                <td align="center">
                    <tt>CRC</tt>
                </td>
                <td align="center">
                    ₡
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Côte d'Ivoire
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/West_African_CFA_franc">West African CFA franc</a>
                </td>
                <td align="center">
                    <tt>XOF</tt>
                </td>
                <td align="center">
                    CFA
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Croatia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Croatian_kuna">Croatian kuna</a>
                </td>
                <td align="center">
                    <tt>HRK</tt>
                </td>
                <td align="center">
                    kn
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Cuba
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Cuban_peso">Cuban peso</a>
                </td>
                <td align="center">
                    <tt>CUC</tt>
                </td>
                <td align="center">
                    $
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Cyprus
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/European_euro">European euro</a>
                </td>
                <td align="center">
                    <tt>EUR</tt>
                </td>
                <td align="center">
                    €
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Czech Republic
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Czech_koruna">Czech koruna</a>
                </td>
                <td align="center">
                    <tt>CZK</tt>
                </td>
                <td align="center">
                    Kč
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">D</font>
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Denmark
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Danish_krone">Danish krone</a>
                </td>
                <td align="center">
                    <tt>DKK</tt>
                </td>
                <td align="center">
                    Kr
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Djibouti
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Djiboutian_franc">Djiboutian franc</a>
                </td>
                <td align="center">
                    <tt>DJF</tt>
                </td>
                <td align="center">
                    Fdj
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Dominica
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/East_Caribbean_dollar">East Caribbean dollar</a>
                </td>
                <td align="center">
                    <tt>XCD</tt>
                </td>
                <td align="center">
                    EC$
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Dominican Republic
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Dominican_peso">Dominican peso</a>
                </td>
                <td align="center">
                    <tt>DOP</tt>
                </td>
                <td align="center">
                    RD$
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Dronning Maud Land
                </td>
                <td align="left" colspan="3">
                    see Norway
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">E</font>
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    East Timor
                </td>
                <td align="left" colspan="3">
                    see Timor-Leste
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Ecuador
                </td>
                <td align="left" colspan="3">
                    uses the U.S. Dollar
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Egypt
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Egyptian_pound">Egyptian pound</a>
                </td>
                <td align="center">
                    <tt>EGP</tt>
                </td>
                <td align="center">
                    £
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    El Salvador
                </td>
                <td align="left" colspan="3">
                    uses the U.S. Dollar
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Equatorial Guinea
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Central_African_CFA_franc">Central African CFA
                        franc</a>
                </td>
                <td align="center">
                    <tt>GQE</tt>
                </td>
                <td align="center">
                    CFA
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Eritrea
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Eritrean_nakfa">Eritrean nakfa</a>
                </td>
                <td align="center">
                    <tt>ERN</tt>
                </td>
                <td align="center">
                    Nfa
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Estonia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Estonian_kroon">Estonian kroon</a>
                </td>
                <td align="center">
                    <tt>EEK</tt>
                </td>
                <td align="center">
                    KR
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Ethiopia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Ethiopian_birr">Ethiopian birr</a>
                </td>
                <td align="center">
                    <tt>ETB</tt>
                </td>
                <td align="center">
                    Br
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">F</font>
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Faeroe Islands (Føroyar)
                </td>
                <td align="left" colspan="3">
                    see Denmark
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Falkland Islands
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Falkland_Islands_pound">Falkland Islands pound</a>
                </td>
                <td align="center">
                    <tt>FKP</tt>
                </td>
                <td align="center">
                    £
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Fiji
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Fijian_dollar">Fijian dollar</a>
                </td>
                <td align="center">
                    <tt>FJD</tt>
                </td>
                <td align="center">
                    FJ$
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Finland
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/European_euro">European euro</a>
                </td>
                <td align="center">
                    <tt>EUR</tt>
                </td>
                <td align="center">
                    €
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    France
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/European_euro">European euro</a>
                </td>
                <td align="center">
                    <tt>EUR</tt>
                </td>
                <td align="center">
                    €
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    French Guiana
                </td>
                <td align="left" colspan="3">
                    see France
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    French Polynesia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/CFP_franc">CFP franc</a>
                </td>
                <td align="center">
                    <tt>XPF</tt>
                </td>
                <td align="center">
                    F
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">G</font>
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Gabon
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Central_African_CFA_franc">Central African CFA
                        franc</a>
                </td>
                <td align="center">
                    <tt>XAF</tt>
                </td>
                <td align="center">
                    CFA
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Gambia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Gambian_dalasi">Gambian dalasi</a>
                </td>
                <td align="center">
                    <tt>GMD</tt>
                </td>
                <td align="center">
                    D
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Georgia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Georgian_lari">Georgian lari</a>
                </td>
                <td align="center">
                    <tt>GEL</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Germany
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/European_euro">European euro</a>
                </td>
                <td align="center">
                    <tt>EUR</tt>
                </td>
                <td align="center">
                    €
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Ghana
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Ghanaian_cedi">Ghanaian cedi</a>
                </td>
                <td align="center">
                    <tt>GHS</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Gibraltar
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Gibraltar_pound">Gibraltar pound</a>
                </td>
                <td align="center">
                    <tt>GIP</tt>
                </td>
                <td align="center">
                    £
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Great Britain
                </td>
                <td align="left" colspan="3">
                    see United Kingdom
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Greece
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/European_euro">European euro</a>
                </td>
                <td align="center">
                    <tt>EUR</tt>
                </td>
                <td align="center">
                    €
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Greenland
                </td>
                <td align="left" colspan="3">
                    see Denmark
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Grenada
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/East_Caribbean_dollar">East Caribbean dollar</a>
                </td>
                <td align="center">
                    <tt>XCD</tt>
                </td>
                <td align="center">
                    EC$
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Guadeloupe
                </td>
                <td align="left" colspan="3">
                    see France
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Guam
                </td>
                <td align="left" colspan="3">
                    see United States
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Guatemala
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Guatemalan_quetzal">Guatemalan quetzal</a>
                </td>
                <td align="center">
                    <tt>GTQ</tt>
                </td>
                <td align="center">
                    Q
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Guernsey
                </td>
                <td align="left" colspan="3">
                    see United Kingdom
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Guinea-Bissau
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/West_African_CFA_franc">West African CFA franc</a>
                </td>
                <td align="center">
                    <tt>XOF</tt>
                </td>
                <td align="center">
                    CFA
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Guinea
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Guinean_franc">Guinean franc</a>
                </td>
                <td align="center">
                    <tt>GNF</tt>
                </td>
                <td align="center">
                    FG
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Guyana
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Guyanese_dollar">Guyanese dollar</a>
                </td>
                <td align="center">
                    <tt>GYD</tt>
                </td>
                <td align="center">
                    GY$
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">H</font>
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Haiti
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Haitian_gourde">Haitian gourde</a>
                </td>
                <td align="center">
                    <tt>HTG</tt>
                </td>
                <td align="center">
                    G
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Heard and McDonald Islands
                </td>
                <td align="left" colspan="3">
                    see Australia
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Honduras
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Honduran_lempira">Honduran lempira</a>
                </td>
                <td align="center">
                    <tt>HNL</tt>
                </td>
                <td align="center">
                    L
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Hong Kong
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Hong_Kong_dollar">Hong Kong dollar</a>
                </td>
                <td align="center">
                    <tt>HKD</tt>
                </td>
                <td align="center">
                    HK$
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Hungary
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Hungarian_forint">Hungarian forint</a>
                </td>
                <td align="center">
                    <tt>HUF</tt>
                </td>
                <td align="center">
                    Ft
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">I</font>
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Iceland
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Icelandic_kr%C3%B3na">Icelandic króna</a>
                </td>
                <td align="center">
                    <tt>ISK</tt>
                </td>
                <td align="center">
                    kr
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    India
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Indian_rupee">Indian rupee</a>
                </td>
                <td align="center">
                    <tt>INR</tt>
                </td>
                <td align="center">
                    Rs
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Indonesia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Indonesian_rupiah">Indonesian rupiah</a>
                </td>
                <td align="center">
                    <tt>IDR</tt>
                </td>
                <td align="center">
                    Rp
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    International Monetary Fund
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Special_Drawing_Rights">Special Drawing Rights</a>
                </td>
                <td align="center">
                    <tt>XDR</tt>
                </td>
                <td align="center">
                    SDR
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Iran
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Iranian_rial">Iranian rial</a>
                </td>
                <td align="center">
                    <tt>IRR</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Iraq
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Iraqi_dinar">Iraqi dinar</a>
                </td>
                <td align="center">
                    <tt>IQD</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Ireland
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/European_euro">European euro</a>
                </td>
                <td align="center">
                    <tt>EUR</tt>
                </td>
                <td align="center">
                    €
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Isle of Man
                </td>
                <td align="left" colspan="3">
                    see United Kingdom
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Israel
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Israeli_new_sheqel">Israeli new sheqel</a>
                </td>
                <td align="center">
                    <tt>ILS</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Italy
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/European_euro">European euro</a>
                </td>
                <td align="center">
                    <tt>EUR</tt>
                </td>
                <td align="center">
                    €
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Ivory Coast
                </td>
                <td align="left" colspan="3">
                    see Côte d'Ivoire
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">J</font>
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Jamaica
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Jamaican_dollar">Jamaican dollar</a>
                </td>
                <td align="center">
                    <tt>JMD</tt>
                </td>
                <td align="center">
                    J$
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Japan
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Japanese_yen">Japanese yen</a>
                </td>
                <td align="center">
                    <tt>JPY</tt>
                </td>
                <td align="center">
                    ¥
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Jersey
                </td>
                <td align="left" colspan="3">
                    see United Kingdom
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Johnston Island
                </td>
                <td align="left" colspan="3">
                    see United States
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Jordan
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Jordanian_dinar">Jordanian dinar</a>
                </td>
                <td align="center">
                    <tt>JOD</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">K</font>
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Kampuchea
                </td>
                <td align="left" colspan="3">
                    see Cambodia
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Kazakhstan
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Kazakhstani_tenge">Kazakhstani tenge</a>
                </td>
                <td align="center">
                    <tt>KZT</tt>
                </td>
                <td align="center">
                    T
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Kenya
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Kenyan_shilling">Kenyan shilling</a>
                </td>
                <td align="center">
                    <tt>KES</tt>
                </td>
                <td align="center">
                    KSh
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Kiribati
                </td>
                <td align="left" colspan="3">
                    see Australia
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Korea, North
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/North_Korean_won">North Korean won</a>
                </td>
                <td align="center">
                    <tt>KPW</tt>
                </td>
                <td align="center">
                    W
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Korea, South
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/South_Korean_won">South Korean won</a>
                </td>
                <td align="center">
                    <tt>KRW</tt>
                </td>
                <td align="center">
                    W
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Kuwait
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Kuwaiti_dinar">Kuwaiti dinar</a>
                </td>
                <td align="center">
                    <tt>KWD</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Kyrgyzstan
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Kyrgyzstani_som">Kyrgyzstani som</a>
                </td>
                <td align="center">
                    <tt>KGS</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">L</font>
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Laos
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Lao_kip">Lao kip</a>
                </td>
                <td align="center">
                    <tt>LAK</tt>
                </td>
                <td align="center">
                    KN
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Latvia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Latvian_lats">Latvian lats</a>
                </td>
                <td align="center">
                    <tt>LVL</tt>
                </td>
                <td align="center">
                    Ls
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Lebanon
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Lebanese_lira">Lebanese lira</a>
                </td>
                <td align="center">
                    <tt>LBP</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Lesotho
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Lesotho_loti">Lesotho loti</a>
                </td>
                <td align="center">
                    <tt>LSL</tt>
                </td>
                <td align="center">
                    M
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Liberia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Liberian_dollar">Liberian dollar</a>
                </td>
                <td align="center">
                    <tt>LRD</tt>
                </td>
                <td align="center">
                    L$
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Libya
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Libyan_dinar">Libyan dinar</a>
                </td>
                <td align="center">
                    <tt>LYD</tt>
                </td>
                <td align="center">
                    LD
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Liechtenstein
                </td>
                <td align="left" colspan="3">
                    uses the Swiss Franc
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Lithuania
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Lithuanian_litas">Lithuanian litas</a>
                </td>
                <td align="center">
                    <tt>LTL</tt>
                </td>
                <td align="center">
                    Lt
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Luxembourg
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/European_euro">European euro</a>
                </td>
                <td align="center">
                    <tt>EUR</tt>
                </td>
                <td align="center">
                    €
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">M</font>
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Macau
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Macanese_pataca">Macanese pataca</a>
                </td>
                <td align="center">
                    <tt>MOP</tt>
                </td>
                <td align="center">
                    P
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Macedonia (Former Yug. Rep.)
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Macedonian_denar">Macedonian denar</a>
                </td>
                <td align="center">
                    <tt>MKD</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Madagascar
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Malagasy_ariary">Malagasy ariary</a>
                </td>
                <td align="center">
                    <tt>MGA</tt>
                </td>
                <td align="center">
                    FMG
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Malawi
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Malawian_kwacha">Malawian kwacha</a>
                </td>
                <td align="center">
                    <tt>MWK</tt>
                </td>
                <td align="center">
                    MK
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Malaysia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Malaysian_ringgit">Malaysian ringgit</a>
                </td>
                <td align="center">
                    <tt>MYR</tt>
                </td>
                <td align="center">
                    RM
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Maldives
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Maldivian_rufiyaa">Maldivian rufiyaa</a>
                </td>
                <td align="center">
                    <tt>MVR</tt>
                </td>
                <td align="center">
                    Rf
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Mali
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/West_African_CFA_franc">West African CFA franc</a>
                </td>
                <td align="center">
                    <tt>XOF</tt>
                </td>
                <td align="center">
                    CFA
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Malta
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/European_Euro">European Euro</a>
                </td>
                <td align="center">
                    <tt>EUR</tt>
                </td>
                <td align="center">
                    €
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Martinique
                </td>
                <td align="left" colspan="3">
                    see France
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Mauritania
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Mauritanian_ouguiya">Mauritanian ouguiya</a>
                </td>
                <td align="center">
                    <tt>MRO</tt>
                </td>
                <td align="center">
                    UM
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Mauritius
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Mauritian_rupee">Mauritian rupee</a>
                </td>
                <td align="center">
                    <tt>MUR</tt>
                </td>
                <td align="center">
                    Rs
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Mayotte
                </td>
                <td align="left" colspan="3">
                    see France
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Micronesia
                </td>
                <td align="left" colspan="3">
                    see United States
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Midway Islands
                </td>
                <td align="left" colspan="3">
                    see United States
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Mexico
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Mexican_peso">Mexican peso</a>
                </td>
                <td align="center">
                    <tt>MXN</tt>
                </td>
                <td align="center">
                    $
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Moldova
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Moldovan_leu">Moldovan leu</a>
                </td>
                <td align="center">
                    <tt>MDL</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Monaco
                </td>
                <td align="left" colspan="3">
                    see France
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Mongolia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Mongolian_tugrik">Mongolian tugrik</a>
                </td>
                <td align="center">
                    <tt>MNT</tt>
                </td>
                <td align="center">
                    ₮
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Montenegro
                </td>
                <td align="left" colspan="3">
                    see Italy
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Montserrat
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/East_Caribbean_dollar">East Caribbean dollar</a>
                </td>
                <td align="center">
                    <tt>XCD</tt>
                </td>
                <td align="center">
                    EC$
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Morocco
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Moroccan_dirham">Moroccan dirham</a>
                </td>
                <td align="center">
                    <tt>MAD</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Mozambique
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Mozambican_metical">Mozambican metical</a>
                </td>
                <td align="center">
                    <tt>MZM</tt>
                </td>
                <td align="center">
                    MTn
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Myanmar
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Myanma_kyat">Myanma kyat</a>
                </td>
                <td align="center">
                    <tt>MMK</tt>
                </td>
                <td align="center">
                    K
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">N</font>
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Nauru
                </td>
                <td align="left" colspan="3">
                    see Australia
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Namibia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Namibian_dollar">Namibian dollar</a>
                </td>
                <td align="center">
                    <tt>NAD</tt>
                </td>
                <td align="center">
                    N$
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Nepal
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Nepalese_rupee">Nepalese rupee</a>
                </td>
                <td align="center">
                    <tt>NPR</tt>
                </td>
                <td align="center">
                    NRs
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Netherlands Antilles
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Netherlands_Antillean_gulden">Netherlands Antillean
                        gulden</a>
                </td>
                <td align="center">
                    <tt>ANG</tt>
                </td>
                <td align="center">
                    NAƒ
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Netherlands
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/European_euro">European euro</a>
                </td>
                <td align="center">
                    <tt>EUR</tt>
                </td>
                <td align="center">
                    €
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    New Caledonia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/CFP_franc">CFP franc</a>
                </td>
                <td align="center">
                    <tt>XPF</tt>
                </td>
                <td align="center">
                    F
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    New Zealand
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/New_Zealand_dollar">New Zealand dollar</a>
                </td>
                <td align="center">
                    <tt>NZD</tt>
                </td>
                <td align="center">
                    NZ$
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Nicaragua
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Nicaraguan_c%C3%B3rdoba">Nicaraguan córdoba</a>
                </td>
                <td align="center">
                    <tt>NIO</tt>
                </td>
                <td align="center">
                    C$
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Niger
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/West_African_CFA_franc">West African CFA franc</a>
                </td>
                <td align="center">
                    <tt>XOF</tt>
                </td>
                <td align="center">
                    CFA
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Nigeria
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Nigerian_naira">Nigerian naira</a>
                </td>
                <td align="center">
                    <tt>NGN</tt>
                </td>
                <td align="center">
                    ₦
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Niue
                </td>
                <td align="left" colspan="3">
                    see New Zealand
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Norfolk Island
                </td>
                <td align="left" colspan="3">
                    see Australia
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Northern Mariana Islands
                </td>
                <td align="left" colspan="3">
                    see United States
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Norway
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Norwegian_krone">Norwegian krone</a>
                </td>
                <td align="center">
                    <tt>NOK</tt>
                </td>
                <td align="center">
                    kr
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">O</font>
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Oman
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Omani_rial">Omani rial</a>
                </td>
                <td align="center">
                    <tt>OMR</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">P</font>
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Pakistan
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Pakistani_rupee">Pakistani rupee</a>
                </td>
                <td align="center">
                    <tt>PKR</tt>
                </td>
                <td align="center">
                    Rs.
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Palau
                </td>
                <td align="left" colspan="3">
                    see United States
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Panama
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Panamanian_balboa">Panamanian balboa</a>
                </td>
                <td align="center">
                    <tt>PAB</tt>
                </td>
                <td align="center">
                    B./
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Papua New Guinea
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Papua_New_Guinean_kina">Papua New Guinean kina</a>
                </td>
                <td align="center">
                    <tt>PGK</tt>
                </td>
                <td align="center">
                    K
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Paraguay
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Paraguayan_guarani">Paraguayan guarani</a>
                </td>
                <td align="center">
                    <tt>PYG</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Peru
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Peruvian_nuevo_sol">Peruvian nuevo sol</a>
                </td>
                <td align="center">
                    <tt>PEN</tt>
                </td>
                <td align="center">
                    S/.
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Philippines
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Philippine_peso">Philippine peso</a>
                </td>
                <td align="center">
                    <tt>PHP</tt>
                </td>
                <td align="center">
                    ₱
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Pitcairn Island
                </td>
                <td align="left" colspan="3">
                    see New Zealand
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Poland
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Polish_zloty">Polish zloty</a>
                </td>
                <td align="center">
                    <tt>PLN</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Portugal
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/European_euro">European euro</a>
                </td>
                <td align="center">
                    <tt>EUR</tt>
                </td>
                <td align="center">
                    €
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Puerto Rico
                </td>
                <td align="left" colspan="3">
                    see United States
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">Q</font>
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Qatar
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Qatari_riyal">Qatari riyal</a>
                </td>
                <td align="center">
                    <tt>QAR</tt>
                </td>
                <td align="center">
                    QR
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">R</font>
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Reunion
                </td>
                <td align="left" colspan="3">
                    see France
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Romania
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Romanian_leu">Romanian leu</a>
                </td>
                <td align="center">
                    <tt>RON</tt>
                </td>
                <td align="center">
                    L
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Russia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Russian_ruble">Russian ruble</a>
                </td>
                <td align="center">
                    <tt>RUB</tt>
                </td>
                <td align="center">
                    R
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Rwanda
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Rwandan_franc">Rwandan franc</a>
                </td>
                <td align="center">
                    <tt>RWF</tt>
                </td>
                <td align="center">
                    RF
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">S</font>
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Samoa (Western)
                </td>
                <td align="left" colspan="3">
                    see Western Samoa
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Samoa (America)
                </td>
                <td align="left" colspan="3">
                    see United States
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    San Marino
                </td>
                <td align="left" colspan="3">
                    see Italy
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    São Tomé and Príncipe
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/S%C3%A3o_Tom%C3%A9_and_Pr%C3%ADncipe_dobra">São Tomé and Príncipe dobra</a>
                </td>
                <td align="center">
                    <tt>STD</tt>
                </td>
                <td align="center">
                    Db
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Saudi Arabia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Saudi_riyal">Saudi riyal</a>
                </td>
                <td align="center">
                    <tt>SAR</tt>
                </td>
                <td align="center">
                    SR
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Sénégal
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/West_African_CFA_franc">West African CFA franc</a>
                </td>
                <td align="center">
                    <tt>XOF</tt>
                </td>
                <td align="center">
                    CFA
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Serbia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Serbian_dinar">Serbian dinar</a>
                </td>
                <td align="center">
                    <tt>RSD</tt>
                </td>
                <td align="center">
                    din.
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Seychelles
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Seychellois_rupee">Seychellois rupee</a>
                </td>
                <td align="center">
                    <tt>SCR</tt>
                </td>
                <td align="center">
                    SR
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Sierra Leone
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Sierra_Leonean_leone">Sierra Leonean leone</a>
                </td>
                <td align="center">
                    <tt>SLL</tt>
                </td>
                <td align="center">
                    Le
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Singapore
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Singapore_dollar">Singapore dollar</a>
                </td>
                <td align="center">
                    <tt>SGD</tt>
                </td>
                <td align="center">
                    S$
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Slovakia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/European_euro">European euro</a>
                </td>
                <td align="center">
                    <tt>EUR</tt>
                </td>
                <td align="center">
                    €
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Slovenia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/European_euro">European euro</a>
                </td>
                <td align="center">
                    <tt>EUR</tt>
                </td>
                <td align="center">
                    €
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Solomon Islands
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Solomon_Islands_dollar">Solomon Islands dollar</a>
                </td>
                <td align="center">
                    <tt>SBD</tt>
                </td>
                <td align="center">
                    SI$
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Somalia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Somali_shilling">Somali shilling</a>
                </td>
                <td align="center">
                    <tt>SOS</tt>
                </td>
                <td align="center">
                    Sh.
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    South Africa
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/South_African_rand">South African rand</a>
                </td>
                <td align="center">
                    <tt>ZAR</tt>
                </td>
                <td align="center">
                    R
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Spain
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/European_euro">European euro</a>
                </td>
                <td align="center">
                    <tt>EUR</tt>
                </td>
                <td align="center">
                    €
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Sri Lanka
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Sri_Lankan_rupee">Sri Lankan rupee</a>
                </td>
                <td align="center">
                    <tt>LKR</tt>
                </td>
                <td align="center">
                    Rs
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    St. Helena
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Saint_Helena_pound">Saint Helena pound</a>
                </td>
                <td align="center">
                    <tt>SHP</tt>
                </td>
                <td align="center">
                    £
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    St. Kitts and Nevis
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/East_Caribbean_dollar">East Caribbean dollar</a>
                </td>
                <td align="center">
                    <tt>XCD</tt>
                </td>
                <td align="center">
                    EC$
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    St. Lucia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/East_Caribbean_dollar">East Caribbean dollar</a>
                </td>
                <td align="center">
                    <tt>XCD</tt>
                </td>
                <td align="center">
                    EC$
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    St. Vincent and the Grenadines
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/East_Caribbean_dollar">East Caribbean dollar</a>
                </td>
                <td align="center">
                    <tt>XCD</tt>
                </td>
                <td align="center">
                    EC$
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Sudan
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Sudanese_pound">Sudanese pound</a>
                </td>
                <td align="center">
                    <tt>SDG</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Suriname
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Surinamese_dollar">Surinamese dollar</a>
                </td>
                <td align="center">
                    <tt>SRD</tt>
                </td>
                <td align="center">
                    $
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Svalbard and Jan Mayen Islands
                </td>
                <td align="left" colspan="3">
                    see Norway
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Swaziland
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Swazi_lilangeni">Swazi lilangeni</a>
                </td>
                <td align="center">
                    <tt>SZL</tt>
                </td>
                <td align="center">
                    E
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Sweden
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Swedish_krona">Swedish krona</a>
                </td>
                <td align="center">
                    <tt>SEK</tt>
                </td>
                <td align="center">
                    kr
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Switzerland
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Swiss_franc">Swiss franc</a>
                </td>
                <td align="center">
                    <tt>CHF</tt>
                </td>
                <td align="center">
                    Fr.
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Syria
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Syrian_pound">Syrian pound</a>
                </td>
                <td align="center">
                    <tt>SYP</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">T</font>
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Tahiti
                </td>
                <td align="left" colspan="3">
                    see French Polynesia
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Taiwan
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/New_Taiwan_dollar">New Taiwan dollar</a>
                </td>
                <td align="center">
                    <tt>TWD</tt>
                </td>
                <td align="center">
                    NT$
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Tajikistan
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Tajikistani_somoni">Tajikistani somoni</a>
                </td>
                <td align="center">
                    <tt>TJS</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Tanzania
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Tanzanian_shilling">Tanzanian shilling</a>
                </td>
                <td align="center">
                    <tt>TZS</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Thailand
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Thai_baht">Thai baht</a>
                </td>
                <td align="center">
                    <tt>THB</tt>
                </td>
                <td align="center">
                    ฿
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Timor-Leste
                </td>
                <td align="left" colspan="3">
                    uses the U.S. Dollar
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Togo
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/West_African_CFA_franc">West African CFA franc</a>
                </td>
                <td align="center">
                    <tt>XOF</tt>
                </td>
                <td align="center">
                    CFA
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Trinidad and Tobago
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Trinidad_and_Tobago_dollar">Trinidad and Tobago
                        dollar</a>
                </td>
                <td align="center">
                    <tt>TTD</tt>
                </td>
                <td align="center">
                    TT$
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Tunisia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Tunisian_dinar">Tunisian dinar</a>
                </td>
                <td align="center">
                    <tt>TND</tt>
                </td>
                <td align="center">
                    DT
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Turkey
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Turkish_new_lira">Turkish new lira</a>
                </td>
                <td align="center">
                    <tt>TRY</tt>
                </td>
                <td align="center">
                    YTL
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Turkmenistan
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Turkmen_manat">Turkmen manat</a>
                </td>
                <td align="center">
                    <tt>TMT</tt>
                </td>
                <td align="center">
                    m
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Turks and Caicos Islands
                </td>
                <td align="left" colspan="3">
                    see United States
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Tuvalu
                </td>
                <td align="left" colspan="3">
                    see Australia
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">U</font>
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Uganda
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Ugandan_shilling">Ugandan shilling</a>
                </td>
                <td align="center">
                    <tt>UGX</tt>
                </td>
                <td align="center">
                    USh
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Ukraine
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Ukrainian_hryvnia">Ukrainian hryvnia</a>
                </td>
                <td align="center">
                    <tt>UAH</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    United Arab Emirates
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/UAE_dirham">UAE dirham</a>
                </td>
                <td align="center">
                    <tt>AED</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    United Kingdom
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/British_pound">British pound</a>
                </td>
                <td align="center">
                    <tt>GBP</tt>
                </td>
                <td align="center">
                    £
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    United States of America
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/United_States_dollar">United States dollar</a>
                </td>
                <td align="center">
                    <tt>USD</tt>
                </td>
                <td align="center">
                    US$
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Upper Volta
                </td>
                <td align="left" colspan="3">
                    see Burkina Faso
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Uruguay
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Uruguayan_peso">Uruguayan peso</a>
                </td>
                <td align="center">
                    <tt>UYU</tt>
                </td>
                <td align="center">
                    $U
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Uzbekistan
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Uzbekistani_som">Uzbekistani som</a>
                </td>
                <td align="center">
                    <tt>UZS</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">V</font>
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Vanuatu
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Vanuatu_vatu">Vanuatu vatu</a>
                </td>
                <td align="center">
                    <tt>VUV</tt>
                </td>
                <td align="center">
                    VT
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Vatican
                </td>
                <td align="left" colspan="3">
                    see Italy
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Venezuela
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Venezuelan_bolivar">Venezuelan bolivar</a>
                </td>
                <td align="center">
                    <tt>VEB</tt>
                </td>
                <td align="center">
                    Bs
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Vietnam
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Vietnamese_dong">Vietnamese dong</a>
                </td>
                <td align="center">
                    <tt>VND</tt>
                </td>
                <td align="center">
                    ₫
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Virgin Islands
                </td>
                <td align="left" colspan="3">
                    see United States
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">W</font>
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Wake Island
                </td>
                <td align="left" colspan="3">
                    see United States
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Wallis and Futuna Islands
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/CFP_franc">CFP franc</a>
                </td>
                <td align="center">
                    <tt>XPF</tt>
                </td>
                <td align="center">
                    F
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Western Sahara
                </td>
                <td align="left" colspan="3">
                    see Spain, Mauritania and Morocco
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Western Samoa
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Samoan_tala">Samoan tala</a>
                </td>
                <td align="center">
                    <tt>WST</tt>
                </td>
                <td align="center">
                    WS$
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">Y</font>
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Yemen
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Yemeni_rial">Yemeni rial</a>
                </td>
                <td align="center">
                    <tt>YER</tt>
                </td>
                <td align="center">
                </td>
            </tr>
            <tr bgcolor="#cccccc">
                <td align="left" colspan="4">
                    <font size="+1">Z</font>
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Zaïre
                </td>
                <td align="left" colspan="3">
                    see Congo, Democratic Republic
                </td>
            </tr>
            <tr bgcolor="#ddddff">
                <td>
                    Zambia
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Zambian_kwacha">Zambian kwacha</a>
                </td>
                <td align="center">
                    <tt>ZMK</tt>
                </td>
                <td align="center">
                    ZK
                </td>
            </tr>
            <tr bgcolor="#ddffdd">
                <td>
                    Zimbabwe
                </td>
                <td>
                    <a href="http://en.wikipedia.org/wiki/Zimbabwean_dollar">Zimbabwean dollar</a>
                </td>
                <td align="center">
                    <tt>ZWR</tt>
                </td>
                <td align="center">
                    Z$
                </td>
            </tr>
        </tbody>
    </table>
</asp:Content>
