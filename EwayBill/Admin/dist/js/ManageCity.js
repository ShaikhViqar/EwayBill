$(document).ready(function () {
    let currentPage = 1;
    let pageSize = 5;
    let totalItems = 0;
    let totalPages = 0;
    let deleteStateID = null;
    let searchQuery = '';
    let stateCode = null;

    fetchStates();
    fetchCities(currentPage, pageSize, searchQuery, stateCode);

    $('#pageSize').change(function () {
        pageSize = parseInt($(this).val());
        currentPage = 1;
        fetchCities(currentPage, pageSize, searchQuery, stateCode);
    });

    function fetchCities(page, size, search, state) {
        $.ajax({
            url: `/Masters/GetManageCity?page=${page}&pageSize=${size}&searchQuery=${search}&StateCode=${state}`,
            method: 'GET',
            success: function (response) {
                totalItems = response.totalItems;
                totalPages = Math.ceil(totalItems / pageSize);
                renderTable(response.cities);
                updatePagination();
            },
            error: function (error) {
                showToast('Error', 'Error fetching cities:', 'danger');
            }
        });
    }

    function renderTable(cities) {
        var tableBody = '';
        $.each(cities, function (index, city) {
            tableBody += `
                <tr class="align-middle">
                    <td>${city.State}</td>
                    <td>${city.City}</td>
                    <td>
                        <button class="btn btn-warning btn-sm editCity" data-cityid="${city.CityID}" data-statecode="${city.StateCode}" data-cityname="${city.City}" data-statename="${city.State}">Edit</button>
                        <button class="btn btn-danger btn-sm deleteCity" data-cityid="${city.CityID}">Delete</button>
                    </td>
                </tr>`;
        });
        $('#citiesTable tbody').html(tableBody);
    }

    function updatePagination() {
        $('#pageNumber').text(currentPage);
        
        // Disable the first and previous buttons if on the first page
        $('#firstPage').prop('disabled', currentPage === 1);
        $('#prevPage').prop('disabled', currentPage === 1);
        
        // Disable the next and last buttons if on the last page or if there's no data
        $('#nextPage').prop('disabled', currentPage >= totalPages || totalItems === 0);
        $('#lastPage').prop('disabled', currentPage >= totalPages || totalItems === 0);
    }

    $('#firstPage').click(function () {
        currentPage = 1;
        fetchCities(currentPage, pageSize, searchQuery, stateCode);
    });

    $('#prevPage').click(function () {
        if (currentPage > 1) {
            currentPage--;
            fetchCities(currentPage, pageSize, searchQuery, stateCode);
        }
    });

    $('#nextPage').click(function () {
        if (currentPage < totalPages) {
            currentPage++;
            fetchCities(currentPage, pageSize, searchQuery, stateCode);
        }
    });

    $('#lastPage').click(function () {
        currentPage = totalPages;
        fetchCities(currentPage, pageSize, searchQuery, stateCode);
    });

    // Bind click event for the Edit button
$('#citiesTable').on('click', '.editCity', function () {
    const CityID = $(this).data('cityid');
    const stateCode = $(this).data('statecode');
    const cityName = $(this).data('cityname');
    const state = $(this).data('statename');
    
    // Set stateCode in hidden field
    $('#stateCode').val(stateCode);

    // Reset the state dropdown and select the correct state based on the StateID
    $('#stateDropdown').val('');  // Clear selection
    $('#stateDropdown').find('option').each(function() {
        if ($(this).text() === state) {  // Compare the state name
            $(this).prop('selected', true);
        }
    });

    $('#city').val(cityName);
    $('#cityID').val(CityID);
    $('#submitButton').text('Update');
    $('#cancelButton').show();
    $('#cityError').hide();
});

    $('#citiesTable').on('click', '.deleteCity', function () {
        deleteCityID = $(this).data('cityid');
        $('#deleteConfirmationModal').modal('show');
    });

    $('#confirmDeleteButton').on('click', function () {
        if (deleteCityID) {
            $.ajax({
                url: `/Masters/DeleteManageCity?cityID=${deleteCityID}`,
                method: 'DELETE',
                success: function (response) {
                    $('#deleteConfirmationModal').modal('hide');
                    showToast('Success', 'City deleted successfully!', 'success');
                    fetchCities(currentPage, pageSize, searchQuery, stateCode);
                    resetForm();
                },
                error: function (error) {
                    showToast('Error', 'Error deleting city. Please try again.', 'danger');
                }
            });
        }
    });

    function resetForm() {
    $('#stateCode').val(''); // Clear the hidden state code field
    $('#stateDropdown').val(''); // Clear the state dropdown selection
    $('#city').val(''); // Clear city input field
    $('#cityID').val(''); // Clear city ID
    $('#submitButton').text('Submit'); // Change button text to 'Submit'
    $('#cancelButton').hide(); // Hide the cancel button
    deleteCityID = null; // Reset any variable tracking city to delete
    $('#search').val('');
    searchQuery = "";
    }

    $('#cancelButton').on('click', function () {
        resetForm();
    });

    $('#alertCancelButton').on('click', function () {
        $('#alertMessage').fadeOut();
    });

    // Update the search function to work with AJAX
    $('#search').on('keyup', function () {
        searchQuery = $(this).val(); // Update searchQuery based on input
        currentPage = 1; // Reset to the first page
        fetchCities(currentPage, pageSize, searchQuery, stateCode);
    });
    
// Function to fetch the states from the backend
function fetchStates() {
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
                    // Display StateCode and State together in the dropdown
                    stateDropdown.append('<option value="' + state.StateID + '" data-statecode="' + state.StateCode + '">'+ state.State + '</option>');
                });
            } else if (response.error) {
                showToast('Error', response.error, 'danger');
            }
        },
        error: function (xhr, status, error) {
            showToast('Error', response.error, 'danger');
        }
    });
}

    // Update StateCode hidden input and fetch cities when a state is selected
    $('#stateDropdown').on('change', function() {
        var selectedOption = $(this).find(':selected');
        stateCode = selectedOption.data('statecode'); // Update stateCode with selected state's code
        $('#stateCode').val(stateCode); // Set the StateCode hidden input value
        currentPage = 1; // Reset to the first page
        fetchCities(currentPage, pageSize, searchQuery, stateCode); // Fetch cities for the selected state
    });


