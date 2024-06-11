(function () {

    'use strict';

    //open password reset modal
    $(document).on('click', '.password-reset', function () {
        var id = $(this).data("id");
        $("#UserId").val(id);
        $("#modal-reset-password-form").modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });
        return false;
    });

    var isAdminChecked = function (isAdmin) { 
         
        if ($(isAdmin).is(":checked")) {
            $("#container").hide();
        } else {
            $("#container").show();
        }
    };
     
    $("#IsAdmin").click(function () {
        isAdminChecked(this);
    });

    $("#CompanyId").change(function() {
        var id = $(this).val();

        if (id > 0) {
            $.ajax({
                type: "POST",
                url: "/Users/GetCustomers/" + id,
                processdata: false,
                async: true,
                success: function (result) { //json

                    $("#su_multiselect_from_1").empty();

                    $.each(result, function (index, item) {
                        $("#su_multiselect_from_1").append($('<option>').text(item.Text).attr('value', item.Value));
                    });
                },
                error: function (xhr, status, error) {
                    VT.Util.Notification(false, error);
                }
            });
            return false;
        }
        return false;
    });

    //function to get permission details
    var getPermissionDetail = function(id) {
        $.ajax({
            type: "POST",
            url: "/Users/GetUserCustomerAccess/" + id,
            processdata: false,
            async: true,
            success: function(result) { //json

                $("#multiselect_from_1").empty();
                $("#multiselect_to_1").empty();

                $.each(result.FromList, function(index, item) {
                    $("#multiselect_from_1").append($('<option>').text(item.Text).attr('value', item.Value));
                });
                $.each(result.ToList, function(index, item) {
                    $("#multiselect_to_1").append($('<option>').text(item.Text).attr('value', item.Value));
                });

            },
            error: function(xhr, status, error) {
                VT.Util.Notification(false, error);
            }
        });
        return false;
    }; 

    $('#btnUp').click(function () {
        $('#multiselect_to_1 option:selected').each(function () {
            $(this).insertBefore($(this).prev());
        });
    });

    $('#btnDown').click(function () {
        $('#multiselect_to_1 option:selected').each(function () {
            $(this).insertAfter($(this).next());
        });
    });

    $('#subtnUp').click(function () {
        $('#su_multiselect_to_1 option:selected').each(function () {
            $(this).insertBefore($(this).prev());
        });
    });

    $('#subtnDown').click(function () {
        $('#su_multiselect_to_1 option:selected').each(function () {
            $(this).insertAfter($(this).next());
        });
    });

    //function to show save user modal during  edit
    var getUserDetail = function (id) {
        $.ajax({
            type: "POST",
            url: "/Users/GetUserDetail/" + id,
            processdata: false,
            async: true,
            success: function (result) { 
                $("#CompanyId").val(result.CompanyId);
                $("#FirstName").val(result.FirstName);
                $("#MiddleName").val(result.MiddleName);
                $("#LastName").val(result.LastName);
                $("#Username").val(result.Username);
                $("#IsAdmin").prop('checked', result.IsAdmin);
                $("#CompanyWorkerId").val(result.CompanyWorkerId);

                $("#modal-add-user-form").modal({
                    backdrop: 'static',
                    keyboard: false,
                    show: true
                });
                $("#modal-user-form-title").html("Edit Employee");
                $("#divUserDetails").html("");
                $("#passwordGroup").hide();
                $("#OriginalEmail").val(result.Username);

                $("#su_multiselect_from_1").empty();
                $("#su_multiselect_to_1").empty();

                $.each(result.FromList, function (index, item) {
                    $("#su_multiselect_from_1").append($('<option>').text(item.Text).attr('value', item.Value));
                });
                $.each(result.ToList, function (index, item) {
                    $("#su_multiselect_to_1").append($('<option>').text(item.Text).attr('value', item.Value));
                });

                isAdminChecked($("#IsAdmin"));
            },
            error: function (xhr, status, error) {
                VT.Util.Notification(false, error);
            }
        });
        return false;
    }

    //edit user
    $(document).on('click', '.edit-user', function () {
        var id = $(this).data("id"); 
        getUserDetail(id); 
    });

    //view user's detail
    $(document).on('click', '.view-user', function () {
        var id = $(this).data("id");
        $("#divUserDetails").html('<i class="fa fa-spinner fa-spin"></i>&nbsp; Please wait..');
        $.ajax({
            url: '/Users/GetUserAllDetails/' + id,
            type: 'POST',
            success: function (result) {
                $("#divUserDetails").html(result);
                $('html,body').animate({
                    scrollTop: $("#divUserDetails").offset().top
                }, 'slow');
            },
            error: function (xhr, status, error) {
                $("#divUserDetails").html('No data found.');
            }
        });
        return false;
    });

    //edit user
    $(document).on('click', '.customers-permission', function () {

        $('#left_All_1').trigger("click");

        var id = $(this).data("id");
        $("#CompanyWorkerUserId").val(id);

        $("#modal-customer-access-form").modal({
            backdrop: 'static',
            keyboard: false,
            show: true
        });

        getPermissionDetail(id);
        return false;
    });

    //delete single/multiple user(s) confirm modal
    $("#btnDeleteUser").click(function() {
        var checkedCount = $("#UsersListGrid input:checked").length;
        if (checkedCount > 0) {
            $("#delete-user-confirm-modal").modal({
                backdrop: 'static',
                keyboard: false,
                show: true
            });
        } else {
            VT.Util.Notification(false, "Please select at least one employee to delete.");
        }
    });
     
    //delete single/multiple user(s) submit
    $("#btnModalSubmit").click(function () {

        var buttonText = $("#btnModalSubmit").html();
        $("#btnModalSubmit").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

        var ids = [];
        $("#UsersListGrid input:checked").each(function () {
            ids.push($(this).val());
        });
        $.ajax({
            url: '/Users/DeleteUsers',
            type: "POST",
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({ Ids: ids }),
            success: function (result) {
                if (result.success) {
                    VT.Util.Notification(true, "Selected Employees have been successfully deleted.");
                    $("#delete-user-confirm-modal").modal("hide");
                    //refresh grid
                    var grid = $("#UsersListGrid").data("kendoGrid");
                    grid.dataSource.read();
                } else {
                    VT.Util.HandleLogout(result.message);
                    VT.Util.Notification(false, "Some error occured while deleting selected employees.");
                }

                $("#btnModalSubmit").attr('disabled', null).html(buttonText);
            },
            error: function (xhr, status, error) {
                VT.Util.Notification(false, error);
                $("#btnModalSubmit").attr('disabled', null).html(buttonText);
            }
        });
    });

    var isValidEmail = function (value, element) {
        var originalEmail = $("#OriginalEmail").val();

        if (value === originalEmail)
            return true;

        var response = $.ajax({
            url: "/Users/CheckEmail",
            type: "POST",
            async: false,
            data: {
                email: value
            }
        }).responseText;
        return response === "false";
    };

    $.validator.addMethod("IsEmailValid", isValidEmail, "Email already exists.");

    // basic
    var saveUserForm = $("#saveUserForm").validate({
        rules: {
            CompanyId: {
                required: true
            },
            FirstName: {
                required: true
            },
            LastName: {
                required: true
            },
            Username: {
                required: true,
                email: true,
                IsEmailValid: true
            },
            AuthKey: {
                required:
                {
                    depends: function () {
                        return $("#CompanyWorkerId").val() === 0;
                    }
                }
            },
            Confirm: {
                equalTo: "#AuthKey"
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
            /*if ($("#IsAdmin").is(":checked") == false) {
                var length = $('#su_multiselect_to_1 option').length;

                if (length === 0) {
                    VT.Util.Notification(false, "Please select at least one customer from left to right.");
                    return false;
                }
            }*/
            
            $('#su_multiselect_to_1 option').prop('selected', true);
            var buttonText = $("#btnSaveUser").html();

            $("#btnSaveUser").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

            $(form).ajaxSubmit({
                success: function (data) {
                    if (data && data.success) {
                        $("#btnSaveUser").attr('disabled', null).html('Submit');
                        $("#modal-add-user-form").modal("hide");
                        $("#modal-user-form-title").html("Add Employee");
                        //refresh grid
                        var grid = $("#UsersListGrid").data("kendoGrid");
                        grid.dataSource.read();
                        VT.Util.Notification(true, "User has been successfully saved.");
                    } else {
                        VT.Util.HandleLogout(data.message);
                        $('#saveUserForm .alert-danger').removeClass("hide").find(".error-message").html(data.message);
                        VT.Util.Notification(false, "Some error occured while saving current user.");
                    }
                    $(form).resetForm();
                },
                error: function (xhr, status, error) {
                    $("#btnSaveUser").attr('disabled', null).html(buttonText);
                }
            });
            return false;
        }
    });

    $('#modal-add-user-form').on('hidden.bs.modal', function () {
        $("#modal-user-form-title").html("Add Employee");
        saveUserForm.resetForm();
        $("#CompanyId").val(0);
        $("#CompanyWorkerId").val(0);
        $("#passwordGroup").show();
        $(".has-error").removeClass("has-error");
        $("#su_left_All_1").trigger("click");
        $("#container").show();
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
                    url: '/Users/VerifyImport',
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
                            columns: [
                                {
                                    field: "FirstName",
                                    title: "Name",
                                    template: "#: FirstName #" + " " + "#: MiddleName #" + " " + "#: LastName #"
                                }, {
                                    field: "Email",
                                    width: 250
                                }, {
                                    field: "Password"
                                }, {
                                    field: "Status"
                                }, {
                                    field: "Reason"
                                }]
                        });

                        if (result.Css === "danger") {
                            $("#btnImportSubmit").attr('disabled', '');
                        } else {
                            $("#btnImportSubmit").attr('disabled', null);
                        }

                    },
                    error: function (xhr, status, p3, p4) {
                        var err = "Error " + " " + status + " " + p3 + " " + p4;
                        if (xhr.responseText && xhr.responseText[0] === "{")
                            err = JSON.parse(xhr.responseText).Message;
                        console.log(err);
                    }
                });
            } else {
                alert("This browser doesn't support HTML5 file uploads!");
            }
        }
    });

    var importUsersForm = $("#importUsersForm").validate({
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
                        $("#modal-import-users").modal("hide");
                        //refresh grid
                        var grid = $("#UsersListGrid").data("kendoGrid");
                        grid.dataSource.read();
                        VT.Util.Notification(true, "User(s) has been imported saved.");
                    } else {
                        VT.Util.HandleLogout(data.message);
                        $('#btnImportSubmit .alert-danger').removeClass("hide").find(".error-message").html(data.message);
                        VT.Util.Notification(false, "Some error occured while saving current user.");
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

    var userAccessForm = $("#customerAccessForm").validate({
        rules: {
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

            $('#multiselect_to_1 option').prop('selected', true);

            var buttonText = $("#btnCustomerAccess").html();
            $("#btnCustomerAccess").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

            $(form).ajaxSubmit({
                success: function (data) {
                    if (data.success) {
                        VT.Util.Notification(true, "Permission for selected customers has been granted to this user.");
                        $("#modal-customer-access-form").modal("hide");
                    } else {
                        VT.Util.Notification(false, data.message);
                        VT.Util.HandleLogout(data.message);
                    }

                    $("#btnCustomerAccess").attr('disabled', null).html(buttonText);
                    $(form).resetForm();
                },
                error: function (xhr, status, error) {
                    $("#btnCustomerAccess").attr('disabled', null).html(buttonText);
                }
            });
            return false;
        }
    });

    $("#modal-import-users").on('hidden.bs.modal', function () {
        importUsersForm.resetForm();
        $("#messagebox").html("");
        $(".has-error").removeClass("has-error");
    });

    var resetPasswordForm = $("#resetPasswordForm").validate({
        rules: {
            NewPassword: {
                required: true
            },
            ConfirmPassword: {
                minlength: 5,
                equalTo: "#NewPassword"
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
            var buttonText = $("#btnResetPassword").html();
            $("#btnResetPassword").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

            $(form).ajaxSubmit({
                success: function (data) {
                    if (data && data.success) {
                        $("#modal-reset-password-form").modal("hide");
                        VT.Util.Notification(true, "Password has been successfully changed.");
                    } else {
                        VT.Util.HandleLogout(data.message);
                        $('#resetPasswordForm .alert-danger').removeClass("hide").find(".error-message").html(data.message);
                        VT.Util.Notification(false, "Some error occured while saving current organization.");
                    }
                    $("#btnResetPassword").attr('disabled', null).html(buttonText);
                    $(form).resetForm();
                },
                error: function (xhr, status, error) {
                    $("#btnResetPassword").attr('disabled', null).html(buttonText);
                }
            });
            return false;
        }
    });

    $("#modal-reset-password-form").on('hidden.bs.modal', function () {
        resetPasswordForm.resetForm();
        $(".has-error").removeClass("has-error");
    });

    $("#modal-customer-access-form").on('hidden.bs.modal', function () {
        userAccessForm.resetForm();
        $("#CompanyWorkerUserId").val('');
        $(".has-error").removeClass("has-error");
        $("#left_All_1").trigger("click");
    });


}).apply(this, [jQuery]);