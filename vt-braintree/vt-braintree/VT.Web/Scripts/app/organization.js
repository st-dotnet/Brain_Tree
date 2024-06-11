(function () {

    'use strict';

    //function to show save organization modal during edit
    var saveOrganizationModal = function(id) {
        $.ajax({
            type: "POST",
            url: "/Organizations/GetOrganizationData/" + id,
            processdata: false,
            async: true,
            success: function (result) {
                $("#OrganizationId").val(result.OrganizationId);
                $("#Name").val(result.Name);
                $("#ServiceFeePercentage").val(result.ServiceFeePercentage);
                $("#ContactFirstName").val(result.ContactFirstName);
                $("#ContactMiddleName").val(result.ContactMiddleName);
                $("#ContactLastName").val(result.ContactLastName);
                $("#ContactEmail").val(result.ContactEmail);
                $("#ContactMobile").val(result.ContactMobile);
                $("#ContactTelephone").val(result.ContactTelephone);
                $("#Address").val(result.Address);
                $("#City").val(result.City);
                $("#State").val(result.State);
                $("#PostalCode").val(result.PostalCode);
                $("#Country").val(result.Country);
                $("#PaymentGatewayType").val(result.PaymentGatewayType);
                $("#modal-org-form-title").html("Edit Organization");
                $("#modal-add-org-form").modal({
                    backdrop: 'static',
                    keyboard: false,
                    show: true
                });
                $("#divOrgDetails").html("");
                $("#OriginalName").val(result.Name);
            },
            error: function (xhr, status, error) {
                VT.Util.Notification(false, error);
            }
        });
        return false;
    }

    //edit organization
    $(document).on('click', '.edit-org', function () {
        var id = $(this).data("id");
        saveOrganizationModal(id);
    });

    //view organization
    $(document).on('click', '.view-organization', function () {
        var id = $(this).data("id");
        $("#divOrgDetails").html('<i class="fa fa-spinner fa-spin"></i>&nbsp; Please wait..');
        $.ajax({
            url: '/Organizations/GetOrganizationDetail/' + id,
            type: 'POST',
            success: function (result) {
                $("#divOrgDetails").html(result);
                $('html,body').animate({
                    scrollTop: $("#divOrgDetails").offset().top
                }, 'slow');
            },
            error: function (xhr, status, error) {
                $("#divOrgDetails").html('No data found.');
            }
        });
        return false;
    });

    //delete single/multiple organization(s) confirm modal
    $("#btnDeleteOrg").click(function () {

        var checkedCount = $("#OrganizationListGrid input:checked").length;
        if (checkedCount > 0) {
            $("#modal-del-org-form").modal({
                backdrop: 'static',
                keyboard: false,
                show: true
            });
        } else {
            VT.Util.Notification(false, "Please select at least one user to delete.");
        }
    });

    //delete single/multiple organization(s)
    $("#btnModalSubmit").click(function () {
        var buttonText = $("#btnModalSubmit").html();
        $("#btnModalSubmit").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

        var ids = [];
        $("#OrganizationListGrid input:checked").each(function () {
            ids.push($(this).val());
        });
        $.ajax({
            url: '/Organizations/DeleteOrgs',
            type: "POST",
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({ Ids: ids }),
            success: function (result) {
                if (result.success) {
                    VT.Util.Notification(true, "Selected Organization(s) have been successfully deleted.");
                    $("#modal-del-org-form").modal("hide");
                    //refresh grid
                    var grid = $("#OrganizationListGrid").data("kendoGrid");
                    grid.dataSource.read();
                } else {
                    VT.Util.HandleLogout(result.message);
                    VT.Util.Notification(false, "Some error occured while deleting selected organizations.");
                }

                $("#btnModalSubmit").attr('disabled', null).html('Submit');
            },
            error: function (xhr, status, error) {
                VT.Util.Notification(false, error);
                $("#btnModalSubmit").attr('disabled', null).html('Submit');
            }
        });
    });

    var isValidOrgName = function (value, element) {
        var originalName = $("#OriginalName").val();

        if (value == originalName)
            return true;

        var response = $.ajax({
            url: "/Organizations/CheckOrgName",
            type: "POST",
            async: false,
            data: {
                name: value
            }
        }).responseText;
        return response === "false";
    };

    $.validator.addMethod("IsValidOrgName", isValidOrgName, "Organization already exists.");
    $.validator.addMethod("greaterThanZero", function (value, element) {
        return this.optional(element) || ((parseFloat(value) > 0) && (parseFloat(value) < 100));
    }, "Service Fee Amount must be greater than zero");

    var saveOrganizationForm =  $("#saveOrganizationForm").validate({
        rules: {
            Name: {
                required: true,
                IsValidOrgName: true
            },
            ServiceFeeAmount: {
                required: true,
                greaterThanZero: true,
                number: true
            },
            Address: {
                required: true
            },
            City: {
                required: true
            },
            State: {
                required: true
            },
            PostalCode: {
                required: true
            },
            ContactFirstName: {
                required: true
            },
            ContactLastName: {
                required: true
            },
            ContactEmail: {
                required: true,
                email: true
            },
            ContactMobile: {
                required: {
                    depends: function (element) {
                        return $("#ContactTelephone").val() == '';
                    }
                } 
            },
            ContactTelephone : {
                required: {
                    depends: function (element) {
                        return $("#ContactMobile").val() == '';
                    }
                }
            }
        },
        submitHandler: function (form) {
            var buttonText = $("#btnSaveOrg").html();
            $("#btnSaveOrg").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

            $(form).ajaxSubmit({
                success: function (data) {
                    if (data && data.success) {
                        $("#modal-add-org-form").modal("hide");
                       
                        $("#modal-org-form-title").html("Add Organization");
                       
                        //refresh grid
                        var grid = $("#OrganizationListGrid").data("kendoGrid");
                        grid.dataSource.read();

                        VT.Util.Notification(true, "Organization has been successfully saved.");
                    } else {
                        VT.Util.HandleLogout(data.message);
                        $('#saveOrganizationForm .alert-danger').removeClass("hide").find(".error-message").html(data.message);
                        VT.Util.Notification(false, "Some error occured while saving current organization.");
                    }

                    $("#btnSaveOrg").attr('disabled', null).html(buttonText);
                    $(form).resetForm();
                    $("#OrganizationId").val(0);
                },
                error: function (xhr, status, error) {
                    $("#btnSaveOrg").attr('disabled', null).html(buttonText);
                }
            });
            return false;
        }
    });

    $("#modal-add-org-form").on('hidden.bs.modal', function () {
        $("#modal-org-form-title").html("Add Organization");
        saveOrganizationForm.resetForm();
        $("#OrganizationId").val(0);
        $(".error").removeClass("error");
    });

}).apply(this, [jQuery]);

//Sample complete ajax request
/*
$.ajax({
                type: "POST",
                url: "/ReportPortal1/DisplayPivotGrid",
                data: getSelectedFilterData(),
                processdata: false,
                async: true,
                beforeSend: function() {
                    showLoader(containerId);
                },
                success: function(viewResult) {
                    jQuery(containerId).html(viewResult);
                },
                complete: function() {
                    hideLoader(containerId);
                },
                error: function(xhr, status, error) {
                    var err = eval("(" + xhr.responseText + ")");
                    JMR.Util.ShowNotification("Error", err.Message, "cross");
                }
            });
*/