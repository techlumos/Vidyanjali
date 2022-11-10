// Initiate The Uplodify with Default attributes
$(function () {
    $("#file_upload").uploadify({
        'swf': '/Areas/FileManagement/Content/Uploadify/uploadify.swf',
        'buttonText': 'Select File(s)',
        'uploader': '/ImageManager/SaveToTemp',
        'fileObjName': 'file',
        'folder': '/Areas/FileManagement/Content/Temporary',
        'fileTypeDesc': 'All Files',
        'fileTypeExts': '*.*',
        'fileSizeLimit': 0,
        'multi': true,
        'auto': true,
        'debug': false,
        'preventCaching': false,
        'onCancel': function (file) {
            $('.notificationText').empty().html('The file ' + file.name + ' was cancelled.');
            $('#notificationContainer').show().delay(4000).fadeOut();
            $("#btnUpload").hide();
        },
        'onInit': function () { //Delete the files in Temporary folder on Initilization
            $.ajax({
                url: '/ImageManager/ClearTemp',
                context: 'body',
                success: function () {
                    $('.notificationText').empty().html('The file queue has been cleared');
                    $('#notificationContainer').show().delay(4000).fadeOut();
                }
            });
        },
        'onFallback': function () {
            $('.notificationText').empty().html('Flash was not detected.');
            $('#notificationContainer').show().show().delay(4000).fadeOut();
        },
        'onQueueComplete': function (queueData) {
            $('.notificationText').empty().html(queueData.uploadsSuccessful + ' file(s) availabe for your Upload.');
            $('#notificationContainer').show().show().delay(4000).fadeOut();
        },
        'onUploadSuccess': function (file, data, response) {
            if (response == true) {
                $("div#listPreview").append(
                    '<div class="preview pcon prodcont actioninside gap">' +
                        '<a class="fancyboxPreview" href="/Areas/FileManagement/Content/Temporary/' + file.name + '">' +
                        '<img class="preview_image" width="100%" src="/Areas/FileManagement/Content/Temporary/' + file.name + '"/></a><br/>' +
                        '<input name="txtTitle" class="preview_Title" placeholder="title"/><br/>' +
                        '<textarea name="txtAreaDescription" class="preview_description" placeholder="say something about this photo!" ></textarea><br/>' +
                        '<input name="txtTag" class="preview_Title" placeholder="add tags"/><br/>' +
                        '<input name="txtEurl" class="preview_Title" placeholder="add url"/><br/>' +
                        '<input type="hidden" name="hdImageName" value="' + file.name + '"/>' +
                    '</div>'
                );

                // Arrange all temporary Images for preview
                $('#listPreview').pinbox({ subcontainer: '.actioninside' }).hide(0).fadeIn(1000);

                // Set Croping functionality to each Image, selected through Fancybox
                if ($('a.fancyboxPreview').length > 0) {

                    var imgUrl = '';

                    $('a.fancyboxPreview').fancybox({
                        afterShow: function () {
                            //get loaded image Src
                            imgUrl = this.inner.find('img').attr('src');
                            this.inner.find('img').Jcrop({
                                onSelect: showCoords,
                                onChange: showCoords
                            }, function () {
                                this.animateTo([0, 0, $('#txtWidth').val(), $('#txtHeight').val()]);
                            });

                            this.inner.prepend('<a href="#" class="cropLink">Crop It!</a>');
                        }
                    });

                    function showCoords(c) {
                        // variables can be accessed here as
                        // c.x, c.y, c.x2, c.y2, c.w, c.h
                        $('input[name="x"]').val(c.x);
                        $('input[name="y"]').val(c.y);
                        $('input[name="x1"]').val(c.x2);
                        $('input[name="y1"]').val(c.y2);
                    };

                    $(document).on("click", "a.cropLink", function () {
                        $.ajax({
                            url: '/ImageManager/CropTempImage',
                            context: 'body',
                            data: { 'x': $('input[name="x"]').val(), 'y': $('input[name="y"]').val(), 'x1': $('input[name="x1"]').val(), 'y1': $('input[name="y1"]').val(), 'fileAbsPath': imgUrl },
                            dataType: 'html',
                            success: function (imgpath) {
                                $('img[src="' + imgpath + '"]').attr('src', imgpath).load(function () {
                                    $.fancybox.close();
                                });
                            }
                        });
                    });
                }
            }
        },
        'onUploadError': function (file, errorCode, errorMsg, errorString) {
            $('.notificationText').empty().html('The file ' + file.name + ' could not be uploaded: ' + errorString);
            $('#notificationContainer').show().show().delay(4000).fadeOut();
        },
        'onSelectOnce': function (event, data) { $("#btnUpload").show(); }
    });
});

/* Used to read the values of Querystring
------------------------------------------------------------
*/
function GetQueryStringParams(sParam) {
    var sPageUrl = window.location.search.substring(1);
    var sUrlVariables = sPageUrl.split('&');
    for (var i = 0; i < sUrlVariables.length; i++) {
        var sParameterName = sUrlVariables[i].split('=');
        if (sParameterName[0] == sParam) {
            return sParameterName[1];
        }
    }
    return '';
}

