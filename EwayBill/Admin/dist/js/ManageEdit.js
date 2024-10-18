$(document).ready(function() {
    fetchStates();
    fetchRoles();
    let selectedHobbies = []; // Declare the selectedHobbies array in the outer scope
    let isPageLoad = true;
    let removedFileIDs = [];
    const childFilesContainer = $('#childFilesContainer');

    function fetchStates() {
        $.ajax({
            url: `/Masters/GetManageState`,
            type: 'GET',
            dataType: 'json',
            success: function(response) {
                if (response.states) {
                    var stateDropdown = $('#stateDropdownEdit');
                    stateDropdown.empty(); // Clear previous options
                    stateDropdown.append('<option selected disabled value="">Choose State...</option>');

                    $.each(response.states, function(index, state) {
                        // Check if the state already exists in the dropdown
                        if ($('#stateDropdownEdit option[value="' + state.StateCode + '"]').length === 0) {
                            stateDropdown.append('<option value="' + state.StateCode + '">' + state.State + '</option>');
                        }
                    });

                    // After the states are fetched, bind the user state if user data exists
                    const user = getUserData();
                    if (user && user.State) {
                        // Find the matching state and bind the StateCode
                        let selectedStateCode = null;

                        $.each(response.states, function(index, state) {
                            if (state.State === user.State) {
                                selectedStateCode = state.StateCode; // Store the matched StateCode
                                $('#stateDropdownEdit').val(state.StateCode); // Set dropdown to match user's state
                                return false; // Exit the loop once a match is found
                            }
                        });

                        if (selectedStateCode) {
                            // Fetch cities based on the selected state code
                            fetchCities(selectedStateCode);
                        }
                    }
                } else if (response.error) {
                    showToast('Error', response.error, 'danger');
                }
            },
            error: function(xhr, status, error) {
                showToast('Error', error, 'danger');
            }
        });
    }

    function fetchRoles() {
        $.ajax({
            url: `/Masters/GetManageRole`,
            type: 'GET',
            dataType: 'json',
            success: function(response) {
                if (response.roles) {
                    var roleDropdown = $('#roleDropdownEdit');
                    roleDropdown.empty(); // Clear previous options
                    roleDropdown.append('<option selected disabled value="">Choose Role...</option>');

                    $.each(response.roles, function(index, role) {
                        // Check if the role already exists in the dropdown
                        if ($('#roleDropdownEdit option[value="' + role.Role + '"]').length === 0) {
                            roleDropdown.append('<option value="' + role.Role + '">' + role.Role + '</option>');
                        }
                    });

                    // After the roles are fetched, bind the user role if user data exists
                    const user = getUserData();
                    if (user && user.Role) {
                        bindUserRole(user.Role);
                    }
                } else if (response.error) {
                    showToast('Error', response.error, 'danger');
                }
            },
            error: function(xhr, status, error) {
                showToast('Error', error, 'danger');
            }
        });

        // Drop zone for child file uploads
        const dropzoneArea = document.getElementById("dropzoneArea");
        const childFileUploads = document.getElementById("childFileUploads");
        const previewImages = document.getElementById("previewImages");

        dropzoneArea.addEventListener("click", function() {
            childFileUploads.click();
        });

        dropzoneArea.addEventListener("dragover", function(event) {
            event.preventDefault();
            dropzoneArea.style.borderColor = "#28a745";
        });

        dropzoneArea.addEventListener("dragleave", function(event) {
            event.preventDefault();
            dropzoneArea.style.borderColor = "#007bff";
        });

        dropzoneArea.addEventListener("drop", function(event) {
            event.preventDefault();
            dropzoneArea.style.borderColor = "#007bff";

            const files = event.dataTransfer.files;
            childFileUploads.files = files; // Assign dropped files to input
            handleFileUpload(files);
        });

        childFileUploads.addEventListener("change", function() {
            handleFileUpload(childFileUploads.files);
        });

        function handleFileUpload(files) {
            previewImages.innerHTML = ""; // Clear previous previews
            for (let i = 0; i < files.length; i++) {
                const file = files[i];
                const reader = new FileReader();

                reader.onload = function(e) {
                    const img = document.createElement("img");
                    img.src = e.target.result;
                    img.alt = file.name;
                    img.style.maxWidth = "150px";
                    img.style.margin = "10px";

                    // Create a remove button
                    const removeButton = document.createElement("button");
                    removeButton.innerText = "Remove";
                    removeButton.classList.add("btn", "btn-danger", "ml-2");
                    removeButton.style.width = "100px";
                    removeButton.style.height = "40px";
                    removeButton.onclick = function () {
                        removeFile(file.name);
                        img.remove(); // Remove image preview
                        removeButton.remove(); // Remove the button
                    };
                    previewImages.appendChild(img);
                    previewImages.appendChild(removeButton);
                };

                reader.readAsDataURL(file);
            }
        }

        function removeFile(fileName) {
        // Logic to remove the file from the childFileUploads input or upload queue
        const dataTransfer = new DataTransfer(); // Create a new DataTransfer object
        const currentFiles = childFileUploads.files;

        for (let i = 0; i < currentFiles.length; i++) {
            if (currentFiles[i].name !== fileName) {
                dataTransfer.items.add(currentFiles[i]); // Add all files except the removed one
            }
        }

        childFileUploads.files = dataTransfer.files; // Update the input files
        }
    }

    // Function to get user data from session storage
    function getUserData() {
        const userData = sessionStorage.getItem('userData');
        if (userData) {
            return JSON.parse(userData); // Parse the JSON string back to an object
        } else {
            showToast('Error', 'No user data found in session storage.', 'danger');
            return null;
        }
    }

    // Function to bind user state
    function bindUserState(userState) {
        userState = userState.trim(); // Trim to avoid extra spaces
        if ($('#stateDropdownEdit option[value="' + userState + '"]').length > 0) {
            $('#stateDropdownEdit').val(userState); // Set state if it exists in the dropdown
        } else {
            $('#stateDropdownEdit').append(new Option(userState, userState, true, true));
        }
    }

    // Function to bind user role
    function bindUserRole(userRole) {
        userRole = userRole.trim(); // Trim to avoid extra spaces
        if ($('#roleDropdownEdit option[value="' + userRole + '"]').length > 0) {
            $('#roleDropdownEdit').val(userRole); // Set role if it exists in the dropdown
        } else {
            $('#roleDropdownEdit').append(new Option(userRole, userRole, true, true));
        }
    }

    // Function to bind user role
    function bindUserCity(userCity) {
        userCity = userCity.trim(); // Trim to avoid extra spaces
        if ($('#cityDropdownEdit option[value="' + userCity + '"]').length > 0) {
            $('#cityDropdownEdit').val(userCity); // Set role if it exists in the dropdown
        } else {
            $('#cityDropdownEdit').append(new Option(userCity, userCity, true, true));
        }
    }

    // Populate the input fields with user data
    const user = getUserData();
    if (user) {
        const existingHobbies = user.Hobbies ? user.Hobbies.split(', ') : []; // Split the hobbies by comma and space

        $('#firstNameEdit').val(user.FirstName);
        $('#lastNameEdit').val(user.LastName);
        $('#dateOfBirthEdit').val(user.DateOfBirth);
        $('#postalCodeEdit').val(user.PostalCode);
        $('#countryEdit').val(user.Country);
        $('#cityEdit').val(user.City);
        $('#phoneNumberEdit').val(user.PhoneNumber);
        $('#emailEdit').val(user.Email);
        $('#addressEdit').val(user.Address);
        if (user.Gender === "male") {
            $('#maleEdit').prop('checked', true);
        } else {
            $('#femaleEdit').prop('checked', true);
        }
        $('#usernameEdit').val(user.UserName);
        $('#passwordEdit').val(user.Password);

        // Show the previously uploaded image file name if it exists
        const previousImageFileName = user.FileName || "No file selected"; // Use FileName as per your context
        const previousImageUrl = user.FileName ? `http://localhost:50763/Uploads/${user.FileName}` : "#"; // Construct the URL properly
        // Display the file name
        const fileNameDisplayEdit = document.getElementById('fileNameDisplayEdit');
        fileNameDisplayEdit.textContent = previousImageFileName;

        // Handle visibility of the "View" button based on the image availability
        const viewButtonEdit = document.getElementById('viewImageButtonEdit');
        const modalImageEdit = document.getElementById('modalImageEdit');

        if (previousImageUrl !== "#") {
            modalImageEdit.src = previousImageUrl; // Set the image source for the modal
            viewButtonEdit.style.display = 'inline-block'; // Show the "View" button
        } else {
            viewButtonEdit.style.display = 'none'; // Hide the "View" button if no image is available
        }

        // Pre-check the hobbies that exist in the user's data
        existingHobbies.forEach(hobby => {
            $(`.hobbies-checkbox[value="${hobby}"]`).prop('checked', true);
        });

        // Set the initial selectedHobbies array
        selectedHobbies = existingHobbies;
        $('#selectedHobbiesDisplay').text(selectedHobbies.join(', '));

        if (selectedHobbies.length > 0) {
            hobbiesDropdownEditBtn.textContent = 'Hobbies Selected'; // Change button text if hobbies are selected
        } else {
            hobbiesDropdownEditBtn.textContent = 'Select Hobbies'; // Revert to default text if no hobbies are selected
        }

        // Existing child files display with view and remove buttons
        if (user.ChildFileNames && user.ChildFileNames.length > 0) {
            user.ChildFileNames.forEach(file => {
                const fileName = file.FileName;
                const fileID = file.FileID;
        
                // Create a new div for each child file
                const fileDiv = $(`
                    <div class="child-file mb-2">
                        <span>${fileName}</span>
                        <button class="btn btn-secondary view-child-file" type="button" data-file-id="${fileID}" data-file-name="${fileName}" data-bs-toggle="modal" data-bs-target="#childImageModal">
                            <i class="fas fa-eye"></i>
                        </button>
                        <button class="btn btn-danger remove-child-file ml-2" type="button" data-file-id="${fileID}">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                `);
        
                // Append the fileDiv to the container
                childFilesContainer.append(fileDiv);
        
                // Handle remove button click for each fileDiv
                fileDiv.find('.remove-child-file').on('click', function () {
                    $(this).parent().remove(); // Remove the file's HTML element
                    removeExistingFile(fileID); // Mark the file as removed
                });
            });
        }
    }

    // Function to remove existing child file
    function removeExistingFile(fileID) {
        // Implement the logic to remove the file from the database or server using AJAX
        removedFileIDs.push(fileID);
        showToast('Error', 'Removing file with ID:' + fileID, 'danger');

        // AJAX call can be made here to delete the file on the server
    }

    // Function to fetch cities based on the selected state
    function fetchCities(stateCode) {
        $.ajax({
            url: `/Masters/GetManageCity?StateCode=${stateCode}`,
            type: 'GET',
            dataType: 'json',
            success: function(response) {
                var cityDropdown = $('#cityDropdownEdit'); // Use cityDropdownEdit for editing
                cityDropdown.empty(); // Clear previous options

                // Append the default option first
                cityDropdown.append('<option selected disabled value="">Choose City...</option>');

                if (response.cities && response.cities.length > 0) {
                    $.each(response.cities, function(index, city) {
                        if ($('#cityDropdownEdit option[value="' + city.City + '"]').length === 0) {
                            cityDropdown.append('<option value="' + city.City + '">' + city.City + '</option>');
                        }
                    });

                    // Only bind the user's city during the initial page load
                    if (isPageLoad) {
                        const user = getUserData();
                        if (user && user.City) {
                            bindUserCity(user.City);
                        }
                        // Set the flag to false after the page has loaded
                        isPageLoad = false;
                    }
                } else {
                    // Optional: Handle case where no cities are found
                    showToast('Error', 'No cities found for the selected state.', 'danger');
                }
            },
            error: function(xhr, status, error) {
                showToast('Error', error, 'danger');
            }
        });
    }

    // Fetch cities when a state is selected in the edit form
    $('#stateDropdownEdit').change(function() {
        var selectedStateCode = $(this).val(); // Get the selected state code
        if (selectedStateCode) {
            fetchCities(selectedStateCode); // Fetch cities based on selected state code
        } else {
            $('#cityDropdownEdit').empty().append('<option selected disabled value="">Choose City...</option>'); // Clear city dropdown if no state is selected
        }
    });

    $('#doneHobbiesEdit').on('click', function() {
        let selectedHobbies = [];
        $('.hobbies-checkbox:checked').each(function() {
            selectedHobbies.push($(this).val());
        });
        $('#selectedHobbiesDisplay').text(selectedHobbies.join(', '));

        const user = getUserData();
        user.Hobbies = selectedHobbies.join(', ');
        sessionStorage.setItem('userData', JSON.stringify(user));
        $('#hobbiesModalEdit').modal('hide');
    });

    // Function to handle the "Done" button click in the modal for hobbies
    document.getElementById('doneHobbiesEdit').addEventListener('click', function() {
        selectedHobbies = []; // Clear previous selection
        // Get all checked checkboxes
        const checkboxes = document.querySelectorAll('.hobbies-checkbox:checked');
        checkboxes.forEach(function(checkbox) {
            selectedHobbies.push(checkbox.nextSibling.textContent.trim()); // Get checkbox label and add to selectedHobbies array
        });

        // Update the button text after hobby selection
        const hobbiesDropdownEditBtn = document.getElementById('hobbiesDropdownEditBtn');
        if (selectedHobbies.length > 0) {
            hobbiesDropdownEditBtn.textContent = 'Hobbies Selected'; // Change button text if hobbies are selected
        } else {
            hobbiesDropdownEditBtn.textContent = 'Select Hobbies'; // Revert to default text if no hobbies are selected
        }
        // Close the modal after selection
        document.querySelector('.btn-close').click();
    });




    // Form submission logic for editing
    document.getElementById("submitAll").addEventListener("click", function() {
        const personalDetailsFormEdit = document.getElementById("personalDetailsFormEdit");
        const otherDetailsFormEdit = document.getElementById("otherDetailsFormEdit");
        const loginDetailsFormEdit = document.getElementById("loginDetailsFormEdit");

        // Validate each form before processing
        let valid = true;
        const errorMessageContainer = $('#errorMessageContainer');
        errorMessageContainer.empty(); // Clear previous error messages

        [personalDetailsFormEdit, otherDetailsFormEdit, loginDetailsFormEdit].forEach(form => {
            if (!form.checkValidity()) {
                valid = false;
                [...form.elements].forEach(input => {
                    input.classList.toggle('is-invalid', !input.validity.valid); // Toggle class based on validity
                });
            }
        });

        if (!valid) {
            errorMessageContainer.text("Please fill out all required fields.");
            return;
        }

        // Collect form data for submission
        const personalDetails = new FormData(personalDetailsFormEdit);
        const otherDetails = new FormData(otherDetailsFormEdit);
        const loginDetails = new FormData(loginDetailsFormEdit);

        const selectedRole = $('#roleDropdownEdit option:selected').text();
        const selectedState = $('#stateDropdownEdit option:selected').text();
        const selectedCity = $('#cityDropdownEdit option:selected').text();

        // Use FormData to combine all form data
        const combinedData = new FormData();

        // Append data from personal, other, and login details forms
        personalDetails.forEach((value, key) => combinedData.append(key, value));
        otherDetails.forEach((value, key) => combinedData.append(key, value));
        loginDetails.forEach((value, key) => combinedData.append(key, value));

        combinedData.delete('role');
        combinedData.delete('state');
        combinedData.delete('city');
        combinedData.append('role', selectedRole);
        combinedData.append('state', selectedState);
        combinedData.append('city', selectedCity);

        // Add UserID
        const userId = user.UserID || 1; // Hardcoded value of UserID if not available
        combinedData.append('userId', userId);

        // Add selected hobbies as a comma-separated string
        if (selectedHobbies.length > 0) {
            const hobbiesString = selectedHobbies.join(', '); // Join hobbies array into a comma-separated string
            combinedData.append('hobbies', hobbiesString); // Append the comma-separated hobbies string
        }

        // Add the file input (if it exists) to the combined data
        const fileInput = document.getElementById("userFileUploadEdit"); // Ensure this matches your file input's ID
        // Check if a new file is selected
        if (fileInput.files.length > 0) {
            combinedData.append('userFileUpload', fileInput.files[0]); // Append the first file selected
        } else {
            // No new file chosen, pass a flag to use the previous one
            const previousImageFileName = user.FileName; // Assuming you have user object from session storage
            if (previousImageFileName) {
                combinedData.append('userFileUpload', ''); // Append an empty string or special value
                combinedData.append('previousFileName', previousImageFileName); // Append the previous file name to inform the server
            }
        }

        // Append existing child file details (UserID, FileID, FileName)
        /*if (user.ChildFileNames && user.ChildFileNames.length > 0) {
            user.ChildFileNames.forEach((file, index) => {
                console.log(`removedFileIDs: ${JSON.stringify(removedFileIDs)}`);
                // Append each property as part of the existingChildFiles array
                combinedData.append(`existingChildFiles[${index}].FileID`, file.FileID);
                combinedData.append(`existingChildFiles[${index}].UserID`, file.UserID);
                combinedData.append(`existingChildFiles[${index}].FileName`, file.FileName);
            });
        }*/

        if (user.ChildFileNames && user.ChildFileNames.length > 0) {
            let appendIndex = 0;  // Counter for correct appending
            user.ChildFileNames.forEach((file) => {
                
                // Check if the file.FileID is in removedFileIDs by comparing each id
                const isRemoved = removedFileIDs.some(id => id === file.FileID);
                
                if (!isRemoved) {
                    
                    // Append data if the file is not removed
                    combinedData.append(`existingChildFiles[${appendIndex}].FileID`, file.FileID);
                    combinedData.append(`existingChildFiles[${appendIndex}].UserID`, file.UserID);
                    combinedData.append(`existingChildFiles[${appendIndex}].FileName`, file.FileName);
                    
                    appendIndex++;  // Only increment when appending data
                } else {
                    //combinedData.append(`existingChildFiles[${appendIndex}].FileID`, file.FileID);
                }
            });
        }

        // Append child file uploads
        const childFileUploads = document.getElementById("childFileUploads");
        if (childFileUploads.files.length > 0) {
            for (let i = 0; i < childFileUploads.files.length; i++) {
                combinedData.append('childFileUploads', childFileUploads.files[i]);
            }
        }

        // Submit data
        fetch('/Account/Register', {
                method: 'POST',
                body: combinedData
            })
            .then(response => response.json())
            .then(data => {
                if (data.redirectUrl) {
                    sessionStorage.setItem('registrationEditSuccess', 'true');
                    window.location.href = data.redirectUrl;
                } else {
                    showToast('Error', 'An error occurred during the update.', 'danger');
                }
            })
            .catch(error => {
                showToast('Error', error, 'danger');
            });
    });


    $('#editBackButton').on('click', function() {
        window.history.back();
    });

    document.getElementById('togglePassword').addEventListener('click', function() {
        const passwordInput = document.getElementById('passwordEdit');
        const eyeIcon = document.getElementById('eyeIcon');
        if (passwordInput.type === 'password') {
            passwordInput.type = 'text';
            eyeIcon.classList.remove('fa-eye');
            eyeIcon.classList.add('fa-eye-slash');
        } else {
            passwordInput.type = 'password';
            eyeIcon.classList.remove('fa-eye-slash');
            eyeIcon.classList.add('fa-eye');
        }
    });

    // Handle file input change to show selected file name and preview
    document.getElementById('userFileUploadEdit').addEventListener('change', function(event) {
        const file = event.target.files[0]; // Get the selected file

        // Check if a file is selected and it's an image
        if (file && file.type.startsWith('image/')) {
            const reader = new FileReader(); // Create a FileReader to read the file

            // Once the file is read, prepare the modal for image display
            reader.onload = function(e) {
                const modalImageEdit = document.getElementById('modalImageEdit');
                const viewButtonEdit = document.getElementById('viewImageButtonEdit');
                const fileNameDisplayEdit = document.getElementById('fileNameDisplayEdit');

                // Set the modal image source to the file's data URL
                modalImageEdit.src = e.target.result;

                // Show the "View" button to open the modal
                viewButtonEdit.style.display = 'inline-block';

                // Update the file name display
                fileNameDisplayEdit.textContent = file.name;
            };

            reader.readAsDataURL(file); // Read the file as a data URL (base64)
        } else {
            // If the file is not an image, hide the "View" button
            showToast('Error', 'Please select a valid image file.', 'danger');
            document.getElementById('viewImageButtonEdit').style.display = 'none';
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

    // Function to show toast messages
    function showToast(title, message, type) {
        // Set the toast message
        $('#toastBody').text(message);
    
        // Apply the appropriate background color (success or danger)
        if (type === 'success') {
            $('#toastMessage').removeClass('bg-danger').addClass('bg-success');
        } else if (type === 'danger') {
            $('#toastMessage').removeClass('bg-success').addClass('bg-danger');
        }
    
        // Initialize the toast with a custom delay (e.g., 10 seconds = 10000 ms)
        var toastElement = new bootstrap.Toast(document.getElementById('toastMessage'), {
            delay: 5000, // Set the delay to 5 seconds
            autohide: true
        });
    
        // Show the toast
        toastElement.show();
    }

});
