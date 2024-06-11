(function () {

    'use strict';

    //function to show change status modal
    var getChangeStatusModal = function (id) {

        $("#ServiceRecordId").val(id);
        $("#change-status-modal").modal({
            backdrop: 'static',
            keyboard: false,
            show:true
        });
        $("#divOrgDetails").html("");

        return false;
    }

    //edit organization
    $(document).on('click', '.change-status', function () {
        var id = $(this).data("id");
        getChangeStatusModal(id);
    });

    var saveOrganizationForm = $("#changeStatusForm").validate({
        rules: {
            Address: {
                required: true
            },

        },
        submitHandler: function (form) {
            var buttonText = $("#btnSaveOrg").html();
            $("#btnSaveOrg").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

            $(form).ajaxSubmit({
                success: function (data) {
                    if (data && data.success) {
                        $("#modal-add-org-form").modal("hide");
                        //refresh grid
                        var grid = $("#BillingListGrid").data("kendoGrid");
                        grid.dataSource.read();

                        VT.Util.Notification(true, "Organization has been successfully saved.");
                    } else {
                        VT.Util.HandleLogout(data.message);
                        $('#saveOrganizationForm .alert-danger').removeClass("hide").find(".error-message").html(data.message);
                        VT.Util.Notification(false, "Some error occured while saving current organization.");
                    }

                    $("#btnSaveOrg").attr('disabled', null).html(buttonText);
                    $(form).resetForm();
                },
                error: function (xhr, status, error) {
                    $("#btnSaveOrg").attr('disabled', null).html(buttonText);
                }
            });
            return false;
        }
    });

    $("#modal-add-org-form").on('hidden.bs.modal', function () {
        saveOrganizationForm.resetForm();
        $(".error").removeClass("error");
    });

    var calculateSummary = function() {
        var totalAmount = 0;
        var serviceFee = 0;
        var serviceRecordIds = '';
        $('.serviceRecordCheck').each(function() {
            if ($(this).is(":checked")) {
                var serviceRecordId = $(this).val();
                var serviceFeePercent = $("#ServiceFee-" + serviceRecordId).val();
                var amount = $("#Amount-" + serviceRecordId).val();
                serviceFee += parseFloat((serviceFeePercent * amount) / 100);
                totalAmount += parseFloat(amount);
                serviceRecordIds += serviceRecordId + ',';
            }
        });
        $("#totalServiceFeeAmount").val(serviceFee);
        $("#sTotalAmount").html('$' + totalAmount.toFixed(2));
        $("#sServiceFee").html('$' + serviceFee.toFixed(2));
        $("#serviceRecordIds").val(serviceRecordIds);
        $("#divSummary").removeClass("hidden");
 
        if (serviceFee == 0) {
            $(".chargeCustomer").hide(); 
        }else {
            $(".chargeCustomer").show();
        }
    
        //$('html,body').animate({
        //    scrollTop: $("#divSummary").offset().top
        //}, 'slow');
    };

    $(document).on('click', '.all-checks', function () {
        if (this.checked) {
            $('.serviceRecordCheck').each(function () {
                this.checked = true;
            });
        } else {
            $('.serviceRecordCheck').each(function () {
                this.checked = false;
            });
        }
        calculateSummary();
    });

    $(document).on('change', '.serviceRecordCheck', function () {
        calculateSummary();
    });

    $(document).on('click', '.edit-cost', function() {
        var serviceItemId = $(this).data("id");
        var description = $(this).data("description");

        $("#description").html(description);
        $("#ServiceRecordItemId").val(serviceItemId);

        $("#set-cost-modal").modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
    });

    
    var setCostForm = $("#setCostForm").validate({
        rules: {
            CostOfService: {
                required: true
            },

        },
        submitHandler: function (form) {
            var buttonText = $("#btnSetCost").html();
            $("#btnSetCost").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

            $(form).ajaxSubmit({
                success: function (data) {
                    if (data && data.success) {
                        $("#set-cost-modal").modal("hide");
                        //refresh grid
                        var grid = $("#BillingListGrid").data("kendoGrid");
                        grid.dataSource.read();

                        VT.Util.Notification(true, "Cost successfully saved.");
                    } else {
                        VT.Util.HandleLogout(data.message);
                        $('#setCostForm .alert-danger').removeClass("hide").find(".error-message").html(data.message);
                        VT.Util.Notification(false, "Some error occured while saving saving cost.");
                    }

                    $("#btnSetCost").attr('disabled', null).html(buttonText);
                    $(form).resetForm();
                },
                error: function (xhr, status, error) {
                    $("#btnSetCost").attr('disabled', null).html(buttonText);
                }
            });
            return false;
        }
    });

    $(".chargeCustomer").click(function () {
        
        $("#message").html("Are you sure you want to charge customer <b>"
            + $("#gridOrganization option:selected").text() + "</b> an amount of <b>" + $("#sServiceFee").html() + "</b>?");
        $("#modal-charge-form").modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
    });
    
    $("#btnModalSubmit").click(function () {
        var buttonText = $("#btnModalSubmit").html();
        $("#btnModalSubmit").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);
        
        $.ajax({
            url: '/Billing/ChargeCustomer',
            type: "POST",
            data: {
                CompanyId: $("#gridOrganization").val(),
                Amount: $("#totalServiceFeeAmount").val(),
                ServiceRecordIds: $("#serviceRecordIds").val()
            },
            success: function (result) {
                if (result.success) {
                    VT.Util.Notification(true, result.message);
                    $("#modal-charge-form").modal("hide");

                    //refresh grid
                    var grid = $("#BillingListGrid").data("kendoGrid");
                    grid.dataSource.read();

                } else {
                    VT.Util.HandleLogout(result.message);
                    VT.Util.Notification(false, result.message);
                }

                $("#btnModalSubmit").attr('disabled', null).html('Submit');
            },
            error: function (xhr, status, error) {
                VT.Util.Notification(false, error);
                $("#btnModalSubmit").attr('disabled', null).html('Submit');
            }
        });
    });

}).apply(this, [jQuery]);
