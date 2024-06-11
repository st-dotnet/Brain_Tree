(function () {

    'use strict';

    //Disabled for now ToDo: Change after payment method is activated
    //var elem = document.querySelector('.js-switch');//
    //var switchery;

    //if (typeof elem != 'undefined') {
    //    switchery = new Switchery(elem, { color: '#1AB394' });
    //}



    var customerPopup = function (id) {
        $("#modal-save-customer-title").html("Add Customer");
        $("#modal-add-customer-form").modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
        if (id > 0) {
            fillCustomerServiceGrid(id, 0);
        } else {
            $("#customerServiceGrid").html('');
        }

        return false;
    }

    var fillCustomerServiceGrid = function (companyId, customerId) {
        $.ajax({
            url: '/Customers/GetCustomerServicesGrid',
            data: { CompanyId: companyId, CustomerId: customerId },
            type: "POST",
            success: function (result) {
                $("#customerServiceGrid").html(result);
            },
            error: function (xhr, status, error) {
                VT.Util.Notification(false, error);
            }
        });
    };

    $(document).on('change', '#CompanyId', function () {
        var companyId = $(this).val();
        if (companyId.length == 0) return false;

        var customerId = $("#CustomerId").val();
        fillCustomerServiceGrid(companyId, customerId);
        return false;
    });

    $(document).on('click', '.sendEmail', function () {
        var customerId = $(this).data("id");
        $("#modal-send-email").find("input").val(customerId);
        $("#modal-send-email").modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
        return false;
    });

    $("#btnSendEmail").click(function () {
        var buttonText = $(this).html();
        $(this).attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

        $.ajax({
            url: '/Customers/SendEmail',
            type: "POST",
            data: { CustomerId: $("#modal-send-email").find("input").val() },
            success: function (result) {
                if (result.success) {
                    VT.Util.Notification(true, "Email has been successfully sent.");
                    $("#modal-send-email").modal("hide");
                } else {
                    VT.Util.HandleLogout(data.message);
                    VT.Util.Notification(false, result.message);
                }
                $("#btnSendEmail").attr('disabled', null).html('Submit');
            },
            error: function (xhr, status, error) {
                VT.Util.Notification(false, error);
                $("#btnSendEmail").attr('disabled', null).html('Submit');
            }
        });
    });

    $("#btnDeleteCustomer").click(function () {

        var checkedCount = $("#CustomerListGrid input:checked").length;
        if (checkedCount > 0) {
            $("#modal-del-customer-form").show();
            $("#modal-del-customer-form").modal({
                backdrop: 'static',
                keyboard: false,
                show: true
            });
        } else {
            VT.Util.Notification(false, "Please select at least one customer to delete.");
        }
    });

    $("#btnModalSubmit").click(function () {
        var buttonText = $("#btnModalSubmit").html();
        $("#btnModalSubmit").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

        var ids = [];
        $("#CustomerListGrid input:checked").each(function () {
            ids.push($(this).val());
        });
        $.ajax({
            url: '/Customers/DeleteCustomers',
            type: "POST",
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({ Ids: ids }),
            success: function (result) {
                debugger;
                if (result.success) {
                    VT.Util.Notification(true, "Selected Customer(s) have been successfully deleted.");
                    $("#modal-del-customer-form").modal("hide");
                    //refresh grid
                    var grid = $("#CustomerListGrid").data("kendoGrid");
                    grid.dataSource.read();
                } else {
                    VT.Util.HandleLogout(data.message);
                    VT.Util.Notification(false, "Some error occured while deleting selected customers.");
                }

                $("#btnModalSubmit").attr('disabled', null).html('Submit');
            },
            error: function (xhr, status, error) {
                VT.Util.Notification(false, error);
                $("#btnModalSubmit").attr('disabled', null).html('Submit');
            }
        });
    });

    $(document).on('click', '.edit-customer', function () {
        var id = $(this).data("id");
        $.ajax({
            url: '/Customers/EditCustomer/' + id,
            type: 'POST',
            success: function (result) {
                debugger;
                $("#CustomerId").val(result.CustomerId);
                $("#CompanyId").val(result.CompanyId);
                $("#Name").val(result.Name);
                $("#Address").val(result.Address);
                $("#City").val(result.City);
                $("#State").val(result.State);
                $("#Country").val(result.Country);
                $("#PostalCode").val(result.PostalCode);

                $("#ContactFirstName").val(result.ContactFirstName);
                $("#ContactMiddleName").val(result.ContactMiddleName);
                $("#ContactLastName").val(result.ContactLastName);
                $("#ContactEmail").val(result.ContactEmail);
                $("#ContactMobile").val(result.ContactMobile);
                $("#ContactTelephone").val(result.ContactTelephone);
                //$("#IsActive").val(result.IsActive);

                if (result.HasGatewayCustomer) {
                    $("#divIsActive").removeClass("hide");
                }

                if (result.IsActive == true) {
                    $('#IsActive').prop('checked', true);
                } else {
                    $('#IsActive').prop('checked', false);
                }

                $("#modal-save-customer-title").html("Edit Customer");
                $("#modal-add-customer-form").modal({
                    backdrop: 'static',
                    keyboard: false,
                    show: true
                });

                var companyId = $("#CompanyId").val();
                var customerId = $("#CustomerId").val();
                fillCustomerServiceGrid(companyId, customerId);
            }
        });
        return false;
    });

    $(document).on('click', '.view-customer', function () {
        var id = $(this).data("id");
        $("#divCustomerDetails").html('<i class="fa fa-spinner fa-spin"></i>&nbsp; Please wait..');
        $.ajax({
            url: '/Customers/GetCustomerDetail/' + id,
            type: 'POST',
            success: function (result) {
                $("#divCustomerDetails").html(result);
                $('html,body').animate({
                    scrollTop: $("#divCustomerDetails").offset().top
                }, 'slow');
            },
            error: function (xhr, status, error) {
                $("#divCustomerDetails").html('No data found.');
            }
        });
        return false;
    });

    $(document).on('click', '.manage-services', function () {
        debugger;
        var id = $(this).data("id");
        var companyId = $(this).data("compid");

        $.ajax({
            url: '/Customers/GetCustomerServicesDetail',
            data: { CustomerId: id, CompanyId: companyId },
            type: 'POST',
            success: function (result) {
                $("#modal-manage-services-form").html(result);
                $("#modal-manage-services-form").modal({
                    backdrop: 'static',
                    keyboard: false,
                    show: true
                });
            },
            error: function (xhr, status, error) {
                VT.Util.Notification(false, "Some error occured while pulling information.");
            }
        });
        return false;
    });

    var saveCustomerForm = $("#saveCustomerForm").validate({
        rules: {
            Name: {
                required: true
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
            ContactCountry: {
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
            ContactTelephone: {
                required: {
                    depends: function (element) {
                        return $("#ContactMobile").val() == '';
                    }
                }
            },
            CompanyId: {
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

                    debugger;
                    if (data && data.success) {
                        $("#modal-add-customer-form").modal("hide");
                        //refresh grid
                        var grid = $("#CustomerListGrid").data("kendoGrid");
                        grid.dataSource.read();

                        VT.Util.Notification(true, "Customer has been successfully saved.");
                    } else {
                        VT.Util.HandleLogout(data.message);
                        $('#btnSaveCustomer .alert-danger').removeClass("hide").find(".error-message").html(data.message);
                        VT.Util.Notification(false, "Some error occured while saving current customer.");
                    }

                    $("#btnSaveCustomer").attr('disabled', null).html(buttonText);
                    $(form).resetForm();
                    //$("#CompanyId").val('');
                    $("#CustomerId").val(0);
                },
                error: function (xhr, status, error) {
                    $("#btnSaveCustomer").attr('disabled', null).html(buttonText);
                }
            });
            return false;
        }
    });

    $("#modal-add-customer-form").on('hidden.bs.modal', function () {
        saveCustomerForm.resetForm();
        $("#CustomerId").val(0);
        $("#divIsActive").addClass("hide");
        $(".has-error").removeClass("has-error");
    });



    $("#btnAddCustomer").click(function () {
        $("#CustomerId").val(0);

        var companyId = $("#CompanyId").val();
        saveCustomerForm.resetForm();
        $.ajax({
            url: '/Customers/AddCustomer/',
            type: 'POST',
            success: function (result) {
                customerPopup(companyId);
                $('#CompanyId').get(0).selectedIndex = 0;
            }
        });
    });

    $('#uploadCsv').on('change', function (e) {
        var files = e.target.files;
        if (files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (var x = 0; x < files.length; x++) {
                    data.append("file" + x, files[x]);
                }

                $.ajax({
                    type: "POST",
                    url: '/Customers/VerifyImport',
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (result) {

                        $("#messagebox").html(result.Message);
                        $("#messagebox").removeClass().addClass("alert alert-" + result.Css);

                        //create dynamic grid
                        $("#grid").kendoGrid({
                            dataSource: result.Data,
                            height: 250,
                            filterable: true,
                            sortable: true,
                            columns: [{
                                field: "CustomerName",
                                title: "Customer Name"
                            }, {
                                field: "Address",
                                template: "#: Address #" + "," + "#: City #" + " " + "#: State #" + " " + "#: PostalCode #" + " " + "#: Country #"
                            }, {
                                field: "FirstName",
                                title: "Contact Name",
                                template: "#: FirstName #" + "," + "#: MiddleName #" + " " + "#: LastName #"
                            }, {
                                field: "Email"
                            }, {
                                field: "Telephone"
                            }, {
                                field: "Mobile"
                            }, {
                                field: "Status"
                            }, {
                                field: "Reason"
                            }]
                        });

                        if (result.Css == "danger") {
                            $("#btnImportSubmit").attr('disabled', '');
                        } else {
                            $("#btnImportSubmit").attr('disabled', null);
                        }

                    },
                    error: function (xhr, status, p3, p4) {
                        var err = "Error " + " " + status + " " + p3 + " " + p4;
                        if (xhr.responseText && xhr.responseText[0] == "{")
                            err = JSON.parse(xhr.responseText).Message;
                        console.log(err);
                    }
                });
            } else {
                alert("This browser doesn't support HTML5 file uploads!");
            }
        }
    });

    var importCustomerForm = $("#importCustomerForm").validate({
        rules: {
            CompanyId: {
                required: true
            },
            uploadCsv: {
                required: true,
                extension: "csv"
            },
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
            var buttonText = $("#btnImportSubmit").html();

            $("#btnImportSubmit").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

            $(form).ajaxSubmit({
                success: function (data) {
                    if (data.success) {
                        $("#btnImportSubmit").attr('disabled', null).html('Submit');
                        $("#modal-import-customers").modal("hide");
                        //refresh grid
                        var grid = $("#CustomerListGrid").data("kendoGrid");
                        grid.dataSource.read();
                        VT.Util.Notification(true, "Customer(s) has been successfully imported.");
                    } else {
                        VT.Util.HandleLogout(data.message);
                        $('#btnImportSubmit .alert-danger').removeClass("hide").find(".error-message").html(data.message);
                        VT.Util.Notification(false, "Some error occured while executing import process.");
                    }
                    $(form).resetForm();
                },
                error: function (xhr, status, error) {
                    $("#btnImportSubmit").attr('disabled', null).html(buttonText);
                }
            });
            return false;
        }
    });

    $("#modal-import-customers").on('hidden.bs.modal', function () {
        importCustomerForm.resetForm();
        $("#messagebox").html('');
        $("#messagebox").removeClass();
        $("#grid").html('');
        $(".has-error").removeClass("has-error");
    });

}).apply(this, [jQuery]);