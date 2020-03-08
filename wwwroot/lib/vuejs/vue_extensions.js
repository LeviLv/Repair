﻿

String.prototype.format = function (args) {
    var result = this;
    if (arguments.length > 0) {
        if (arguments.length == 1 && typeof (args) == "object") {
            for (var key in args) {
                if (args[key] != undefined) {
                    var reg = new RegExp("({" + key + "})", "g");
                    result = result.replace(reg, args[key]);
                }
            }
        } else {
            for (var i = 0; i < arguments.length; i++) {
                if (arguments[i] != undefined) {
                    //var reg = new RegExp("({[" + i + "]})", "g"); 
                    var reg = new RegExp("({)" + i + "(})", "g");
                    result = result.replace(reg, arguments[i]);
                }
            }
        }
    }
    return result;
};


Date.prototype.format = function (format) {
    var o = {
        "M+": this.getMonth() + 1,
        "d+": this.getDate(),
        "h+": this.getHours(),
        "m+": this.getMinutes(),
        "s+": this.getSeconds(),
        "q+": Math.floor((this.getMonth() + 3) / 3),
        "S": this.getMilliseconds()
    }
    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    for (var k in o) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
        }
    }
    return format;
}

Vue.prototype.openIframe = function (title, url, area, callback) {
    var self = this;
    layer.open({
        type: 2
        , title: title
        , area: area
        , maxmin: true
        , content: url
        , zIndex: layer.zIndex,
        end: function () {
            if (typeof (callback) == "function") {
                callback();
            }
        }
    });
};
//深拷贝对象
Vue.prototype.cloneModel = function (obj) {
    var model = {};
    for (var item in obj) {
        if (obj[item] != null) {
            model[item] = obj[item];
        } else {
            model[item] = "";
        }
    }
    return model;
}

Vue.prototype.layerConfirm = function (title, btn1, btn2, func1, func2) {
    layer.confirm(title, {
        btn: [btn1, btn2] //按钮
    }, function () {
        func1();
        }, function () {

    });
}

Vue.prototype.getSync = function (url, callBack) {
    $.ajax({
        url: url,
        type: 'get',
        async: false,
        success: function (msg) {
            if (callBack && (typeof callBack === 'function')) {
                callBack(msg);
            }
        }
    });
}

Vue.prototype.postAsync = function (_url, _postData, callBack) {
    $.ajax({
        url: _url,
        type: 'post',
        data: _postData,
        success: function (msg) {
            if (callBack && (typeof callBack === 'function')) {
                callBack(msg);
            }
        },
        error: function (request, msg) {
            console.error(request, msg);
        }
    });
}

Vue.prototype.postSync = function ( _url, _postData, callBack) {
    $.ajax({
        url: _url,
        type: 'post',
        async: false,
        data: _postData,
        success: function (msg) {
            if (callBack && (typeof callBack === 'function')) {
                callBack(msg);
            }
        },
        error: function (request, msg) {
            console.error(request, msg);
        }
    });
}


