(function () {

    'use strict';

    // basic
    $("#loginForm").validate({
        rules: {
            UserName: {
                required: true
            },
            Password: {
                required: true
            },
            RememberMe: {
                required: false
            }
        },
        highlight: function (label) {
            $(label).closest('.form-group').removeClass('has-success').addClass('has-error');
        },
        success: function (label) {
            $(label).closest('.form-group').removeClass('has-error');
            label.remove();
        },
        errorPlacement: function (error, element) {
            var placement = element.closest('.input-group');
            if (!placement.get(0)) {
                placement = element;
            }
            if (error.text() !== '') {
                placement.after(error);
            }
        },
        submitHandler: function (form) {
            var buttonText = $("#btnLogin").html();

            $("#btnLogin").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

            $(form).ajaxSubmit({
                success: function (data) {
                    if (data && data.success) {
                        window.location = data.redirectUrl;
                    } else {
                        $('#loginForm').find('.custom-danger').removeClass("hide").html(data.message);
                        $("#btnLogin").attr('disabled', null).html('Submit');
                    }
                },
                error: function (xhr, status, error) {
                    $("#btnLogin").attr('disabled', null).html(buttonText);
                }
            });
        }
    });

}).apply(this, [jQuery]);