
$(document).ready(function () {
    // === 左侧导航选中 === //
    var indexActive = true;
    var locationUrl = location.href;
    $(".submenu a").each(function () {
        var rel = $(this).attr('href');
        if (locationUrl.indexOf(rel) > -1) {
            $(this).parent().addClass("active");
            $(this).parent().parent().parent().addClass("open");

            $(".child-active span").addClass("menu-text");

            indexActive = false;
            return true;
        }
    });
    if (indexActive) {
        $(".index").addClass("active");
    }

    // === 加载图片窗口 === //
    lightbox.option({
        wrapAround: false,
        albumLabel: "",
        resizeDuration: 200,
        maxWidth: 800,
        maxHeight: 600,
        disableScrolling: false
    });
});

/*******生成表格*********/
function GetMyDataTable(tableId, aoColumn, actionUrl, serverParams) {
    var myTable =
           $('#' + tableId)
           //.wrap("<div class='dataTables_borderWrap' />")   //if you are applying horizontal scrolling (sScrollX)
           .dataTable({
               bAutoWidth: false,
               "aoColumns": aoColumn,
               "aaSorting": [],
               "oLanguage": {
                   "sSearch": "搜索",
                   "sLengthMenu": "每页显示 _MENU_ 条记录",
                   "sZeroRecords": "抱歉， 没有找到",
                   "sProcessing": "正在加载中...",
                   "sInfo": "从 _START_ 到 _END_ /共 _TOTAL_ 条数据",
                   "sInfoEmpty": "没有数据",
                   "sInfoFiltered": "(从 _MAX_ 条数据中检索)",
                   "oPaginate": {
                       "sFirst": "首页",
                       "sPrevious": "前一页",
                       "sNext": "后一页",
                       "sLast": "尾页"
                   },
                   "sZeroRecords": "没有检索到数据",
                   "sProcessing": "正在加载中...",
                   //"sProcessing": "<img src='~/Content/ace/assets/img/loading.gif' />",
               },
               "bPaginate": true,//是否分页
               "bFilter": false,//是否使用内置的过滤功能------//"searching":false,//是否显示搜索框
               "bLengthChange": true, //是否允许自定义每页显示条数.
               //"iDisplayLength":10, //每页显示10条记录
               "bProcessing": true,//载入数据的时候是否显示“载入中”
               "bServerSide": true,//分页，取数据等等的都放到服务端去
               //'bStateSave': true,
               "bRetrieve": true,
               "bDestroy": true,
               "bSort": false, //排序功能
               "sAjaxSource": actionUrl,
               "sServerMethod": "POST",
               "fnServerParams": function (aoData) {
                   if (null != serverParams && "" != serverParams) {
                       var dataObj = eval(serverParams);
                       $.each(dataObj, function (idx, item) {
                           aoData.push({ "name": item.name, "value": item.value });
                       });
                   }
                   //aoData = serverParams;
                   //aoData.push({ "name": "Name", "value": $('#Name').val() });
                   //aoData.push({ "name": "Code", "value": $('#Code').val() });
               },

               //如果加上下面这段内容，则使用post方式传递数据
               //"fnServerData": function ( sSource, aoData, fnCallback ) {
               //    $.ajax( {
               //        "dataType": 'json',
               //        "type": "POST",
               //        "url": sSource,
               //        "data": aoData,
               //        "success": fnCallback
               //    } );
               //},

               //"sScrollY": "200px",
               //"bPaginate": false,

               //"sScrollX": "100%",
               //"sScrollXInner": "120%",
               //"bScrollCollapse": true,
               //Note: if you are applying horizontal scrolling (sScrollX) on a ".table-bordered"
               //you may want to wrap the table inside a "div.dataTables_borderWrap" element

               //"iDisplayLength": 50
               select: {
                   style: 'multi'
               }
           });
    var myTableApi = myTable.api();
    $.fn.dataTable.ext.errMode = function (s, h, m) { location.href = '/home/index'; }//错误跳转首页
    //导出按钮
    //$.fn.dataTable.Buttons.swfPath = "/Content/ace/assets/js/dataTables/extensions/buttons/swf/flashExport.swf"; //in Ace demo ../assets will be replaced by correct assets path
    //$.fn.dataTable.Buttons.defaults.dom.container.className = 'dt-buttons btn-overlap btn-group btn-overlap';

    //new $.fn.dataTable.Buttons(myTableApi, {
    //    buttons: [
    //      {
    //          "extend": "colvis",
    //          "text": "<i class='fa fa-search bigger-110 blue'></i> <span class='hidden'>显示/隐藏 列 </span>",
    //          "className": "btn btn-white btn-primary btn-bold",
    //          columns: ':not(:first):not(:last)'
    //      },
    //      {
    //          "extend": "copy",
    //          "text": "<i class='fa fa-copy bigger-110 pink'></i> <span class='hidden'>复制到剪切板</span>",
    //          "className": "btn btn-white btn-primary btn-bold"
    //      },
    //      {
    //          "extend": "csv",
    //          "text": "<i class='fa fa-database bigger-110 orange'></i> <span class='hidden'>导出 CSV</span>",
    //          "className": "btn btn-white btn-primary btn-bold"
    //      },
    //      {
    //          "extend": "excel",
    //          "text": "<i class='fa fa-file-excel-o bigger-110 green'></i> <span class='hidden'>导出 Excel</span>",
    //          "className": "btn btn-white btn-primary btn-bold"
    //      },
    //      {
    //          "extend": "pdf",
    //          "text": "<i class='fa fa-file-pdf-o bigger-110 red'></i> <span class='hidden'>导出 PDF</span>",
    //          "className": "btn btn-white btn-primary btn-bold"
    //      },
    //      {
    //          "extend": "print",
    //          "text": "<i class='fa fa-print bigger-110 grey'></i> <span class='hidden'>打印</span>",
    //          "className": "btn btn-white btn-primary btn-bold",
    //          autoPrint: false,
    //          message: 'This print was produced using the Print button for DataTables'
    //      }
    //    ]
    //});
    //myTableApi.buttons().container().appendTo($('.tableTools-container'));

    //style the message box
    var defaultCopyAction = myTableApi.button(1).action();
    myTableApi.button(1).action(function (e, dt, button, config) {
        defaultCopyAction(e, dt, button, config);
        $('.dt-button-info').addClass('gritter-item-wrapper gritter-info gritter-center white');
    });


    var defaultColvisAction = myTableApi.button(0).action();
    myTableApi.button(0).action(function (e, dt, button, config) {

        defaultColvisAction(e, dt, button, config);
        if ($('.dt-button-collection > .dropdown-menu').length == 0) {
            $('.dt-button-collection')
            .wrapInner('<ul class="dropdown-menu dropdown-light dropdown-caret dropdown-caret" />')
            .find('a').attr('href', '#').wrap("<li />")
        }
        $('.dt-button-collection').appendTo('.tableTools-container .dt-buttons')
    });

    ////

    setTimeout(function () {
        $($('.tableTools-container')).find('a.dt-button').each(function () {
            var div = $(this).find(' > div').first();
            if (div.length == 1) div.tooltip({ container: 'body', title: div.parent().text() });
            else $(this).tooltip({ container: 'body', title: $(this).text() });
        });
    }, 500);


    $('#' + tableId + ' tbody').on('click', 'tr', function () {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        }
        else {
            myTable.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }
    });

    ////multiple check
    //if ($('th input[type=checkbox], td input[type=checkbox]') != null && $('th input[type=checkbox], td input[type=checkbox]').length > 0) {
    //    //table checkboxes
    //    $('th input[type=checkbox], td input[type=checkbox]').prop('checked', false);
    //    myTableApi.on('select', function (e, dt, type, index) {
    //        if (type === 'row') {
    //            $(myTableApi.row(index).node()).find('input:checkbox').prop('checked', true);
    //        }
    //    });
    //    myTableApi.on('deselect', function (e, dt, type, index) {
    //        if (type === 'row') {
    //            $(myTableApi.row(index).node()).find('input:checkbox').prop('checked', false);
    //        }
    //    });
    //    //select/deselect all rows according to table header checkbox
    //    $('#dynamic-table > thead > tr > th input[type=checkbox], #dynamic-table_wrapper input[type=checkbox]').eq(0).on('click', function () {
    //        var th_checked = this.checked;//checkbox inside "TH" table header

    //        $('#dynamic-table').find('tbody > tr').each(function () {
    //            var row = this;
    //            if (th_checked) myTableApi.row(row).select();
    //            else myTableApi.row(row).deselect();
    //        });
    //    });

    //    //select/deselect a row when the checkbox is checked/unchecked
    //    $('#dynamic-table').on('click', 'td input[type=checkbox]', function () {
    //        var row = $(this).closest('tr').get(0);
    //        if (!this.checked) myTableApi.row(row).deselect();
    //        else myTableApi.row(row).select();
    //    });
    //}
    //    //single row select
    //else {
    //    myTableApi.on('select', function (e, dt, type, index) {
    //        if (type === 'row') {
    //            $('#dynamic-table').find('tbody > tr').each(function (trindex, element) {
    //                var row = this;
    //                if (trindex != index) myTableApi.row(row).deselect();
    //            });
    //        }
    //    });
    //}


    $(document).on('click', '#dynamic-table .dropdown-toggle', function (e) {
        e.stopImmediatePropagation();
        e.stopPropagation();
        e.preventDefault();
    });

    /********************************/
    //add tooltip for small view action buttons in dropdown menu
    $('[data-rel="tooltip"]').tooltip({ placement: tooltip_placement });

    //tooltip placement on right or left
    function tooltip_placement(context, source) {
        var $source = $(source);
        var $parent = $source.closest('table')
        var off1 = $parent.offset();
        var w1 = $parent.width();

        var off2 = $source.offset();
        //var w2 = $source.width();

        if (parseInt(off2.left) < parseInt(off1.left) + parseInt(w1 / 2)) return 'right';
        return 'left';
    }

    return myTable;
}

