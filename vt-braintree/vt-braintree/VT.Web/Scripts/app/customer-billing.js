(function () {

    'use strict';

    var calculateSummary = function () {
        var totalAmount = 0;
        var serviceFee = 0;
        var serviceRecordIds = '';
        var hasCc = '';
        debugger;

        $('.serviceRecordCheck').each(function () {
            if ($(this).is(":checked")) {
                var serviceRecordId = $(this).val();
                var serviceFeePercent = $("#ServiceFee-" + serviceRecordId).val();
                var amount = $("#Amount-" + serviceRecordId).val();
                hasCc = $("#HasCC-" + serviceRecordId).val();

                serviceFee += parseFloat((serviceFeePercent * amount) / 100);
                totalAmount += parseFloat(amount);
                serviceRecordIds += serviceRecordId + ',';
            }
        });
        $("#totalAmount").val(totalAmount);
        $("#sTotalAmount").html('$' + totalAmount.toFixed(2));
        $("#hasCreditCard").val(hasCc);
        $("#serviceRecordIds").val(serviceRecordIds);

        if (hasCc == 'false') {
            $(".chargeCustomer").html("<i class='fa fa-money'></i> Charge Customer Externally");
        }
        else {
            $(".chargeCustomer").html("<i class='fa fa-money'></i> Charge Customer");
        }

        $("#divSummary").removeClass("hidden");

        $('html,body').animate({
            scrollTop: $("#divSummary").offset().top
        }, 'slow');
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

    $(document).on('click', '.edit-cost', function () {  
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
       
        var hasCc = $("#hasCreditCard").val();
        var message = "This customer does not have a CC on file. We will mark these records as Paid Externally, which means you MUST invoice your customer through your billing system to collect the money owed to you. Are you sure you want to continue?";

        if (hasCc == "true") {
            message = "We will now bill <b>" + $("#gridOrganization option:selected").text() + "</b> for a amount of <b>" + $("#sTotalAmount").html() + "</b>?";
        }

        $("#message").html(message);
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
            url: '/Billing/ChargeCustomerAccount',
            type: "POST",
            data: {
                CustomerId: $("#gridOrganization").val(),
                Amount: $("#totalAmount").val(),
                ServiceRecordIds: $("#serviceRecordIds").val(),
                ChargeExternal: $("#hasCreditCard").val() == "false"
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
