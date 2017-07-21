
function submitForm(json, formId) {
    for (var i = 0; i < json.length; i++) {
        for (var key in json[i]) {
            document.getElementById(key).value = json[i][key];
        }
    }
    document.getElementById(formId).submit();
    return false;
}