/*******清除表单数据*********/
function ClearForm(obj) {
    obj.find(':input').not(':button, :submit, :reset').val('').removeAttr('checked').removeAttr('selected');
    obj.find("select").prop('selectedIndex', 0);

    //判断是否存在省市联动存在则只保留第一项
    obj.find(".dropDownCity:not(.dontClear) option:not(:first)").remove();
    obj.find(".dropDownArea:not(.dontClear) option:not(:first)").remove();

}


/*******注册验证脚本*********/
function RegisterForm() {
    $('#modal-content').removeData('validator');
    $('#modal-content').removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse('#modal-content');
}

function RegisterForm2() {
    $('#modal-content2').removeData('validator');
    $('#modal-content2').removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse('#modal-content2');
}


/*******关闭弹出框*********/
function CloseModal() {
    $('#modal-form').modal('hide');
    ClearForm($("#modal-content"));
}

function CloseModal2() {
    $('#modal-form2').modal('hide');
    ClearForm($("#modal-content2"));
}


/*******刷新表格*********/
function ReloadDataTable(obj) {
    obj.fnDraw(); //重新加载数据

}

/*******弹出表单*********/
function ShowModal(actionUrl, param, title, flag) {

    //表单初始化
    $(".modal-title").html(title);
    $("#modal-content").attr("action", actionUrl);

    if (actionUrl.indexOf('?') > 0) {
        actionUrl = actionUrl + "&rand=" + Math.random();
    }
    else {
        actionUrl = actionUrl + "?rand=" + Math.random();
    }

    $.ajax({
        type: "GET",
        url: actionUrl,
        data: param,
        beforeSend: function () {
            //$("body").showLoading();//打开加载中...
        },
        success: function (result) {
           // $("body").hideLoading();//关闭加载中...
            $(".modal-footer ").show();
            if (flag != null && flag == true) {
                $(".modal-footer").html('<button type="button" class="btn btn-default" onclick="CloseModal()">返回列表</button>');
            } else {
                if (flag == "next") {
                    //下一步弹窗需要隐藏原按钮样式。
                    $(".modal-footer ").hide();
                } else {
                    $(".modal-footer").html('<button type="button" class="btn btn-primary" onclick="Save()">保存</button><button type="button" class="btn btn-default" onclick="CloseModal()">取消</button>')
                }
            }
            $("#modal-content").html(result);
            $('#modal-form').modal('show');
            RegisterForm();
        },
        error: function () {
            //
        },
        complete: function () {
            //
        }
    });
}

