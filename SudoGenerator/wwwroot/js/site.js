$(document).ready(function () {
    $('.input-validation-error').parents('.form-group').addClass('has-error');
    $('.field-validation-error').addClass('text-danger');
});

function loadData(result) {
    var payload = {
        type: "os",
        ProductVersion: $("#ProductVersion").val()
    };

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

$("#ProductVersion").change(function () {
    loadData();
});

$(function () {
    loadData()
});