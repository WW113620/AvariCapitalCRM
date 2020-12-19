
layui.define(['table', 'form'], function (exports) {
    var $ = layui.$
        , table = layui.table
        , layer = layui.layer
        , form = layui.form;

    //订单管理
    table.render({
        elem: '#LAY-user-manage'
        , url: layui.setter.baseUrl + 'Order/GetList'
        , cols: [[
            { field: '_row_number_', width: 60, align: 'center', title: '序号' }
            , { field: 'OrderSn', width: 200, title: '订单号' }
            , { field: 'PlatformName', title: '订单来源', width: 120 }
            , { field: 'UserName', title: '用户名', minWidth: 160 }
            , {
                field: 'PayStateName', title: '支付状态', width: 120, templet: function (d) {
                    if (d.PayState === 0) {
                        return '<span style="color:#FF4500;">' + d.PayStateName + '</span>';
                    }
                    else if (d.PayState === 4) {
                        return '<span style="color:#008000;">' + d.PayStateName + '</span>';
                    }
                    else if (d.PayState === 5) {
                        return '<span style="color:#c00;">' + d.PayStateName + '</span>';
                    }
                    else if (d.PayState === 6) {
                        return '<span style="color:#c3b0b0;">' + d.PayStateName + '</span>';
                    }
                    else {
                        return '<span style="color:#30614d;">' + d.PayStateName + '</span>';
                    }

                }
            }
            , { field: 'PayChannelName', title: '支付方式', width: 110 }
            , { field: 'AddTimeValue', width: 150, title: '下单时间' }
            , { field: 'PayTimeValue', width: 150, title: '支付时间' }
            , { field: 'TotalAccount', width: 100, title: '订单金额' }
            , {
                field: 'PayAccount', width: 100, title: '支付金额', templet: function (d) {
                    if (d.PayState === 0) {
                        return '<span style="color:#FF4500;">' + d.PayAccount + '</span>';
                    }
                    else if (d.PayState === 4) {
                        return '<span style="color:#008000;">' + d.PayAccount + '</span>';
                    }
                    else if (d.PayState === 5) {
                        return '<span style="color:#c00;">' + d.PayAccount + '</span>';
                    }
                    else if (d.PayState === 6) {
                        return '<span style="color:#c3b0b0;">' + d.PayAccount + '</span>';
                    }
                    else {
                        return '<span style="color:#30614d;">' + d.PayAccount + '</span>';
                    }
                }
            }
            , {
                field: 'SignatureText', width: 200, title: '奶粉签名', templet: function (d) {
                    if (d.IsSignature) {
                        if (!d.SignatureText)
                            d.SignatureText = "---";
                        return '<span style="color:#FF4500;">' + d.SignatureText + '</span>';
                    }
                    else {
                        return '<span style="color:#30614d;">---</span>';
                    }
                }
            }
            , { title: '操作', width: 180, align: 'center', fixed: 'right', toolbar: '#table-useradmin-webuser' }
        ]]
        , page: true
        , limit: 10
        , text: {
            none: '暂无相关数据'
        }
    });

    form.on('submit(editorder)', function (obj) {
        console.log(obj.field);
        layer.confirm("确定修改此订单吗？", { title: false, closeBtn: 0, offset: ['30%', '32%'] }, function (index) {
            layer.close(index);
            $ajaxLoadingFunc(layui.setter.baseUrl + 'Order/Update', obj.field, function (res) {
                if (res.code === 0) {
                    layer.msg("修改成功", { time: 1000, offset: 'auto' }, function (index) {
                        layer.close(index);
                        location.href = layui.setter.baseUrl + 'Order/Edit/' + obj.field.OrderSn;
                    });
                } else {
                    layer.msg(res.msg, { time: 1200, offset: 'auto' });
                }
            });
        });

        return false;
    });


    //监听工具条
    table.on('tool(LAY-user-manage)', function (obj) {
        var data = obj.data;
        var orderSn = data.OrderSn;
        if (obj.event === 'detail') {
            location.href = layui.setter.baseUrl + 'Order/Detail/' + orderSn;
        } else if (obj.event === 'edit') {
            var win = window.open();
            win.location.href = layui.setter.baseUrl + 'Order/Edit/' + orderSn;
        }
    });

    var orderDetailUrl = 'Order/GetDetailList';
    if ($("#hidOrderSn").val()) {
        orderDetailUrl = orderDetailUrl + "?OrderSn=" + $("#hidOrderSn").val();
    }
    //订单详情
    table.render({
        elem: '#LAY-order-datail'
        , url: layui.setter.baseUrl + orderDetailUrl
        , cols: [[
            { field: '_row_number_', width: 60, align: 'center', title: '序号' }
            , { field: 'ProductName', minWidth: 300, title: '商品名称' }
            , { field: 'OriginalPriceValue', width: 100, title: '商品原价' }
            , { field: 'WebPriceValue', title: '折后价', width: 100 }
            , { field: 'OrderSn', minWidth: 180, title: '订单号' }
            , { field: 'UserName', title: '用户名', minWidth: 120 }
            , { field: 'OrderProductAmount', width: 100, title: '购买数量' }
            , {
                field: 'PayStateName', title: '支付状态', width: 120, templet: function (d) {
                    if (d.PayState === 0) {
                        return '<span style="color:#FF4500;">' + d.PayStateName + '</span>';
                    }
                    else if (d.PayState === 4) {
                        return '<span style="color:#008000;">' + d.PayStateName + '</span>';
                    }
                    else if (d.PayState === 5) {
                        return '<span style="color:#c00;">' + d.PayStateName + '</span>';
                    }
                    else if (d.PayState === 6) {
                        return '<span style="color:#c3b0b0;">' + d.PayStateName + '</span>';
                    }
                    else {
                        return '<span style="color:#30614d;">' + d.PayStateName + '</span>';
                    }

                }
            }
            , { field: 'PayChannelName', title: '支付方式', width: 120 }
            , { field: 'AddTimeValue', title: '下单时间', width: 150 }
            , { field: 'PayTimeValue', title: '支付时间', width: 150 }
            , {
                field: 'SignatureText', width: 200, title: '奶粉签名', templet: function (d) {
                    if (d.IsOrderSignature && d.ProductIsSignature === 1) {
                        if (!d.SignatureText)
                            d.SignatureText = "---";
                        return '<span style="color:#FF4500;">' + d.SignatureText + '</span>';
                    }
                    else {
                        return '<span style="color:#30614d;">---</span>';
                    }
                }
            }
        ]]
        , page: true
        , limit: 10
        , text: {
            none: '暂无相关数据'
        }
    });

    exports('order', {});
});