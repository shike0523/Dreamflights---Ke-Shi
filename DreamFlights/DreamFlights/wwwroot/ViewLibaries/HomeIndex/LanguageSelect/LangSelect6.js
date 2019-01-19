//The whole file is for multilanguage selection, used in tandom with "LanguageSelect" folder in wwwroot and "SetLanguage" method in HomeController and file "Resources" and StartUp
$(document).ready(function () {
    $(".customCombobox").click(function () {
        //Get ul tag
        var dropDwn = $(".ulcustomCombobox");
        //Show Dropdown
        if (dropDwn.is(":visible"))
            dropDwn.hide();
        else
            dropDwn.show();

        //var currentVal = $("#languageSelect").val();
        //$("#customCombobox1").html($("li[data-language-type=" + currentVal + "]").html());  //li为tag的值
    });
    //Dropdown element click
    $("#ulcustomCombobox1 li").click(function () {
        //Get div(customCombobox1) tag
        var cmbBox = $(this).parent().prev();
        //Set div tag text/image
        cmbBox.html($(this).html());
        $("#languageSelect").val($(this).attr("data-language-type"));
        //Hide Dropdown
        $(this).parent().hide();
        $("#selectLanguage").submit();
    });
    //Hide Dropdown If User click outside 
    $(document).on('click', function (e) {
        var element, evt = e ? e : event;
        if (evt.srcElement)
            element = evt.srcElement;
        else if (evt.target)
            element = evt.target;
        //Hide if clicked outside;在下拉框收起时如果点击到国旗图标或者文字时也要弹出下拉框(两个element重叠时会影响selector正确判断出元素点击的位置,导致点击$(".customCombobox")与国旗重叠的部分没反应)
        if (element.className == "imgDisplay" || element.className == "imageText") {
            var dropDwn = $("ul.ulcustomCombobox");
            //Show Dropdown
            if (dropDwn.css("display") == "block")
                dropDwn.show();
            else
                dropDwn.hide();
        } else
        if (element.className != "customCombobox") {
            $("ul.ulcustomCombobox").hide();
        }
    });
})