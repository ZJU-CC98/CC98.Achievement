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


var popoverElementList = document.querySelectorAll('[data-bs-toggle="popover"]');
var popoverList = popoverElementList.forEach(function (ele, key, parent) {
	var popover = new bootstrap.Popover(ele, {
		content: document.getElementById(ele.getAttribute('data-bs-content-id'))
	});
});

window.onload = function () {
	document.querySelectorAll('[data-toggle="remote-dialog"]').forEach(function (ele) {
		ele.addEventListener('click',
			function () {
				loadDialogFromButton(this);
			});
	});

	document.querySelectorAll('[data-toggle="progress"]').forEach(function (ele) {

		var max = parseInt(ele.dataset.maxValue);
		var now = parseInt(ele.dataset.nowValue);

		var rate = now / max;
		if (isNaN(rate) || rate < 0) {
			rate = 0;
		}

		if (rate > 1) {
			rate = 1;
		}

		ele.style.width = String.format('{0}%', rate * 100);
	});
}


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