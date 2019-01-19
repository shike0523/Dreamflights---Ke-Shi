
    
    var riqiid = 0;
    var clickCount = 0;
    var coordinateX;
    var coordinateY;
    var signal = 0;
    var clickMonth =0;
    var clickYear = 0;
    var oneway = 0;   //控制是否要return
    
//    var clickDay = 0;  //试验

var months = new Array("一", "二", "三","四", "五", "六", "七", "八", "九","十", "十一", "十二");
var daysInMonth = new Array(31, 28, 31, 30, 31, 30, 31, 31,30, 31, 30, 31);
var days = new Array("日","一", "二", "三","四", "五", "六");
var classTemp;
var today=new getToday();  //得到当天的完整日期(年/月/日)
var year=today.year;
var month=today.month;
var newCal; 
    
//if(clickCount){            //试验
 //   year=clickYear;
 //   month=clickMonth;
//}
    
function getDays(month, year) {
if (1 == month) 
return ((0 == year % 4) && (0 != (year % 100))) ||(0 == year % 400) ? 29 : 28;
    //计算2月份的天数(基于平年还是闰年)
else 
return daysInMonth[month];
}  //计算当月天数
function getToday() {
this.now = new Date();
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
newCal = new Date(year,month,1);//获取完整格式日期,例：Thu Jun 24 1999 00:00:00 GMT+1000 (AUS Eastern Standard Time)，目的是获得一号是周几(weekday)
today = new getToday();  
var day = -1;
var startDay = newCal.getDay(); //获取每个月的一号是星期几
var endDay=getDays(newCal.getMonth(), newCal.getFullYear());//获取当月总天数
var daily = 0;
if ((today.year == newCal.getFullYear()) &&(today.month == newCal.getMonth())){
day = today.day;
    
//    if(clickCount){day=clickDay;}    //试验
    
    
}
var caltable = document.all.caltable.tBodies.calendar;
var intDaysInMonth =getDays(newCal.getMonth(), newCal.getFullYear());
for (var intWeek = 0; intWeek < caltable.rows.length;intWeek++)
for (var intDay = 0;intDay < caltable.rows[intWeek].cells.length;intDay++)
{
var cell = caltable.rows[intWeek].cells[intDay];
var montemp=(newCal.getMonth()+1)<10?("0"+(newCal.getMonth()+1)):(newCal.getMonth()+1);
if ((intDay == startDay) && (0 == daily)){ 
daily = 1;
}  //?判断是否循环找到每月开始的那天的weekday，好让后面的程序(if ((daily > 0) && (daily <= intDaysInMonth))判断是否要进行daily++的计数,daily用于暂时存储每天的日期
var daytemp=daily<10?("0"+daily):(daily);
var d="<"+newCal.getFullYear()+"-"+montemp+"-"+daytemp+">";
if((day==daily)&&(month==today.month)){    //
    cell.className="DayNow";   //当天日期变红
   // cell.className="DayNow";
    //window.alert(today.month);
    signal=!signal; //表示从这里开始标记：不再绘制过去的日期
} 
   

else if(intDay==6) 
//cell.className = "DaySat";  
    cell.className="Day";    //为了简便起见节省时间,周日周六的样式不做区分
else if (intDay==0) 
//cell.className ="DaySun";
    cell.className="Day";
else 
cell.className="Day";
if ((daily > 0) && (daily <= intDaysInMonth)){
//cell.innerText = daily;
cell.innerHTML = daily;    
daily++;
    if(signal){  //使今天以后的日子可以点击
    cell.addEventListener("click", getDiary,false); //匿名函数体,无法解除绑定！！！(已纠正)  
    //cell.removeEventListener("click", getDiary,false);//可删除点击事件,但是整个单元格都删完了(已纠正) 
    //cell.className="btn btn-default";
    }
   // if((!signal)||(month<=today.month)){
      //  if(month<=today.month){
    if(!signal&&(((month<=today.month)&&(year==today.year))||year<today.year)){ //在出发时间和返程时间跨年时保持第二年的日历样式不会变成过去的样式,上面的"((month<=today.month)&&(year==today.year))"发挥了这个作用
       cell.className="DayPast";
       cell.removeEventListener("click", getDiary,false);
        //window.alert("aaa");
    }
} else{
//cell.className="CalendarTD";
cell.innerHTML = " ";
cell.className = "Past";    
}   
}
document.all.year.value=year;  //？注：value就是客户输入的值00000
   
document.all.month.value=month+1;  //?
    
//caltable.rows[7].cells[0].removeEventListener("click", getDiary(this));
//window.alert(caltable.rows[7].cells[0].innerText);    
}
function subMonth(){
    signal=0;
if ((month-1)<0){  //如果当前是1月(也就是month=0)，则它的上一个月是12月(也就是month=11)
month=11;   
year=year-1;
}else{
month=month-1;
}
//Calendar();
//？？？为什么必须i>=1而不是i>=0(用i>=0时无法执行,用IE浏览器纠错工具时会在i=0时循环报错)
//(接上文)为什么i的初始值是7(因为table有7行,caltable代表的是整个table而不是tbody!!)    
    for(var i=document.all.caltable.rows.length-1;i>=1;i--){ //注：获取表格列的长度
        for(var j=document.all.caltable.rows[1].cells.length-1;j>=0;j--){
           if((today.day!=document.getElementById("caltable").rows[i].cells[j].innerHTML)||(month!=today.month)){ //保留当天日期为红色
               document.getElementById("caltable").rows[i].cells[j].className="Day"; 
           }
        }
    } 
    //日历换页后不出现当天日期红色(此案例的方法在getDiary(x)刷新日历后只是日历表格的值变化,但是表格属性不变,所以需要在刷新后把颜色恢复
Calendar();
}
function addMonth(){
   
//window.alert(caltable.rows[7].cells[0].innerText);     
    
if((month+1)>11){
month=0;
year=year+1;
}else{
month=month+1;   
}
//Calendar();
     
    for(var i=document.all.caltable.rows.length-1;i>=2;i--){  //注：获取表格列的长度；i>=2是为了不让表示周几的那一行受到影响变得和日期的样式一样
        for(var j=document.all.caltable.rows[1].cells.length-1;j>=0;j--){
           if((today.day!=document.getElementById("caltable").rows[i].cells[j].innerHTML)||(month!=today.month)){ //保留当天日期为红色
               document.getElementById("caltable").rows[i].cells[j].className="Day"; 
           }
        }
    }  //注：换页后使页面不再有今日日期标注
Calendar();    
}
function setDate(){
if (document.all.month.value<1||document.all.month.value>12){
alert("月的有效范围在1-12之间!");
return;
}
year=Math.ceil(document.all.year.value);//？注：value就是客户输入的值
month=Math.ceil(document.all.month.value-1);//？注：Math.ceil为向上取整,以防客户输入小数点
Calendar();
}

  function buttonOver(x){
    //x.style.col or="red"; 
   // window.alert(document.all.caltable.rows.length);
   // window.alert("aaa");
    if(!clickCount){   //只对鼠标下面的格子进行高亮显示
    if(x.className!="DayPast"&&x.className!="Past"&&x.className!="DayNow"){
        x.className="DaySelected";
        //window.alert(x.getAttribute('class'));
        //window.alert(x.className);
    }  
       // x.parentNode.style="cursor:hand";
    }else { 
        if((month>clickMonth)||((year>clickYear)&&(month<clickMonth))||((year>clickYear)&&(month>clickMonth))){//当出发日和返程日不在同一个月时候的情况
            for(var i=2;i<=x.parentNode.rowIndex-1;i++){ //获取鼠标当前在表格中的纵坐标
                for(var j=0;j<=document.all.caltable.rows[1].cells.length-1;j++){
               if(document.getElementById("caltable").rows[i].cells[j].className!="Past"){
                   //window.alert(x.cellIndex);
    document.getElementById("caltable").rows[i].cells[j].className="DaySelected";//通过坐标(行/列号)直接修改表格特定元素的属性
               } 
                }
            }
            for(var i=0;i<=x.cellIndex;i++){  //将鼠标当前所在的那一行变色 
             //   if(x.parentNode.rowIndex>coordinateY){
               if(document.getElementById("caltable").rows[x.parentNode.rowIndex].cells[i].className!="Past"){
                   document.getElementById("caltable").rows[x.parentNode.rowIndex].cells[i].className="DaySelected"; 
               } 
            }
        }else{
        if(coordinateY==x.parentNode.rowIndex){//鼠标hover的日历格子和选中的出发点在同一行时的区间显示
            for(var i=coordinateX;i<=x.cellIndex;i++){
                document.getElementById("caltable").rows[coordinateY].cells[i].className="DaySelected"; 
                //window.alert(document.all.caltable.rows[1].cells.length-1);
            }     
        }else {  //鼠标hover的日历格子和选中的出发点不在同一行时的区间显示
            if(x.parentNode.rowIndex>coordinateY){
            //window.alert(document.all.caltable.length); 
            for(var i=coordinateX;i<=document.all.caltable.rows[1].cells.length-1;i++){ //注：获取表格的行的长度
                document.getElementById("caltable").rows[coordinateY].cells[i].className="DaySelected"; 
               // window.alert(document.all.caltable.rows[1].cells.length-1);
            } 
            }
            for(var i=coordinateY+1;i<=x.parentNode.rowIndex-1;i++){ 
                for(var j=0;j<=document.all.caltable.rows[1].cells.length-1;j++){
                    if(document.getElementById("caltable").rows[i].cells[j].className!="Past"){
                       // window.alert(document.all.caltable.rows[1].cells.length-1);
                      document.getElementById("caltable").rows[i].cells[j].className="DaySelected";  //  
                    }
                }
            }
            for(var i=0;i<=x.cellIndex;i++){   
                if(x.parentNode.rowIndex>coordinateY&&document.getElementById("caltable").rows[x.parentNode.rowIndex].cells[i].className!="Past"){   //首次点击鼠标后使鼠标在出发日之前上面时无任何高亮
            document.getElementById("caltable").rows[x.parentNode.rowIndex].cells[i].className="DaySelected"; 
       // window.alert(x.cellIndex);
                }
            }
        }
      }
    } 
   
    //   caltable.rows[7].cells[0].removeEventListener("click", getDiary); 
      
//var obj = window.event.srcElement; //注：在做事件处理时候区分IE和其他浏览器事件对象时常用的写法。是兼容性代码
//obj.runtimeStyle.cssText = "background-color:#FFFFFF";//？？？
// obj.className="Hover"; 
}  
function buttonOut(x){
   // window.alert(today.month);
     for(var i=document.all.caltable.rows.length-1;i>=2;i--){  //注：获取表格列的长度
        for(var j=document.all.caltable.rows[1].cells.length-1;j>=0;j--){
           if(((today.day!=document.getElementById("caltable").rows[i].cells[j].innerHTML)||(month!=today.month))&&document.getElementById("caltable").rows[i].cells[j].className!="Past"){ 
               //保留当天日期为红色,而不是在执行getDairy(x)刷新后被恢复成默认色; 日历换页后不出现当天日期红色(此案例的方法在getDiary(x)刷新日历后只是日历表格的值变化,但是表格属性不变,所以需要在刷新后把颜色恢复)
    if(document.getElementById("caltable").rows[i].cells[j].className!="DayPast"){
            document.getElementById("caltable").rows[i].cells[j].className="Day";
               } 
           }
        }
    }     //为试验三而取消
    
 //document.getElementById("caltable").rows[x.parentNode.rowIndex].cells[x.cellIndex].className="Day";  //试验三
    
  //  window.alert(caltable.rows[7].cells[0].innerHTML);
   // window.alert(x.innerHTML);
var obj = window.event.srcElement;
window.setTimeout(function(){obj.runtimeStyle.cssText = "";},300);
}

