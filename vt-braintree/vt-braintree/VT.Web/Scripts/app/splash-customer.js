(function () {

    'use strict';

    //Save Customer
    var saveCustomerForm = $("#saveCustomerForm").validate({
        rules: {
            FirstName: {
                required: true
            },
            LastName: {
                required: true
            },
            Email: {
                required: true
            },
            CardNumber: {
                required: true
            },
            CardCVVNumber: {
                required: true
            },
            PaymentMethod: {
                required: true
            },
            ExpirationDate: {
                required: true
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
            var buttonText = $("#btnSaveCustomer").html();
            $("#btnSaveCustomer").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

            $(form).ajaxSubmit({
                success: function (data) {
                    if (data && data.success) {
                        VT.Util.Notification(true, "Customer has been successfully saved.");
                        setTimeout(function () {
                            window.location.reload();
                        }, 3000);
                    } else {
                        VT.Util.HandleLogout(data.message);
                        VT.Util.Notification(false, data.message);
                    }

                    $("#btnSaveCustomer").attr('disabled', null).html(buttonText);
                    //$(form).resetForm();
                    //$("#CompanyId").val('');
                    //$("#CompanyId").val(0);
                },
                error: function (xhr, status, error) {
                    $("#btnSaveCustomer").attr('disabled', null).html(buttonText);
                }
            });
            return false;
        }
    });


}).apply(this, [jQuery]);