function ShowModal2(actionUrl, param, title, flag) {

    //表单初始化
    $("#modal-title2").html(title);
    $("#modal-content2").attr("action", actionUrl);

    $.ajax({
        type: "GET",
        url: actionUrl,
        data: param,
        beforeSend: function () {
            $("body").showLoading();//打开加载中...
        },
        success: function (result) {
            $("body").hideLoading();//关闭加载中...
            $("#modal-footer2").show();
            if (flag != null && flag == true) {
                $("#modal-footer2").html('<button type="button" class="btn btn-default" onclick="CloseModal2()">返回列表</button>');
            } else {
                if (flag == "next") {
                    //下一步弹窗需要隐藏原按钮样式。
                    $("#modal-footer2").hide();
                } else {
                    $("#modal-footer2").html('<button type="button" class="btn btn-primary" onclick="Save2()">保存</button><button type="button" class="btn btn-default" onclick="CloseModal2()">取消</button>')
                }
            }
            $("#modal-content2").html(result);
            $('#modal-form2').modal('show');
            // RegisterForm2();
        },
        error: function () {
            //
        },
        complete: function () {
            //
        }
    });
}

/*******保存表单*********/
function SaveModal(oTable) {
    var actionUrl = $("#modal-content").attr("action");
    var $form = $("#modal-content");
    
    $.ajax({
        type: "POST",
        url: actionUrl,
        data: $form.serialize(),
        beforeSend: function () {
            if (!$form.valid()) {
                return false;
            }
            $(".modal-content").showLoading();
        },
        success: function (data) {
            $(".modal-content").hideLoading();
            //判断返回值，若为Object类型，即操作成功
            var result = ((typeof (data) == 'object') && (data.constructor == Object));
            if (result) {
                bootbox.alert(data.Message);
                $('#modal-form').modal('hide');
                ReloadDataTable(oTable);
            }
            else {
                $("#modal-content").html(data);
            }
        }
    });
}
function SaveModal2(oTable) {
    var actionUrl = $("#modal-content2").attr("action");
    var $form = $("#modal-content2");
     
    $.ajax({
        type: "POST",
        url: actionUrl,
        data: $form.serialize(),
        beforeSend: function () {
            if (!$form.valid()) {
                return false;
            }
            $(".modal-content").showLoading();
        },
        success: function (data) {
            $(".modal-content").hideLoading();
            //判断返回值，若为Object类型，即操作成功
            var result = ((typeof (data) == 'object') && (data.constructor == Object));
            if (result) {
                bootbox.alert(data.Message);
                $('#modal-form2').modal('hide');
                ReloadDataTable(oTable);
            }
            else {
                $("#modal-content2").html(data);
            }
        }
    });
}