function getDiary(event){     //试验二
//function getDiary(x){
     clickCount=!clickCount;              //记录本次点击的状态
     clickMonth=month;
     clickYear=year;
     coordinateX=this.cellIndex;             //记录本次点击的状态
     coordinateY=this.parentNode.rowIndex;   //记录本次点击的状态
     
 //   clickDay=x.innerHTML;    //试验
  //  window.alert(clickDay);   
    // window.alert(coordinateY);
 if(!clickCount||oneway){ //把第二次选中的值放入第二个输入框
 //if(!clickCount){    
     y=document.getElementById("year").value;  //从表头直接取出年和月
     d=document.getElementById("month").value;
      //点击的日期的值
     if(oneway){
     document.getElementById("showdate").value=y+"-"+d+"-"+event.srcElement.innerHTML;
     document.getElementById("showdate").style.cssText="text-align:center"; //文字居中显示
     document.getElementById("caltable").style.display="none";
     }else{
     document.getElementById("showdateReturn").value=y+"-"+d+"-"+event.srcElement.innerHTML;
     document.getElementById("showdateReturn").style.cssText="text-align:center"; //文字居中显示
     document.getElementById("caltable").style.display="none";
     signal=!signal; //回归最初的状态，以确保下次打开日历时已过去的日期显示正常
     }
 }else{
     y=document.getElementById("year").value;
     d=document.getElementById("month").value;
     document.getElementById("showdate").value=y+"-"+d+"-"+event.srcElement.innerHTML;  //点击的日期的值
     document.getElementById("showdate").style.cssText="text-align:center"; //文字居中显示
     
//    Calendar();  //试验
     
 }
}
    
/*function calclick(x) { //计算点击的情况(未使用)
   // cordinate=x.id;
   // document.getElementById("showclick").value=coordinateX;
    clickMonth=month;
}  */

function rili(){
document.getElementById('caltable').style.display='';//点击按钮显示弹出日历
Calendar();   //程序的入口
}

$(document).ready(function(){
 // $("p").click(function(){
  $("#showdateReturn").hide();
//  });
});
