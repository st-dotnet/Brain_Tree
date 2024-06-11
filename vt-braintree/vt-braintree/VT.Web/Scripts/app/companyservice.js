(function () {

    'use strict';

    $("#btnDeleteCs").click(function () {

        var checkedCount = $("#CompanyServiceListGrid input:checked").length;
        if (checkedCount > 0) {
            $("#modal-del-cs-form").show();
            $("#modal-del-cs-form").modal({
                backdrop: 'static',
                keyboard: false,
                show: true
            });
        } else {
            VT.Util.Notification(false, "Please select at least one user to delete.");
        }
    });

    $("#btnModalSubmit").click(function () {
        debugger;
        var buttonText = $("#btnModalSubmit").html();
        $("#btnModalSubmit").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

        var ids = [];
        $("#CompanyServiceListGrid input:checked").each(function () {
            ids.push($(this).val());
        });
        $.ajax({
            url: '/CompanyServices/DeleteCompanyServices',
            type: "POST",
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({ Ids: ids }),
            success: function (result) {
                debugger;
                if (result.success) {
                    VT.Util.Notification(true, "Selected CompanyService(s) have been successfully deleted.");
                    //refresh grid
                    var grid = $("#CompanyServiceListGrid").data("kendoGrid");
                    grid.dataSource.read();
                } else {
                    VT.Util.HandleLogout(result.message);
                    VT.Util.Notification(false, result.message);
                }

                $("#modal-del-cs-form").modal("hide");
                $("#btnModalSubmit").attr('disabled', null).html('Submit');
            },
            error: function (xhr, status, error) {
                VT.Util.Notification(false, error);
                $("#btnModalSubmit").attr('disabled', null).html('Submit');
            }
        });
    });

    $(document).on('click', '.edit-companyservice', function () {
        debugger;
        var id = $(this).data("id");
        $.ajax({
            url: '/CompanyServices/GetCompanyService/' + id,
            type: 'POST',
            success: function (result) {
                debugger;
                if (result && result.success == true) {

                    $("#CompanyServiceId").val(result.cs.Id);
                    $("#Name").val(result.cs.Name);
                    $("#Description").val(result.cs.Description);
                    $("#CompanyId").val(result.cs.CompanyId);

                    $("#modal-save-cs-title").html("Edit Company Service");
                    $("#modal-add-cs-form").modal({
                        backdrop: 'static',
                        keyboard: false,
                        show: true
                    });

                } else {
                    VT.Util.HandleLogout(result.message);
                    VT.Util.Notification(false, result.message);
                }
            }
        });
        return false;
    });

    $(document).on('click', '.view-companyservice', function () {
        var id = $(this).data("id");
        $("#divcsDetails").html('<i class="fa fa-spinner fa-spin"></i>&nbsp; Please wait..');
        $.ajax({
            url: '/CompanyServices/GetCompanyServiceDetail/' + id,
            type: 'POST',
            success: function (result) {
                $("#divcsDetails").html(result);
                $('html,body').animate({
                    scrollTop: $("#divcsDetails").offset().top
                }, 'slow');
            },
            error: function (xhr, status, error) {
                $("#divcsDetails").html('No data found.');
            }
        });
        return false;
    });

    // basic
    var saveCompanyServiceForm  = $("#saveCompanyServiceForm").validate({
        rules: {
            CompanyId : {
                required: true
            },
            Name: {
                required: true
            },
            Description: {
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
            var buttonText = $("#btnSaveCompanyService").html();
            $("#btnSaveCompanyService").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

            $(form).ajaxSubmit({
                success: function (data) {
                    if (data && data.success) {
                        $("#modal-add-cs-form").modal("hide");
                       
                        $("#modal-save-cs-title").html("Add Company Service");
                        //refresh grid
                        var grid = $("#CompanyServiceListGrid").data("kendoGrid");
                        grid.dataSource.read();

                        VT.Util.Notification(true, "Company Service has been successfully saved.");
                    } else {
                        VT.Util.HandleLogout(data.message);
                        $('#saveCompanyServiceForm .alert-danger').removeClass("hide").find(".error-message").html(data.message);
                        VT.Util.Notification(false, "Some error occured while saving current company service.");
                    }

                    $("#btnSaveCompanyService").attr('disabled', null).html(buttonText);
                    $(form).resetForm();
                    $("#CompanyServiceId").val(0);
                },
                error: function (xhr, status, error) {
                    $("#btnSaveCompanyService").attr('disabled', null).html(buttonText);
                }
            });
            return false;
        }
    });

    $("#modal-add-cs-form").on('hidden.bs.modal', function () {
        $("#modal-save-cs-title").html("Add Company Service");
        saveCompanyServiceForm.resetForm();
        $("#CompanyServiceId").val(0);
        $(".has-error").removeClass("has-error");
    });

}).apply(this, [jQuery]);