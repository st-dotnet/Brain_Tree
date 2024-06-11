(function () {

    'use strict';
     
    // basic
    var braintreeCustomerForm = $("#braintree-customer-form").validate({
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

            $("#Nonce").val('');
            
            var buttonText = $("#btnSaveCustomer").html();
            $("#btnSaveCustomer").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; Submitting');
                        
            var checkExist = setInterval(function () {
                var nonce = $("#Nonce").val();
                if (nonce.length > 0) {
                    console.log("nonce received.");
                    clearInterval(checkExist);

                    if (nonce == 'ERROR') {
                        VT.Util.Notification(false, 'Invalid credit card details provided.');
                        $("#btnSaveCustomer").attr('disabled', null).html(buttonText);
                        return false;
                    }

                    $(form).ajaxSubmit({
                        success: function (data) {
                            if (data && data.success) {
                                debugger;
                                $("#btnSaveCustomer").attr('disabled', null).html('Submit');
                                VT.Util.Notification(true, data.message);
                                var url = $("#RedirectUrl").val();
                                if (url.length > 0) {
                                    window.location.href = $("#RedirectUrl").val();
                                }
                            } else {
                                VT.Util.HandleLogout(data.message);
                                $('#braintree-customer-form .alert-danger').removeClass("hide").find(".error-message").html(data.message);
                                VT.Util.Notification(false, data.message);
                            }
                           
                            $("#btnSaveCustomer").attr('disabled', null).html(buttonText);
                        },
                        error: function (xhr, status, error) {
                            $("#btnSaveCustomer").attr('disabled', null).html(buttonText);
                        }
                    });

                } else {
                    console.log("waiting for nonce.");
                }
            }, 100);

            return false;
            
        }
    });
}).apply(this, [jQuery]);