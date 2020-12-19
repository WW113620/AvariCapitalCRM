/**

 @Name：layuiAdmin 主页控制台
 @Author：贤心
 @Site：http://www.layui.com/admin/
 @License：GPL-2
    
 */


layui.define(function (exports) {

    /*
      下面通过 layui.use 分段加载不同的模块，实现不同区域的同时渲染，从而保证视图的快速呈现
    */


    //区块轮播切换
    layui.use(['admin', 'carousel'], function () {
        var $ = layui.$
            , admin = layui.admin
            , carousel = layui.carousel
            , element = layui.element
            , device = layui.device();

        //轮播切换
        $('.layadmin-carousel').each(function () {
            var othis = $(this);
            carousel.render({
                elem: this
                , width: '100%'
                , arrow: 'none'
                , interval: othis.data('interval')
                , autoplay: othis.data('autoplay') === true
                , trigger: (device.ios || device.android) ? 'click' : 'hover'
                , anim: othis.data('anim')
            });
        });

        element.render('progress');

    });

    //数据概览
    layui.use(['admin', 'carousel', 'echarts'], function () {
        var $ = layui.$
            , admin = layui.admin
            , carousel = layui.carousel
            , echarts = layui.echarts;

        function initChart() {
            $ajaxFunc(layui.setter.baseUrl + "Home/GetChartData", {}, function (res) {
                if (res.code === 0) {
                    console.log(res.data);
                    bindChart(res.data);
                }
            });
        }

        initChart();

        function bindChart(data) {
            var line1 = data.MediaFileLineData;
            var pie = data.MaterialPieData;
            var line2 = data.UserLineData;
            var echartsApp = [];
            var options = [
                //line 1
                {
                    title: {
                        text: line1.name,
                        x: 'center',
                        textStyle: {
                            fontSize: 14
                        }
                    },
                    tooltip: {
                        trigger: 'axis'
                    },
                    legend: {
                        data: ['', '']
                    },
                    xAxis: [{
                        type: 'category',
                        boundaryGap: false,
                        data: line1.xAxisData
                    }],
                    yAxis: [{
                        type: 'value',
                        name: 'Files count'
                    }],
                    series: line1.series
                },

                //pie
                {
                    title: {
                        text: pie.name,
                        x: 'center',
                        textStyle: {
                            fontSize: 14
                        }
                    },
                    tooltip: {
                        trigger: 'item',
                        formatter: "{a} <br/>{b} : {c} ({d}%)"
                    },
                    legend: {
                        orient: 'vertical',
                        x: 'left',
                        data: pie.xAxisData
                    },
                    series: [{
                        name: 'Classified data statistics',
                        type: 'pie',
                        radius: '55%',
                        center: ['50%', '50%'],
                        data: pie.series
                    }]
                },

                //line2
                {
                    title: {
                        text: line2.name,
                        x: 'center',
                        textStyle: {
                            fontSize: 14
                        }
                    },
                    tooltip: {
                        trigger: 'axis'
                    },
                    xAxis: [{ //X轴
                        type: 'category',
                        data: line2.xAxisData
                    }],
                    yAxis: [{  //Y轴
                        type: 'value',
                        name: 'Number of new users'
                    }],
                    series: line2.series
                }
            ];

            var elemDataView = $('#LAY-index-dataview').children('div')
                , renderDataView = function (index) {
                    echartsApp[index] = echarts.init(elemDataView[index], layui.echartsTheme);
                    echartsApp[index].setOption(options[index]);
                    //window.onresize = echartsApp[index].resize;
                    admin.resize(function () {
                        echartsApp[index].resize();
                    });
                };


            //没找到DOM，终止执行
            if (!elemDataView[0]) return;



            renderDataView(0);

            //监听数据概览轮播
            var carouselIndex = 0;
            carousel.on('change(LAY-index-dataview)', function (obj) {
                renderDataView(carouselIndex = obj.index);
            });

            //监听侧边伸缩
            layui.admin.on('side', function () {
                setTimeout(function () {
                    renderDataView(carouselIndex);
                }, 300);
            });

            //监听路由
            layui.admin.on('hash(tab)', function () {
                layui.router().path.join('') || renderDataView(carouselIndex);
            });

        }


      
    });

    //最新订单
    layui.use('table', function () {
        var $ = layui.$
            , table = layui.table;

        //今日热搜
        table.render({
            elem: '#LAY-index-topSearch'
            , url: layui.setter.baseUrl + 'Home/GetDownloadStatistics'
            , cols: [[
                { field: '_row_number_', title: 'Sequence number', align: 'center', width: 200, sort: true }
                , { field: 'MediaFileName', title: 'Download file', minWidth: 300 }
                , { field: 'UserName', title: 'Download user', minWidth: 200 }
                , { field: 'DownloadCount', title: 'Download count', width: 300, align: 'center', sort: true }
            ]]
            , page: true
            , limit: 10
            , text: {
                none: 'No download data'
            }
        });

    });

    exports('console', {})
});