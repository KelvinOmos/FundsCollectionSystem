// Add event listener to the file upload input
document.getElementById('FileUpload').addEventListener('change', handleFileUpload);

// Handle file upload event
function handleFileUpload(event) {
    var file = event.target.files[0];

    // Check if a file is selected
    if (file) {
        var fileName = file.name;
        var fileExtension = fileName.split('.').pop().toLowerCase();

        // Check if the file extension is valid
        if (fileExtension === 'csv') {
            // Parse CSV file
            Papa.parse(file, {
                complete: function (results) {
                    renderTable(results.data);
                },
                error: function (error) {
                    console.error('Error parsing CSV:', error);
                }
            });
        } else if (fileExtension === 'xlsx' || fileExtension === 'xls') {
            // Read Excel file
            var reader = new FileReader();
            reader.onload = function (e) {
                var data = new Uint8Array(e.target.result);
                var workbook = XLSX.read(data, { type: 'array' });
                var worksheet = workbook.Sheets[workbook.SheetNames[0]];
                var excelData = XLSX.utils.sheet_to_json(worksheet, { header: 1 });
                renderTable(excelData);
            };
            reader.onerror = function (error) {
                console.error('Error reading Excel file:', error);
            };
            reader.readAsArrayBuffer(file);
        } else {
            // Invalid file extension, display error message
            document.getElementById('fileValidation').textContent = 'Invalid file format. Only CSV and Excel files are allowed.';
        }
    }
}

// Render the table with the parsed data
function renderTable(data) {
    var tableBody = document.getElementById('tableBody');
    tableBody.innerHTML = '';

    for (var i = 0; i < data.length; i++) {
        var row = document.createElement('tr');
        for (var j = 0; j < data[i].length; j++) {
            var cell = document.createElement(i === 0 ? 'th' : 'td');
            cell.textContent = data[i][j];
            row.appendChild(cell);
        }
        tableBody.appendChild(row);
    }
}