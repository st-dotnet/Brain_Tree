(function () {

    'use strict';

    //Save Merchant info
    var customerDetailForm = $("#customerDetailForm").validate({
        rules: {
            CustomerFirstName: {
                required: true
            },
            CustomerLastName: {
                required: true
            },
            CustomerEmail: {
                required: true
            }
        },
        highlight: function(label) {
            $(label).closest('.form-group').removeClass('has-success').addClass('has-error');
        },
        success: function(label) {
            $(label).closest('.form-group').removeClass('has-error');
            label.remove();
        },
        errorPlacement: function(error, element) {
            var placement = element.closest('.input-group');
            if (!placement.get(0)) {
                placement = element;
            }
            if (error.text() !== '') {
                placement.after(error);
            }
        },
        submitHandler: function(form) {
            var buttonText = $("#btnSaveCustomerDetail").html();
            $("#btnSaveCustomerDetail").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

            $(form).ajaxSubmit({
                success: function(data) {
                    debugger;
                    if (data && data.success) {
                        VT.Util.Notification(true, "Customer information has been successfully saved.");
                    } else {
                        VT.Util.HandleLogout(data.message);
                        VT.Util.Notification(false, data.message);
                    }

                    $("#btnSaveCustomerDetail").attr('disabled', null).html(buttonText);
                },
                error: function(xhr, status, error) {
                    $("#btnSaveCustomerDetail").attr('disabled', null).html(buttonText);
                }
            });
            return false;
        }
    });
     
      

    //Disable-Delete payment method
    $("#deletePayment").click(function () {
        debugger;
        var customerId = $(this).data("customerid");
        $("#hdnEntityId").val(customerId);
        $("#delete-payment-confirm-modal").modal({
            backdrop: 'static',
            keyboard: false,
            show: true

        });
    }); 

    $("#delete-payment-confirm-modal").on('hidden.bs.modal', function () { 
        $("#hdnEntityId").val(''); 
    });

    $("#btnModalSubmit").click(function () {
        debugger;
        var buttonText = $("#btnModalSubmit").html();
        $("#btnModalSubmit").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

        var customerId = $("#hdnEntityId").val(); 
        var url = (customerId !== "0" && customerId.length > 0) ? "/Splash/DisableCustomerCc/" + customerId : "/Splash/DisableCompanyCc";

        $.ajax({
            url: url,
            type: "POST", 
            success: function (result) { 
                debugger;

                if (result.success) {
                    VT.Util.Notification(true, "Payment information has been successfully deleted.");
                    $("#delete-payment-confirm-modal").modal("hide");
                    //reload
                    window.location.reload();
                } else {
                    VT.Util.HandleLogout(result.message);
                    VT.Util.Notification(false, "Some error occured while deleting.");
                }

                $("#btnModalSubmit").attr('disabled', null).html(buttonText);
            },
            error: function (xhr, status, error) {
                VT.Util.Notification(false, error);
                $("#btnModalSubmit").attr('disabled', null).html(buttonText);
            }
        });
    });
     

    
    //Add payment details
    var savePaymentDetailForm = $("#savePaymentDetailForm").validate({
        rules: {
            PaymentNumber: {
                required: true
            },
            PaymentMethod: {
                required: true
            },
            PaymentCVV: {
                required: true
            },
            PaymentExpiration: {
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
            var buttonText = $("#btnSavePaymentDetail").html();
            debugger;
            $("#btnSavePaymentDetail").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);
            debugger;
            $(form).ajaxSubmit({ 
                success: function (data) {
                    debugger;
                    if (data && data.success) {
                        $("#modal-add-payment-form").modal("hide");
                        debugger;
                        $(form).resetForm();
                        VT.Util.Notification(true, "Customer card has been successfully saved.");
                        //reload
                        window.location.reload(); 
                    } else {
                        VT.Util.HandleLogout(data.message);
                        VT.Util.Notification(false, data.message);
                    } 
                    $("#btnSavePaymentDetail").attr('disabled', null).html(buttonText); 
                },
                error: function (xhr, status, error) {
                    $("#btnSavePaymentDetail").attr('disabled', null).html(buttonText);
                }
            });
            return false;
        }
    });

    $("#modal-add-payment-form").on('hidden.bs.modal', function () {
        $("#modal-payment-form-title").html("Add Payment Details");
        savePaymentDetailForm.resetForm();
        $("#CompanyId").val(0);
        $(".has-error").removeClass("has-error");
    });
     
}).apply(this, [jQuery]);