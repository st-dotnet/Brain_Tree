(function () {
    jQuery(function () {
        var delay;
        delay = function (ms, func) {
            return setTimeout(func, ms);
        };

        $('a.logout').click(function (ev) {
            var logoutUrl;
            ev.preventDefault();
            logoutUrl = $(this).attr("href");
            return VT.Util.ConfirmModal("Please Confirm", "Are you sure you want to log out?", function () {
                return window.location = logoutUrl;
            });
        });

        if (jQuery.validator) {
            return jQuery.validator.setDefaults({
                showErrors: function (errorMap, errorList) {
                    $.each(this.successList, function (index, value) {
                        return $(value).closest(".control-group").removeClass("error").find(".input-error").remove();
                    });
                    return $.each(errorList, function (index, value) {
                        var $element;
                        $element = void 0;
                        $element = $(value.element);
                        return $element.closest(".control-group").removeClass("success").addClass("error").find(".input-error").remove();
                    });
                },
                highlight: function (element) {
                    $(element).closest(".help-inline").removeClass("ok");
                    return $(element).closest(".control-group").removeClass("success").addClass("error");
                },
                unhighlight: function (element) {
                    return $(element).closest(".control-group").removeClass("error");
                },
                invalidHandler: function (form, validator) {
                    return $(this).find(".form-feedback.error").removeClass("hide");
                }
            });
        }
    });


    VT.Util.GetUuid = function () {
        var d = new Date().getTime();
        var uuid = 'xxxxxxxxxxxx4xxxyxxxxxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = (d + Math.random() * 16) % 16 | 0;
            d = Math.floor(d / 16);
            return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
        });
        return uuid;
    };

    VT.Util.ConnectKendoGrids = function () {
        $("input[type='search'][data-grid]").each(function (e) {
            var grid, id;
            id = "#" + $(this).attr("id");
            grid = "#" + $(this).data("grid");
            return VT.Util.ConnectKendoGridSearch(id, grid);
        });

        $("#gridOrganization").change(function () {
            var grid;
            grid = $("#" + $(this).data("grid")).data("kendoGrid");
            $filter = new Array();

            var type = $(this).data("dd");

            if ($(this).val().length > 0) {
                if (type == "organizations") {
                    $filter.push({ field: "CompanyId", operator: "eq", value: $(this).val() });
                } else {
                    $filter.push({ field: "CustomerId", operator: "eq", value: $(this).val() });
                }
            }

            grid.dataSource.filter($filter);

            if ($("#divSummary").length > 0) {
                $("#divSummary").addClass("hidden");
            }

            return false;
        });

        return $("#gridPageSize").change(function () {
            var grid;
            grid = $("#" + $(this).data("grid")).data("kendoGrid");
            return grid.dataSource.pageSize($(this).val());
        });
    };

    VT.Util.ConnectKendoGridSearch = function (searchBoxId, gridId) {
        $(searchBoxId).typeWatch({
            captureLength: 1,
            callback: function (value) {
                var andfilter, c, column, kgrid, orfilter, selectedArray, selecteditem, stringColumns;
                kgrid = $(gridId).data("kendoGrid");
                stringColumns = [];
                for (column in kgrid.dataSource.options.schema.model.fields) {
                    c = kgrid.dataSource.options.schema.model.fields[column];
                    if (c.type === "string") {
                        stringColumns.push(column);
                    }
                }
                selecteditem = $(searchBoxId).val().toUpperCase();
                selectedArray = selecteditem.split(" ");
                if (selecteditem) {
                    orfilter = {
                        logic: "or",
                        filters: []
                    };
                    andfilter = {
                        logic: "and",
                        filters: []
                    };
                    $.each(selectedArray, function (i, v) {
                        if (v.trim() !== "") {
                            $.each(selectedArray, function (i, v1) {
                                var col;
                                if (v1.trim() !== "") {
                                    for (col in stringColumns) {
                                        orfilter.filters.push({
                                            field: stringColumns[col],
                                            operator: "contains",
                                            value: v1
                                        });
                                    }
                                    andfilter.filters.push(orfilter);
                                    orfilter = {
                                        logic: "or",
                                        filters: []
                                    };
                                }
                            });
                        }
                    });
                    kgrid.dataSource.filter(andfilter);
                } else {
                    kgrid.dataSource.filter({});
                }
            }
        });
    };

    VT.Util.SelectTab = function (selector, index) {
        return $(selector).removeClass("active").filter(":eq(" + index + ")").addClass("active");
    };

    VT.Util.RefreshUI = function (id) {
        $(id).find("input[type='radio'], input[type='checkbox']").uniform();
        return $(id).find(".date-picker").datepicker();
    };

    VT.Util.IsEmail = function (email) {
        var regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        if (!regex.test(email)) {
            return false;
        } else {
            return true;
        }
    }

    VT.Util.LoadUrl = function (id, url, callback) {
        var $container;
        $container = $(id);
        $container.empty();
        return $.post(url, function (data) {
            $container.html(data);
            VT.Util.RefreshUI($container);
            return typeof callback === "function" ? callback() : void 0;
        });
    };

    VT.Util.SelectTabAndLoadUrl = function (tabContainer, tabIndex, contentContainer, contentUrl, callback) {
        VT.Util.SelectTab(tabContainer, tabIndex);
        return VT.Util.LoadUrl(contentContainer, contentUrl, callback);
    };

    VT.Util.ConfirmModal = function (header, text, action) {
        var $modal;
        $modal = $("#ConfirmationModal");
        $modal.find("#ConfirmationModalHeader").html(header);
        $modal.find("#ConfirmationModalText").html(text);
        $modal.find(".confirm").unbind('click').click(function () {
            var _ref;
            return (_ref = typeof action === "function" ? action() : void 0) != null ? _ref : true;
        });
        return $modal.modal('show');
    };

    VT.Util.SetupForm = function (form, rules, success, error) {
        return $(form).validate({
            rules: rules || {},
            submitHandler: function (f) {
                var submitButton;
                submitButton = $(f).find("button[type='submit']");
                if (!(typeof submitButton.data === "function" ? submitButton.data("loadingText") : void 0)) {
                    submitButton.data("loadingText", "Saving...");
                }
                submitButton.button("loading");
                $(f).ajaxSubmit({
                    success: function (data) {
                        if (data && data.success) {
                            if (typeof success === "function") {
                                success(data);
                            }
                        } else {
                            if (typeof error === "function") {
                                error(data);
                            }
                        }
                        return submitButton.button("reset");
                    }
                });
                return false;
            }
        });
    };

    VT.Util.ShowNotification = function (title, text, icon) {
        return $.gritter.add({
            title: title,
            text: text,
            image: icon ? "/Assets/img/icons/" + icon + ".png" : void 0,
            sticky: false,
            time: 6000
        });
    };

    VT.Util.MakeDataTable = function (selector, options) {
        var defaults, settings;
        defaults = {
            aLengthMenu: [[10, 20, 30, -1], [10, 20, 30, "All"]],
            iDisplayLength: 10,
            sDom: "<'row-fluid'<'span6'l><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
            sPaginationType: "bootstrap",
            oLanguage: {
                sLengthMenu: "_MENU_ per page",
                oPaginate: {
                    sPrevious: "Prev",
                    sNext: "Next"
                }
            },
            aoColumnDefs: [
              {
                  bSortable: false,
                  aTargets: [-1]
              }
            ]
        };
        settings = $.extend({}, defaults, options);
        return $(selector).dataTable(settings);
    };

    VT.Util.DisplayActiveKendoGrids = function () {
        var selectedStatus;
        selectedStatus = $("#gridActiveDisplay").val();
        return {
            status: selectedStatus
        };
    };

    VT.Util.ResyncNumbers = function (selector) {
        return $(selector).each(function (counter) {
            return $(this).find("input").each(function () {
                return VT.Util.ReplacePropVal($(this), "name", counter);
            });
        });
    };

    VT.Util.IsTextSelected = function (input) {
        var startPos = input.selectionStart;
        var endPos = input.selectionEnd;
        var doc = document.selection;

        if (doc && doc.createRange().text.length != 0) {
            return true;
        } else if (!doc && input.value.substring(startPos, endPos).length != 0) {
            return true;
        }
        return false;
    };

    VT.Util.FillDropdown = function(dd, url) {
        var $dropdown = $(dd);
        $dropdown.empty();
        return $.post(url, function (result) {
            $.each(result, function (index, item) {
                $dropdown.append($('<option>').text(item.Text).attr('value', item.Value));
            });
        });
    };


    VT.Util.CustomMask = function (selector, digits) { // digits : number of decimal places
        $(selector).on("keypress", function (e) {

            var charCode = (e.which) ? e.which : event.keyCode;

            //If input text is selected, then overwrite the text with key pressed
            var isSelected = VT.Util.IsTextSelected($(this)[0]);
            if (isSelected == true) {
                if (charCode != 9) {
                    $(this).val('');
                }
            }

            var keyVal = $(e.originalTarget).val();
            var existingVal = $(this).val();



            // Check for plus and minus
            if (charCode == 43 || charCode == 45) {
                return (existingVal.indexOf("+") == -1 && existingVal.indexOf("-") == -1);
            }

            // Check for numerics
            if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57))
                return false;

            // Check for decimal
            if (charCode == 46) {
                if (existingVal.indexOf('.') != -1) {
                    return false;
                }
                if (keyVal.length == 0) {
                    return false;
                }
            }

            // Check for decimal places
            if (charCode != 8) {
                if (existingVal.indexOf('.') != -1) {
                    var arr = existingVal.split('.');
                    if (arr[1].length > (digits - 1))
                        return false;
                }
            }

            return true;
        });
    };

    VT.Util.ClearAllKendoGridFilter = function (gridId) { 
        //$("form.k-filter-menu button[type='reset']").trigger("click");
        $(gridId).data('kendoGrid').dataSource.filter({});

        if ($("#divSummary").length > 0) {
            $("#divSummary").addClass("hidden");
        }
        
        return false;
    };

    VT.Util.ReplacePropVal = function (elem, prop, index) {
        var val;
        val = elem.attr(prop);
        if (typeof val !== "undefined") {
            val = val.replace(/_[0-9+]__/g, "_" + index + "__").replace(/\[[0-9+]\]/g, "[" + index + "]").replace(/\_[0-9+]\__/g, "_" + index + "__").replace(/#[0-9+]/g, "#" + index);
            return elem.attr(prop, val);
        } else {
            return null;
        }
    };

    VT.Util.Notification = function (success, message) {
        if (success) {
            toastr.success(message);
        } else {
            toastr.warning(message);
        }
    };

    VT.Util.HandleLogout = function (message) {
        if (message == "logout") {
            window.location.href = "/Logout";
        }
    };

    VT.Util.RedirectPageTo = function (url) {
        setTimeout(function () {
            window.location.href = url;
        }, 3000);
    };

}).call(this);