// Prepage a Filetypes, Descriptions, Button text etc for Uplodify Instence
// Based on your 'Type' selection (i.e. Image, Document)
function GetSubTypes(type, typeData) {
    try {
        if (type == "Document") {
            $('#file_upload').uploadify('settings', 'buttonText', 'Select Document(s)');
            $('#file_upload').uploadify('settings', 'fileTypeDesc', 'All Text Files');
        }
        else if (type == "Image") {
            $('#file_upload').uploadify('settings', 'buttonText', 'Select Image(s)');
            $('#file_upload').uploadify('settings', 'fileTypeDesc', 'All Image Files');
        }

        if (typeData != '') {
            var values = typeData.split('|');
            if (values[0] != '' && values[1] != '') {
                var fileSizeLimit = values[0];
                var fileTypeExten = values[1];
                $('#file_upload').uploadify('settings', 'fileSizeLimit', fileSizeLimit);
                $('#file_upload').uploadify('settings', 'fileTypeExts', fileTypeExten);
            }
        }
    }
    catch (err) {
    }
}

function prepareImageList(group, section, referenceId) {
    $('#optUploader').show();
    $.ajax({
        url: '/ImageManager/DisplayList',
        data: { 'group': group, 'section': section, 'referenceId': referenceId },
        dataType: 'html',
        context: 'body',
        success: function (data) {
            $('#listPreview').empty();
            $('#imageList_box').empty().html(data);
            //Appy Datatable to Grid
            if ($('.gridData').length > 0) {
                $('.gridData').dataTable({
                    "bStateSave": true,
                    "iDisplayLength": 5,
                    "aLengthMenu": [[5, 10, 15, 20, -1], [5, 10, 15, 20, "All"]],
                    "bJQueryUI": true,
                    "sPaginationType": "full_numbers"
                });

                // Fancybox to view uploaded image from list
                if ($('a.fancybox').length > 0) {
                    $('a.fancybox').fancybox();
                }

                // Fancybox to edit the File details
                if ($('a.editFile').length > 0) {
                    $('a.editFile').fancybox({
                        maxWidth: 500,
                        maxHeight: 600,
                        fitToView: false,
                        width: '70%',
                        height: '70%',
                        autoSize: false,
                        closeClick: false,
                        openEffect: 'none',
                        closeEffect: 'none'
                    });
                }

                if ($("#processedStatus").length > 0) {
                    if ($.trim($("#processedStatus").html())) {
                        $('.notificationText').empty().html($("div#processedStatus").html());
                        $('#notificationContainer').show().show().delay(4000).fadeOut();
                    }
                }
            }
        },
        error: function (xhr, errorType, exception) {
            var errorMessage = exception || xhr.statusText;
            alert("There was an error : " + errorMessage);
        }
    });
}

//Get Sections by Type and Prepare Uplodify
$(document).on("change", "select#selectType", function () {
    var typeName = $('#selectType option').filter(":selected").val();
    var typeValues = $('#selectType option').filter(":selected").attr('data');

    var group = GetQueryStringParams('group');
    var section = GetQueryStringParams('section');

    typeName == "Document" ? $('#showCustomHeightWidth').hide() : $('#showCustomHeightWidth').show();

    if (typeName != '') {
        $.ajax({
            url: '/ImageManager/GetSections',
            //data: 'selectedType=' + typeName,
            data: { 'selectedType': typeName, 'selected': section, 'groupName': group },
            dataType: 'html',
            context: 'body',
            success: function (data) {
                $('#resultSelectSection').empty().html(data);
                $('#optSection').show();
                GetSubTypes(typeName, typeValues);
            },
            error: function (xhr, errorType, exception) {
                var errorMessage = exception || xhr.statusText;
                alert("There was an error : " + errorMessage);
            }
        });
    }
    else {
        $('#optSection, #optUploader,#sectionDescription').hide();
        $('#resultSelectSection, #listPreview, #imageList_box').empty();
    }
});

//Get Image list by Section selected
$(document).on("change", "select#selectSection", function () {
    var typeName = $('#selectType option').filter(":selected").val();
    var sectionName = $('#selectSection option').filter(":selected").val();
    var groupName = $("input[name='uploadGroup']").val();
    var referenceId = $("input[name='hdRerefenceId']").val();

    var dataValues = $('#selectSection option').filter(":selected").attr('data');
    if (dataValues != '') {
        var values = dataValues.split('|');
        if (values[0] != '' && values[1] != '') {
            $('#sectionDescription').empty().html(values[0]).show();

            if (typeName != "Document" && typeName != '') {
                var res = values[1].split('x');
                $('#txtWidth').val(res[0]);
                $('#txtHeight').val(res[1]);

                $('#txtWidth').attr('data', res[0]);
                $('#txtHeight').attr('data', res[1]);

                $('#showCustomHeightWidth').show();
            }
        }
    }

    if (sectionName != '' && groupName != '' && referenceId != '') {
        prepareImageList(groupName, sectionName, referenceId);
    } else {
        $('#optUploader,#showCustomHeightWidth,#sectionDescription').hide();
        $('#listPreview, #imageList_box').empty();
    }
});

