<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.users.UserPermission>" %>
<%@ Import Namespace="tradelr.Models.users" %>
<div class="section_header">
    User Permissions
</div>
<div class="form_group">
    <span class="tip">Fine tune what this contact can do on your network.</span>
    <p class="font_grey">
        Preset permissions: <a title="Usually administrators and staff will have these permissions" id="permissionAdmin"
            href="#">administrator/staff</a> | <a title="Common permissions that are given to normal networks members" id="permissionUser"
                href="#">normal contact</a></p>
    <ul id="userPermissions">
        <li>
            <div class="form_entry">
                <div class="form_label">
                    <strong>Orders / Invoices</strong>
                </div>
                <ul>
                    <li>
                        <input type="checkbox" id="orders_add" name="orders_add" <%= (Model & UserPermission.ORDERS_ADD) != 0?"checked='checked'":""%> /><label
                            for="orders_add">create new orders</label></li>
                    <li>
                        <input type="checkbox" id="invoices_add" name="invoices_add" <%= (Model & UserPermission.INVOICES_ADD) != 0?"checked='checked'":""%> /><label
                            for="invoices_add">create new invoices</label></li>
                    <li>
                        <input type="checkbox" id="transaction_view" name="transaction_view" <%= (Model & UserPermission.TRANSACTION_VIEW) != 0?"checked='checked'":""%> /><label
                            for="transaction_view">view transactions</label></li>
                    <li>
                        <input type="checkbox" id="transaction_modify" name="transaction_modify" <%= (Model & UserPermission.TRANSACTION_MODIFY) != 0?"checked='checked'":""%> /><label
                            for="transaction_modify">edit/delete transactions</label></li>
                    <li>
                        <input type="checkbox" id="transaction_send" name="transaction_send" <%= (Model & UserPermission.TRANSACTION_SEND) != 0?"checked='checked'":""%> /><label
                            for="invoices_send">email transactions</label></li>
                </ul>
            </div>
        </li>
        <li>
            <div class="form_entry">
                <div class="form_label">
                    <strong>Inventory</strong>
                </div>
                <ul>
                    <li>
                        <input type="checkbox" id="inventory_add" name="inventory_add" <%= (Model & UserPermission.INVENTORY_ADD) != 0?"checked='checked'":""%> /><label
                            for="inventory_add">add new products</label></li>
                    <li>
                        <input type="checkbox" id="inventory_modify" name="inventory_modify" <%= (Model & UserPermission.INVENTORY_MODIFY) != 0?"checked='checked'":""%> /><label
                            for="inventory_modify">edit/delete products</label></li>
                    <li>
                        <input type="checkbox" id="inventory_view" name="inventory_view" <%= (Model & UserPermission.INVENTORY_VIEW) != 0?"checked='checked'":""%> /><label
                            for="inventory_view">view inventory</label></li>
                </ul>
            </div>
        </li>
        <li>
            <div class="form_entry">
                <div class="form_label">
                    <strong>Contacts</strong>
                </div>
                <ul>
                    <li>
                        <input type="checkbox" id="contacts_add" name="contacts_add" <%= (Model & UserPermission.CONTACTS_ADD) != 0?"checked='checked'":""%> /><label
                            for="contacts_add">add new contacts</label></li>
                    <li>
                        <input type="checkbox" id="contacts_modify" name="contacts_modify" <%= (Model & UserPermission.CONTACTS_MODIFY) != 0?"checked='checked'":""%> /><label
                            for="contacts_modify">edit/delete contacts</label></li>
                    <li>
                        <input type="checkbox" id="contacts_view" name="contacts_view" <%= (Model & UserPermission.CONTACTS_VIEW) != 0?"checked='checked'":""%> /><label
                            for="contacts_view">view contacts</label></li>
                </ul>
            </div>
        </li>
        <li>
            <div class="form_entry">
                <div class="form_label">
                    <strong>Network</strong>
                </div>
                <ul>
                    <li>
                        <input type="checkbox" id="network_settings" name="network_settings" <%= (Model & UserPermission.NETWORK_SETTINGS) != 0?"checked='checked'":""%> /><label
                            for="network_settings">modify account settings</label></li>
                    <li>
                <input type="checkbox" id="network_store" name="network_store" <%= (Model & UserPermission.NETWORK_STORE) != 0?"checked='checked'":""%> /><label
                    for="network_store">modify store settings</label></li>
                </ul>
            </div>
        </li>
    </ul>
    <div class="clear">
    </div>
    <%= Html.Hidden("permissions", (uint)Model) %>
    
