layui.define(['table', 'form'], function (exports) {
    var $ = layui.$
        , table = layui.table
        , layer = layui.layer
        , form = layui.form;

    //优惠券管理
    table.render({
        elem: '#LAY-user-manage'
        , url: layui.setter.baseUrl + 'Coupon/GetList'
        , cols: [[
            { field: '_row_number_', width: 50, align: 'center', title: '序号' }
            , { field: 'CouponName', minWidth: 120, title: '优惠券名称' }
            , {
                field: 'CouponTypeName', title: '优惠券类型', width: 100, templet: function (d) {
                    if (d.CouponType === 2) {
                        return '<span style="color:#2c15ef;">' + d.CouponTypeName + '</span>';
                    }
                    else {
                        return '<span style="color:#d41010;">' + d.CouponTypeName + '</span>';
                    }

                }
            }
            , {
                field: 'OrderAmount', title: '满额数值', width: 90, templet: function (d) {

                    if (d.CouponType === 2) {
                        return '<span style="color:#2c15ef;">' + d.OrderAmount + '</span>';
                    }
                    else {
                        return '<span style="color:#d41010;">---</span>';
                    }

                }

            }
            , { field: 'ReductionAmount', title: '减免金额', width: 90 }

            , { field: 'StartDateValue', width: 110, title: '起始时间' }
            , { field: 'EndDateValue', width: 110,title: '结束时间' }
            , {
                field: 'CouponStatusName', title: '优惠券状态', width: 100, templet: function (d) {
                    if (d.CouponStatus === 0) {
                        return '<span style="color:#FF4500;">' + d.CouponStatusName + '</span>';
                    }
                    else if (d.PayState === 1) {
                        return '<span style="color:#008000;">' + d.CouponStatusName + '</span>';
                    }
                    else {
                        return '<span style="color:#c00;">' + d.CouponStatusName + '</span>';
                    }

                }
            }
            , { field: 'Description', minWidth: 120, title: '优惠券说明' }
            , { field: 'CouponCount', width: 90, title: '发放总量' }
            , {
                field: 'ReceiveCount', width: 90, title: '领取数量', templet: function (d) {
                    if (d.ReceiveCount) {
                        return d.ReceiveCount;
                    }
                    else {
                        return 0;
                    }

                } }
            , { field: 'AddDateValue', width: 150, title: '添加时间' }
            , { title: '操作', width: 180, align: 'center', fixed: 'right', toolbar: '#table-useradmin-webuser' }
        ]]
        , page: true
        , limit: 10
        , text: {
            none: '暂无相关数据'
        }
    });

    //监听工具条
    table.on('tool(LAY-user-manage)', function (obj) {
        var data = obj.data;
        var id = data.Id;
        var couponStatus = data.CouponStatus;
        if (obj.event === 'edit') {
            var win = window.open();
            win.location.href = layui.setter.baseUrl + 'Coupon/Add/' + id;
        }
        else if (obj.event === 'edit') {
            var win2 = window.open();
            win2.location.href = layui.setter.baseUrl + 'Coupon/Detail/' + id;
        }
        else if (obj.event === 'del') {
            layer.confirm("是否删除当前优惠券？", { title: false, closeBtn: 0 }, function (index) {
                layer.close(index);
                $ajaxLoadingFunc(layui.setter.baseUrl + 'Coupon/Delete', { Id: id }, function (res) {
                    if (res.code === 0) {
                        layer.msg("删除成功", { time: 1000, offset: 'auto' }, function (index) {
                            layer.close(index);
                            table.reload('LAY-user-manage');
                        });
                    } else {
                        layer.msg(res.msg, { time: 1200, offset: 'auto' });
                    }
                });
            });

        }
        else if (obj.event === 'publish') {
            layer.confirm("是否上架此优惠券？", { title: false, closeBtn: 0 }, function (index) {
                layer.close(index);
                $ajaxLoadingFunc(layui.setter.baseUrl + 'Coupon/UpdateCouponStatus', { Id: id, oldCouponStatus: couponStatus }, function (res) {
                    if (res.code === 0) {
                        layer.msg("上架成功", { time: 1000, offset: 'auto' }, function (index) {
                            layer.close(index);
                            table.reload('LAY-user-manage');
                        });
                    } else {
                        layer.msg(res.msg, { time: 1200, offset: 'auto' });
                    }
                });
            });

        } else if (obj.event === 'down') {
            layer.confirm("是否下架此优惠券？", { title: false, closeBtn: 0 }, function (index) {
                layer.close(index);
                $ajaxLoadingFunc(layui.setter.baseUrl + 'Coupon/UpdateCouponStatus', { Id: id, oldCouponStatus: couponStatus }, function (res) {
                    if (res.code === 0) {
                        layer.msg("下架成功", { time: 1000, offset: 'auto' }, function (index) {
                            layer.close(index);
                            table.reload('LAY-user-manage');
                        });
                    } else {
                        layer.msg(res.msg, { time: 1200, offset: 'auto' });
                    }
                });
            });
        }
    });

    exports('coupon', {});
});