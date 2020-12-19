var layerIndex = null;

function layerMes(msg, dom) {
    layerIndex = layer.tips(msg, dom, {
        tips: [3, '#78BA32']
    });
}

function CuteStr(str, len) {
    if (!str)
        return str;
    if (str.length > len) {
        return str.substr(0, len) + '...';
    } else {
        return str;
    }

}

function layerClose() {
    if (layerIndex)
        layer.close(layerIndex);
}

function LayerAlert(msg) {
    layerIndex = layer.alert(msg, {
        title: false, closeBtn: 0, offset: 'auto', btn: ['OK'], yes: function (index) {
            layer.close(index);
        }});
}


function LayerMsg(msg) {
    layerIndex = layer.msg(msg, { time: 1100, offset: 'auto' });
}



function CheckMail(mail) {
    var filter = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (filter.test(mail)) {
        return true;
    }
    else {
        return false;
    }
}


function StrTrim(str) {
    if (!str)
        return "";
    return str.replace(/(^\s*)|(\s*$)/g, "");
}


function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}


function isPositiveInteger(s) {
    var re = /^[0-9]+$/;
    return re.test(s);
}
function isEmail(str) {
    var reg = /^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+(.[a-zA-Z0-9_-])+/;
    return reg.test(str);
}
function getStorageItem(key) {
    var itemtemp = window.localStorage.getItem(key);
    var rettemp = "";
    if (itemtemp !== null && itemtemp !== "" && itemtemp !== "undefined" && itemtemp !== "null") {
        rettemp = itemtemp;
    }
    return rettemp;
}


function setStorageItem(key, value) {
    window.localStorage.setItem(key, value);
}

function removeStorageItem(key) {
    window.localStorage.removeItem(key);
}

function IsURL(str_url) {
    var strRegex = '[a-zA-z]+://[^\s]*';
    var reg = new RegExp(strRegex);
    return reg.test(str_url);
}

function IsNum(num) {
    var strRegex = /^(([1-9][0-9]*)|(([0]\.\d{1,2}|[1-9][0-9]*\.\d{1,2})))$/;
    var reg = new RegExp(strRegex);
    return reg.test(num);
}



function selected(id, val) {
    var obj = document.getElementById(id);
    for (i = 0; i < obj.length; i++) {
        if (obj.options[i].value === val) {
            obj.options[i].selected = true;
        }
    }
}

function isExistValue(text, domainUrl) {
    var bo = false;
    $.ajax({
        url: domainUrl,
        data: { text: text },
        dataType: 'json', type: "POST", async: false,
        success: function (res) {
            if (res.code === 0) {
                bo = true;
            } else {
                bo = false;
            }
        }
    });
    return bo;
}



function convertStrToDate(str) {
    if (str === undefined || str.length <= 0) {
        return null;
    }
    else {
        var arr = str.match(/\d+/g);
        if (arr !== null && arr.length >= 3) {
            var date = new Date(arr[0], arr[1], arr[2]);
            switch (arr.length) {
                case 4:
                    date.setHours(arr[3]);
                    break;
                case 5:
                    date.setHours(arr[3]);
                    date.setMinutes(arr[4]);
                    break;
                case 6:
                    date.setHours(arr[3]);
                    date.setMinutes(arr[4]);
                    date.setSeconds(arr[5]);
                    break;
            }
            return date;
        }
        else
            return null;
    }
}

function formatDate(date) {
    if (date === "" || date === null)
        return "";
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [year, month, day].join('-');
}


function CountWords(textarea, number, total1, total2) {
    var counter = textarea.val().length;
    number.text(Math.abs(total1 - counter));
    textarea.keyup(function () {
        var text = textarea.val();
        var counter = text.length;
        number.text(Math.abs(total2 - counter));
    });
}

function setCheckBoxCheckedValue(el, values, splitStr) {
    var mycheck = document.getElementsByName(el);
    for (var k = 0; k < mycheck.length; k++) {
        mycheck[k].checked = false;
    }
    if (values === undefined || values === "") {
        return;
    }
    var valuesArr = values.split(splitStr);
    var len = valuesArr.length;
    var value = "";
    for (var i = 0; i < len; i++) {
        value = valuesArr[i];
        value = value.replace(splitStr, "")
        for (var j = 0; j < mycheck.length; j++) {
            if (mycheck[j].value === value) {
                mycheck[j].checked = true;
                break;
            }
        }
    }
}


function setCheckedValue(el, checkedValue) {
    var myradio = document.getElementsByName(el);
    for (var i = 0; i < myradio.length; i++) {
        if (myradio[i].value === checkedValue) {
            myradio[i].checked = true;
            break;
        }
    }
}

var $ajaxLoadingFunc = function (reqUrl, jsonData, callback) {
    var load = layer.load(0);
    $.ajax({
        type: "POST",
        url: reqUrl,
        data: jsonData,
        success: function (data) {
            if (load)
                layer.close(load);
            if (typeof callback === "function") {
                callback(data);
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            if (load)
                layer.close(load);
            console.log("ajax error:" + xhr.statusText);
        }

    });
};

var $ajaxFunc = function (reqUrl, jsonData, callback) {
    $.ajax({
        type: "POST",
        url: reqUrl,
        data: jsonData,
        success: function (data) {
            if (typeof callback === "function") {
                callback(data);
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            console.log("ajax error:" + xhr.statusText);
        }

    });
};

function parsePrice(s) {
    var n = 2; //设置保留的小数位数 
    if (!s)
        return 0;
    if (!parseFloat(s))
        return 0;

    s = parseFloat((s + "").replace(/[^\d\.-]/g, "")).toFixed(n) + "";
    var l = s.split(".")[0].split("").reverse();
    var r = s.split(".")[1];
    var t = "";
    for (i = 0; i < l.length; i++) {
        t += l[i];
    }
    return  t.split("").reverse().join("") + "." + r;
} 

//金额类型输入
//<input type="text" onkeyup="toMoney(this)" size="10"/>元
//限制只能输入数字：
//<input type="text" onkeyup='this.value=this.value.replace(/\D/gi,"")'/>
//限制只能输入数字、字母和"-"：
//<input type="text" onkeyup='value=value.replace(/[^A-Za-z0-9\-]+/g,"")'/>
function toMoney(obj) {
    obj.value = obj.value.replace(/[^\d.]/g, ""); //清除"数字"和"."以外的字符
    obj.value = obj.value.replace(/^\./g, ""); //验证第一个字符是数字
    obj.value = obj.value.replace(/\.{2,}/g, "."); //只保留第一个, 清除多余的
    obj.value = obj.value.replace(".", "$#$").replace(/\./g, "").replace("$#$", ".");
    obj.value = obj.value.replace(/^(\-)*(\d+)\.(\d\d).*$/, '$1$2.$3'); //只能输入两个小数
}

function clearNoNum(obj) {
    obj.value = obj.value.replace(/[^\d.]/g, ""); //清除"数字"和"."以外的字符  
    obj.value = obj.value.replace(/^\./g, ""); //验证第一个字符是数字而不是  
    obj.value = obj.value.replace(/^00/g, ""); //验证前两位不能同时为0
    obj.value = obj.value.replace(/\.{2,}/g, "."); //只保留第一个. 清除多余的  
    obj.value = obj.value.replace(".", "$#$").replace(/\./g, "").replace("$#$", ".");
    obj.value = obj.value.replace(/^(\-)*(\d+)\.(\d\d).*$/, '$1$2.$3');
}


function regNum(val) {
    var reg = /^\d*$/;
    return reg.test(val);
}
