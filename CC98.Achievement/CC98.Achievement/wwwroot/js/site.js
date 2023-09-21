// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// 字符串格式化函数
if (!String.format) {
	String.format = function (format) {
		var args = Array.prototype.slice.call(arguments, 1);
		return format.replace(/{(\d+)}/g, function (match, number) {
			return typeof args[number] != 'undefined'
				? args[number]
				: match
				;
		});
	};
}

//window.onload = function () {
//	document.querySelectorAll('[data-toggle="remote-dialog"]').forEach(function (ele) {
//		ele.addEventListener('click',
//			function () {
//				loadDialogFromButton(this);
//			});
//	});

//	document.querySelectorAll('[data-toggle="progress"]').forEach(function (ele) {

//		var max = parseInt(ele.dataset.maxValue);
//		var now = parseInt(ele.dataset.nowValue);

//		var rate = now / max;
//		if (isNaN(rate) || rate < 0) {
//			rate = 0;
//		}

//		if (rate > 1) {
//			rate = 1;
//		}

//		var style = 'bg-primary';
//		if (rate < 1 / 3) {
//			style = 'bg-danger';
//		}
//		else if (rate < 2 / 3) {
//			style = 'bg-warning';
//		}

//		ele.classList.add(style);

//		ele.style.width = String.format('{0}%', rate * 100);
//	});
//}


function ajaxLoad(url, targetSelector) {

	return fetch(url).then(function (response) {

		return response.text().then(function (data) {
			var target = document.querySelector(targetSelector);
			target.innerHTML = data;
		});

	});
}

function ajaxLoadDialog(url, dialogSelector) {
	return ajaxLoad(url, dialogSelector + " .modal-content").then(function () {
		var dialog = bootstrap.Modal.getOrCreateInstance(dialogSelector);
		dialog.show();
	});
}

function loadDialogFromButton(ele) {
	var remoteUrl = ele.dataset.remoteUrl;
	var dialog = ele.dataset.target;
	return ajaxLoadDialog(remoteUrl, dialog);
}


$(function () {

	// 初始化全部进度条数值
	$('.ui.progress').progress();

	// 初始化下拉列表
	$('.ui.dropdown').dropdown();

	// 初始化单选框和复选框
	$('.ui.checkbox').checkbox();

	$('.ui.form').form();

	// 初始化所有消息框的关闭按钮效果
	$('.message .close').on('click',
		function () {
			$(this).closest('.message').transaction('fade');
		});


	// data 对话框支持
	$('[data-toggle="modal"]').each(function (_, ele) {

		$(ele).click(function () {
			var target = $(ele).data('target');
			$(target).modal('show');
		});
	});

	// data 对话框关闭支持
	$('[data-dismiss="modal"]').each(function (_, ele) {

		$(ele).click(function () {
			var target = $(ele).closest('.modal');
			$(target).modal('hide');
		});
	});

	// data 远程对话框支持
	$('[data-toggle="remote-dialog"]').each(function (_, ele) {

		var target = $(ele).data('target');

		$(ele).click(function () {

			var url = $(ele).data('remote-url');

			$.ajax(url, {
				success: function (data) {
					$(target).html(data);
					$('.modal', target).modal('show');
				}
			});

		});

	});
});