$(document).ready(function () {
    $('.input-validation-error').parents('.form-group').addClass('has-error');
    $('.field-validation-error').addClass('text-danger');
});

function loadData(result) {
    var productVersion = $("#ProductVersion").val(); 

    var payload = {
        type: "os",
        ProductVersion: productVersion
    };

    if(productVersion == "2016-rtm") {
        showOssMgmtCheckbox();
    } else {
        hideOssMgmtCheckbox();
    }

    $.getJSON('/Home/Data', payload, function (result) {
        var selectedOperatingSysten = $('#SelectedOperatingSystem').val();
        var dropdown = $('#OperatingSystem');
        dropdown.empty();

        $(result).each(function () {
            var option = $(document.createElement('option'))
                .attr('value', this.id)
                .text(this.value)
                .appendTo(dropdown);

            if(this.id == selectedOperatingSysten) {
                option.attr('selected', 'selected');
            }
        });

        dropdown.prop("disabled", false);
    });
}

function hideOssMgmtCheckbox() {
    $('#OssManagement input').prop('checked', false);
    $("#OssManagement").addClass("hidden");
}

function showOssMgmtCheckbox() {
    $("#OssManagement").removeClass("hidden");
}

$("#ProductVersion").change(function () {
    loadData();
});

$(function () {
    loadData()
});