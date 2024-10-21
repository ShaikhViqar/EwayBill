$(document).ready(function () {
    let currentPage = 1;
    let pageSize = 5;
    let totalItems = 0;
    let totalPages = 0;
    let deleteStateID = null;
    let searchQuery = '';

    fetchStates(currentPage, pageSize, searchQuery);

    $('#pageSize').change(function () {
        pageSize = parseInt($(this).val());
        currentPage = 1;
        fetchStates(currentPage, pageSize, searchQuery);
    });

    function fetchStates(page, size, search) {
        $.ajax({
            url: `/Masters/GetManageState?page=${page}&pageSize=${size}&searchQuery=${search}`,
            method: 'GET',
            success: function (response) {
                totalItems = response.totalItems;
                totalPages = Math.ceil(totalItems / pageSize);
                renderTable(response.states);
                updatePagination();
            },
            error: function (error) {
                showToast('Error', 'fetching states:', 'danger');
            }
        });
    }

    function renderTable(states) {
        var tableBody = '';
        $.each(states, function (index, state) {
            tableBody += `
                <tr class="align-middle">
                    <td>${state.StateCode}</td>
                    <td>${state.State}</td>
                    <td>
                        <button class="btn btn-warning btn-sm editState" data-stateid="${state.StateID}" data-statecode="${state.StateCode}" data-statename="${state.State}">Edit</button>
                        <button class="btn btn-danger btn-sm deleteState" data-stateid="${state.StateID}">Delete</button>
                    </td>
                </tr>`;
        });
        $('#statesTable tbody').html(tableBody);
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
        fetchStates(currentPage, pageSize, searchQuery);
    });

    $('#prevPage').click(function () {
        if (currentPage > 1) {
            currentPage--;
            fetchStates(currentPage, pageSize, searchQuery);
        }
    });

    $('#nextPage').click(function () {
        if (currentPage < totalPages) {
            currentPage++;
            fetchStates(currentPage, pageSize, searchQuery);
        }
    });

    $('#lastPage').click(function () {
        currentPage = totalPages;
        fetchStates(currentPage, pageSize, searchQuery);
    });

    // Bind click event for the Edit button
    $('#statesTable').on('click', '.editState', function () {
        const stateID = $(this).data('stateid');
        const stateCode = $(this).data('statecode'); // Fetch the state code
        const stateName = $(this).data('statename');

        $('#statecode').val(stateCode); // Bind the state code to the input
        $('#state').val(stateName);
        $('#stateID').val(stateID);
        $('#submitButton').text('Update');
        $('#cancelButton').show();
        $('#stateError').hide();
    });

    $('#statesTable').on('click', '.deleteState', function () {
        deleteStateID = $(this).data('stateid');
        $('#deleteConfirmationModal').modal('show');
    });

    $('#confirmDeleteButton').on('click', function () {
        if (deleteStateID) {
            $.ajax({
                url: `/Masters/DeleteManageState?stateID=${deleteStateID}`,
                method: 'DELETE',
                success: function (response) {
                    $('#deleteConfirmationModal').modal('hide');
                    showToast('Success', 'State deleted successfully!', 'success');
                    fetchStates(currentPage, pageSize, searchQuery);
                    resetForm();
                },
                error: function (error) {
                    showToast('Error', 'Error deleting state. Please try again.', 'danger');
                }
            });
        }
    });

    function resetForm() {
        $('#statecode').val('');
        $('#state').val('');
        $('#stateID').val('');
        $('#submitButton').text('Submit');
        $('#cancelButton').hide();
        deleteStateID = null;
        $('#search').val('');
        searchQuery = "";
    }

    $('#manageStateForm').on('submit', function (e) {
        e.preventDefault();

        // Check for any validation errors before submitting
        if ($('#stateError').is(':visible')) {
            return;
        }

        // Determine if it's an update or a new save
        const isUpdate = $('#submitButton').text().trim() === 'Update';

        $.ajax({
            url: '/Masters/SaveManageState',
            method: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                resetForm();
                fetchStates(currentPage, pageSize, searchQuery);
                $('#stateError').hide();
                if (isUpdate) {
                    showToast('Success', 'State updated successfully!', 'success');
                } else {
                    showToast('Success', 'State saved successfully!', 'success');
                }
            },
            error: function (error) {
                if (isUpdate) {
                    showToast('Error', 'Error updated state. Please try again.', 'danger');
                } else {
                    showToast('Error', 'Error saved state. Please try again.', 'danger');
                }
            }
        });
    });

    $('#cancelButton').on('click', function () {
        resetForm();
    });

    $('#alertCancelButton').on('click', function () {
        $('#alertMessage').fadeOut();
    });

    // Check if the state already exists
    $('#state').on('input', function () {
        var state = $(this).val();
        $('#stateError').hide();
        $('#submitButton').prop('disabled', false);

        if (state.length > 0) {
            $.ajax({
                url: `/Masters/CheckManageState?state=${state}`,
                method: 'GET',
                data: { state: state },
                success: function (response) {
                    if (!response) {
                        $('#stateError').text('This state is already taken.').show();
                        $('#submitButton').prop('disabled', true);
                    }
                },
                error: function (xhr, status, error) {
                    showToast('Error', 'AJAX Error:', 'danger');
                }
            });
        }
    });

    // Check if the statecode already exists
    $('#statecode').on('input', function () {
        var statecode = $(this).val();
        $('#statecodeError').hide();
        $('#submitButton').prop('disabled', false);

        if (statecode.length > 0) {
            $.ajax({
                url: `/Masters/CheckManageStateCode?statecode=${statecode}`,
                method: 'GET',
                data: { statecode: statecode },
                success: function (response) {
                    if (!response) {
                        $('#statecodeError').text('This statecode is already taken.').show();
                        $('#submitButton').prop('disabled', true);
                    }
                },
                error: function (xhr, status, error) {
                    showToast('Error', 'AJAX Error:', 'danger');
                }
            });
        }
    });

    // Update the search function to work with AJAX
    $('#search').on('keyup', function () {
        searchQuery = $(this).val(); // Update searchQuery based on input
        currentPage = 1; // Reset to the first page
        fetchStates(currentPage, pageSize, searchQuery);
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

    // Add reset button functionality
    $('#resetButton').on('click', function () {
        resetForm(); // Reset form fields
        currentPage = 1; // Reset to the first page
        fetchStates(currentPage, pageSize, searchQuery);
    });
});