/*******查询*********/
function SearchRecord(actionUrl, oTable) {
    oTable.fnReloadAjax(actionUrl);
}

/*******删除操作*********/
function DeleteRecord(actionUrl, param, oTable) {
    bootbox.confirm("确定删除此记录?", function (result) {
        if (result) {
            $.ajax({
                type: "POST",
                url: actionUrl,
                data: param,
                beforeSend: function () {
                    //
                },
                success: function (result) {
                    bootbox.alert(result.Message);
                    if (result.ResultType == 0) {
                        ReloadDataTable(oTable);
                    }
                },
                error: function () {
                    //
                },
                complete: function () {
                    //
                }
            });
        } else {
        }
    });
}

/*******删除全部操作*********/
function DeleteAllRecord(actionUrl, oTable) {
    bootbox.confirm("确定删除此记录?", function (result) {
        if (result) {
            $.ajax({
                type: "POST",
                url: actionUrl,
                beforeSend: function () {
                    //
                },
                success: function (result) {
                    bootbox.alert(result.Message);
                    if (result.ResultType == 0) {
                        ReloadDataTable(oTable);
                    }
                },
                error: function () {
                    //
                },
                complete: function () {
                    //
                }
            });
        } else {

        }
    });
}

/*******饼状图形报表,position暂时不用>_<!*********/
function GetChartsPie(placeholder, data, position) {
    $.plot(placeholder, data, {
        series: {
            pie: {
                show: true,
                tilt: 0.8,
                highlight: {
                    opacity: 0.25
                },
                stroke: {
                    color: '#fff',
                    width: 2
                },
                startAngle: 2
            }
        },
        legend: {
            show: true,
            position: position || "ne",
            labelBoxBorderColor: null,
            margin: [-30, 15]
        }
          ,
        grid: {
            hoverable: true,
            clickable: true
        }
    })
    var $tooltip = $("<div class='tooltip top in'><div class='tooltip-inner'></div></div>").hide().appendTo('body');
    var previousPoint = null;

    placeholder.on('plothover', function (event, pos, item) {
        if (item) {
            if (previousPoint != item.seriesIndex) {
                previousPoint = item.seriesIndex;
                var tip = item.series['label'] + " : " + item.series['percent'] + '%';
                $tooltip.show().children(0).text(tip);
            }
            $tooltip.css({ top: pos.pageY + 10, left: pos.pageX + 10 });
        } else {
            $tooltip.hide();
            previousPoint = null;
        }

    });
}

