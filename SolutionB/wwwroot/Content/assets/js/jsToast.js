function showToast(message) {
    var notification = document.querySelector('.notification');
    var toastMessage = document.querySelector('#toastMessage');
    toastMessage.textContent = message;
    notification.classList.add('show');

    // Tự động ẩn thông báo sau 2 giây
    setTimeout(function () {
        notification.classList.remove('show');
    }, 2000);
}