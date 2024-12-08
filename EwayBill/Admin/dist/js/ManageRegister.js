$(document).ready(function () {
    //fetchStates();
    fetchCountries();
    fetchRoles();

    $('#username').on('input', function () {
        var username = $(this).val();

        if (username.length > 0) {
            $.ajax({
                url: '/Account/CheckUsername',
                type: 'GET',
                data: { username: username },
                success: function (isAvailable) {
                    var usernameField = $('#username');
                    var feedback = $('#usernameFeedback');

                    if (isAvailable) {
                        usernameField.removeClass('is-invalid').addClass('is-valid');
                        feedback.removeClass('invalid-feedback').addClass('valid-feedback');
                        feedback.text('Username is available');
                    } else {
                        usernameField.removeClass('is-valid').addClass('is-invalid');
                        feedback.removeClass('valid-feedback').addClass('invalid-feedback');
                        feedback.text('Username is already taken');
                    }
                },
                error: function (xhr, status, error) {
                    showToast('Error', error, 'danger');
                }
            });
        } else {
            $('#username').removeClass('is-valid is-invalid');
            $('#usernameFeedback').text('');
        }
    });

    $('#email').on('input', function () {
    var email = $(this).val();
    var emailFormat = /^[^\s@]+@[^\s@]+\.[^\s@]+$/; // Basic email format validation

    if (email.length > 0) {
            // Check for valid email format
            if (!emailFormat.test(email)) {
                $('#email').removeClass('is-valid').addClass('is-invalid');
                $('#emailFeedback').removeClass('valid-feedback').addClass('invalid-feedback');
                $('#emailFeedback').text('Invalid email format');
                return; // Exit if the format is invalid
            }
    
            $.ajax({
                url: '/Account/CheckEmail',
                type: 'GET',
                data: { email: email },
                success: function (isAvailable) {
                    var emailField = $('#email');
                    var feedback = $('#emailFeedback');
    
                    if (isAvailable) {
                        emailField.removeClass('is-invalid').addClass('is-valid');
                        feedback.removeClass('invalid-feedback').addClass('valid-feedback');
                        feedback.text('Email is available');
                    } else {
                        emailField.removeClass('is-valid').addClass('is-invalid');
                        feedback.removeClass('valid-feedback').addClass('invalid-feedback');
                        feedback.text('Email is already taken');
                    }
                },
                error: function (xhr, status, error) {
                    showToast('Error', error, 'danger');
                }
            });
        } else {
            $('#email').removeClass('is-valid is-invalid');
            $('#emailFeedback').text('');
        }
     });

    // Drop zone for child file uploads
    const dropzoneArea = document.getElementById("dropzoneArea");
    const childFileUploads = document.getElementById("childFileUploads");
    const previewImages = document.getElementById("previewImages");

    dropzoneArea.addEventListener("click", function () {
        childFileUploads.click();
    });

    dropzoneArea.addEventListener("dragover", function (event) {
        event.preventDefault();
        dropzoneArea.style.borderColor = "#28a745";
    });

    dropzoneArea.addEventListener("dragleave", function (event) {
        event.preventDefault();
        dropzoneArea.style.borderColor = "#007bff";
    });

    dropzoneArea.addEventListener("drop", function (event) {
        event.preventDefault();
        dropzoneArea.style.borderColor = "#007bff";

        const files = event.dataTransfer.files;
        childFileUploads.files = files;  // Assign dropped files to input
        handleFileUpload(files);
    });

    childFileUploads.addEventListener("change", function () {
        handleFileUpload(childFileUploads.files);
    });

    function handleFileUpload(files) {
        previewImages.innerHTML = "";  // Clear previous previews
        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            const reader = new FileReader();

            reader.onload = function (e) {
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

    // Fetch countries
    function fetchCountries() {
        $.ajax({
            url: `/Masters/GetManageCountry`,
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                if (response.countries) {
                    var countryDropdown = $('#countryDropdown');
                    countryDropdown.empty();
                    countryDropdown.append('<option selected disabled value="">Choose Country...</option>');
                    $.each(response.countries, function (index, country) {
                        countryDropdown.append('<option value="' + country.CountryCode + '">' + country.CountryName + '</option>');
                    });
                } else if (response.error) {
                    showToast('Error', response.error, 'danger');
                }
            },
            error: function (xhr, status, error) {
                showToast('Error', error, 'danger');
            }
        });
    }

    // Fetch states based on the selected country
    function fetchStates(countryCode) {
        $.ajax({
            url: `/Masters/GetManageState?CountryCode=${countryCode}`,
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                var stateDropdown = $('#stateDropdown');
                stateDropdown.empty();
                stateDropdown.append('<option selected disabled value="">Choose State...</option>');
                if (response.states && response.states.length > 0) {
                    $.each(response.states, function (index, state) {
                        stateDropdown.append('<option value="' + state.StateCode + '">' + state.State + '</option>');
                    });
                } else {
                    showToast('Error', 'No states found for the selected country.', 'danger');
                }
            },
            error: function (xhr, status, error) {
                showToast('Error', error, 'danger');
            }
        });
    }

    // Fetch states
    /*function fetchStates() {
        $.ajax({
            url: `/Masters/GetManageState`,
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                if (response.states) {
                    var stateDropdown = $('#stateDropdown');
                    stateDropdown.empty();
                    stateDropdown.append('<option selected disabled value="">Choose State...</option>');
                    $.each(response.states, function (index, state) {
                        stateDropdown.append('<option value="' + state.StateCode + '">' + state.State + '</option>');
                    });
                } else if (response.error) {
                    showToast('Error', response.error, 'danger');
                }
            },
            error: function (xhr, status, error) {
                showToast('Error', error, 'danger');
            }
        });
    }*/

    // Fetch cities based on the selected state
    function fetchCities(stateCode) {
        $.ajax({
            url: `/Masters/GetManageCity?StateCode=${stateCode}`,
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                var cityDropdown = $('#cityDropdown');
                cityDropdown.empty();
                cityDropdown.append('<option selected disabled value="">Choose City...</option>');
                if (response.cities && response.cities.length > 0) {
                    $.each(response.cities, function (index, city) {
                        cityDropdown.append('<option value="' + city.CityID + '">' + city.City + '</option>');
                    });
                } else {
                    showToast('Error', 'No cities found for the selected state.', 'danger');
                }
            },
            error: function (xhr, status, error) {
                showToast('Error', error, 'danger');
            }
        });
    }

    $('#countryDropdown').change(function () {
        var countryCode = $(this).val();
        if (countryCode) {
            fetchStates(countryCode);
        } else {
            // Clear the state dropdown if no country is selected
            $('#stateDropdown').empty().append('<option selected disabled value="">Choose State...</option>');
        }

        var selectedStateCode = $(this).val();
        if (selectedStateCode) {
            fetchCities(selectedStateCode);
        } else {
            $('#cityDropdown').empty().append('<option selected disabled value="">Choose City...</option>');
        }
    });

    $('#stateDropdown').change(function () {
        var selectedStateCode = $(this).val();
        if (selectedStateCode) {
            fetchCities(selectedStateCode);
        } else {
            $('#cityDropdown').empty().append('<option selected disabled value="">Choose City...</option>');
        }
    });

    // Fetch roles
    function fetchRoles() {
        $.ajax({
            url: `/Masters/GetManageRole`,
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                if (response.roles) {
                    var roleDropdown = $('#roleDropdown');
                    roleDropdown.empty();
                    roleDropdown.append('<option selected disabled value="">Choose Role...</option>');
                    $.each(response.roles, function (index, role) {
                        roleDropdown.append('<option value="' + role.RoleID + '">' + role.Role + '</option>');
                    });
                } else if (response.error) {
                    showToast('Error', response.error, 'danger');
                }
            },
            error: function (xhr, status, error) {
                showToast('Error', error, 'danger');
            }
        });
    }

    document.getElementById('doneHobbies').addEventListener('click', function () {
        selectedHobbies = [];
        const checkboxes = document.querySelectorAll('.hobbies-checkbox:checked');
        checkboxes.forEach(function (checkbox) {
            selectedHobbies.push(checkbox.nextSibling.textContent.trim());
        });

        const hobbiesDropdownBtn = document.getElementById('hobbiesDropdownBtn');
        if (selectedHobbies.length > 0) {
            hobbiesDropdownBtn.textContent = 'Hobbies Selected';
        } else {
            hobbiesDropdownBtn.textContent = 'Select Hobbies';
            showToast('Error', 'No Hobbies Selected', 'danger');
        }

        const modalElement = document.getElementById('hobbiesModal');
        const modalInstance = bootstrap.Modal.getInstance(modalElement);
        if (modalInstance) {
            modalInstance.hide();
        }
    });

    document.getElementById("submitAll").addEventListener("click", function () {
        const personalDetailsForm = document.getElementById("personalDetailsForm");
        const otherDetailsForm = document.getElementById("otherDetailsForm");
        const loginDetailsForm = document.getElementById("loginDetailsForm");

        let valid = true;
        const errorMessageContainer = $('#errorMessageContainer');
        errorMessageContainer.empty();

        [personalDetailsForm, otherDetailsForm, loginDetailsForm].forEach(form => {
            if (!form.checkValidity()) {
                valid = false;
                [...form.elements].forEach(input => {
                    if (!input.validity.valid) {
                        input.classList.add('is-invalid');
                    } else {
                        input.classList.remove('is-invalid');
                    }
                });
            } else {
                [...form.elements].forEach(input => {
                    input.classList.remove('is-invalid');
                });
            }
        });

        if (!valid) {
            errorMessageContainer.text("Please fill out all required fields.");
            return;
        }

        const personalDetails = new FormData(personalDetailsForm);
        const otherDetails = new FormData(otherDetailsForm);
        const loginDetails = new FormData(loginDetailsForm);

        const selectedRole = $('#roleDropdown option:selected').text();
        const selectedCountry = $('#countryDropdown option:selected').text();
        const selectedState = $('#stateDropdown option:selected').text();
        const selectedCity = $('#cityDropdown option:selected').text();

        const combinedData = new FormData();

        personalDetails.forEach((value, key) => combinedData.append(key, value));
        otherDetails.forEach((value, key) => combinedData.append(key, value));
        loginDetails.forEach((value, key) => combinedData.append(key, value));

        combinedData.delete('role');
        combinedData.delete('country');
        combinedData.delete('state');
        combinedData.delete('city');

        combinedData.append('role', selectedRole);
        combinedData.append('country', selectedCountry);
        combinedData.append('state', selectedState);
        combinedData.append('city', selectedCity);

        if (selectedHobbies.length > 0) {
            const hobbiesString = selectedHobbies.join(', ');
            combinedData.append('hobbies', hobbiesString);
        }

        const fileInput = document.getElementById("userFileUpload");
        if (fileInput.files.length > 0) {
            combinedData.append('userFileUpload', fileInput.files[0]);
        }

        // Append child file uploads
        const childFileUploads = document.getElementById("childFileUploads");
        if (childFileUploads.files.length > 0) {
            for (let i = 0; i < childFileUploads.files.length; i++) {
                combinedData.append('childFileUploads', childFileUploads.files[i]);
            }
        }

        fetch('/Account/Register', {
            method: 'POST',
            body: combinedData
        })
        .then(response => response.json())
        .then(data => {
            if (data.redirectUrl) {
                sessionStorage.setItem('registrationSuccess', 'true');
                window.location.href = data.redirectUrl;
            } else {
                showToast('Error', 'An error occurred during registration.', 'danger');
            }
        })
        .catch(error => {
            showToast('Error', error, 'danger');
        });
    });

    document.getElementById("addBackButton").addEventListener("click", function () {
        window.location.href = "/Admin/dist/pages/Users/ManageUsers.html";
    });

    document.getElementById('togglePassword').addEventListener('click', function () {
        const passwordInput = document.getElementById('password');
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