var vueTableBase = Vue.extend({
    data: function () {
        return {
            baseUrl: '',
            defaultSearch: {},
            searchModel: {},
            items: [],
            selectedSelection: [],
            tableColumns: [],
            totalCount: 0,
            getlistUrl: '',
            currPage: 1,
            pageSize: 1
        }
    },
    methods: {
        getSmpFormatDate: function (date, isFull) {
            if (!date) {
                return '';
            }
            var pattern = "";
            if (isFull == true || isFull == undefined) {
                pattern = "yyyy-MM-dd hh:mm:ss";
            } else {
                pattern = "yyyy-MM-dd";
            }

            return date.format(pattern);
        },
        getDateByTime: function (times, isFull) {
            if (!times) {
                return '-';
            }
            var pattern = "";
            if (isFull == true || isFull == undefined) {
                pattern = "yyyy-MM-dd hh:mm:ss";
            } else {
                pattern = "yyyy-MM-dd";
            }

            return (new Date(times)).format(pattern);

        },
        getNameByVal: function (array, value) {
            var array = array.filter(function (item) {
                return item.Text == value;
            });
            if (array.length == 0) {
                return "-";
            }
            return array[0].ShowText.replace('请选择', '-');
        },
        search: function (curr) { 
            curr = curr || 1;
            this.searchModel.PageIndex = curr;
            this.currIndex = curr;
            var index = 0;
            if (window.layer) {
                index = layer.load(0);
            }
            if (this.searchExt) {
                this.searchExt();
            }
            var self = this;
            //$.ajax({
            //    url: this.getlistUrl,
            //    data: {
            //        search: this.searchModel
            //    },
            //    contentType: "application/json ; charset=utf-8",  
            //    type: 'post',
            //    dataType: "json",
            //    success: function (result) {
            //        if (window.layer) {
            //            layer.close(index);
            //        }
            //        if (result.ResponseStatus != 0) {
            //            layer.msg(result.ResponseMsg);
            //            return false;
            //        }

            //        self.items = result.ResponseData.List;
            //        self.totalCount = result.ResponseData.Total;
            //    }
            //});


            //开发记得改成post
            $.post(this.getlistUrl,  this.searchModel , function (result) {
                if (window.layer) {
                    layer.close(index);
                }
                if (result.responseStatus != 0) {
                    layer.msg(result.ResponseMsg);
                    return false;
                }

                self.items = result.responseData.list;
                self.totalCount = result.responseData.total;
            })
        },
        changeSelection: function (item) {
            this.selectedSelection = item;
            console.log(this.selectedSelection.length);
        },
        changePage: function (page) {
            this.search(page);
        },
        pageClick: function (size) {
            this.searchModel.PageSize = size;
            this.pageSize = size;
            this.search();
        }
        ,
        exportExcel: function () {
            var data = this.searchModel;
            var params = $.param(data);
            window.location.href = this.exportUrl + "?" + params;
        },
        reset: function () {
            var self = this;
            try {
                self.searchModel = self.cloneModel(JSON.parse(self.defaultSearch));
            } catch (e) {
                self.searchModel = self.cloneModel(self.defaultSearch);
            }
            for (var i in self.$refs) {
                if (!self.$refs[i]) {
                    continue;
                }
                if (typeof (self.$refs[i]['reset']) == "function") {
                    self.$refs[i]['reset']();
                }
            }
        }
    }
    ,
    created: function () {
        var self = this;
        try {
            self.searchModel = self.cloneModel(JSON.parse(self.defaultSearch));
        } catch (e) {
            self.searchModel = self.cloneModel(self.defaultSearch);
        }
        layui.use(['layer'], function () {

            var $ = layui.jquery, layer = layui.layer;
            if (window.vue && typeof (vue.search) == 'function') {
                self.search();
            }
        });
    }
});
var vueAccountTable = vueTableBase.extend({

    methods: {
        search: function (curr) {

        }
    }

});
//日期控件options
var vueDateOption = {
    shortcuts: [
        {
            text: '最近一周',
            value: function () {
                var end = new Date();
                var start = new Date();
                start.setTime(start.getTime() - 3600 * 1000 * 24 * 7);
                return [start, end];
            },
            getDefault: function () {
                var array = this.value();
                var start = array[0], end = array[1];
                start = start.format('yyyy-MM-dd');
                end = end.format('yyyy-MM-dd');
                return [start, end];
            }
        },
        {
            text: '最近一个月',
            value: function () {
                var end = new Date();
                var start = new Date();
                start.setTime(start.getTime() - 3600 * 1000 * 24 * 30);
                return [start, end];
            },
            getDefault: function () {
                var array = this.value();
                var start = array[0], end = array[1];
                start = start.format('yyyy-MM-dd');
                end = end.format('yyyy-MM-dd');
                return [start, end];
            }
        },
        {
            text: '最近三个月',
            value: function () {
                var end = new Date();
                var start = new Date();
                start.setTime(start.getTime() - 3600 * 1000 * 24 * 90);
                return [start, end];
            },
            getDefault: function () {
                var array = this.value();
                var start = array[0], end = array[1];
                start = start.format('yyyy-MM-dd');
                end = end.format('yyyy-MM-dd');
                return [start, end];
            }
        }
    ]
};

function ConvertDate(obj) {
    var c = new Date(parseInt(String(obj).substr(6, 13)));
    return c;
}

