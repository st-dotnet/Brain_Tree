(function () {

    'use strict';
      
    //Save Merchant
    var saveMerchantForm = $("#saveMerchantForm").validate({
        rules: {
            AnnualCCSales: {
                required: true
            },
            Established: {
                required: true
            },
            MerchantCategoryCode: {
                required: true
            },
            MemberTitle: {
                required: true
            },
            MemberDateOfBirth: {
                required: true
            },
            MemberDriverLicense: {
                required: true
            },
            MemberDriverLicenseState: {
                required: true
            },
            MemberEmail: {
                required: true
            },
            MemberFirstName: {
                required: true 
            },                 
            MemberLastName: {  
                required: true 
            },                 
            MemberOwnerShip: { 
                required: true 
            },
            MemberSocialSecurityNumber: {
                required: true 
            },                 
            EntityName: {      
                required: true 
            },                 
            EntityAddress1: {  
                required: true 
            },                 
            EntityCity: {
                required: true 
            },                 
            EntityCountry: {   
                required: true  
            },
            EntityState: {
                required: true 
            },                 
            EntityWebsite: {   
                required: true 
            },
            EntityZip: {
                required: true 
            },
            CardOrAccountNumber: {
                required: true 
            },                 
            AccountsPaymentMethod: {
                required: true 
            },
            AccountsRoutingCode: {
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
            var buttonText = $("#btnAddMerchant").html();
            $("#btnAddMerchant").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

            $(form).ajaxSubmit({
                success: function (data) { 
                    if (data && data.success) { 
                        VT.Util.Notification(true, "Merchnat has been successfully saved.");
                        setTimeout(function () {
                            window.location.reload();
                        }, 3000);
                    } else {
                        VT.Util.HandleLogout(data.message);
                        VT.Util.Notification(false, data.message);
                    }

                    $("#btnAddMerchant").attr('disabled', null).html(buttonText);
                    //$(form).resetForm();
                    //$("#CompanyId").val('');
                    //$("#CompanyId").val(0);
                },
                error: function (xhr, status, error) {
                    $("#btnAddMerchant").attr('disabled', null).html(buttonText);
                }
            });
            return false;
        }
    }); 
       

}).apply(this, [jQuery]);