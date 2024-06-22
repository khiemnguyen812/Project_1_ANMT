document.addEventListener("DOMContentLoaded", function () {
    // Initialize each dropzone
    initializeDropzone("cipherFile_Upload");
    initializeDropzone("Kx_HKPrivate_Upload");
});

function initializeDropzone(dropzoneId) {
    const dropzoneBox = document.getElementById(dropzoneId);
    const inputElement = dropzoneBox.querySelector("input[type='file']");
    const dropZoneElement = inputElement.closest(".dropzone-area");

    inputElement.addEventListener("change", function (e) {
        if (inputElement.files.length) {
            updateDropzoneFileList(dropZoneElement, inputElement.files[0]);
        }
    });

    dropZoneElement.addEventListener("dragover", function (e) {
        e.preventDefault();
        dropZoneElement.classList.add("dropzone--over");
    });

    ["dragleave", "dragend"].forEach(function (type) {
        dropZoneElement.addEventListener(type, function (e) {
            dropZoneElement.classList.remove("dropzone--over");
        });
    });

    dropZoneElement.addEventListener("drop", function (e) {
        e.preventDefault();
        if (e.dataTransfer.files.length) {
            inputElement.files = e.dataTransfer.files;
            updateDropzoneFileList(dropZoneElement, e.dataTransfer.files[0]);
        }
        dropZoneElement.classList.remove("dropzone--over");
    });

    dropzoneBox.addEventListener("reset", function (e) {
        let dropzoneFileMessage = dropZoneElement.querySelector(".message");
        dropzoneFileMessage.innerHTML = `Không có file được chọn`;
    });

    dropzoneBox.addEventListener("submit", function (e) {
        e.preventDefault();
        const file = inputElement.files[0];
        if (file) {
            const reader = new FileReader();
            reader.readAsText(file, "UTF-8");
            reader.onload = function (evt) {
                console.log(evt.target.result);
            };
            reader.onerror = function (evt) {
                console.error("An error occurred reading the file");
            };
        }
    });
}

function updateDropzoneFileList(dropzoneElement, file) {
    let dropzoneFileMessage = dropzoneElement.querySelector(".message");
    dropzoneFileMessage.innerHTML = `${file.name}, ${file.size} bytes`;
}