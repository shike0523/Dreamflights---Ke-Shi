/*
 * 修改特定功能：在“！！！！！！！参数调整”处
 * 
'&&!(clickMonth==0&&clickYear==year+1)'这个语句是为了使跨年的时候出发日和返程日能够在月份切换时能够正确显示
* 对外接口：$("#languageSelect").val(), 来自于.Net Core的多语言选择模块
*/

var riqiid = 0;
var clickCount = 0;
var coordinateX;
var coordinateY;
var signal = 0;
var clickMonth = 0;
var clickYear = 0;
var clickDay = 0;
var clickMonth2 = 0;
var clickYear2 = 0;
var clickDay2 = 0;
var oneway = 0;   //控制是否要return
var tempClass;
var clickCalendar;  //记录第一次点击时所在的日历
//var clickMonthR = 0;

//    var clickDay = 0;  //试验

var monthsChina = new Array("一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二");
var months = new Array("", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec");
var daysInMonth = new Array(31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31);
var days = new Array("Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat");
var classTemp;
var today = new getToday();  //得到当天的完整日期(年/月/日)
var year = today.year;
var month = today.month;     //每个日历的月份(默认值是当月)
var newCal;

//if(clickCount){            //试验
//   year=clickYear;
//   month=clickMonth;
//}

function getDays(month, year) {
    if (1 == month)
        return ((0 == year % 4) && (0 != (year % 100))) || (0 == year % 400) ? 29 : 28;
    //计算2月份的天数(基于平年还是闰年)
    else
        return daysInMonth[month];
}  //计算当月天数
function getToday() {
    this.now = new Date();  //通过系统获取当天日期
    this.year = this.now.getFullYear();
    this.month = this.now.getMonth();
    this.day = this.now.getDate();

    //    if(clickCount){            //试验
    //   this.year=clickYear;
    //    this.month=clickMonth;
    //    this.day=clickDay;
    //}

}
function Calendar() {
    newCal = new Date(year, month, 1);//获取完整格式日期,例：Thu Jun 24 1999 00:00:00 GMT+1000 (AUS Eastern Standard Time)，目的是获得一号是周几(weekday)?疑问：此函数具备智能判断month==13时该将month置为1的能力？
    today = new getToday();
    var day = -1;
    var startDay = newCal.getDay(); //获取每个月的一号是星期几
    var endDay = getDays(newCal.getMonth(), newCal.getFullYear());//获取当月总天数
    var daily = 0;
    var endDay_pre;
    var pseudoDay;

    //防止跨年时左边日历pseudoDay显示异常
    if (month == 0) {
        endDay_pre = getDays(newCal.getMonth(), newCal.getFullYear());
    } else {
        endDay_pre = getDays(newCal.getMonth() - 1, newCal.getFullYear());
    }
    pseudoDay = endDay_pre - startDay + 1;

    if ((today.year == newCal.getFullYear()) && (today.month == newCal.getMonth())) {
        day = today.day;
        //    if(clickCount){day=clickDay;}    //试验   
    }

    var caltable = document.getElementsByTagName("*").caltable.tBodies.calendar;
    var intDaysInMonth = getDays(newCal.getMonth(), newCal.getFullYear());
    for (var intWeek = 0; intWeek < caltable.rows.length; intWeek++)
        for (var intDay = 0; intDay < caltable.rows[intWeek].cells.length; intDay++) {
            var cell = caltable.rows[intWeek].cells[intDay];
            var montemp = (newCal.getMonth() + 1) < 10 ? ("0" + (newCal.getMonth() + 1)) : (newCal.getMonth() + 1);
            if ((intDay == startDay) && (0 == daily)) {
                daily = 1;
                pseudoDay = 1;
            } //?判断是否循环找到每月开始的那天的weekday，好让后面的程序(if ((daily > 0) && (daily <= intDaysInMonth))判断是否要进行daily++的计数,daily用于暂时存储每天的日期
            var daytemp = daily < 10 ? ("0" + daily) : (daily);
            var d = "<" + newCal.getFullYear() + "-" + montemp + "-" + daytemp + ">";
            if (cell.className = "DayGo") {
                cell.className = "Day";
                cell.addEventListener("click", getDiary, false);
            }
            if (clickCount) {   //如果绘图到了第一次点击的那天则进行signal标记
                if (daily == clickDay && month == clickMonth && year == clickYear) {
                    signal = !signal;
                    cell.className = "DayGo";
                }
                //window.alert(today.month);
            } else {
                if (day == daily && month == today.month && year == today.year) {    //
                    cell.className = "DayNow";   //当天日期变红
                    cell.addEventListener("click", getDiary, false);
                    // cell.className="DayNow";
                    //window.alert(daily);
                    signal = !signal; //表示从这里开始标记：不再绘制过去的日期
                }
            }



            if (intDay == 6 && cell.className != "DayNow" && cell.className != "DayGo") {  //让周六也能保持正常显示当天 
                //window.alert(getDiffDays(555));
                cell.className = "Day";    //为了简便起见节省时间,周日周六的样式不做区分
                cell.addEventListener("click", getDiary, false);
            } else if (intDay == 0 && cell.className != "DayNow" && cell.className != "DayGo") {
                //cell.className ="DaySun";
                cell.className = "Day";
                cell.addEventListener("click", getDiary, false);
            } else if (cell.className != "DayNow" && cell.className != "DayGo") { //防止把启程日期DayGo又变回了Day
                cell.className = "Day";
                //document.getElementsById("end").setAttribute("data-dismiss","modal"); 
                cell.addEventListener("click", getDiary, false);
            }
            //开始构建日历(新建或者是刷新时)
            if ((daily > 0) && (daily <= intDaysInMonth)) {
                if (getDiffDays(daily) > 180) {  //180天以后不可以订票；调整天数只需调整这里的参数即可
                    cell.className = "DayPast";
                    cell.removeEventListener("click", getDiary, false);
                }
                cell.innerHTML = daily;
                daily++;
                if (signal) {  //使今天以后的日子可以点击
                    //cell.addEventListener("click", getDiary,false); //匿名函数体,无法解除绑定！！！(已纠正)  
                    //cell.removeEventListener("click", getDiary,false);//可删除点击事件,但是整个单元格都删完了(已纠正)    
                }
                if (clickCount) {
                    if (!signal && (((month <= clickMonth) && (year == clickYear)) || year < clickYear)) {
                        cell.className = "DayPast";
                        cell.removeEventListener("click", getDiary, false);
                        //window.alert("aaa");
                    }
                } else {
                    if (!signal && (((month <= today.month) && (year == today.year)) || year < today.year)) { //在出发时间和返程时间跨年时保持第二年的日历样式不会变成过去的样式,上面的"((month<=today.month)&&(year==today.year))"发挥了这个作用
                        cell.className = "DayPast";
                        cell.removeEventListener("click", getDiary, false);//?不管有没有click功能都执行,省事
                        //window.alert("aaa");
                    }
                }

            } else {
                //cell.className="CalendarTD";
                cell.innerHTML = pseudoDay++;
                cell.className = "Past";
                cell.removeEventListener("click", getDiary, false);
            }
            if (!clickCount) {   //防止在选择了启程日后,后面新增加的灰色区域DayPast中有一个日期被变回为DayGo
                if (cell.innerHTML == clickDay && month == clickMonth && year == clickYear && cell.className != "Past") {//让选中的日期保持颜色(日历切换后)
                    cell.className = "DayGo";
                    //document.getElementsById("end").setAttribute("data-dismiss","modal"); 
                    cell.addEventListener("click", getDiary, false);
                } else if (cell.innerHTML == clickDay2 && month == clickMonth2 && year == clickYear2 && !oneway && cell.className != "Past") {//让返程日保持颜色(日历切换后)
                    cell.className = "DayReturn";
                    //document.getElementsById("end").setAttribute("data-dismiss","modal"); 
                    cell.addEventListener("click", getDiary, false);
                }
            }

        }

    document.getElementsByTagName("*").year.value = year;  //？注：value就是客户输入的值00000
    //document.getElementById("caltable").rows[0].innerHTML=1111;
    //window.alert(document.getElementById("caltable").year.size);     
    document.getElementsByTagName("*").month.value = month + 1;  //日历第一行的月份;注:这里的mouth是input里面的name
    document.getElementById("month-display").value = months[month + 1];
    //caltable.rows[7].cells[0].removeEventListener("click", getDiary(this));
    //window.alert(caltable.rows[7].cells[0].innerText);  
    if (signal) {
        signal = !signal;
    }
    if (document.getElementById("tip-panel").innerHTML == "Please select a return date" && clickCount == 0) {
        document.getElementById("tip-panel").innerHTML = "Confirm?";
    } else if (clickCount == 0) {
        document.getElementById("tip-panel").innerHTML = "Please select a depart date";
    } else if (clickCount == 1) {
        document.getElementById("tip-panel").innerHTML = "Please select a return date";
    }

}

function subMonth() {
    signal = 0;
    if ((month - 1) < 0) {  //如果当前是1月(也就是month=0)，则它的上一个月是12月(也就是month=11)
        month = 11;
        year = year - 1;
    } else {
        month = month - 1;
    }
    //Calendar();
    //？？？为什么必须i>=1而不是i>=0(用i>=0时无法执行,用IE浏览器纠错工具时会在i=0时循环报错)
    //(接上文)为什么i的初始值是7(因为table有7行,caltable代表的是整个table而不是tbody!!)    
    for (var i = document.getElementsByTagName("*").caltable.rows.length - 1; i >= 2; i--) { //注：获取表格列的长度
        for (var j = document.getElementsByTagName("*").caltable.rows[1].cells.length - 1; j >= 0; j--) {
            if ((today.day != document.getElementById("caltable").rows[i].cells[j].innerHTML) || (month != today.month)) { //保留当天日期为红色
                document.getElementById("caltable").rows[i].cells[j].className = "Day";
            }

        }
    }
    //日历换页后不出现当天日期红色(此案例的方法在getDiary(x)刷新日历后只是日历表格的值变化,但是表格属性不变,所以需要在刷新后把颜色恢复
    for (var i = document.getElementsByTagName("*").caltable.rows.length - 1; i >= 2; i--) { //注：获取表格列的长度
        for (var j = document.getElementsByTagName("*").caltable.rows[1].cells.length - 1; j >= 0; j--) {
            if ((today.day != document.getElementById("caltable2").rows[i].cells[j].innerHTML) || (month + 1 != today.month)) { //保留当天日期为红色
                document.getElementById("caltable2").rows[i].cells[j].className = "Day";
            }

        }
    }
    Calendar();
    Calendar2();
}
function addMonth() {
    signal = 0;
    //window.alert(caltable.rows[7].cells[0].innerText);     

    if ((month + 1) > 11) {
        month = 0;
        year = year + 1;
    } else {
        month = month + 1;
    }
    //month=month+1;
    //Calendar();
    /* 
    for(var i=document.getElementsByTagName("*").caltable.rows.length-1;i>=2;i--){  //注：获取表格列的长度；i>=2是为了不让表示周几的那一行受到影响变得和日期的样式一样
        for(var j=document.getElementsByTagName("*").caltable.rows[1].cells.length-1;j>=0;j--){
           if((today.day!=document.getElementById("caltable").rows[i].cells[j].innerHTML)||(month!=today.month)){ //保留当天日期为红色
               document.getElementById("caltable").rows[i].cells[j].className="Day"; 
           }
        }
    }  //注：换页后使页面不再有今日日期标注
    for(var i=document.getElementsByTagName("*").caltable.rows.length-1;i>=2;i--){  //注：获取表格列的长度；i>=2是为了不让表示周几的那一行受到影响变得和日期的样式一样
        for(var j=document.getElementsByTagName("*").caltable.rows[1].cells.length-1;j>=0;j--){
           if((today.day!=document.getElementById("caltable2").rows[i].cells[j].innerHTML)||(month!=today.month)){ //保留当天日期为红色
               document.getElementById("caltable2").rows[i].cells[j].className="Day"; 
           }
        }
    }
    */
    Calendar();
    Calendar2();
}
function setDate() {  //此函数貌似没有被执行
    if (document.all.month.value < 1 || document.all.month.value > 12) {
        alert("月的有效范围在1-12之间!");
        return;
    }
    year = Math.ceil(document.all.year.value);//？注：value就是客户输入的值
    month = Math.ceil(document.all.month.value - 1);//？注：Math.ceil为向上取整,以防客户输入小数点
    Calendar();
    //window.alert("aaa");
}

//x是前台传递过来的参数(这里是一个DOM元素)
function buttonOver(x) {
    tempClass = x.className;
    //x.style.col or="red"; 
    //window.alert(tempClass);

    if (!clickCount || oneway) {   //只对鼠标下面的格子进行高亮显示
        if (x.className != "DayPast" && x.className != "Past" && x.className != "DayNow" && x.className != "DayGo" && x.className != "DayReturn") {
            x.className = "DaySelected";
            //window.alert(x.getAttribute('class'));
            //window.alert(x.className);
        }
        // x.parentNode.style="cursor:hand";
    } else {   //点击鼠标后实现拖动效果
        if (x.parentNode.parentNode.parentNode.id == "caltable" && x.className != "DayPast" && (clickMonth <= month || ((year > clickYear) && (month < clickMonth)) || ((year > clickYear) && (month > clickMonth)))) { //当鼠标在第一个表格时候的拖动效果
            //window.alert(clickMonth);
            if ((month > clickMonth) || ((year > clickYear) && (month < clickMonth)) || ((year > clickYear) && (month > clickMonth))) {//当出发日和返程日不在同一个月时候的情况
                for (var i = 2; i <= x.parentNode.rowIndex - 1; i++) { //获取鼠标当前在表格中的纵坐标
                    for (var j = 0; j <= document.getElementsByTagName("*").caltable.rows[1].cells.length - 1; j++) {
                        if (document.getElementById("caltable").rows[i].cells[j].className != "Past") {
                            //window.alert(x.cellIndex);
                            document.getElementById("caltable").rows[i].cells[j].className = "DaySelected";//通过坐标(行/列号)直接修改表格特定元素的属性
                        }
                    }
                }
                for (var i = 0; i <= x.cellIndex; i++) {  //将鼠标当前所在的那一行变色 
                    //   if(x.parentNode.rowIndex>coordinateY){
                    if (document.getElementById("caltable").rows[x.parentNode.rowIndex].cells[i].className != "Past") {
                        document.getElementById("caltable").rows[x.parentNode.rowIndex].cells[i].className = "DaySelected";
                    }
                }
            } else {    //第三层中间！！！！！！！！！！！！
                if (coordinateY == x.parentNode.rowIndex) {//鼠标hover的日历格子和选中的出发点在同一行时的区间显示
                    for (var i = coordinateX + 1; i <= x.cellIndex; i++) {
                        document.getElementById("caltable").rows[coordinateY].cells[i].className = "DaySelected";
                        //window.alert(document.all.caltable.rows[1].cells.length-1);
                    }
                } else {  //鼠标hover的日历格子和选中的出发点不在同一行时的区间显示
                    if (x.parentNode.rowIndex > coordinateY) {
                        //window.alert(document.all.caltable.length); 
                        for (var i = coordinateX + 1; i <= document.getElementsByTagName("*").caltable.rows[1].cells.length - 1; i++) { //注：获取表格的行的长度
                            document.getElementById("caltable").rows[coordinateY].cells[i].className = "DaySelected";
                            // window.alert(document.all.caltable.rows[1].cells.length-1);
                        }
                    }
                    for (var i = coordinateY + 1; i <= x.parentNode.rowIndex - 1; i++) {
                        for (var j = 0; j <= document.getElementsByTagName("*").caltable.rows[1].cells.length - 1; j++) {
                            if (document.getElementById("caltable").rows[i].cells[j].className != "Past") {
                                // window.alert(document.all.caltable.rows[1].cells.length-1);
                                document.getElementById("caltable").rows[i].cells[j].className = "DaySelected";  //  
                            }
                        }
                    }
                    for (var i = 0; i <= x.cellIndex; i++) {
                        if (x.parentNode.rowIndex > coordinateY && document.getElementById("caltable").rows[x.parentNode.rowIndex].cells[i].className != "Past") {   //首次点击鼠标后使鼠标在出发日之前上面时无任何高亮
                            document.getElementById("caltable").rows[x.parentNode.rowIndex].cells[i].className = "DaySelected";
                            // window.alert(x.cellIndex);
                        }
                    }
                }
            }
        } else if (x.parentNode.parentNode.parentNode.id == "caltable2" && x.className != "DayPast" && (month + 1 >= clickMonth || ((year > clickYear) && (month < clickMonth)) || ((year > clickYear) && (month > clickMonth)))) {  //当鼠标在第二个日历时候的拖动效果  第二层！！！！！！！！！！！！！！！
            //window.alert(month);
            //if(clickCalendar=="caltable"&&month==clickMonth){
            /*
            if(document.getElementsByTagName("*").month2.value==clickMonth){ //当第一次点击的月份和鼠标所在月份相邻时   
                window.alert(document.getElementsByTagName("*").month2.value);
              for(var i=coordinateX;i<=document.getElementsByTagName("*").caltable.rows[1].cells.length-1;i++){ //注：获取表格的行的长度
              document.getElementById("caltable").rows[coordinateY].cells[i].className="DaySelected"; 
                   // window.alert(document.all.caltable.rows[1].cells.length-1);
              }
              for(var i=coordinateY+1;i<=7;i++){ 
                  for(var j=0;j<=document.getElementsByTagName("*").caltable.rows[1].cells.length-1;j++){
                    if(document.getElementById("caltable").rows[i].cells[j].className!="Past"){
                           // window.alert(document.all.caltable.rows[1].cells.length-1);
                    document.getElementById("caltable").rows[i].cells[j].className="DaySelected";  //  
                    }
                  }
              }
              for(var i=2;i<=x.parentNode.rowIndex-1;i++){ //当第一次点击在第二个日历()，然后点击了0次"下个月"时的对右边日历的处理方法vvvvvvvvvvvvvvvvvvvv
                for(var j=0;j<=document.getElementsByTagName("*").caltable2.rows[1].cells.length-1;j++){
                  if(document.getElementById("caltable2").rows[i].cells[j].className!="Past"){
                       //window.alert(x.cellIndex);
                  document.getElementById("caltable2").rows[i].cells[j].className="DaySelected";//通过坐标(行/列号)直接修改表格特定元素的属性
                  } 
                }
              }
              for(var i=0;i<=x.cellIndex;i++){  //将鼠标当前所在的那一行变色(最后以行需要变色的) 
                 //   if(x.parentNode.rowIndex>coordinateY){
                if(document.getElementById("caltable2").rows[x.parentNode.rowIndex].cells[i].className!="Past"){
                document.getElementById("caltable2").rows[x.parentNode.rowIndex].cells[i].className="DaySelected"; 
                } 
              }
            }  
            */
            if ((month + 1 > clickMonth || (year > clickYear && month + 1 < clickMonth) || (year > clickYear && month + 1 > clickMonth)) && !(clickMonth == 0 && clickYear == year + 1)) {//当出发日和返程日不在同一个月时候的情况
                if ((month - clickMonth) == 0) {   //当第一次点击在第二个日历()，然后点击了一次"下个月"时的对左边日历的处理方法vvvvvvvvvvvvvvvvv
                    //window.alert(document.all.caltable.rows[1].cells.length-1);
                    for (var i = coordinateX + 1; i <= document.getElementsByTagName("*").caltable.rows[1].cells.length - 1; i++) { //注：获取表格的行的长度
                        document.getElementById("caltable").rows[coordinateY].cells[i].className = "DaySelected";
                        //window.alert(document.all.caltable.rows[1].cells.length-1);
                    }
                    if (clickCalendar == "caltable") {  //在左边日历点击出发时间且切换了月份(即执行addmonth()后),对左边日历的处理办法
                        //window.alert("aaa");
                        for (var i = coordinateY + 1; i <= 7; i++) {
                            for (var j = 0; j <= document.getElementsByTagName("*").caltable.rows[1].cells.length - 1; j++) {
                                if (document.getElementById("caltable").rows[i].cells[j].className != "Past") {
                                    // window.alert(document.all.caltable.rows[1].cells.length-1);
                                    document.getElementById("caltable").rows[i].cells[j].className = "DaySelected";  //  
                                }
                            }
                        }
                    } else {
                        for (var i = coordinateY + 1; i <= 7; i++) {
                            for (var j = 0; j <= document.getElementsByTagName("*").caltable.rows[1].cells.length - 1; j++) {
                                if (document.getElementById("caltable").rows[i].cells[j].className != "Past") {
                                    // window.alert(document.all.caltable.rows[1].cells.length-1);
                                    document.getElementById("caltable").rows[i].cells[j].className = "DaySelected";  //  
                                }
                            }
                        }
                    }
                    //当第一次点击在第二个日历，然后点击了一次"下个月"时的对左边日历的处理方法^^^^^^^^^^^^^^^^^^^
                } else {                       //当第一次点击在第二个日历，然后点击了多次"下个月"时的对左边日历的处理方法vvvvvvvvvvvvvvvvvvv
                    //window.alert(document.all.caltable.rows[1].cells.length-1);
                    for (var i = 2; i <= 7; i++) {
                        for (var j = 0; j <= document.getElementsByTagName("*").caltable.rows[1].cells.length - 1; j++) {
                            if (document.getElementById("caltable").rows[i].cells[j].className != "Past") {
                                // window.alert(document.all.caltable.rows[1].cells.length-1);
                                document.getElementById("caltable").rows[i].cells[j].className = "DaySelected";  //  
                            }
                        }
                    }
                }                             //当第一次点击在第二个日历，然后点击了多次"下个月"时的对左边日历的处理方法^^^^^^^^^^^^^^^^^^
                for (var i = 2; i <= x.parentNode.rowIndex - 1; i++) { //当第一次点击在第二个日历()，然后点击了n次"下个月"时的对右边日历的处理方法vvvvvvvvvvvvvvvvvvvv
                    for (var j = 0; j <= document.getElementsByTagName("*").caltable2.rows[1].cells.length - 1; j++) {
                        if (document.getElementById("caltable2").rows[i].cells[j].className != "Past") {
                            //window.alert(x.cellIndex);
                            document.getElementById("caltable2").rows[i].cells[j].className = "DaySelected";//通过坐标(行/列号)直接修改表格特定元素的属性
                        }
                    }
                }
                for (var i = 0; i <= x.cellIndex; i++) {  //将鼠标当前所在的那一行变色(最后以行需要变色的) 
                    //   if(x.parentNode.rowIndex>coordinateY){
                    if (document.getElementById("caltable2").rows[x.parentNode.rowIndex].cells[i].className != "Past") {
                        document.getElementById("caltable2").rows[x.parentNode.rowIndex].cells[i].className = "DaySelected";
                    }
                }                                      //当第一次点击在第二个日历()，然后点击了n次"下个月"时的对右边日历的处理方法^^^^^^^^^^^^^^^^^^^^^^^^^
            } else {    //第三层中间！！！！！！！！！！！！
                if (coordinateY == x.parentNode.rowIndex) {//鼠标hover的日历格子和选中的出发点在同一行时的区间显示;第一次点击和当前鼠标方位都在第右边日历时对右边日历的处理方法
                    for (var i = coordinateX + 1; i <= x.cellIndex; i++) {
                        document.getElementById("caltable2").rows[coordinateY].cells[i].className = "DaySelected";
                        //window.alert(document.all.caltable2.rows[1].cells.length-1);
                    }
                } else {  //第一次点击和当前鼠标方位都在第右边日历时对右边日历的处理方法
                    if (x.parentNode.rowIndex > coordinateY) {
                        //window.alert(document.all.caltable2.length); 
                        for (var i = coordinateX + 1; i <= document.getElementsByTagName("*").caltable2.rows[1].cells.length - 1; i++) { //注：获取表格的行的长度
                            document.getElementById("caltable2").rows[coordinateY].cells[i].className = "DaySelected";
                            // window.alert(document.all.caltable2.rows[1].cells.length-1);
                        }
                    }
                    for (var i = coordinateY + 1; i <= x.parentNode.rowIndex - 1; i++) {
                        for (var j = 0; j <= document.getElementsByTagName("*").caltable2.rows[1].cells.length - 1; j++) {
                            if (document.getElementById("caltable2").rows[i].cells[j].className != "Past") {
                                // window.alert(document.all.caltable2.rows[1].cells.length-1);
                                document.getElementById("caltable2").rows[i].cells[j].className = "DaySelected";  //  
                            }
                        }
                    }
                    for (var i = 0; i <= x.cellIndex; i++) {
                        if (x.parentNode.rowIndex > coordinateY && document.getElementById("caltable2").rows[x.parentNode.rowIndex].cells[i].className != "Past") {   //首次点击鼠标后使鼠标在出发日之前上面时无任何高亮
                            document.getElementById("caltable2").rows[x.parentNode.rowIndex].cells[i].className = "DaySelected";
                            // window.alert(x.cellIndex);
                        }
                    }
                }
            }
        }
    }  //第一层！！！！！！！！！！！ 

    //   caltable.rows[7].cells[0].removeEventListener("click", getDiary); 

    //var obj = window.event.srcElement; //注：在做事件处理时候区分IE和其他浏览器事件对象时常用的写法。是兼容性代码
    //obj.runtimeStyle.cssText = "background-color:#FFFFFF";//？？？
    // obj.className="Hover"; 
}
function buttonOut(x) {
    // window.alert(x.parentNode.parentNode.parentNode.id);
    //x.className=tempClass;
    // if(x.parentNode.parentNode.parentNode.id="caltable"){
    for (var i = document.getElementsByTagName("*").caltable.rows.length - 1; i >= 2; i--) {  //注：获取表格列的长度
        for (var j = document.getElementsByTagName("*").caltable.rows[1].cells.length - 1; j >= 0; j--) {
            if ((today.day != document.getElementById("caltable").rows[i].cells[j].innerHTML || (month != today.month)) && document.getElementById("caltable").rows[i].cells[j].className != "Past" && !(clickDay == document.getElementById("caltable").rows[i].cells[j].innerHTML && clickMonth == month && clickYear == year)) { //保留当天日期为红色,而不是在执行getDairy(x)刷新后被恢复成默认色; 日历换页后不出现当天日期红色(此案例的方法在getDiary(x)刷新日历后只是日历表格的值变化,但是表格属性不变,所以需要在刷新后把颜色恢复)
                if (document.getElementById("caltable").rows[i].cells[j].className != "DayPast" && (document.getElementById("caltable").rows[i].cells[j].className != "DayGo" && document.getElementById("caltable").rows[i].cells[j].className != "DayReturn" || clickCount)) {
                    document.getElementById("caltable").rows[i].cells[j].className = "Day";
                }
            }
        }
    }     //为试验三而取消    
    // }else{
    for (var i = document.getElementsByTagName("*").caltable.rows.length - 1; i >= 2; i--) {  //注：获取表格列的长度
        for (var j = document.getElementsByTagName("*").caltable.rows[1].cells.length - 1; j >= 0; j--) {
            if ((today.day != document.getElementById("caltable2").rows[i].cells[j].innerHTML || (month + 1 != today.month)) && document.getElementById("caltable2").rows[i].cells[j].className != "Past" && !(clickDay == document.getElementById("caltable2").rows[i].cells[j].innerHTML && ((clickMonth == month + 1 && clickYear == year) || (clickYear == year + 1 && clickMonth == 0)))) { //保留当天日期为红色,而不是在执行getDairy(x)刷新后被恢复成默认色; 日历换页后不出现当天日期红色(此案例的方法在getDiary(x)刷新日历后只是日历表格的值变化,但是表格属性不变,所以需要在刷新后把颜色恢复)
                if (document.getElementById("caltable2").rows[i].cells[j].className != "DayPast" && (document.getElementById("caltable2").rows[i].cells[j].className != "DayGo" && document.getElementById("caltable2").rows[i].cells[j].className != "DayReturn" || clickCount)) {
                    document.getElementById("caltable2").rows[i].cells[j].className = "Day";
                }
            }
        }
    }
    // }


    //document.getElementById("caltable").rows[x.parentNode.rowIndex].cells[x.cellIndex].className="Day";  //试验三

    //  window.alert(caltable.rows[7].cells[0].innerHTML);
    //window.alert(clickDay);
    //window.alert(clickDay);
    var obj = window.event.srcElement;
    window.setTimeout(function () { obj.runtimeStyle.cssText = ""; }, 300);
}

function getDiary(event) {     //注：绑定带参数的函数的标准方法(试了很久才找到的)!!!!!!!!!!!!!!!!!!!!
    //function getDiary(x){
    // document.getElementById("tip-panel").innerHTML="Please select a return date";
    if (!clickCount) {
        clickCalendar = this.parentNode.parentNode.parentNode.id;
        if (this.parentNode.parentNode.parentNode.id == "caltable2") {
            //clickMonthR=month+1;
            //clickMonth=month+1;
            //window.alert(clickCalendar);
            //clickYear=year; 
            if ((month + 1) > 11) {
                clickMonth = 0;
                clickYear = year + 1;
            } else {
                clickMonth = month + 1;
                clickYear = year;
            }
        } else {
            clickMonth = month;
            clickYear = year;                      //记录本次点击的所在的年 需改进!!!!!!!!!!!!!!!!!!!!!!!!!
        }
    } else if (!oneway) {
        if (this.parentNode.parentNode.parentNode.id == "caltable2") {
            if ((month + 1) > 11) {
                clickMonth2 = 0;
                clickYear2 = year + 1;
            } else {
                clickMonth2 = month + 1;
                clickYear2 = year;
            }
            //clickMonth2=month+1;
            //window.alert(clickCalendar);
        } else {
            clickMonth2 = month;
            clickYear2 = year;
        }
        clickDay2 = this.innerHTML;
        //记录2次点击的所在的年 需改进!!!!!!!!!!!!!!!!!!!!!!!!!
    }
    clickCount = !clickCount;              //记录本次点击的状态


    coordinateX = this.cellIndex;             //记录本次点击的状态(日)
    coordinateY = this.parentNode.rowIndex;   //记录本次点击的状态(日)

    //   clickDay=x.innerHTML;    //试验
    //window.alert(this.innerHTML);
    if (oneway) {
        if (this.parentNode.parentNode.parentNode.id == "caltable") { //判断点击的是哪个日历
            y = document.getElementById("year").value;
            d = document.getElementById("month").value;
            if ($("#languageSelect").val() === "zh") {
                document.getElementById("showdate").value = y + "/" + d + "/" + this.innerHTML;  //点击的日期的值           
                document.getElementById("showdate-panel").value = y + "/" + d + "/" + this.innerHTML;
            } else {
                document.getElementById("showdate").value = this.innerHTML + "/" + d + "/" + y;  //点击的日期的值           
                document.getElementById("showdate-panel").value = this.innerHTML + "/" + d + "/" + y;
            }

        } else {
            y = document.getElementById("year2").value;
            d = document.getElementById("month2").value;
            if ($("#languageSelect").val() === "zh") {
                document.getElementById("showdate").value = y + "/" + d + "/" + this.innerHTML;  //点击的日期的值           
                document.getElementById("showdate-panel").value = y + "/" + d + "/" + this.innerHTML;
            } else {
                document.getElementById("showdate").value = this.innerHTML + "/" + d + "/" + y;  //点击的日期的值           
                document.getElementById("showdate-panel").value = this.innerHTML + "/" + d + "/" + y;
            }
        }
        clickDay = this.innerHTML;
        //清空其他被点击的地方vvvvvvvvvvvvvv   
        for (var i = document.getElementsByTagName("*").caltable.rows.length - 1; i >= 2; i--) {  //注：获取表格列的长度
            for (var j = document.getElementsByTagName("*").caltable.rows[1].cells.length - 1; j >= 0; j--) {
                if ((today.day != document.getElementById("caltable").rows[i].cells[j].innerHTML || (month != today.month)) && document.getElementById("caltable").rows[i].cells[j].className != "Past" && !(clickDay == document.getElementById("caltable").rows[i].cells[j].innerHTML && clickMonth == month && clickYear == year)) { //保留当天日期为红色,而不是在执行getDairy(x)刷新后被恢复成默认色; 日历换页后不出现当天日期红色(此案例的方法在getDiary(x)刷新日历后只是日历表格的值变化,但是表格属性不变,所以需要在刷新后把颜色恢复)
                    if (document.getElementById("caltable").rows[i].cells[j].className != "DayPast") {
                        document.getElementById("caltable").rows[i].cells[j].className = "Day";
                    }
                }
            }
        }

        for (var i = document.getElementsByTagName("*").caltable.rows.length - 1; i >= 2; i--) {  //注：获取表格列的长度
            for (var j = document.getElementsByTagName("*").caltable.rows[1].cells.length - 1; j >= 0; j--) {
                if ((today.day != document.getElementById("caltable2").rows[i].cells[j].innerHTML || (month + 1 != today.month)) && document.getElementById("caltable2").rows[i].cells[j].className != "Past" && !(clickDay == document.getElementById("caltable2").rows[i].cells[j].innerHTML && clickMonth == (month + 1) && clickYear == year)) { //保留当天日期为红色,而不是在执行getDairy(x)刷新后被恢复成默认色; 日历换页后不出现当天日期红色(此案例的方法在getDiary(x)刷新日历后只是日历表格的值变化,但是表格属性不变,所以需要在刷新后把颜色恢复)
                    if (document.getElementById("caltable2").rows[i].cells[j].className != "DayPast") {
                        document.getElementById("caltable2").rows[i].cells[j].className = "Day";
                    }
                }
            }
        }
        //清空其他被点击的地方^^^^^^^^^^^^^^^^^^
        clickCount = !clickCount;//抵消掉之前的相同的语句，单程订票时点击次数不影响表格绘制(不然点击日之前的日期全部不能点击了)
        this.className = "DayGo";
    } else {   //返程的情况

        if (!clickCount) { //把第二次选中的值放入第二个输入框
            //if(!clickCount){  
            if (this.parentNode.parentNode.parentNode.id == "caltable") {
                y = document.getElementById("year").value;  //从表头直接取出年和月
                d = document.getElementById("month").value;
                if ($("#languageSelect").val() === "zh") {
                    document.getElementById("showdateReturn").value = y + "/" + d + "/" + this.innerHTML;  //点击的日期的值           
                    document.getElementById("showdateReturn-panel").value = y + "/" + d + "/" + this.innerHTML;
                } else {
                    document.getElementById("showdateReturn").value = this.innerHTML + "/" + d + "/" + y;  //点击的日期的值           
                    document.getElementById("showdateReturn-panel").value = this.innerHTML + "/" + d + "/" + y;
                }
                //回归最初的状态，以确保下次打开日历时已过去的日期显示正常
                this.className = "DayReturn";
            } else {
                y = document.getElementById("year2").value;  //从表头直接取出年和月
                d = document.getElementById("month2").value;
                if ($("#languageSelect").val() === "zh") {
                    document.getElementById("showdateReturn").value = y + "/" + d + "/" + this.innerHTML;  //点击的日期的值           
                    document.getElementById("showdateReturn-panel").value = y + "/" + d + "/" + this.innerHTML;
                } else {
                    document.getElementById("showdateReturn").value = this.innerHTML + "/" + d + "/" + y;  //点击的日期的值           
                    document.getElementById("showdateReturn-panel").value = this.innerHTML + "/" + d + "/" + y;
                }
                this.className = "DayReturn";
            }
            //document.getElementById("caltable").style.display="none";
            //document.getElementById("calenderBoard").style.display="none";
            if (signal) {
                signal = !signal;
            }


            //document.getElementById("caltable2").style.display="none";
            //document.getElementById("calenderBoard").style.display="none";
        } else {
            if (this.parentNode.parentNode.parentNode.id == "caltable") { //判断点击的是哪个日历
                y = document.getElementById("year").value;
                d = document.getElementById("month").value;
                if ($("#languageSelect").val() === "zh") {
                    document.getElementById("showdate").value = y + "/" + d + "/" + this.innerHTML;  //点击的日期的值           
                    document.getElementById("showdate-panel").value = y + "/" + d + "/" + this.innerHTML;
                } else {
                    document.getElementById("showdate").value = this.innerHTML + "/" + d + "/" + y;  //点击的日期的值           
                    document.getElementById("showdate-panel").value = this.innerHTML + "/" + d + "/" + y;
                }
                this.className = "DayReturn";
            } else {
                y = document.getElementById("year2").value;
                d = document.getElementById("month2").value;
                if ($("#languageSelect").val() === "zh") {
                    document.getElementById("showdate").value = y + "/" + d + "/" + this.innerHTML;  //点击的日期的值           
                    document.getElementById("showdate-panel").value = y + "/" + d + "/" + this.innerHTML;
                } else {
                    document.getElementById("showdate").value = this.innerHTML + "/" + d + "/" + y;  //点击的日期的值           
                    document.getElementById("showdate-panel").value = this.innerHTML + "/" + d + "/" + y;
                }
                this.className = "DayReturn";
            }
            //    Calendar();  //试验
            clickDay = this.innerHTML;
            //window.alert(clickDay);
        }

    }

    //！！！！！！！参数调整.决定了return trip中所包含的所有日子的蓝色高亮在日期选择完毕后是否保留, 如果不想保留就将这下面4行代码注解掉即可(可用于适应手机浏览器)
    if (!oneway) {
        if (clickCount) {
            Calendar();
            Calendar2();
        }
        
    }

}

/*function calclick(x) { //计算点击的情况(未使用)
   // cordinate=x.id;
   // document.getElementById("showclick").value=coordinateX;
    clickMonth=month;
}  */

//第二个日历表格！！！！！！！！！！！！！！！！！！！！！！！！！！！
function Calendar2() {
    //signal=0;
    if (month + 1 > 11) {
        newCal = new Date(year + 1, 0, 1);//获取完整格式日期,例：Thu Jun 24 1999 00:00:00 GMT+1000 (AUS Eastern Standard Time)，目的是获得一号是周几(weekday)
    } else {
        newCal = new Date(year, month + 1, 1);//获取完整格式日期,例：Thu Jun 24 1999 00:00:00 GMT+1000 (AUS Eastern Standard Time)，目的是获得一号是周几(weekday)
    }
    //newCal = new Date(year, month + 1, 1);//获取完整格式日期,例：Thu Jun 24 1999 00:00:00 GMT+1000 (AUS Eastern Standard Time)，目的是获得一号是周几(weekday)
    today = new getToday();
    var day = -1;
    var startDay = newCal.getDay(); //获取每个月的一号是星期几
    var endDay = getDays(newCal.getMonth(), newCal.getFullYear());//获取当月总天数
    //var endDay_pre = getDays(newCal.getMonth() - 1, newCal.getFullYear());
    var daily = 0;
    //var pseudoDay = endDay_pre - startDay + 1;
    if ((today.year == newCal.getFullYear()) && (today.month == newCal.getMonth())) {
        day = today.day;
        //    if(clickCount){day=clickDay;}    //试验
    }

    //防止跨年时右边日历pseudoDay显示异常
    var endDay_pre;
    var pseudoDay;
    if (month == 11) {
        endDay_pre = getDays(newCal.getMonth(), newCal.getFullYear());
    } else {
        endDay_pre = getDays(newCal.getMonth() - 1, newCal.getFullYear());
    }
    pseudoDay = endDay_pre - startDay + 1;

    var caltable2 = document.getElementsByTagName("*").caltable2.tBodies.calendar;
    var intDaysInMonth = getDays(newCal.getMonth(), newCal.getFullYear());
    for (var intWeek = 0; intWeek < caltable2.rows.length; intWeek++)
        for (var intDay = 0; intDay < caltable2.rows[intWeek].cells.length; intDay++) {
            var cell = caltable2.rows[intWeek].cells[intDay];
            var montemp = (newCal.getMonth() + 1) < 10 ? ("0" + (newCal.getMonth() + 1)) : (newCal.getMonth() + 1);
            if ((intDay == startDay) && (0 == daily)) {
                daily = 1;
                pseudoDay = 1;
            }  //?判断是否循环找到每月开始的那天的weekday，好让后面的程序(if ((daily > 0) && (daily <= intDaysInMonth))判断是否要进行daily++的计数,daily用于暂时存储每天的日期
            var daytemp = daily < 10 ? ("0" + daily) : (daily);
            var d = "<" + newCal.getFullYear() + "-" + montemp + "-" + daytemp + ">";
            if (cell.className = "DayGo") {
                cell.className = "Day";
            }
            if (clickCount) {
                if (daily == clickDay && ((month + 1 == clickMonth && year == clickYear) || (clickMonth == 0 && clickYear == year + 1))) {
                    signal = !signal;
                    cell.className = "DayGo";
                }
                //window.alert(today.month);
            } else {
                if (day == daily && month + 1 == today.month && year == today.year) {    //
                    cell.className = "DayNow";   //当天日期变红
                    // cell.className="DayNow";
                    //window.alert(today.month);
                    signal = !signal; //表示从这里开始标记：不再绘制过去的日期
                }
            }


            if (intDay == 6 && cell.className != "DayNow" && cell.className != "DayGo")
                //cell.className = "DaySat";  
                cell.className = "Day";    //为了简便起见节省时间,周日周六的样式不做区分
            else if (intDay == 0 && cell.className != "DayNow" && cell.className != "DayGo")
                //cell.className ="DaySun";
                cell.className = "Day";
            else if (cell.className != "DayNow" && cell.className != "DayGo")
                cell.className = "Day";
            cell.addEventListener("click", getDiary, false);
            if ((daily > 0) && (daily <= intDaysInMonth)) {
                if (getDiffDays2(daily) > 180) {
                    cell.className = "DayPast";
                    cell.removeEventListener("click", getDiary, false);
                }
                cell.innerHTML = daily;
                daily++;
                if (signal) {  //使今天以后的日子可以点击
                    //cell.addEventListener("click", getDiary,false); //匿名函数体,无法解除绑定！！！(已纠正)  
                    //cell.removeEventListener("click", getDiary,false);//可删除点击事件,但是整个单元格都删完了(已纠正)  
                    //window.alert(clickDay);
                }
                //window.alert(getDiffDays(daily));
                if (clickCount) {
                    if (!signal && (((month + 1 <= clickMonth && year == clickYear) || (year + 1 == clickYear && clickMonth == 0)) || (year < clickYear && (year + 1 != clickYear && clickMonth == 0)))) {
                        cell.className = "DayPast";
                        cell.removeEventListener("click", getDiary, false);
                    }
                } else {
                    if (!signal && (((month + 1 <= today.month) && (year == today.year)) || year < today.year)) { //在出发时间和返程时间跨年时保持第二年的日历样式不会变成过去的样式,上面的"((month<=today.month)&&(year==today.year))"发挥了这个作用
                        cell.className = "DayPast";
                        cell.removeEventListener("click", getDiary, false);
                        //window.alert("aaa");
                    }
                }
            } else {
                //cell.className="CalendarTD";
                cell.innerHTML = pseudoDay++;
                cell.className = "Past";
                cell.removeEventListener("click", getDiary, false);
            }
            if (!clickCount) {
                if (cell.innerHTML == clickDay && ((month + 1 == clickMonth && year == clickYear) || (clickMonth == 0 && clickYear == year + 1 && month == 11)) && cell.className != "Past") {
                    cell.className = "DayGo";
                    //document.getElementsById("end").setAttribute("data-dismiss","modal"); 
                    cell.addEventListener("click", getDiary, false);
                } else if (cell.innerHTML == clickDay2 && ((month + 1 == clickMonth2 && year == clickYear2) || month == 11 && clickMonth2 == 0 && clickYear2 == year + 1) && !oneway && cell.className != "Past") {//让返程日保持颜色
                    cell.className = "DayReturn";
                    //document.getElementsById("end").setAttribute("data-dismiss","modal"); 
                    cell.addEventListener("click", getDiary, false);
                    //alert("aaa");
                }//让选中的日期保持颜色  
            }

        }

    if ((month + 1) > 11) {
        document.getElementsByTagName("*").month2.value = 1;
        document.getElementsByTagName("*").year2.value = year + 1;
        document.getElementById("month2-display").value = months[document.getElementsByTagName("*").month2.value];
    } else {
        document.getElementsByTagName("*").year2.value = year;  //？注：value就是客户输入的值00000
        //document.getElementById("caltable2").rows[0].innerHTML=1111;

        document.getElementsByTagName("*").month2.value = month + 1 + 1;  //日历第一行的月份;注:这里的mouth是input里面的name  
        //window.alert(document.getElementsByTagName("*").month2.value);
        document.getElementById("month2-display").value = months[document.getElementsByTagName("*").month2.value];
    }

    if (signal) {//临时解决方案,待进一步调查！！！！！！！！！！！
        signal = !signal;
    }

    //caltable2.rows[7].cells[0].removeEventListener("click", getDiary(this));
    //window.alert(caltable2.rows[7].cells[0].innerText); 
    //signal=!signal
}


function rili() {
    document.getElementById('caltable').style.display = '';//点击按钮显示弹出日历
    document.getElementById('caltable2').style.display = '';//点击按钮显示弹出日历
    Calendar();   //程序的入口
    Calendar2();
    //window.alert(document.getElementById("toggle").innerHTML);
    //document.getElementById("toggle").data-toggle="none";
}
//获取某一日(d)与当前时间相差的天数
function getDiffDays(d) {
    var yearDiff;
    var monthDiff;
    if (month + 1 > 11) {
        yearDiff = year + 1;
        monthDiff = 1;
    } else {
        yearDiff = year;
        monthDiff = month + 1;
    }
    var date = new Date(yearDiff + "/" + monthDiff + "/" + d);
    var timeDiff = Math.abs(today.now - date.getTime());
    return Math.ceil(timeDiff / (1000 * 3600 * 24));
}
//同上（右边日历的）
function getDiffDays2(d) {
    var yearDiff;
    var monthDiff;
    if (month + 2 > 11) {
        yearDiff = year + 1;
        monthDiff = 1;
    } else {
        yearDiff = year;
        monthDiff = month + 2;
    }
    var date = new Date(yearDiff + "/" + monthDiff + "/" + d);
    var timeDiff = Math.abs(today.now - date.getTime());
    return Math.ceil(timeDiff / (1000 * 3600 * 24));
}