$('#manageCityForm').on('submit', function (e) {
        e.preventDefault();

        // Check for any validation errors before submitting
        if ($('#cityError').is(':visible')) {
            return;
        }

        // Determine if it's an update or a new save
        const isUpdate = $('#submitButton').text().trim() === 'Update';

        $.ajax({
            url: '/Masters/SaveManageCity',
            method: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                resetForm();
                stateCode = null; // Reset stateCode
                $('#stateDropdown').val(''); // Clear state dropdown selection
                currentPage = 1; // Reset to the first page
                fetchCities(currentPage, pageSize, searchQuery, stateCode);
                $('#cityError').hide();
                if (isUpdate) {
                    showToast('Success', 'City updated successfully!', 'success');
                } else {
                    showToast('Success', 'City saved successfully!', 'success');
                }
            },
            error: function (error) {
                if (isUpdate) {
                    showToast('Error', 'Error updated city. Please try again.', 'danger');
                } else {
                    showToast('Error', 'Error saved city. Please try again.', 'danger');
                }
            }
        });
    });

    // Add reset button functionality
    $('#resetButton').on('click', function () {
        resetForm(); // Reset form fields
        stateCode = null; // Reset stateCode
        $('#stateDropdown').val(''); // Clear state dropdown selection
        currentPage = 1; // Reset to the first page
        fetchCities(currentPage, pageSize, searchQuery, stateCode); // Fetch initial cities list
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
