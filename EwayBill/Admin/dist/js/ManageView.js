$(document).ready(function() {
    // Retrieve user data from session storage
    const userData = sessionStorage.getItem('userData');

    if (userData) {
        const user = JSON.parse(userData); // Parse the JSON string back to an object

        // Populate the labels with user data
        $('#firstNameLabel').text(user.FirstName);
        $('#lastNameLabel').text(user.LastName);
        $('#dateOfBirthLabel').text(formatDateToDDMMYYYY(user.DateOfBirth));
        $('#postalCodeLabel').text(user.PostalCode);
        $('#countryLabel').text(user.Country);
        $('#stateLabel').text(user.State);
        $('#cityLabel').text(user.City);
        $('#phoneNumberLabel').text(user.PhoneNumber);
        $('#emailLabel').text(user.Email);
        $('#genderLabel').text(user.Gender);
        $('#addressLabel').text(user.Address);
        $('#usernameLabel').text(user.UserName);
        $('#passwordLabel').text(user.Password); // Optionally, you can set a fixed value for password
        $('#roleLabel').text(user.Role);
        $('#hobbiesLabel').text(user.Hobbies);

        // Show the previously uploaded image file name if it exists
        const previousImageFileName = user.FileName || "No file selected"; // Use FileName as per your context
        const previousImageUrl = user.FileName ? `http://localhost:50763/Uploads/${user.FileName}` : "#"; // Construct the URL properly

        // Display the file name
        $('#uploadedFileNameDisplay').text(previousImageFileName);

        // Handle visibility of the "View" button based on the image availability
        const viewButtonLabel = $('#viewImageButtonLabel');
        const modalImageLabel = $('#modalImageLabel');

        if (previousImageUrl !== "#") {
            modalImageLabel.attr('src', previousImageUrl);  // Set the image source for the modal
            viewButtonLabel.show();  // Show the "View" button
        } else {
            viewButtonLabel.hide();  // Hide the "View" button if no image is available
        }

        // Populate Child Files
        const childFilesContainer = $('#childFilesContainer');
        if (user.ChildFileNames && user.ChildFileNames.length > 0) {
            user.ChildFileNames.forEach(file => {
                const fileName = file.FileName;
                const fileID = file.FileID; // Assuming you have some identifier

                // Create a new div for each child file
                const fileDiv = $(`
                    <div class="child-file mb-2">
                        <span>${fileName}</span>
                        <button class="btn btn-secondary view-child-file" type="button" data-file-id="${fileID}" data-file-name="${fileName}" data-bs-toggle="modal" data-bs-target="#childImageModal">
                            <i class="fas fa-eye"></i>
                        </button>
                    </div>
                `);
                childFilesContainer.append(fileDiv); // Append to the container
            });
        }

    } else {
        alert('No user data found in session storage.');
    }

    // Back button functionality
    $('#addBackButton').on('click', function() {
        window.history.back(); // Go back to the previous page
    });

    // Handle file input change to show selected file name and preview
    $('#userFileUpload').on('change', function(event) {
        const file = event.target.files[0];  // Get the selected file

        // Check if a file is selected and it's an image
        if (file && file.type.startsWith('image/')) {
            const reader = new FileReader();  // Create a FileReader to read the file

            // Once the file is read, prepare the modal for image display
            reader.onload = function(e) {
                const modalImageLabel = $('#modalImageLabel');
                const viewButtonLabel = $('#viewImageButtonLabel');

                // Set the modal image source to the file's data URL
                modalImageLabel.attr('src', e.target.result);

                // Show the "View" button to open the modal
                viewButtonLabel.show();

                // Update the file name display
                $('#uploadedFileNameDisplay').text(file.name);
            };

            reader.readAsDataURL(file);  // Read the file as a data URL (base64)
        } else {
            // If the file is not an image, hide the "View" button
            alert('Please select a valid image file.');
            $('#viewImageButtonLabel').hide();
        }
    });

    // Handle viewing of child files
    $(document).on('click', '.view-child-file', function() {
        const fileName = $(this).data('file-name');

        // Construct the image URL based on the file name
        const childImageUrl = `http://localhost:50763/Uploads/${fileName}`; // Adjust URL as needed

        // Set the modal image source and show the modal
        $('#childModalImage').attr('src', childImageUrl);
    });

    // Function to format date to dd/MM/yyyy
    function formatDateToDDMMYYYY(dateString) {
      var date = new Date(dateString);
      var day = String(date.getDate()).padStart(2, '0');
      var month = String(date.getMonth() + 1).padStart(2, '0'); // Month is 0-indexed
      var year = date.getFullYear();
      return `${day}-${month}-${year}`;
    }

});
