$(document).ready(function () {
    customizeViewer();
});

function updateSelectedServer() {
    setParameter('Server', ServerSelection.GetSelectedItem().value);
}

//Performs initialization steps after the report is built
function initializeViewer() {
    alignToolbarOnTimeout();
    applyTooltips();
    addFont();
}

function applyTooltips() {
    var other = $("td:contains('Other')", $("#Viewer_Splitter_Viewer_ContentFrame").contents());
    if (other.length > 0) {
        //Set titles
        var tooltipText = '"Other" consists of queries, imports, Relativity script execution, and pivot queries.';
        other.attr('title', tooltipText);
        other.children().attr('title', tooltipText);
    }
}