window.downloadFile = (fileName, base64Data) => {
    const link = document.createElement("a");

    link.download = fileName;
    link.href = "data:application/pdf;base64," + base64Data;

    document.body.appendChild(link);

    link.click();

    document.body.removeChild(link);
};







window.downloadFile1 = function (fileName, contentType, byteArray) {
    const blob = new Blob(
        [new Uint8Array(byteArray)],
        { type: contentType }
    );

    const url = URL.createObjectURL(blob);

    const link = document.createElement("a");

    link.href = url;
    link.download = fileName;

    document.body.appendChild(link);

    link.click();

    document.body.removeChild(link);

    URL.revokeObjectURL(url);
};