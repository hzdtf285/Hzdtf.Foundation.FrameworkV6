var webUploader;
jQuery(function () {
    function e(e) {
        var a = o('<li id="' + e.id + '"><p class="title">' + e.name + '</p><p class="imgWrap"></p><p class="progress"><span></span></p></li>'),
            s = o('<div class="file-panel"><span class="cancel">删除</span><span class="rotateRight">向右旋转</span><span class="rotateLeft">向左旋转</span></div>').appendTo(a),
            i = a.find("p.progress span"),
            t = a.find("p.imgWrap"),
            r = o('<p class="error"></p>'),
            d = function (e) {
                switch (e) {
                    case "exceed_size":
                        text = "文件大小超出";
                        break;
                    case "interrupt":
                        text = "上传暂停";
                        break;
                    default:
                        text = "上传失败，请重试"
                }
                r.text(text).appendTo(a)
            };
        "invalid" === e.getStatus() ? d(e.statusText) : (t.text("预览中"), n.makeThumb(e, function (e, a) {
            if (e) return void t.text("不能预览");
            var s = o('<img src="' + a + '">');
            t.empty().append(s)
        }, v, b), w[e.id] = [e.size, 0], e.rotation = 0),
            e.on("statuschange", function (t, n) {
                "progress" === n ? i.hide().width(0) : "queued" === n && (a.off("mouseenter mouseleave"), s.remove()),
                    "error" === t || "invalid" === t ? (console.log(e.statusText), d(e.statusText), w[e.id][1] = 1) : "interrupt" === t ? d("interrupt") : "queued" === t ? w[e.id][1] = 0 : "progress" === t ? (r.remove(), i.css("display", "block")) : "complete" === t && a.append('<span class="success"></span>'),
                    a.removeClass("state-" + n).addClass("state-" + t)
            }),
            a.on("mouseenter", function () {
                s.stop().animate({
                    height: 30
                })
            }),
            a.on("mouseleave", function () {
                s.stop().animate({
                    height: 0
                })
            }),
            s.on("click", "span", function () {
                var a, s = o(this).index();
                switch (s) {
                    case 0:
                        return void n.removeFile(e);
                    case 1:
                        e.rotation += 90;
                        break;
                    case 2:
                        e.rotation -= 90
                }
                x ? (a = "rotate(" + e.rotation + "deg)", t.css({
                    "-webkit-transform": a,
                    "-mos-transform": a,
                    "-o-transform": a,
                    transform: a
                })) : t.css("filter", "progid:DXImageTransform.Microsoft.BasicImage(rotation=" + ~~(e.rotation / 90 % 4 + 4) % 4 + ")")
            }),
            a.appendTo(l)
    }
    function a(e) {
        var a = o("#" + e.id);
        delete w[e.id],
            s(),
            a.off().find(".file-panel").off().end().remove()
    }
    function s() {
        var e, a = 0,
            s = 0,
            t = f.children();
        o.each(w, function (e, i) {
            s += i[0],
                a += i[0] * i[1]
        }),
            e = s ? a / s : 0,
            t.eq(0).text(Math.round(100 * e) + "%"),
            t.eq(1).css("width", Math.round(100 * e) + "%"),
            i()
    }
    function i() {
        var e, a = "";
        "ready" === k ? a = "选中" + m + "张图片，共" + WebUploader.formatSize(h) + "。" : "confirm" === k ? (e = n.getStats(), e.uploadFailNum && (a = "已成功上传" + e.successNum + "张照片至XX相册，" + e.uploadFailNum + '张照片上传失败，<a class="retry" href="#">重新上传</a>失败图片或<a class="ignore" href="#">忽略</a>')) : (e = n.getStats(), a = "共" + m + "张（" + WebUploader.formatSize(h) + "），已上传" + e.successNum + "张", e.uploadFailNum && (a += "，失败" + e.uploadFailNum + "张")),
            p.html(a)
    }
    function t(e) {
        var a;
        if (e !== k) {
            switch (c.removeClass("state-" + k), c.addClass("state-" + e), k = e) {
                case "pedding":
                    u.removeClass("element-invisible"),
                        l.parent().removeClass("filled"),
                        l.hide(),
                        d.addClass("element-invisible"),
                        n.refresh();
                    break;
                case "ready":
                    u.addClass("element-invisible"),
                        o("#filePicker2").removeClass("element-invisible"),
                        l.parent().addClass("filled"),
                        l.show(),
                        d.removeClass("element-invisible"),
                        n.refresh();
                    break;
                case "uploading":
                    o("#filePicker2").addClass("element-invisible"),
                        f.show(),
                        c.text("暂停上传");
                    break;
                case "paused":
                    f.show(),
                        c.text("继续上传");
                    break;
                case "confirm":
                    if (f.hide(), c.text("开始上传").addClass("disabled"), a = n.getStats(), a.successNum && !a.uploadFailNum) return void t("finish");
                    break;
                case "finish":
                    a = n.getStats();
                    if (a.successNum) {
                        alert({ text: "上传成功", type: "success" });

                        if (typeof (webUploader.uploadSuccess) == "function") {
                            webUploader.uploadSuccess();
                        }

                        $("#filePicker2").removeClass("element-invisible");
                        $("#uploader .uploadBtn").removeClass("disabled"); 

                        var files = n.getFiles();
                        for (var j = 0; j < files.length;j++) {
                            n.removeFile(files[j]);
                        }
                        n.reset();
                    }
                    else {
                        (k = "done");
                        alert({ text: "上传失败", type: "error" });
                    }

                    break;
            }
            i()
        }
    }
    var n, o = jQuery,
        r = o("#uploader"),
        l = o('<ul class="filelist"></ul>').appendTo(r.find(".queueList")),
        d = r.find(".statusBar"),
        p = d.find(".info"),
        c = r.find(".uploadBtn"),
        u = r.find(".placeholder"),
        f = d.find(".progress").hide(),
        m = 0,
        h = 0,
        g = window.devicePixelRatio || 1,
        v = 110 * g,
        b = 110 * g,
        k = "pedding",
        w = {},
        x = function () {
            var e = document.createElement("p").style,
                a = "transition" in e || "WebkitTransition" in e || "MozTransition" in e || "msTransition" in e || "OTransition" in e;
            return e = null,
                a
        }();
    if (!WebUploader.Uploader.support()) throw alert("Web Uploader 不支持您的浏览器！如果你使用的是IE浏览器，请尝试升级 flash 播放器"),
        new Error("WebUploader does not support the browser you are using.");
    n = WebUploader.create({
        pick: {
            id: "#filePicker",
            label: "点击选择图片"
        },
        dnd: "#uploader .queueList",
        paste: document.body,
        accept: {
            title: "Images",
            extensions: "gif,jpg,jpeg,bmp,png",
            mimeTypes: "image/*"
        },
        swf: BASE_URL + "/Uploader.swf",
        disableGlobalDnd: !0,
        chunked: !0,
        server: $("#filePicker").attr("server"),
        fileNumLimit: 10,
        fileSizeLimit: 209715200,//总最大100M
        fileSingleSizeLimit: 20971520,//单个最大20M
        compressSize: 512000 //小于500KB不会压缩
    }),
        webUploader = n,
        n.addButton({
            id: "#filePicker2",
            label: "继续添加"
        }),
        n.onUploadProgress = function (e, a) {
            var i = o("#" + e.id),
                t = i.find(".progress span");
            t.css("width", 100 * a + "%"),
                w[e.id][1] = a,
                s()
        },
        n.onFileQueued = function (a) {
            m++ ,
                h += a.size,
                1 === m && (u.addClass("element-invisible"), d.show()),
                e(a),
                t("ready"),
                s()
        },
        n.onFileDequeued = function (e) {
            m-- ,
                h -= e.size,
                m || t("pedding"),
                a(e),
                s()
        },
        n.on("all", function (e) {
            switch (e) {
                case "uploadFinished":
                    t("confirm");
                    break;
                case "startUpload":
                    t("uploading");
                    break;
                case "stopUpload":
                    t("paused")
            }
        }),
        n.onError = function (e) {
            alert("Eroor: " + e)
        },
        c.on("click", function () {
            return o(this).hasClass("disabled") ? !1 : void ("ready" === k ? n.upload() : "paused" === k ? n.upload() : "uploading" === k && n.stop())
        }),
        p.on("click", ".retry", function () {
            n.retry()
        }),
        p.on("click", ".ignore", function () {
            alert("todo")
        }),
        c.addClass("state-" + k),
        s()
});