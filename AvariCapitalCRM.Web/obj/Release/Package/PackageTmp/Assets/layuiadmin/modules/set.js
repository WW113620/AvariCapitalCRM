/**

 @Name：layuiAdmin（iframe版） 设置
 @Author：贤心
 @Site：http://www.layui.com/admin/
 @License: LPPL
    
 */

layui.define(['form', 'upload'], function (exports) {
    var $ = layui.$
        , layer = layui.layer
        , laytpl = layui.laytpl
        , setter = layui.setter
        , view = layui.view
        , admin = layui.admin
        , form = layui.form
        , upload = layui.upload;

    var $body = $('body');

    //自定义验证
    form.verify({
        username: function (value) {
            value = StrTrim(value);
            if (!value) {
                return 'The user name cannot be empty';
            }
            if ((/^[a-zA-Z0-9_@.]{2,20}$/.test(value) === false) && (/^[a-z0-9]+([._\\-]*[a-z0-9])*@@([a-z0-9]+[-a-z0-9]*[a-z0-9]+.){2,20}[a-z0-9]+$/.test(value) === false)) {
                return "The user name is composed of 2-20 English letters, Numbers, or underscores";
            }
            var domainUrl = layui.setter.baseUrl + "User/Exist";
            var isExistEmail = isExistValue(value, domainUrl);
            if (isExistEmail === true) {
                return "The user name already exists";
            }


        }
       ,nickname: function (value, item) { //value：表单的值、item：表单的DOM对象
            if (!new RegExp("^[a-zA-Z0-9_\u4e00-\u9fa5\\s·]+$").test(value)) {
                return '用户名不能有特殊字符';
            }
            if (/(^\_)|(\__)|(\_+$)/.test(value)) {
                return '用户名首尾不能出现下划线\'_\'';
            }
            if (/^\d+\d+\d$/.test(value)) {
                return '用户名不能全为数字';
            }
        }
        , onlyLetter: function (value) {
            value = StrTrim(value);
            if (value) {
                if (value.length < 2) {
                    return 'The length cannot be less than 2';
                }
                if (value.length > 100) {
                    return 'The length cannot be greater than 100';
                }
                if (!new RegExp(/^[a-zA-Z]+$/).test(value)) {
                    return 'Please enter letters';
                }
            }
        }
        , newPhone: function (value) {
            value = StrTrim(value);
            if (value) {
                if (!new RegExp("^[0-9]{9,11}$").test(value)) {
                    return 'Please enter the correct phone number format';
                }

            }
            //else {
            //    return 'Phone number cannot be empty';
            //}

        }
        , newEmail: function (value) {
            value = StrTrim(value);
            if (value) {
                var myreg = /^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/;
                if (!myreg.test(value)) {
                    return 'Please enter the correct email format';
                }

            }
            //else {
            //    return 'Email cannot be empty';
            //}
        }
        //我们既支持上述函数式的方式，也支持下述数组的形式
        //数组的两个值分别代表：[正则匹配、匹配不符时的提示文字]
        , pass: [
            /^[\S]{6,12}$/
            , 'The password must be 6 to 12 bits and no Spaces are allowed'
        ]

        //确认密码
        , repass: function (value) {
            if (value !== $('#LAY_password').val()) {
                return 'The confirm password and the new password do not match';
            }
        }
    });

    //网站设置
    form.on('submit(set_website)', function (obj) {
        layer.msg(JSON.stringify(obj.field));

        //提交修改
        /*
        admin.req({
          url: ''
          ,data: obj.field
          ,success: function(){
            
          }
        });
        */
        return false;
    });

    //邮件服务
    form.on('submit(set_system_email)', function (obj) {
        layer.msg(JSON.stringify(obj.field));

        //提交修改
        /*
        admin.req({
          url: ''
          ,data: obj.field
          ,success: function(){
            
          }
        });
        */
        return false;
    });


    //设置我的资料
    form.on('submit(setmyinfo)', function (obj) {
        layer.msg(JSON.stringify(obj.field));

        //提交修改
        /*
        admin.req({
          url: ''
          ,data: obj.field
          ,success: function(){
            
          }
        });
        */
        return false;
    });

    //上传头像
    var avatarSrc = $('#LAY_avatarSrc');
    upload.render({
        url: '/api/upload/'
        , elem: '#LAY_avatarUpload'
        , done: function (res) {
            if (res.status == 0) {
                avatarSrc.val(res.url);
            } else {
                layer.msg(res.msg, { icon: 5 });
            }
        }
    });

    //查看头像
    admin.events.avartatPreview = function (othis) {
        var src = avatarSrc.val();
        layer.photos({
            photos: {
                "title": "查看头像" //相册标题
                , "data": [{
                    "src": src //原图地址
                }]
            }
            , shade: 0.01
            , closeBtn: 1
            , anim: 5
        });
    };



    //对外暴露的接口
    exports('set', {});
});