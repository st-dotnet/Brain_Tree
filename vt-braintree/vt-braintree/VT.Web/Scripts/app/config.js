(function () {

    'use strict';

    // basic
    var saveCompanyServiceForm = $("#orgMerchantAccountForm").validate({
        rules: {
            FirstName: {
                required: true
            },
            LastName: {
                required: true
            },
            Email: {
                required: true,
                email: true
            },
            DateOfBirth: {
                required: true
            },
            IndStreetAddress: {
                required: true
            },
            IndLocality: {
                required: true,
            },
            IndRegion: {
                required: true
            },
            IndPostalCode: {
                required: true
            },
            LegalName: {
                required: true,
                maxlength: 40
            },
            TaxId: {
                required: true
            },
            BusStreetAddress: {
                required: true
            },
            BusLocality: {
                required: true,
            },
            BusRegion: {
                required: true
            },
            BusPostalCode: {
                required: true
            },
            Descriptor: {
                required: true,
            },
            FundEmail: {
                required: true,
                email:true
            },
            FundMobilePhone: {
                required: true,
            },
            AccountNumber: {
                required: true
            },
            RoutingNumber: {
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
            var buttonText = $("#btnSaveMerchantAccount").html();
            $("#btnSaveMerchantAccount").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; Submitting');

            $(form).ajaxSubmit({
                success: function (data) {
                    if (data && data.success) {
                        $("#btnSaveMerchantAccount").attr('disabled', null).html('Submit');
                        VT.Util.Notification(true, data.message);
                        window.location.href = $("#RedirectUrl").val();
                    } else {
                        VT.Util.HandleLogout(data.message);
                        $('#orgMerchantAccountForm .alert-danger').removeClass("hide").find(".error-message").html(data.message);
                        VT.Util.Notification(false, data.message);
                    }
                    $("#btnSaveMerchantAccount").attr('disabled', null).html(buttonText);
                },
                error: function (xhr, status, error) {
                    $("#btnSaveMerchantAccount").attr('disabled', null).html(buttonText);
                }
            });
            return false;
        }       
    });
}).apply(this, [jQuery]);