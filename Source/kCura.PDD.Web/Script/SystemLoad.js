$(document).ready(function() {
    customizeViewer();
});

function updateSelectedServer() {
    setParameter('Server', ServerSelection.GetSelectedItem().value);
}

//Performs initialization steps after the report is built
function initializeViewer() {
    alignToolbarOnTimeout();
    addFont();
}