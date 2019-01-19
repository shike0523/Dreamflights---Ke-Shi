$(function () {
    $("#slider-rangeFromDepart").slider({
        range: true,
        min: 0,
        max: 1439,
        //左右两个slider的初始取值来源
        values: [$("#timeEarliestFromDepart").val(), $("#timeLatiestFromDepart").val()],
        //slider滑动时的事件响应函数
        slide: slideTimeFromDepart,
        ////slider滑动结束后鼠标左键弹起时的事件响应函数(这里是提交表单(submit form))
        //change: function (event, ui) { $("#FilterForm").submit(); }
    });
    //滑动条滑动时计算各个参数的变化
    function slideTimeFromDepart(event, ui) {
        //分别获取左右两个slider所对应的数值
        var val0 = $("#slider-rangeFromDepart").slider("values", 0),
            val1 = $("#slider-rangeFromDepart").slider("values", 1),
            minutes0 = parseInt(val0 % 60, 10),
            hours0 = parseInt(val0 / 60 % 24, 10),
            minutes1 = parseInt(val1 % 60, 10),
            hours1 = parseInt(val1 / 60 % 24, 10);
        //将slider所对应的数值换算成时间
        startTime = getTime(hours0, minutes0);
        endTime = getTime(hours1, minutes1);
        //将以上换算好的时间输出到页面上
        $("#timeFromDepart").text(startTime + ' - ' + endTime);

        $("#timeEarliestFromDepart").val(val0);
        $("#timeLatiestFromDepart").val(val1);
    }

    $("#slider-rangeFromArrive").slider({
        range: true,
        min: 0,
        max: 1439,
        values: [$("#timeEarliestFromArrive").val(), $("#timeLatiestFromArrive").val()],
        slide: slideTimeFromArrive,
        ////slider滑动结束后鼠标左键弹起时的事件响应函数(这里是提交表单(submit form))
        //change: function (event, ui) { $("#FilterForm").submit(); }
    });
    //滑动条滑动时计算各个参数的变化
    function slideTimeFromArrive(event, ui) {
        var val0 = $("#slider-rangeFromArrive").slider("values", 0),
            val1 = $("#slider-rangeFromArrive").slider("values", 1),
            minutes0 = parseInt(val0 % 60, 10),
            hours0 = parseInt(val0 / 60 % 24, 10),
            minutes1 = parseInt(val1 % 60, 10),
            hours1 = parseInt(val1 / 60 % 24, 10);
        startTime = getTime(hours0, minutes0);
        endTime = getTime(hours1, minutes1);
        $("#timeFromArrive").text(startTime + ' - ' + endTime);

        $("#timeEarliestFromArrive").val(val0);
        $("#timeLatiestFromArrive").val(val1);
    }

    $("#slider-rangeToDepart").slider({
        range: true,
        min: 0,
        max: 1439,
        values: [$("#timeEarliestToDepart").val(), $("#timeLatiestToDepart").val()],
        slide: slideTimeToDepart,
        ////slider滑动结束后鼠标左键弹起时的事件响应函数(这里是提交表单(submit form))
        //change: function (event, ui) { $("#FilterForm").submit(); }
    });
    //滑动条滑动时计算各个参数的变化
    function slideTimeToDepart(event, ui) {
        var val0 = $("#slider-rangeToDepart").slider("values", 0),
            val1 = $("#slider-rangeToDepart").slider("values", 1),
            minutes0 = parseInt(val0 % 60, 10),
            hours0 = parseInt(val0 / 60 % 24, 10),
            minutes1 = parseInt(val1 % 60, 10),
            hours1 = parseInt(val1 / 60 % 24, 10);
        startTime = getTime(hours0, minutes0);
        endTime = getTime(hours1, minutes1);
        $("#timeToDepart").text(startTime + ' - ' + endTime);

        $("#timeEarliestToDepart").val(val0);
        $("#timeLatiestToDepart").val(val1);
    }

    $("#slider-rangeToArrive").slider({
        range: true,
        min: 0,
        max: 1439,
        values: [$("#timeEarliestToArrive").val(), $("#timeLatiestToArrive").val()],
        slide: slideTimeToArrive,
        ////slider滑动结束后鼠标左键弹起时的事件响应函数(这里是提交表单(submit form))
        //change: function (event, ui) { $("#FilterForm").submit(); }
    });
    //滑动条滑动时计算各个参数的变化
    function slideTimeToArrive(event, ui) {
        var val0 = $("#slider-rangeToArrive").slider("values", 0),
            val1 = $("#slider-rangeToArrive").slider("values", 1),
            minutes0 = parseInt(val0 % 60, 10),
            hours0 = parseInt(val0 / 60 % 24, 10),
            minutes1 = parseInt(val1 % 60, 10),
            hours1 = parseInt(val1 / 60 % 24, 10);
        startTime = getTime(hours0, minutes0);
        endTime = getTime(hours1, minutes1);
        $("#timeToArrive").text(startTime + ' - ' + endTime);

        $("#timeEarliestToArrive").val(val0);
        $("#timeLatiestToArrive").val(val1);
    }

    function getTime(hours, minutes) {
        var time = null;
        minutes = minutes + "";
        if (hours < 12) {
            time = "AM";
        }
        else {
            time = "PM";
        }
        if (hours === 0) {
            hours = 12;
        }
        if (hours > 12) {
            hours = hours - 12;
        }
        if (minutes.length === 1) {
            minutes = "0" + minutes;
        }
        return hours + ":" + minutes + " " + time;
    }
    //Set the time value displayed under the slider-range respectively 
    slideTimeFromDepart();
    slideTimeFromArrive();
    slideTimeToDepart();
    slideTimeToArrive();
});