// Set the selected image as default Image
$('#imageList_box').on("click", "input[type='radio'].rdSetDefault", function () {
    var values = $(this).attr('data').split('|');
    var type = values[0];
    var group = values[1];
    var section = values[2];
    var id = values[3];
    var referenceId = values[4];

    if (type != '' && group != '' && section != '' && id != '' && referenceId != '') {
        $.ajax({
            url: '/ImageManager/SetDefault',
            context: 'body',
            data: { 'type': type, 'groupName': group, 'section': section, 'id': id, 'referenceId': referenceId },
            dataType: 'html',
            success: function (data) {

                $("[id^='imgList_']").removeClass('default_file');
                $('img#imgList_' + id).addClass('default_file');

                $('.notificationText').empty().html(data);
                $('#notificationContainer').show().delay(4000).fadeOut();
            },
            error: function (xhr, errorType, exception) {
                var errorMessage = exception || xhr.statusText;
                alert("There was an error : " + errorMessage);
            }
        });
    }
});

$(document).ajaxStart(function () {
    $('#ajaxLoader').show();
});
$(document).ajaxStop(function () {
    $('#ajaxLoader').hide();
});


// Executes when DOM is Ready!
$(document).ready(function () {
    //Get Types on PageLoad
    $(function () {
        // Read URL parameters
        var group = GetQueryStringParams('group');
        var section = GetQueryStringParams('section');
        var referenceId = GetQueryStringParams('referenceId');

        //If something is availabe in URL during pageLoad. Prepare the 'Type' and 'Section' select 
        //based on URL values (parameters: 'group','section', and 'referenceId')
        if (group != '' && section != '' && referenceId != '') {
            // Assuming every request is for Image only!
            $.ajax({
                url: '/ImageManager/GetTypes?selected=Image',
                dataType: 'html',
                context: 'body',
                success: function (data) {
                    $('#resultSelectType').empty().html(data);

                    // Bind 'Section' select
                    var typeName = $('#selectType option').filter(":selected").val();
                    var typeValues = $('#selectType option').filter(":selected").attr('data');

                    typeName == "Document" ? $('#showCustomHeightWidth').hide() : $('#showCustomHeightWidth').show();

                    GetSubTypes(typeName, typeValues);

                    $.ajax({
                        url: '/ImageManager/GetSections',
                        data: { 'selectedType': typeName, 'selected': section, 'groupName': group },
                        dataType: 'html',
                        context: 'body',
                        success: function (sdata) {
                            $('#resultSelectSection').empty().html(sdata);
                            $('#optSection').show();

                            var dataValues = $('#selectSection option').filter(":selected").attr('data');
                            if (dataValues != '') {
                                var values = dataValues.split('|');
                                if (values[0] != '' && values[1] != '') {
                                    $('#sectionDescription').empty().html(values[0]).show();

                                    if (typeName != "Document" && typeName != '') {
                                        var res = values[1].split('x');
                                        $('#txtWidth').val(res[0]);
                                        $('#txtHeight').val(res[1]);

                                        $('#txtWidth').attr('data', res[0]);
                                        $('#txtHeight').attr('data', res[1]);

                                        $('#showCustomHeightWidth').show();
                                    }
                                }
                            }

                            prepareImageList(group, section, referenceId);
                        },
                        error: function (xhr, errorType, exception) {
                            var errorMessage = exception || xhr.statusText;
                            alert("There was an error : " + errorMessage);
                        }
                    });
                },
                error: function (xhr, errorType, exception) {
                    var errorMessage = exception || xhr.statusText;
                    alert("There was an error : " + errorMessage);
                }
            });
        }
        else {
            //Nothing is availbel on URL
            $.ajax({
                url: '/ImageManager/GetTypes',
                context: 'body',
                success: function (data) {
                    $('#resultSelectType').empty().html(data);
                }
            });
        }
    });

    //Upload the Images and Bind the List

    $("#frmImgUpload").submit(function (event) {
        var typeName = $('#selectType option').filter(":selected").val();
        var sectionName = $('#selectSection option').filter(":selected").val();
        if (typeName != '' && sectionName != '' && $('#listPreview:has(img)').length > 0) {
            return true;
        } else {
            $('.notificationText').empty().html('Please select a File to be uploaded, and Its "Type" and "Section" ');
            $('#notificationContainer').show().show().delay(4000).fadeOut();
            // Stop form from submitting normally
            return false;
        }
    });

    $('#resetDefaults').click(function () {
        $('#txtWidth').val($('#txtWidth').attr('data'));
        $('#txtHeight').val($('#txtHeight').attr('data'));
        return false;
    });
});
// Dom Ready Ends