/*******折线图形报表*********/
function GetChartsLine(placeholder, data) {

    var ticks = 5;
    var max = 10;
    var min = 0;
    var tickDecimals = 0;

    $.plot(placeholder, data, {
        hoverable: true,
        shadowSize: 0,
        series: {
            lines: { show: true },
            points: { show: true }
        },
        xaxis: {
            tickLength: 0
        },
        yaxis: {
            //ticks: ticks,
            //min: min,
            //max: max,
            //tickDecimals: tickDecimals
        },
        grid: {
            backgroundColor: { colors: ["#fff", "#fff"] },
            borderWidth: 1,
            borderColor: '#555',
            hoverable: true
        }
    });

    var $tooltip = $("<div class='tooltip top in'><div class='tooltip-inner'></div></div>").hide().appendTo('body');
    var previousPoint = null;

    // 绑定提示事件  
    placeholder.bind("plothover", function (event, pos, item) {
        if (item) {
            if (previousPoint != item.dataIndex) {
                previousPoint = item.dataIndex;
                var x = item.datapoint[0];
                var y = item.datapoint[1];
                var tip = item.series['label'] + ": x=" + x + ",y=" + y + "";
                $tooltip.show().children(0).text(tip);
            }
            $tooltip.css({ top: pos.pageY + 10, left: pos.pageX + 10 });
        }
        else {
            $tooltip.hide();
            previousPoint = null;
        }
    });
}

/*******圆形图报表*********/
function GetChartsCircular(placeholder, data, size) {

    if (size == undefined || size == null) size = 75;

    $.each(data, function (i, value) {
        var data = value.data;
        var color = value.color;
        var label = value.label;

        if (data == undefined || data == null) data = 0;
        if (label == undefined || label == null) label = data + "%";
        if (color == undefined || color == null) color = "#87CEEB";

        var $html = $('<div class="easy-pie-chart percentage" data-percent="' + data + '"><span class="percent">' + label + '</span></div>').appendTo(placeholder);

        $html.css({ "float": "left", "margin": "10px" });

        $html.easyPieChart({
            barColor: color,
            trackColor: '#EEEEEE',
            scaleColor: false,
            lineCap: 'butt',
            lineWidth: 12,
            animate: ace.vars['old_ie'] ? false : 1000,
            size: size
        }).css('color', $(this).data('color'));
    })
}

