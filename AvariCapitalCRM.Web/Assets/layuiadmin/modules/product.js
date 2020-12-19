layui.define(['table', 'form'], function (exports) {
    var $ = layui.$
        , table = layui.table
        , layer = layui.layer
        , form = layui.form
        , upload = layui.upload;

    table.render({
        elem: '#LAY-product-manage'
        , url: layui.setter.baseUrl + 'Product/GetList'
        , cols: [[
            { type: 'checkbox' }
            , { field: 'Id', width: 50, align: 'center', title: 'ID' }
            , {
                field: 'ProductName', minWidth: 250, title: '商品名称', templet: function (d) {
                    return '<a href="/Goods/Preview/' + d.Id +'" target="_blank">' + d.ProductName + '</a>';
                }
            }
            , { field: 'BrandName', title: '品牌', width: 100 }
            , { field: 'ShowCategoryName', title: '商品类别', width: 200 }
            , {
                field: 'IsHomePage', width: 90, title: '是否首页', templet: function (d) {
                    if (d.IsHomePage === 0) {
                        return '<span style="color:#30614d;">不展示</span>';
                    }
                    else {
                        return '<span style="color:#FF4500;">展示</span>';
                    }
                }
            }
            , {
                field: 'AuditStatusName', title: '商品状态', width: 90, templet: function (d) {
                    if (d.AuditStatus === 1) {
                        return '<span style="color:#008000;">' + d.AuditStatusName + '</span>';
                    }
                    else if (d.AuditStatus === 2) {
                        return '<span style="color:#c00;">' + d.AuditStatusName + '</span>';
                    }
                    else {
                        return '<span style="color:#FF4500;">' + d.AuditStatusName + '</span>';
                    }

                }
            }
            ,{
                field: 'IsSignature', width: 130, title: '是否签名', templet: function (d) {
                    if (d.IsSignature === 1) {
                        return '<span style="color:#FF5722;">签名</span>';
                    }
                    else {
                        return '<span>不签名</span>';
                    }
                }
            }
            , { field: 'OriginalPriceValue', title: '原始价格', width: 100 }
            , { field: 'WebPriceValue', title: '折扣后价格', width: 110 }
            , {
                field: 'TopStatus', width: 90, title: '是否置顶', templet: function (d) {
                    if (d.IsTop === 0) {
                        return '<span style="color:#30614d;">' + d.TopStatus + '</span>';
                    }
                    else {
                        return '<span style="color:#008000;">' + d.TopStatus + '</span>';
                    }
                }
            }
            , {
                field: 'NewStatus', width: 90, title: '是否新品', templet: function (d) {
                    if (d.IsNew === 0) {
                        return '<span style="color:#30614d;">' + d.NewStatus + '</span>';
                    }
                    else {
                        return '<span style="color:#008000;">' + d.NewStatus + '</span>';
                    }
                }
            }
           

            , { field: 'AddTimeValue', width: 150, title: '添加时间' }
            , { title: '操作', minWidth: 400, align: 'center', fixed: 'right', toolbar: '#tb-operate' }
        ]]
        , done: function (res, curr, count) {
            var state = 0;
            for (var i in res.data) {
                var item = res.data[i];
                if (item.AuditStatus === 1) {
                    $('tr[data-index=' + i + '] input[type="checkbox"]').prop('disabled', true);
                    state = 1;
                    form.render('checkbox');
                }
            }
            if (state === 1) {
                $('th[data-field="0"] input[type="checkbox"]').prop('disabled', true); // 禁止全选
            } else {
                $('th[data-field="0"] input[type="checkbox"]').prop('disabled', false); // 禁止全选
            }
        }
        , page: true
        , limit: 10
        , height: 520
        , text: {
            none: '暂无相关数据'
        }
    });


    form.on('submit(add)', function (obj) {
        if (obj.field.CategoryId == -1 || !obj.field.CategoryId) {
            layer.msg("请选择商品类别", { time: 1200, offset: 'auto', icon: 5 });
            return false;
        }
        if (!obj.field.CoverFileName) {
            layer.msg("请上传封面图片", { time: 1200, offset: 'auto', icon: 5 });
            return false;
        }
        var productId = parseInt(obj.field.Id);
        if (!productId) {
            if (!obj.field.MediaCollection) {
                layer.msg("请上传商品展示图片", { time: 1200, offset: 'auto', icon: 5 });
                return false;
            }

            var MediaCollection = obj.field.MediaCollection.split(',');
            if (MediaCollection.length == 0) {
                layer.msg("请上传商品展示图片", { time: 1200, offset: 'auto', icon: 5 });
                return false;
            }
        }

        $ajaxLoadingFunc(layui.setter.baseUrl + "Product/AddProduct", obj.field, function (res) {
            if (res.code === 0) {
                layer.msg(res.msg, { time: 1000, offset: 'auto' }, function (index) {
                    layer.close(index);
                    if (productId) {
                        location.href = layui.setter.baseUrl + 'Product/Add/' + productId;
                    } else {
                        window.location.reload();
                    }
                });
            }
            else if (res.code === 2) {
                LayerAlert(res.msg);
            }
            else {
                LayerMsg(res.msg);
            }
        });

        return false;
    });


    //监听工具条
    table.on('tool(LAY-product-manage)', function (obj) {
        var data = obj.data;
        if (obj.event === 'del') {
            layer.confirm("是否删除此商品？", { title: false, closeBtn: 0 }, function (index) {
                layer.close(index);
                $ajaxLoadingFunc(layui.setter.baseUrl + 'Product/Delete', { Id: data.Id }, function (res) {
                    if (res.code === 0) {
                        layer.msg("删除成功", { time: 1000, offset: 'auto' }, function (index) {
                            layer.close(index);
                            table.reload('LAY-product-manage');
                        });
                    } else {
                        layer.msg(res.msg, { time: 1200, offset: 'auto' });
                    }
                });
            });
        }
        else if (obj.event === 'publish') {
            layer.confirm("是否上架此商品,将会展示在销售页面？", { title: false, closeBtn: 0 }, function (index) {
                layer.close(index);
                $ajaxLoadingFunc(layui.setter.baseUrl + 'Product/Publish', { Id: data.Id }, function (res) {
                    if (res.code === 0) {
                        layer.msg("上架成功", { time: 1000, offset: 'auto' }, function (index) {
                            layer.close(index);
                            table.reload('LAY-product-manage');
                        });
                    }
                    else if (res.code === 2) {
                        LayerAlert(res.msg);
                    }
                    else {
                        LayerAlert(res.msg);
                    }
                });
            });
        }
        else if (obj.event === 'down') {
            layer.confirm("是否下架此商品，将会销售页面移除？", { title: false, closeBtn: 0 }, function (index) {
                layer.close(index);
                $ajaxLoadingFunc(layui.setter.baseUrl + 'Product/Down', { Id: data.Id }, function (res) {
                    if (res.code === 0) {
                        layer.msg("下架成功", { time: 1000, offset: 'auto' }, function (index) {
                            layer.close(index);
                            table.reload('LAY-product-manage');
                        });
                    } else {
                        layer.msg(res.msg, { time: 1200, offset: 'auto' });
                    }
                });
            });
        }
        else if (obj.event === 'edit') {
            var win = window.open();
            win.location.href = layui.setter.baseUrl + 'Product/Add/' + data.Id;
        }
        else if (obj.event === 'preview') {
            var winp = window.open();
            winp.location.href =  '/Goods/Preview/' + data.Id;
        }
        else if (obj.event === 'detail') {
            var wins = window.open();
            wins.location.href = layui.setter.baseUrl + 'Product/Detail/' + data.Id;
        }
        else if (obj.event === 'attribute') {
            var tr = $(obj.tr);
            layer.open({
                type: 2
                , title: '添加商品属性'
                , content: layui.setter.baseUrl + 'Product/Attribute/' + data.Id
                , maxmin: true
                , area: ['450px', '500px']

            });
        } else if (obj.event === 'signature') {
            var title = !data.IsSignature ? "是否添加罐底签证服务？" : "是否删除罐底签证服务？";
            var isSignature = !data.IsSignature;
            layer.confirm(title, { title: false, closeBtn: 0 }, function (index) {
                layer.close(index);
                $ajaxLoadingFunc(layui.setter.baseUrl + 'Product/Signature', { Id: data.Id, Signature: isSignature }, function (res) {
                    if (res.code === 0) {
                        layer.msg("修改成功", { time: 1000, offset: 'auto' }, function (index) {
                            layer.close(index);
                            table.reload('LAY-product-manage');
                        });
                    }
                    else if (res.code === 2) {
                        LayerAlert(res.msg);
                    }
                    else {
                        LayerAlert(res.msg);
                    }
                });
            });

        }
    });


    exports('product', {});
});