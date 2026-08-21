window.downloadFile = (fileName, base64Data) => {
    const link = document.createElement("a");

    link.download = fileName;
    link.href = "data:application/pdf;base64," + base64Data;

    document.body.appendChild(link);

    link.click();

    document.body.removeChild(link);
};