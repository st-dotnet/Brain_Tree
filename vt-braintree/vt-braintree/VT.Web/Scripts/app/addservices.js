(function() {

    'use strict';

    // See the Configuring section to configure credentials in the SDK

    AWS.config.update({ accessKeyId: 'AKIAIM5TSADPNSAXTKBA', secretAccessKey: 'ycW5cR4QMBqhdzPHxmeu5F8ZW36HZTuD7lfdGMJL' });

    // Configure your region
    AWS.config.region = 'us-east-1';

    var bucket = new AWS.S3({ params: { Bucket: 'verifyteck' } });

    $("#CustomerServiceId").change(function() {
        if ($(this).val() == "" || $(this).val() == "-1") {
            if ($(this).val() == "-1") {
                $("#Name").val("Non-Standard");
                $("#Cost").val('');
                $("#CompanyServiceId").val('');
            }
            return false;
        }
        $.ajax({
            url: "/CompanyUser/GetCustomerServiceDetail/" + $(this).val(),
            type: "POST",
            success: function(result) {
                $("#Name").val(result.name);
                $("textarea#Description").val(result.description);
                $("#Cost").val(result.cost);
                $("#CompanyServiceId").val(result.companyServiceId);
            }
        });
        return false;
    });


    var addServicesForm = $("#addServicesForm").validate({
        rules: {
            CustomerServiceId: {
                required: true
            },
            Description: {
                required: true
            },
        },
        submitHandler: function(form) {
            var buttonText = $("#btnSaveService").html();
            $("#btnSaveService").attr('disabled', '').html('<i class="fa fa-spinner fa-spin"></i>&nbsp; ' + buttonText);

            $(form).ajaxSubmit({
                success: function(data) {
                    debugger;
                    if (data && data.success) {
                        //$("#modal-add-service").modal("hide");
                        $('#resultsAfter').empty();
                        $("#ImageFileNameBefore").val('');
                        //refresh grid
                        //var grid = $("#ServiceRecordItemListGrid").data("kendoGrid");
                        //grid.dataSource.read();
                        VT.Util.Notification(true, "Service item has been successfully saved.");

                    } else {
                        VT.Util.HandleLogout(data.message);
                        $('#addServicesForm .alert-danger').removeClass("hide").find(".error-message").html(data.message);
                        VT.Util.Notification(false, "Some error occured while saving service item.");
                    }

                    $("#btnSaveService").attr('disabled', null).html(buttonText);
                    $(form).resetForm();

                    window.location.href = $("#redirectUrl").val();
                },
                error: function(xhr, status, error) {
                    $("#btnSaveService").attr('disabled', null).html(buttonText);
                }
            });
            return false;
        }
    });
    
    $("#btnAddServiceWithoutImg").click(function () {
        $("#addServicesForm").submit();
    });
    
    $(document).on("click", ".delete-image", function () {
        debugger;

        $("#ImageFileNameAfter").val('');
        $("#imgSize").html('');
        $("#resultsAfter").html('<p class="text-center"><i style="font-size:30px" class="fa fa-spinner fa-spin"></i></p>');
        var btn = $(this);
        var key = $(this).data("key");
        
        var params = {
            Bucket: 'verifyteck',  
            Delete: { 
                Objects: [  
                  {
                      Key: key
                  }
                ]
            }
        };

        bucket.deleteObjects(params, function (err, data) {
            debugger;
            if (err) {
                console.log(err, err.stack);
            } else {
                console.log(data);
                $("#bucketKey").val('');
                $("#resultsAfter").html('');
                $(btn).closest("tr").remove();
            }
        });
    });

    $("#modal-add-service").on('hidden.bs.modal', function () {
        addServicesForm.resetForm();
        $(".error").removeClass("error");
    });

    $("#btnCameraBefore").click(function () {
        $('#captureBefore').trigger("click");
    });

    $("#btnCameraAfter").click(function () {
        $('#captureAfter').trigger("click");
    });

    function resetImage(e) {
        e.wrap('<form>').closest('form').get(0).reset();
        e.unwrap();

        // Prevent form submission
        e.stopPropagation();
        e.preventDefault();
    }

    $('input[name=photo]').change(function (e) {
        debugger;
        var file = e.target.files[0];
        var w = parseInt($("#ImageWidth").val());
        var h = parseInt($("#ImageHeight").val());
        var q = parseInt($("#ImageQuality").val());
        var c = $("#ImageCrop").val() == "true";

        $("#resultsAfter").html('<p class="text-center"><i style="font-size:30px" class="fa fa-spinner fa-spin"></i></p>');

        // CANVAS RESIZING
        $.canvasResize(file, {
            width: w, //TODO : use configuration 
            height: h,
            crop: c,
            quality: q,
            callback: function (data, width, height) {

                // Create a new formdata
                //var fd = new FormData(); 

                var f = $.canvasResize('dataURLtoBlob', data);
                f.name = file.name;


                var uploadInProgress = true;
                
                $.ajax({
                    url: '/CompanyUser/GetImageName',
                    type: 'POST',
                    data: { FileName: file.name, CustomerId: $("#CustomerId").val() },
                    success: function (result) {
                        debugger;
                        if (f) {
                            var fileName = result.fileName;
                            var type = f.type;

                            if (type.indexOf("jpeg") != -1) {
                                fileName += ".jpg";
                            }
                            else if (type.indexOf("png") != -1) {
                                fileName += ".png";
                            }
                            else if (type.indexOf("gif") != -1) {
                                fileName += ".gif";
                            }
                            else if (type.indexOf("bmp") != -1) {
                                fileName += ".bmp";
                            } else {
                                fileName += ".jpg";
                            }

                            var key = "workflowimages/" + fileName;
                            $("#ImageFileNameAfter").val(fileName);
                            $("#bucketKey").val(key);
                             
                            var params = {
                                Key: key,
                                Body: f,
                                Acl: 'public-read'
                            };
                            bucket.upload(params, function (err, res) {
                                debugger;
                                VT.Util.Notification(err == null, err == null ? "File uploaded." : "File couldn't upload");
                                if (err == null) {
                                    var imageUrl = "http://verifyteck.s3.amazonaws.com/" + key;

                                    $("#resultsAfter").html('');

                                    $("#tblImages > tbody").append("<tr><td> <img width='75' src='" + imageUrl + "' alt='" + fileName + "' />" +
                                        " </td><td>" + parseFloat(f.size / 1024).toFixed(2) + "K</td><td><a href='#' class='delete-image' data-key='"+ key +"'><i class='fa fa-trash'></i></a>" +
                                        "<input type='hidden' name='UploadImages' data-key='" + key + "' value='" + fileName + "'/> </td></tr>");

                                    resetImage($("#captureAfter"));

                                    uploadInProgress = false;
                                }
                            });
                        } else {
                            $("#resultsAfter").html('Nothing to upload.');
                        }
                    }
                });
            }
        });

    });

    
    $("#btnAnotherCancel").click(function () {
        debugger;
        var url = $(this).data("url");
        $.ajax({
            url: '/CompanyUser/GetKeys',
            type: 'POST',
            success: function (result) {
                debugger;
                if (result.length == 0) {
                    window.location.href = url;
                } else {
                    var objects = [];

                    for (var a = 1; a <= result.length; a++) {
                        objects.push({ Key: result[a - 1] });
                    }

                    var params = {
                        Bucket: 'verifyteck',
                        Delete: { Objects: objects }
                    };

                    bucket.deleteObjects(params, function (err, data) {
                        debugger;
                        if (err) {
                            console.log(err, err.stack);
                        } else {
                            console.log(data);
                        }

                        window.location.href = url;
                    });
                }
            }
        });

    });

    $("#btnCancel").click(function () {
        debugger;
        var url = $(this).data("url");

        var objects = [];

        $("#tblImages > tbody").find("input").each(function () {
            var keyValue = $(this).data("key");
            objects.push({ Key: keyValue });
        });

        if (objects.length == 0) {
            window.location.href = url;
        } else {
            var params = {
                Bucket: 'verifyteck',
                Delete: { Objects: objects }
            };
            bucket.deleteObjects(params, function (err, data) {
                debugger;
                if (err) {
                    console.log(err, err.stack);
                } else {
                    console.log(data);
                }
                window.location.href = url;
            });
        }
    });


}).apply(this, [jQuery]);