/*******柱状图报表*********/
function GetChartsColumnar(placeholder, data, isHorizontal) {

    if (isHorizontal == null || isHorizontal == undefined || isHorizontal == "") isHorizontal = false;

    $.plot(placeholder, data, {
        hoverable: true,
        shadowSize: 0,
        series: {
            bars: { show: true },//是否显示柱状
            lines: { show: false },//是否显示线条
            points: { show: false }//是否显示原点
        },
        bars: {
            align: "center",
            show: true,
            barWidth: 1,
            horizontal: isHorizontal//是否水平
        },
        xaxis: {

            show: true,
            mode: "categories",
            tickLength: 0
        },
        yaxis: {
            show: true,
            tickLength: 0
        },
        grid: {
            borderWidth: 1,
            borderColor: '#555',
            backgroundColor: { colors: ["#fff", "#fff"] },
            hoverable: true
        }
    });

    var $tooltip = $("<div class='tooltip top in'><div class='tooltip-inner'></div></div>").hide().appendTo('body');
    var previousPoint = null;

    // 绑定提示事件  
    placeholder.bind("plothover", function (event, pos, item) {
        if (item) {
            if (previousPoint != item.dataIndex) {
                previousPoint = item.dataIndex;
                var x = item.datapoint[0];
                var y = item.datapoint[1];
                var tip = item.series['label'] + ": x=" + x + ",y=" + y + "";
                $tooltip.show().children(0).text(tip);
            }
            $tooltip.css({ top: pos.pageY + 10, left: pos.pageX + 10 });
        }
        else {
            $tooltip.hide();
            previousPoint = null;
        }
    });
}

/*******统计图报表*********/
function GetdashboardInfobox(placeholder, data) {
    $.each(data, function (i, value) {
        var color = value.color;
        var icon = value.icon;
        var label = value.label;
        var stat = value.stat;
        var statContent = value.statContent;
        var content = value.content;
        var iclass = value.iclass;
        var statIcon = "", statbage = "", icolor = "", bgcolor = "";;

        switch (stat) {
            case "badge-success":
                statbage = "badge badge-success";
                statIcon = '<i class="ace-icon fa fa-arrow-up"></i>';

                break;
            case "badge-important":
                statbage = "badge badge-important";
                statIcon = '<i class="ace-icon fa fa-arrow-down"></i>';

                break;
            case "stat-success":
                statbage = "stat stat-success";

                break;
            case "stat-important":
                statbage = "stat stat-important";

                break;
            default:
                break;
        }
        if (color != null && color != undefined && color != "") { bgcolor = "background-color:" + color; icolor = "color:" + color; }
        if (label == undefined || label == null) label = "";
        if (content == undefined || content == null) content = "";
        if (statContent == undefined || statContent == null) statContent = "";

        var $html1 = $('<div class="infobox ' + iclass + '" style="' + icolor + '"></div>');
        var html2 = '<div class="infobox-icon"><i class="ace-icon fa ' + icon + '"  style="' + bgcolor + '"></i></div>';
        var html3 = '<div class="infobox-data"><span class="infobox-data-number">' + label + '</span><div class="infobox-content">' + content + '</div></div>';
        var html4 = '<div class="' + statbage + '">' + statContent + statIcon + '</div>';

        $html1.append(html2)
        $html1.append(html3)
        if (statbage != undefined && statbage != null && statbage != "")
            $html1.append(html4)

        $html1.appendTo(placeholder);
    })
}

