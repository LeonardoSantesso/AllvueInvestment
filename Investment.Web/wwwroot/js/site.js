
// Default for value
function DefaultFor(arg, val) {
    return typeof arg != 'undefined' && arg != undefined ? arg : val;
}

// function that creates a modal screen
// Sizes : modal-sm - modal-lg - modal-xl
function CreateModal(idModal, url, size, options) {
    options = options || {};
    var fnClose = DefaultFor(options.fnClose, null);
    var classModal = DefaultFor(size, 'modal-lg');

    $("#" + idModal).remove();

    $("#allModals").append(`
        <div id="${idModal}" class="modal fade" tabindex="-1" role="dialog" aria-hidden="true">
            <div class="modal-dialog ${classModal}" role="document">
            </div>
        </div>
    `);

    $("#" + idModal + " .modal-dialog").load(url, function () {
        if (typeof fnClose === "function") {
            $("#" + idModal).one('hidden.bs.modal', fnClose);
        }

        $("#" + idModal).find('[data-dismiss="modal"]').on('click', function () {
            $("#" + idModal).modal('hide');
        });

        $("#" + idModal).modal('show');
    });
}

// function that removes a modal
function CloseModal(idModal, data) {
    $('#' + idModal).modal('hide').data(data);
}

// function that removes a modal
function CloseAllModals() {
    $('.modal').modal("hide");
}

// Call ajax helper
function CallAjax(url, options) {
    var options = options || {},
        type = DefaultFor(options.type, 'POST'),
        contentType = DefaultFor(options.contentType, 'application/x-www-form-urlencoded; charset=UTF-8'),
        dataType = DefaultFor(options.dataType, null),
        enctype = DefaultFor(options.enctype, null),
        processData = DefaultFor(options.dataType, true),
        data = DefaultFor(options.data, null),
        divMask = DefaultFor(options.divMask, null),
        headers = DefaultFor(options.headers, {}),
        fnDone = DefaultFor(options.fnDone, null),
        fnError = DefaultFor(options.fnError, null),
        fnAlways = DefaultFor(options.fnAlways, null),
        closeMaskWhenDone = DefaultFor(options.closeMaskWhenDone, true);

    if (divMask) LoadMask();

    $.ajax({
        url: url,
        type: type,
        contentType: contentType,
        dataType: dataType,
        processData: processData,
        enctype: enctype,
        data: data,
        headers: headers
    }).done(function (response) {
        if (fnDone) eval(fnDone(response));
    }).fail(function (xhr, ajaxOptions, thrownError) {
        if (fnError) eval(fnError(xhr, ajaxOptions, thrownError));
    }).always(function (response) {
        if (fnAlways) eval(fnAlways(response));
    });
}

// Convert FormData to JSON
function formDataToJson(form) {
    var formData = new FormData(form);
    var jsonObject = {};
    formData.forEach((value, key) => {
        if (key !== '__RequestVerificationToken') {
            jsonObject[key] = value;
        }
    });
    return JSON.stringify(jsonObject);
}

// Show a message
function showMessage(type, message) {
    var typeMessage = 'info';
    switch (type) {
        case "1":
            typeMessage = "success";
            break;
        case "2":
            typeMessage = "info";
            break;
        case "3":
            typeMessage = "error";
            break;
        default:
            typeMessage = "info";
            break;
    }

    Swal.fire({
        position: "top-end",
        icon: typeMessage,
        title: message,
        showConfirmButton: !1,
        timer: 3000
    });
}


// Confirm message
function confirmMessage(title, message, confirmButtonText, cancelButtonText, callbackYes, callBackNo) {

    // Function parameter for callback
    var fnCallbackYes = DefaultFor(callbackYes, null);

    // Function parameter for callback
    var fnCallBackNo = DefaultFor(callBackNo, null);

    Swal.fire({
        title: title,
        text: message,
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: `<i class="fas fa-check"></i> ${confirmButtonText}`,
        cancelButtonText: `<i class="fas fa-times"></i> ${cancelButtonText}`,
        customClass: {
            confirmButton: "btn btn-success mt-2 confirm-button",
            cancelButton: "btn btn-danger mt-2 cancel-button"
        },
        buttonsStyling: false
    }).then((result) => {
        if (result.isConfirmed) {
            if (typeof fnCallbackYes === "function") {
                fnCallbackYes();
            }
        } else {
            if (typeof fnCallBackNo === "function") {
                fnCallBackNo();
            }
        }
    });
}