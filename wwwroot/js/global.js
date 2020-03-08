/**
 * 全局js
 * Autor: Fhua
 * Date: 16-11-25
 */

//Layui 扩展组件入口
layui.config({
    version: true,
    base: '../content/layui/lay/modules/extendplus/' //自定义layui组件的目录
}).extend({//设定组件别名
    common: 'common',
    navbar: 'navbar/navbar',
    tab: 'navbar/tab',
    icheck: 'icheck/icheck'
});

layui.use(['layer', 'element', 'util'], function () {
    var $ = layui.jquery
    , layer = layui.layer
    , device = layui.device()//设备信息
    , element = layui.element();

    //阻止IE7以下访问
    if (device.ie && device.ie < 8) {
        layer.alert('最低支持ie8，您当前使用的是古老的 IE' + device.ie + '！');
    }

    //手机设备的简单适配
    var treeMobile = $('.site-tree-mobile')
    , shadeMobile = $('.site-mobile-shade')

    treeMobile.on('click', function () {
        $('body').addClass('site-mobile');
    });

    shadeMobile.on('click', function () {
        $('body').removeClass('site-mobile');
    });
    $('#git,#weibo,#weixin').on('click', function () {
        layer.tips('暂时没有哦!', this)
    });

    var globalActive = {
        doRefresh: function () {
            var url = $(this).data('href');
            if (url) {
                location.href = url;
            }
            else {
                location.href = location.href;
            }
        },
        doGoTop: function () {
            $(this).click(function () {
                $('body,html').animate({ scrollTop: 0 }, 500);
                return false;
            });
        },
        doGoBack: function () {
            history.go(-1);
        },
        closeTab: function () {
          
            $(parent.document.getElementsByClassName('layui-tab-title')).find('.layui-this .layui-tab-close').click();
        }
    };

    $('.do-action').on('click', function (e) {
       
        var type = $(this).data('type');
        globalActive[type] ? globalActive[type].call(this) : '';
        layui.stope(e);//阻止冒泡事件
    });
});