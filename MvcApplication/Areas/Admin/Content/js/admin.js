$(document).ready(function () {

    // Chosen for Dropdown
    if ($(".chosen-select").length > 0) {
        $(".chosen-select").chosen();
    }

    $(function () {
        var sidebarHook = $('.sidebar-hook'), sidebar = $('.sidebar');
        sidebarHook.click(function (e) {
            sidebar.slideToggle(200, 'easeInSine');
            sidebarHook.find('i').toggleClass('sh-toggle');
            return false;
        });

        if ($('.sidebar ul li a').length > 0) {
            $('.sidebar ul li a').hoverIntent(function () {
                $(this).next('.submenu-lv-01').toggleClass('submenu-toggle');
            });
        }

        if ($('.submenu-lv-01').length > 0) {
            $('.submenu-lv-01').mouseover(function (e) {
                $(this).css('display', 'block');
                e.preventDefault();
            }).mouseout(function (e) {
                $(this).css('display', 'none');
                e.preventDefault();
            });
        }
    });

    //Apply Data table to Grid (Normal Table)
    if ($('.table-list').length > 0) {
        $('.table-list').dataTable({
            "sDom": 'R<"H"lfr>t<"F"iTp>',
            "bJQueryUI": true,
            "aLengthMenu": [[5, 10, 15, 20, -1], [5, 10, 15, 20, "All"]],
            "sPaginationType": "full_numbers",
            "bDestroy": true,
            "oTableTools": {
                "sSwfPath": "/Areas/Admin/Content/datatables/media/copy_csv_xls.swf",
            }
        });
    }

    //Apply Data table to Grid (Grouping table)
    if ($('#groupingTable').length > 0) {
        //var oTable = $('#groupingTable').dataTable({
        //    "fnDrawCallback": function (oSettings) {
        //        if (oSettings.aiDisplay.length == 0) {
        //            return;
        //        }

        //        //var nTrs = $('#groupingTable tbody tr');
        //        //var iColspan = nTrs[2].getElementsByTagName('td').length;
        //        //var sLastGroup = "";
        //        //for (var i = 0; i < nTrs.length; i++) {
        //        //    var iDisplayIndex = oSettings._iDisplayStart + i;
        //        //    var sGroup = oSettings.aoData[oSettings.aiDisplay[iDisplayIndex]]._aData[2];
        //        //    if (sGroup != sLastGroup) {
        //        //        var nGroup = document.createElement('tr');
        //        //        var nCell = document.createElement('td');
        //        //        nCell.colSpan = iColspan;
        //        //        nCell.className = "group";
        //        //        nCell.innerHTML = sGroup;
        //        //        nGroup.appendChild(nCell);
        //        //        nTrs[i].parentNode.insertBefore(nGroup, nTrs[i]);
        //        //        sLastGroup = sGroup;
        //        //    }
        //        //}
        //    },
        //    "sDom": 'R<"H"lfr>t<"F"iTp>',
        //    //"bJQueryUI": true,
        //    "aLengthMenu": [[5, 10, 15, 20, -1], [5, 10, 15, 20, "All"]],
        //    "sPaginationType": "full_numbers",
        //    "bDestroy": true,
        //    "oTableTools": {
        //        "sSwfPath": "/Areas/Admin/Content/datatables/media/copy_csv_xls.swf",
        //    }
        //});
        $('table#groupingTable').find('td.group').addClass('ui-state-default');
    }

    // Apply Tabs
    if ($("#tabs").length > 0) {
        $("#tabs").tabs({
            collapsible: true
        });
    };

    // Filter for URL textbox

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
});

var didScroll;
var lastScrollTop = 0;
var delta = 5;
var navbarHeight = $(".header").outerHeight();

// on scroll, let the interval function know the user has scrolled
$(window).scroll(function () {
    didScroll = true;
});

// run hasScrolled() and reset didScroll status
setInterval(function () {
    if (didScroll) {
        hasScrolled();
        didScroll = false;
    }
}, 250);

function hasScrolled() {
    var st = $(this).scrollTop();
    if (Math.abs(lastScrollTop - st) <= delta) {
        $(".header").slideDown(500);
    }
        // If current position > last position AND scrolled past navbar...
    else if (st > lastScrollTop && st > navbarHeight) {
        // Scroll Down
        $(".header").slideUp(500);
        $('.sidebar').hide();

    } else {

        // Scroll Up
        // If did not scroll past the document (possible on mac)...

        if (st + $(window).height() < $(document).height()) {
            $(".header").slideDown(500);
        }
    }
}
