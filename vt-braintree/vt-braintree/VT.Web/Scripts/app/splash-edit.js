(function () {

    'use strict';
      
    //Save Merchant info
    var saveMerchantForm = $("#merchantForm").validate({
        rules: {
            AnnualCCSales: {
                required: true
            },
            Established: {
                required: true
            },
            MerchantCategoryCode: {
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
            var buttonText = $("#btnSaveMerchant").html();
            $("#btnSaveMerchant").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

            $(form).ajaxSubmit({
                success: function (data) { 
                    if (data && data.success) { 
                        VT.Util.Notification(true, "Merchnat information has been successfully saved."); 
                    } else {
                        VT.Util.HandleLogout(data.message);
                        VT.Util.Notification(false, data.message);
                    }

                    $("#btnSaveMerchant").attr('disabled', null).html(buttonText); 
                },
                error: function (xhr, status, error) {
                    $("#btnSaveMerchant").attr('disabled', null).html(buttonText);
                }
            });
            return false;
        }
    });

    //Save Member 
    var memberForm = $("#memberForm").validate({
        rules: { 
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
            var buttonText = $("#btnSaveMember").html();
            $("#btnSaveMember").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

            $(form).ajaxSubmit({
                success: function (data) {
                    if (data && data.success) {
                        VT.Util.Notification(true, "Merchnat member has been successfully saved.");
                    } else {
                        VT.Util.HandleLogout(data.message);
                        VT.Util.Notification(false, data.message);
                    }

                    $("#btnSaveMember").attr('disabled', null).html(buttonText);
                },
                error: function (xhr, status, error) {
                    $("#btnSaveMember").attr('disabled', null).html(buttonText);
                }
            });
            return false;
        }
    });

    //Save Entity 
    var entityForm = $("#entityForm").validate({
        rules: { 
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
            var buttonText = $("#btnSaveEntity").html();
            $("#btnSaveEntity").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

            $(form).ajaxSubmit({
                success: function (data) {
                    if (data && data.success) {
                        VT.Util.Notification(true, "Merchnat entity has been successfully saved.");
                    } else {
                        VT.Util.HandleLogout(data.message);
                        VT.Util.Notification(false, data.message);
                    }

                    $("#btnSaveEntity").attr('disabled', null).html(buttonText);
                },
                error: function (xhr, status, error) {
                    $("#btnSaveEntity").attr('disabled', null).html(buttonText);
                }
            });
            return false;
        }
    });

    //save account
    var accountForm = $("#accountForm").validate({
        rules: { 
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
            var buttonText = $("#btnAccount").html();
            $("#btnAccount").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

            $(form).ajaxSubmit({
                success: function (data) {
                    if (data && data.success) {
                        VT.Util.Notification(true, "Merchnat account has been successfully saved.");
                    } else {
                        VT.Util.HandleLogout(data.message);
                        VT.Util.Notification(false, data.message);
                    } 
                    $("#btnAccount").attr('disabled', null).html(buttonText);
                },
                error: function (xhr, status, error) {
                    $("#btnAccount").attr('disabled', null).html(buttonText);
                }
            });
            return false;
        }
    });
       

}).apply(this, [jQuery]);