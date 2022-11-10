tinyMCE.init({
    // General options
    mode: "textareas",
    width: "1020px",
    height: "400px",
    theme: "advanced",
    pdw_toggle_on: 1,
    pdw_toggle_toolbars: "2,3",
    editor_selector: "mceEditor",
    force_br_newlines: true,
    force_p_newlines: false,
    plugins: "autolink,lists,spellchecker,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,pdw,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template",

    // Theme options
    theme_advanced_buttons1: "pdw_toggle,save,newdocument,|,bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,styleselect,formatselect,fontselect,fontsizeselect",
    theme_advanced_buttons2: "cut,copy,paste,pastetext,pasteword,|,search,replace,|,bullist,numlist,|,outdent,indent,blockquote,|,undo,redo,|,link,unlink,anchor,image,cleanup,help,code,|,insertdate,inserttime,preview,|,forecolor,backcolor",
    theme_advanced_buttons3: "tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,charmap,emotions,iespell,media,advhr,|,print,|,ltr,rtl,|,fullscreen,|,insertlayer,moveforward,movebackward,absolute,|,styleprops,spellchecker,|,cite,abbr,acronym,del,ins,attribs,|,visualchars,nonbreaking,template,blockquote,pagebreak,|,insertfile,insertimage",
    theme_advanced_toolbar_location: "top",
    theme_advanced_toolbar_align: "left",
    theme_advanced_statusbar_location: "bottom",
    theme_advanced_resizing: false,

    // Skin options
    skin: "o2k7",
    skin_variant: "silver",

    // Example content CSS (should be your site CSS)
    content_css: "/Content/css/style.css",
});


tinyMCE.init({
    mode: "textareas",
    theme: "advanced",
    editor_selector: "mceSimple",
    force_br_newlines: true,
    force_p_newlines: false,
    width: "98%",
    height: "100px",
    plugins: 'advimage'
});