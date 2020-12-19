/**

 @Name：layuiAdmin 用户管理 管理员管理 角色管理
 @Author：star1029
 @Site：http://www.layui.com/admin/
 @License：LPPL
    
 */


layui.define(['table', 'form'], function (exports) {
    var $ = layui.$
        , table = layui.table
        , form = layui.form;

    //用户管理
    table.render({
        elem: '#LAY-user-manage'
        , url: layui.setter.base + 'json/useradmin/webuser.js' //模拟接口
        , cols: [[
            { type: 'checkbox', fixed: 'left' }
            , { field: 'id', width: 100, title: 'ID', sort: true }
            , { field: 'username', title: '用户名', minWidth: 100 }
            , { field: 'avatar', title: '头像', width: 100, templet: '#imgTpl' }
            , { field: 'phone', title: '手机' }
            , { field: 'email', title: '邮箱' }
            , { field: 'sex', width: 80, title: '性别' }
            , { field: 'ip', title: 'IP' }
            , { field: 'jointime', title: '加入时间', sort: true }
            , { title: '操作', width: 150, align: 'center', fixed: 'right', toolbar: '#table-useradmin-webuser' }
        ]]
        , page: true
        , limit: 30
        , height: 'full-220'
        , text: '对不起，加载出现异常！'
    });

    //监听工具条
    table.on('tool(LAY-user-manage)', function (obj) {
        var data = obj.data;
        if (obj.event === 'del') {
            layer.prompt({
                formType: 1
                , title: '敏感操作，请验证口令'
            }, function (value, index) {
                layer.close(index);

                layer.confirm('真的删除行么', function (index) {
                    obj.del();
                    layer.close(index);
                });
            });
        } else if (obj.event === 'edit') {
            var tr = $(obj.tr);

            layer.open({
                type: 2
                , title: '编辑用户'
                , content: '../../../views/user/user/userform.html'
                , maxmin: true
                , area: ['500px', '450px']
                , btn: ['确定', '取消']
                , yes: function (index, layero) {
                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submitID = 'LAY-user-front-submit'
                        , submit = layero.find('iframe').contents().find('#' + submitID);

                    //监听提交
                    iframeWindow.layui.form.on('submit(' + submitID + ')', function (data) {
                        var field = data.field; //获取提交的字段

                        //提交 Ajax 成功后，静态更新表格中的数据
                        //$.ajax({});
                        table.reload('LAY-user-manage'); //数据刷新
                        layer.close(index); //关闭弹层
                    });

                    submit.trigger('click');
                }
                , success: function (layero, index) {

                }
            });
        }
    });

    //管理员管理
    table.render({
        elem: '#LAY-user-back-manage'
        , url: '/admin/user/getlist' //模拟接口
        , cols: [[
            { type: 'checkbox', fixed: 'left' }
            , { field: 'Id', width: 80, title: 'ID', sort: true }
            , { field: 'UserName', title: '登录名' }
            , { field: 'Phone', title: '手机' }
            , { field: 'Email', title: '邮箱' }
            , {
                field: 'RoleType', title: '角色', templet: function (d) {
                    switch (d.RoleType) {
                        case 2:
                            return "管理员";
                        case 1:
                            return "超级管理员";
                        default:
                            return "";
                    }

                }
            }
            , { field: 'AddTimeValue', title: '加入时间', sort: true }
            , {
                field: 'State', title: '审核状态',
                templet: function (d) {
                    return d.State == 0 ? '<button class="layui-btn layui-btn-xs">已审核</button>'
                        : d.State == 1 ? '  <button class="layui-btn layui-btn-primary layui-btn-xs">未审核</button>'
                            : d.State == 2 ? '  <button class="layui-btn layui-btn-primary layui-btn-xs">已锁定</button>'
                                : '  <button class="layui-btn layui-btn-primary layui-btn-xs">已删除</button>';
                },
                minWidth: 80,
                align: 'center'
            }
            , {
                title: '操作', width: 150, align: 'center', fixed: 'right', templet: function (d) {
                    let content = '<a class="layui-btn layui-btn-normal layui-btn-xs" lay-event="edit"><i class="layui-icon layui-icon-edit"></i>编辑</a>';
                    if (d.RoleType == 1) {
                        content += ' <a class="layui-btn layui-btn-danger layui-btn-xs" lay-event="del"><i class="layui-icon layui-icon-delete"></i>删除</a>';
                    } else {
                        content += '<a class="layui-btn layui-btn-disabled layui-btn-xs"><i class="layui-icon layui-icon-delete"></i>删除</a>';
                    }
                    return content;
                }
            }
        ]]
        , text: '对不起，加载出现异常！'
    });

    //主页面列表监听工具条
    table.on('tool(LAY-user-back-manage)', function (obj) {
        var data = obj.data
            , checkStatus = table.checkStatus('LAY-user-back-manage')
            , checkData = checkStatus.data; //得到选中的数据;

        if (obj.event === 'del') {
            layer.prompt({
                formType: 1
                , title: '敏感操作，请验证口令'
            }, function (value, index) {
                console.log(value);
                layer.close(index);
                layer.confirm('确定删除此管理员？', function (index) {
                    $.ajax({
                        url: "/admin/user/deleteaction",    //请求的url地址
                        dataType: "json",   //返回格式为json
                        async: true,//请求是否异步，默认为异步，这也是ajax重要特性
                        data: { id: data.Id },    //参数值
                        type: "POST",   //请求方式
                        beforeSend: function () {
                            //请求前的处理
                        },
                        success: function (res) {
                            //请求成功时处理
                            if (res.code == 0) {
                                obj.del();
                                layer.close(index);
                                layer.msg(res.msg);
                            } else {
                                layui.use('layer', function () {
                                    var layer = layui.layer;
                                    layer.msg(res.msg);
                                });
                            }
                        },
                        complete: function (res) {
                            //请求完成的处理
                        },
                        error: function (res) {
                            //请求出错处理
                        }
                    });
                });
            });
        } else if (obj.event === 'edit') {
            var tr = $(obj.tr);
            layer.open({
                type: 2
                , title: '编辑管理员'
                , content: '/admin/user/add'
                , area: ['420px', '420px']
                , btn: ['确定', '取消']
                , yes: function (index, layero) {
                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submitID = 'LAY-user-back-submit'
                        , submit = layero.find('iframe').contents().find('#' + submitID)

                    //监听提交
                    iframeWindow.layui.form.on('submit(' + submitID + ')', function (data) {
                        var field = data.field; //获取提交的字段
                        //提交 Ajax 成功后，静态更新表格中的数据
                        $.ajax({
                            url: "/Admin/User/UpdateAction",    //请求的url地址
                            dataType: "json",   //返回格式为json
                            async: true,//请求是否异步，默认为异步，这也是ajax重要特性
                            data: field,    //参数值
                            type: "POST",   //请求方式
                            beforeSend: function () {
                                //请求前的处理
                            },
                            success: function (res) {
                                //请求成功时处理
                                if (res.code == 0) {
                                    table.reload('LAY-user-back-manage'); //数据刷新
                                    layer.close(index); //关闭弹层
                                } else {
                                    layui.use('layer', function () {
                                        var layer = layui.layer;
                                        layer.msg(res.msg);
                                    });
                                }
                            },
                            complete: function (res) {
                                //请求完成的处理
                            },
                            error: function (res) {
                                //请求出错处理
                            }
                        });

                    });

                    submit.trigger('click');
                }
                , success: function (layero, index) {
                    //获取弹窗子页面的div
                    var div = layero.find('iframe').contents().find('#layuiadmin-form-admin');
                    div.find("[name='RealName']").val(data.RealName);
                    div.find("[name='UserName']").val(data.UserName).attr("readonly","true");
                    div.find("[name='Password']").parents(".layui-form-item").remove();
                    div.find("[name='Phone']").val(data.Phone);
                    div.find("[name='Email']").val(data.Email);
                    div.find("[name='RoleType']").val(data.RoleType);
                    switch (data.State) {
                        case 0:
                            div.find("[name='State'][title='已审核']").attr("checked", true);
                            break;
                        case 1:
                            div.find("[name='State'][title='未审核']").attr("checked", true);
                            break;
                        case 2:
                            div.find("[name='State'][title='已锁定']").attr("checked", true);
                            break;
                        case 3:
                            div.find("[name='State'][title='已删除']").attr("checked", true);
                            break;
                        default:
                    }
                    form.render();

                }
            })
        }
    });

    //角色管理
    table.render({
        elem: '#LAY-user-back-role'
        , url: layui.setter.base + 'json/useradmin/role.js' //模拟接口
        , cols: [[
            { type: 'checkbox', fixed: 'left' }
            , { field: 'id', width: 80, title: 'ID', sort: true }
            , { field: 'rolename', title: '角色名' }
            , { field: 'limits', title: '拥有权限' }
            , { field: 'descr', title: '具体描述' }
            , { title: '操作', width: 150, align: 'center', fixed: 'right', toolbar: '#table-useradmin-admin' }
        ]]
        , text: '对不起，加载出现异常！'
    });

    //监听工具条
    table.on('tool(LAY-user-back-role)', function (obj) {
        var data = obj.data;
        if (obj.event === 'del') {
            layer.confirm('确定删除此角色？', function (index) {
                obj.del();
                layer.close(index);
            });
        } else if (obj.event === 'edit') {
            var tr = $(obj.tr);

            layer.open({
                type: 2
                , title: '编辑角色'
                , content: '../../../views/user/administrators/roleform.html'
                , area: ['500px', '480px']
                , btn: ['确定', '取消']
                , yes: function (index, layero) {
                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submit = layero.find('iframe').contents().find("#LAY-user-role-submit");

                    //监听提交
                    iframeWindow.layui.form.on('submit(LAY-user-role-submit)', function (data) {
                        var field = data.field; //获取提交的字段

                        //提交 Ajax 成功后，静态更新表格中的数据
                        //$.ajax({});
                        table.reload('LAY-user-back-role'); //数据刷新
                        layer.close(index); //关闭弹层
                    });

                    submit.trigger('click');
                }
                , success: function (layero, index) {

                }
            })
        }
    });

    exports('useradmin', {})
});