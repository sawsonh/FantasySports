var dateFormatString = "%m/%d/%Y %h:%i:%s %p";
var dateConverter = new AnyTime.Converter({
    baseYear: 2000,
    earliest: new Date(2000, 0, 1, 0, 0, 0),
    format: dateFormatString
});
var periodId = 0;
var template = $('#addTemplate').html();
$(function () {
    addTemplate(periodId++);

    $("#addPeriod").click(function (e) {
        e.preventDefault();
        var errorMessage = validateFields();
        if (errorMessage.length > 0) {
            alert(errorMessage);
            return;
        }
        addTemplate(periodId++);
    });

    $("#periods").on("click", "a.remove", function (e) {
        e.preventDefault();
        $(this).closest("div.parent").remove();
    });
});

function validateFields() {
    var isComplete = true;
    $("#periods input[data-val-required]").each(function () {
        if ($(this).val().length == 0) {
            isComplete = false;
            return false;
        }
    });
    if (!isComplete) {
        return "Complete all text fields before adding";
    }
    $("#periods div.parent").each(function () {
        var arr = ["Pick", "Report"];
        for (var i = 0; i < arr.length; i++) {
            var d1 = dateConverter.parse($(this).find("[name^='" + arr[i] + "StartDateTime']").val());
            var d2 = dateConverter.parse($(this).find("[name^='" + arr[i] + "EndDateTime']").val());
            if (d1 > d2) {
                isComplete = false;
                return false;
            }
        }
    });
    if (!isComplete) {
        return "Fix date range before adding";
    }
    return "";
}

function addTemplate(idx) {
    var tmpl = $(template);
    tmpl.find("label").each(function () {
        var id = $(this).attr("for");
        $(this).attr("for", id + "_" + idx);
    });
    tmpl.find("input").each(function () {
        var id = $(this).attr("id");
        $(this).attr("id", id + "_" + idx);
        var name = $(this).attr("name");
        $(this).attr("name", name + "_" + idx);
    });
    if (idx == 0)
        tmpl.find("a.remove").remove();
    else {
        var arr = ["Pick", "Report"];
        for (var i = 0; i < arr.length; i++) {
            var d1 = dateConverter.parse($("#" + arr[i] + "StartDateTime_" + (idx - 1)).val());
            var d2 = dateConverter.parse($("#" + arr[i] + "EndDateTime_" + (idx - 1)).val());
            var diff = new Date(d2 - d1);
            var seconds = diff / 1000;
            d1.setSeconds(d1.getSeconds() + seconds + 1);
            d2.setSeconds(d2.getSeconds() + seconds + 1);
            tmpl.find("#" + arr[i] + "StartDateTime_" + idx).val(dateConverter.format(d1));
            tmpl.find("#" + arr[i] + "EndDateTime_" + idx).val(dateConverter.format(d2));
        }
    }
    $('#periods').append(tmpl);
    $("#periodId").val(idx);
    //$(".datepicker").datepicker();

    $(".datepicker").AnyTime_picker({
        baseYear: 2000,
        earliest: new Date(2000, 0, 1, 0, 0, 0),
        format: dateFormatString
    });
    $(".datepicker").removeClass("datepicker");
}