/*******省级联动*********/
function InitProvincialLinkage() {
    var url = "/Report/Sales/InitProv";
    $.ajaxSetup({
        async: false
    });
    $.post(url, function (data) {
        if (data != "") {
            var optionHtml = "";
            var userType = data.UserType;
            switch (userType) {
                case 1://管理员
                    optionHtml = "<option value='0'>全部</option>";
                    $(data.ProvList).each(function () {
                        optionHtml = optionHtml + "<option value='" + this.Value + "'>" + this.Text + "</option>";
                    });
                    $(".dropDownProv").html(optionHtml);
                    break;
                case 2://省代
                    optionHtml = "";
                    $(data.ProvList).each(function () {
                        optionHtml = optionHtml + "<option value='" + this.Value + "'>" + this.Text + "</option>";
                    });
                    $(".dropDownProv").html(optionHtml);

                    optionHtml = "<option value='0'>全部</option>";
                    $(data.CityList).each(function () {
                        optionHtml = optionHtml + "<option value='" + this.Value + "'>" + this.Text + "</option>";
                    });
                    $(".dropDownCity").html(optionHtml);
                    $(".dropDownCity").addClass("dontClear");
                    break;
                case 3://县代
                    $(".dropDownProv").html("<option value='0'>" + data.ProvName + "</option>");
                    $(".dropDownCity").html("<option value='0'>" + data.CityName + "</option>");
                    $(".dropDownArea").html("<option value='0'>" + data.AreaName + "</option>");

                    break;
                case 6://市代
                    $(".dropDownProv").html("<option value='0'>" + data.ProvName + "</option>");
                    $(".dropDownCity").html("<option value='0'>" + data.CityName + "</option>");
                    optionHtml = optionHtml + "<option value='0'>全部</option>";
                    $(data.AreaList).each(function () {
                        optionHtml = optionHtml + "<option value='" + this.Value + "'>" + this.Text + "</option>";
                    });
                    $(".dropDownArea").html(optionHtml);
                    $(".dropDownArea").addClass("dontClear");
                    break;

            }
        }
    });

    $(".pcaTag").on("change", function () {
        var id = this.value;
        var obj = $(this);

        var neetLoadDataSelect, type;
        var optionHtml;
        if (obj.hasClass("dropDownProv")) {
            type = 0;
            neetLoadDataSelect = ".dropDownCity";
            optionHtml = "<option value='0'>全部</option>";
            $(".dropDownArea").html("<option value='0'>全部</option>");//清空县区选择
            $(".dropDownTown").html("<option value='0'>全部</option>");//清空乡镇选择
        } else if (obj.hasClass("dropDownCity")) {
            type = 1;
            neetLoadDataSelect = ".dropDownArea";
            optionHtml = "<option value='0'>全部</option>";
            $(".dropDownTown").html("<option value='0'>全部</option>");//清空乡镇选择
        }
        else {
            return false;
        }
        //请求获取下级数据
        var url = "/Report/Sales/GetProvincialLinkageData?selectType=" + type + "&parentID=" + id;
        $.get(url, function (data) {
            if (data != "") {
                $(data).each(function () {
                    optionHtml = optionHtml + "<option value='" + this.Value + "'>" + this.Text + "</option>";
                });
            }
            $(neetLoadDataSelect).html(optionHtml);
        });
    });
}
/*******百度地图绘点API*********/
function AjaxBaiDuApi(jsondt, areaname, displayrange) {
    var jsondata = JSON.parse(jsondt);
    // 百度地图API功能	
    map = new BMap.Map("allmap");

    map.enableScrollWheelZoom();   //启用滚轮放大缩小，默认禁用
    map.enableContinuousZoom();    //启用地图惯性拖拽，默认禁用

    var opts = {
        width: 250,     // 信息窗口宽度
        height: 100,     // 信息窗口高度
        title: "信息窗口", // 信息窗口标题
        enableMessage: true//设置允许信息窗发送短息
    };
    var pointArray = new Array();
    for (var i = 0; i < jsondata.length; i++) {
        var marker = new BMap.Marker(new BMap.Point(jsondata[i].longitude, jsondata[i].latitude));  // 创建标注
        var content = jsondata[i].Pic;
        map.addOverlay(marker);               // 将标注添加到地图中
        addClickHandler(content, marker);
        pointArray[i] = new BMap.Point(jsondata[i].longitude, jsondata[i].latitude);
    }
    //让所有点在视野范围内
    map.setViewport(pointArray);
    map.centerAndZoom(areaname, displayrange);
    function addClickHandler(content, marker) {
        marker.addEventListener("click", function (e) {
            openInfo(content, e)
        }
		);
    }
    function openInfo(content, e) {
        var p = e.target;
        var point = new BMap.Point(p.getPosition().lng, p.getPosition().lat);
        var infoWindow = new BMap.InfoWindow(content, opts);  // 创建信息窗口对象 
        map.openInfoWindow(infoWindow, point); //开启信息窗口
    }
}
/*******高德地图绘点API*********/
//map:地图绘制属性,jsondt:json字符串，areaname：城市名称
function AjaxAmapApi(map, jsondt, areaname) {
    if (jsondt == "[]" || jsondt == undefined) {//无数据是加载地区地图
        if (areaname == "" || areaname == undefined) {
            map = new AMap.Map("allmap", { resizeEnable: true, zoom: 3 });
        }
        else {
            map = new AMap.Map("allmap", { resizeEnable: true });
            //map.setCity(areaname);
        }
    }
    else {//有数据是绘点
        var jsondata = JSON.parse(jsondt);//数据
        //初始化地图对象，加载地图
        //以地区名,设置地图大小
        if (areaname == "" || areaname == undefined) {
            map = new AMap.Map("allmap", { resizeEnable: true, zoom: 3 });
        }
        else {
            map = new AMap.Map("allmap", { resizeEnable: true });
            map.setCity(areaname);
        }
        AimingPoint(map, areaname, jsondata);//瞄点
    }
    //map.setFitView();//将所有点设置在视野内
    map.setCity(areaname);
}
//绘图上瞄点
function AimingPoint(map, areaname, jsondata) {
    var infoWindow = new AMap.InfoWindow({ offset: new AMap.Pixel(0, -30) });
    for (var i = 0, marker; i < jsondata.length; i++) {
        var marker = new AMap.Marker({
            position: [jsondata[i].longitude, jsondata[i].latitude],
            map: map
        });
        marker.content = jsondata[i].Pic;
        marker.on('click', markerClick);
        marker.emit('click', { target: marker });
    }
    function markerClick(e) {
        infoWindow.setContent(e.target.content);
        infoWindow.open(map, e.target.getPosition());
    }

}
//设置高德地图中心点
function SetZoomAndCenter(map, longitude, latitude, level)//map:地图,longitude:经度,latitude:纬度,level:等级
{
    // 设置缩放级别和中心点
    map.setZoomAndCenter(level, [longitude, latitude]);
}
//根据地址生成坐标，并设置给地图层data-...属性
function GetCoordinate(StartAddr, container, callback)//StartAddr:地址,container:放置地图层
{
    var map = new AMap.Map(container, {
        resizeEnable: true
    });
    var geocoder = new AMap.Geocoder({
        //city: "010", //城市，默认：“全国”
        //radius: 1000, //范围，默认：500
        resizeEnable: true,
        zoom: 18
    });
    //var StartAddr = "广东省广州市天河区广汕二路";
    var infoWindow;//弹框
    var marker;//标记点
    geocoderToAddr(geocoder, StartAddr, callback);
    function geocoderToAddr(geocoder, StartAddr, callback) {
        //地理编码,返回地理编码结果
        geocoder.getLocation(StartAddr, function (status, result) {
            if (status === 'complete' && result.info === 'OK') {
                geocoder_CallBack(result, geocoder, callback);
            }
            callback.call(this);
        });
    }
    //新增点
    function addMarker(i, d, geocoder, callback) {
        infoWindow = new AMap.InfoWindow({
            content: d.formattedAddress,
            offset: { x: 0, y: -30 }
        });
        marker = new AMap.Marker({
            map: map,
            position: [d.location.getLng(), d.location.getLat()],
            draggable: true,
            cursor: 'move',
            raiseOnDrag: true
        });
        marker.setMap(map);//设置点为可拖拽
        GetSlideCoordinate(marker, callback);//添加点拖拽事件
        marker.on("mouseover", function (e) {
            infoWindow.open(map, marker.getPosition());
        });
    }
    //地理编码返回结果展示
    function geocoder_CallBack(data, geocoder, callback) {
        //地理编码结果数组
        var geocode = data.geocodes;
        for (var i = 0; i < geocode.length; i++) {
            $("#" + container).data("longitude", geocode[i].location.getLng());//设置经度
            $("#" + container).data("latitude", geocode[i].location.getLat());//设置纬度
            addMarker(i, geocode[i], geocoder, callback);
            map.setFitView();
        }
    }
    function geocoder_CallBackToAddr(data) {
        var address = data.regeocode.formattedAddress; //返回地址描述
    }
    function GetSlideCoordinate(marker, callback) {
        var clickEventListener = marker.on('dragend', function (e) {
            var address;
            geocoder.getAddress([e.lnglat.getLng(), e.lnglat.getLat()], function (status, result) {
                if (status === 'complete' && result.info === 'OK') {
                    geocoder_CallBackToAddr(result);
                    address = result.regeocode.formattedAddress;
                    $("#" + container).data("longitude", e.lnglat.getLng());//设置经度
                    $("#" + container).data("latitude", e.lnglat.getLat());//设置纬度
                    infoWindow.setContent(address);
                    infoWindow.open(map, e.target.getPosition());
                    callback.call(this);
                }
            });
        });
    }

}