</div>
<script type="text/javascript">
    $('#permissionAdmin').click(function () {
        $('#userPermissions input:checkbox').attr('checked', true);
        $('#userPermissions input:checkbox').each(function () {
            initPermissionLabels(this);
        });
        return false;
    });

    $('#permissionUser').click(function () {
        $('#userPermissions input:checkbox').attr('checked', false);
        $('#userPermissions input:checkbox').each(function () {
            initPermissionLabels(this);
        });
        return false;
    });

    $('#userPermissions input:checkbox').click(function () {
        initPermissionLabels(this);
    });

    function initPermissionLabels(el) {
        if ($(el).is(':checked')) {
            $(el).next().addClass('selected pr4 pl4 r4 s1');
        }
        else {
            $(el).next().removeClass('selected pr4 pl4 r4 s1');
        }
    }

    function parseUserPermissions(currentvalue) {
        if ($('#inventory_add:checked').length != 0) {
            currentvalue |= tradelr.userpermissions.INVENTORY_ADD;
        }
        else {
            currentvalue &= ~tradelr.userpermissions.INVENTORY_ADD;
        }
        if ($('#inventory_modify:checked').length != 0) {
            currentvalue |= tradelr.userpermissions.INVENTORY_MODIFY;
        }
        else {
            currentvalue &= ~tradelr.userpermissions.INVENTORY_MODIFY;
        }
        if ($('#inventory_view:checked').length != 0) {
            currentvalue |= tradelr.userpermissions.INVENTORY_VIEW;
        }
        else {
            currentvalue &= ~tradelr.userpermissions.INVENTORY_VIEW;
        }
        if ($('#invoices_add:checked').length != 0) {
            currentvalue |= tradelr.userpermissions.INVOICES_ADD;
        }
        else {
            currentvalue &= ~tradelr.userpermissions.INVOICES_ADD;
        }
        if ($('#transaction_modify:checked').length != 0) {
            currentvalue |= tradelr.userpermissions.TRANSACTION_MODIFY;
        }
        else {
            currentvalue &= ~tradelr.userpermissions.TRANSACTION_MODIFY;
        }
        if ($('#transaction_send:checked').length != 0) {
            currentvalue |= tradelr.userpermissions.TRANSACTION_SEND;
        }
        else {
            currentvalue &= ~tradelr.userpermissions.TRANSACTION_SEND;
        }
        if ($('#transaction_view:checked').length != 0) {
            currentvalue |= tradelr.userpermissions.TRANSACTION_VIEW;
        }
        else {
            currentvalue &= ~tradelr.userpermissions.TRANSACTION_VIEW;
        }
        if ($('#orders_add:checked').length != 0) {
            currentvalue |= tradelr.userpermissions.ORDERS_ADD;
        }
        else {
            currentvalue &= ~tradelr.userpermissions.ORDERS_ADD;
        }
        if ($('#contacts_add:checked').length != 0) {
            currentvalue |= tradelr.userpermissions.CONTACTS_ADD;
        }
        else {
            currentvalue &= ~tradelr.userpermissions.CONTACTS_ADD;
        }
        if ($('#contacts_modify:checked').length != 0) {
            currentvalue |= tradelr.userpermissions.CONTACTS_MODIFY;
        }
        else {
            currentvalue &= ~tradelr.userpermissions.CONTACTS_MODIFY;
        }
        if ($('#contacts_view:checked').length != 0) {
            currentvalue |= tradelr.userpermissions.CONTACTS_VIEW;
        }
        else {
            currentvalue &= ~tradelr.userpermissions.CONTACTS_VIEW;
        }
        if ($('#network_settings:checked').length != 0) {
            currentvalue |= tradelr.userpermissions.NETWORK_SETTINGS;
        }
        else {
            currentvalue &= ~tradelr.userpermissions.NETWORK_SETTINGS;
        }
        if ($('#network_store:checked').length != 0) {
            currentvalue |= tradelr.userpermissions.NETWORK_STORE;
        }
        else {
            currentvalue &= ~tradelr.userpermissions.NETWORK_STORE;
        }
        return currentvalue;
    }

    $(document).ready(function () {
        $('#userPermissions input:checkbox').each(function () {
            initPermissionLabels(this);
        });

        $('#userPermissions strong').addClass('font_darkgrey');
    });
</script>
