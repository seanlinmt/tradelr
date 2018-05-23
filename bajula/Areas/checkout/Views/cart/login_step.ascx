<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.checkout.Models.BaseViewModel>" %>
<div id="credentials">
<div class="content">
    <div class="form_entry">
        <div class="form_label">
            <label for="first_name">
                I am
            </label>
        </div>
        <select id="customer_type">
            <option>a new customer</option>
            <option>a returning customer</option>
        </select>
    </div>
    <form id="customer_new" action="/checkout/order/<%= Model.cartid %>/create_account" method="POST">
    <div class="form_entry">
        <div class="form_label">
            <label for="first_name">
                First name</label>
        </div>
        <%= Html.TextBox("first_name") %>
    </div>
    <div class="form_entry">
        <div class="form_label">
            <label for="last_name">
                Last name</label>
        </div>
        <%= Html.TextBox("last_name") %>
    </div>
    <div class="form_entry">
        <div class="form_label">
            <label for="email_new">
                Your contact email</label>
        </div>
        <%= Html.TextBox("email_new")%>
    </div>
    <div class="form_entry">
        <div class="form_label">
            <label for="password_new">
                Choose a password</label>
        </div>
        <%= Html.Password("password_new")%>
    </div>
    <div class="form_entry">
        <div class="form_label">
            <label for="password2_new">
                Type your password again</label>
        </div>
        <%= Html.Password("password2_new")%>
    </div>
    <div class="mt10">
        <button id="buttonCreate" type="button" class="green ajax">
            create account</button>
        <div class="mt10" id="new_error">
        </div>
    </div>
    </form>
    <form id="customer_existing" action="/checkout/order/<%= Model.cartid %>/login_account" method="POST" class="hidden">
    <div class="form_entry">
        <div class="form_label">
            <label for="email_existing">
                Your email</label>
        </div>
        <%= Html.TextBox("email_existing")%>
    </div>
    <div class="form_entry">
        <div class="form_label">
            <label for="password_existing">
                Your password</label>
        </div>
        <%= Html.Password("password_existing")%>
    </div>
    <div class="mt10">
        <button id="buttonLogin" type="button" class="green ajax">
            login</button>
        <div class="mt10" id="existing_error">
        </div>
    </div>
    </form>
</div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $('#customer_type').change(function () {
            $('#customer_new').toggle();
            $('#customer_existing').toggle();
            return false;
        });

        $('#buttonCreate').click(function () {
            $(this).buttonDisable();
            $(this).trigger('submit');
            $('#new_error').html('');
        });

        $('#customer_new').submit(function () {
            var serialized = $(this).serialize();
            var action = $(this).attr("action");

            var ok = $(this).validate({
                invalidHandler: function (form, validator) {
                    $(validator.invalidElements()[0]).focus();
                },
                focusInvalid: false,
                rules: {
                    first_name: {
                        required: true
                    },
                    last_name: {
                        required: true
                    },
                    email_new: {
                        required: true,
                        email: true
                    },
                    password2_new: {
                        required: true
                    },
                    password_new: {
                        required: true
                    }
                }
            }).form();

            if (!ok) {
                $('#buttonCreate').buttonEnable();
                return false;
            }

            $.ajax({
                type: "POST",
                url: action,
                data: serialized,
                success: function (result) {
                    if (typeof (result) == 'string') {
                        $('#credentials').slideUp(function () {
                            $('#credentials').after(result);
                        });
                    }
                    else {
                        $('#new_error').html("<div class='error_post'>" + result.message + "</div>");
                    }
                }
            });
            return false;
        });

        $('#buttonLogin').click(function () {
            $(this).buttonDisable();
            $(this).trigger('submit');
            $('#existing_error').html('');
        });

        $('#customer_existing').submit(function () {
            var serialized = $(this).serialize();
            var action = $(this).attr("action");

            var ok = $(this).validate({
                invalidHandler: function (form, validator) {
                    $(validator.invalidElements()[0]).focus();
                },
                focusInvalid: false,
                rules: {
                    email_existing: {
                        required: true,
                        email: true
                    },
                    password_existing: {
                        required: true
                    }
                }
            }).form();

            if (!ok) {
                $('#buttonLogin').buttonEnable();
                return false;
            }

            $.ajax({
                type: "POST",
                url: action,
                data: serialized,
                success: function (result) {
                    if (typeof (result) == 'string') {
                        $('#credentials').slideUp(function () {
                            $('#credentials').after(result);
                        });
                    }
                    else {
                        $('#existing_error').html("<div class='error_post'>" + result.message + "</div>");
                    }
                }
            });
            return false;
        });
    });
</script>
