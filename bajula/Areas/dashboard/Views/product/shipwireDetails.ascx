<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.shipwire.Shipwire>" %>
<div class="section_header">
    <img src="/Content/img/social/icons/shipwire_16.png" /> Additional Shipwire Details
</div>
<div class="form_group">
    <div class="fl mr40">
        <div class="form_entry">
            <div class="form_label">
                <label for="width">
                    Packing
                </label>
            </div>
            <select id="shipwire_packing" name="shipwire_packing">
                <option value="1">Lick &amp; Stick</option>
                <option value="2">Pick &amp; Pack</option>
            </select>
        </div>
        <div class="form_entry">
            <div class="form_label">
                <label for="width">
                    Shipwire Category</label>
            </div>
            <select id="shipwire_category" name="shipwire_category">
                <option value="1">Apparel, Shoes &amp; Jewelry</option>
                <option value="2">Auto Parts &amp; Accessories</option>
                <option value="3">Books, Movies &amp; Music</option>
                <option value="4">Computers &amp; Electronics</option>
                <option value="5">Food &amp; Beverage</option>
                <option value="6">Furniture &amp; Appliances</option>
                <option value="7">Health &amp; Personal Care</option>
                <option value="8">Home &amp; Garden</option>
                <option value="9">Toys, Sports &amp; Hobbies</option>
                <option value="10">Other</option>
            </select>
        </div>
    </div>
    <div class="fl">
    <div class="form_entry">
    <div class="form_label">
                <label>Attributes</label>
            </div>
            </div>
        <div class="pb5">
            <input type="checkbox" <%= Model.fragile?"checked='checked'":"" %> id="shipwire_fragile"
                name="shipwire_fragile" /><label for="shipwire_fragile">Fragile</label>
        </div>
        <div class="pb5">
            <input type="checkbox" <%= Model.dangerous?"checked='checked'":"" %> id="shipwire_dangerous"
                name="shipwire_dangerous" /><label for="shipwire_dangerous">Dangerous Goods</label>
        </div>
        <div class="pb5">
            <input type="checkbox" <%= Model.perishable?"checked='checked'":"" %> id="shipwire_perishable"
                name="shipwire_perishable" /><label for="shipwire_perishable">Perishable</label>
        </div>
    </div>
    <div class="clear"></div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $('#shipwire_packing').val(<%= Model.pnp %>);
        $('#shipwire_category').val(<%= (int)(Model.category) %>);
    });
</script>
