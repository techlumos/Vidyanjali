$('input#Url').bind('keypress', function (event) {
    var regex = new RegExp("^[a-zA-Z0-9_-]+$");
    var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
    //allow Backspace
    if (event.which == 8) {
        return true;
    }
    if (!regex.test(key)) {
        event.preventDefault();
        return false;
    }
    return true;
});

$("input#Title").keyup(function () {
    $(".replication").val(this.value);
    var inputWithoutSpace = this.value.replace(/[^a-z0-9_&\s]/gi, '').replace(/[\s]/g, '-').replace(/[&]/g, 'and');
    $("input#Url").val(inputWithoutSpace);
});

$(".replication").change(function () {
    $(this).removeClass("replication");
});