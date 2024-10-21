$(document).ready(function () {
    let currentPage = 1;
    let pageSize = 5;
    let totalItems = 0;
    let totalPages = 0;
    let deleteRoleID = null;
    let searchQuery = '';

    fetchRoles(currentPage, pageSize, searchQuery);

    $('#pageSize').change(function () {
        pageSize = parseInt($(this).val());
        currentPage = 1;
        fetchRoles(currentPage, pageSize, searchQuery);
    });

    function fetchRoles(page, size, search) {
        $.ajax({
            url: `/Masters/GetManageRole?page=${page}&pageSize=${size}&searchQuery=${search}`,
            method: 'GET',
            success: function (response) {
                totalItems = response.totalItems;
                totalPages = Math.ceil(totalItems / pageSize);
                renderTable(response.roles);
                updatePagination();
            },
            error: function (error) {
                showToast('Error', 'Error fetching roles:', 'danger');
            }
        });
    }

    function renderTable(roles) {
        var tableBody = '';
        $.each(roles, function (index, role) {
            tableBody += `
                <tr class="align-middle role-row" data-roleid="${role.RoleID}">
                    <td>${role.Role}</td>
                    <td>
                        <button class="btn btn-warning btn-sm editRole" data-roleid="${role.RoleID}" data-rolename="${role.Role}">Edit</button>
                        <button class="btn btn-danger btn-sm deleteRole" data-roleid="${role.RoleID}">Delete</button>
                    </td>
                </tr>`;
        });
        $('#rolesTable tbody').html(tableBody);
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
        fetchRoles(currentPage, pageSize, searchQuery);
    });

    $('#prevPage').click(function () {
        if (currentPage > 1) {
            currentPage--;
            fetchRoles(currentPage, pageSize, searchQuery);
        }
    });

    $('#nextPage').click(function () {
        if (currentPage < totalPages) {
            currentPage++;
            fetchRoles(currentPage, pageSize, searchQuery);
        }
    });

    $('#lastPage').click(function () {
        currentPage = totalPages;
        fetchRoles(currentPage, pageSize, searchQuery);
    });

    // Bind click event for the Edit button
    $('#rolesTable').on('click', '.editRole', function () {
        const roleID = $(this).data('roleid');
        const roleName = $(this).data('rolename');
        $('#role').val(roleName);
        $('#roleID').val(roleID);
        $('#submitButton').text('Update');
        $('#cancelButton').show();
        $('#roleError').hide();
    });

    $('#rolesTable').on('click', '.deleteRole', function () {
        deleteRoleID = $(this).data('roleid');
        $('#deleteConfirmationModal').modal('show');
    });

    $('#confirmDeleteButton').on('click', function () {
        if (deleteRoleID) {
            $.ajax({
                url: `/Masters/DeleteManageRole?roleID=${deleteRoleID}`,
                method: 'DELETE',
                success: function (response) {
                    $('#deleteConfirmationModal').modal('hide');
                    showToast('Success', 'Role deleted successfully!', 'success');
                    fetchRoles(currentPage, pageSize, searchQuery);
                    resetForm();
                },
                error: function (error) {
                    showToast('Error', 'Error deleting role!', 'danger');
                }
            });
        }
    });

    function resetForm() {
        $('#role').val('');
        $('#roleID').val('');
        $('#submitButton').text('Submit');
        $('#cancelButton').hide();
        deleteRoleID = null;
        $('#search').val('');
        searchQuery = "";
    }

    $('#manageRoleForm').on('submit', function (e) {
        e.preventDefault();

        // Check for any validation errors before submitting
        if ($('#roleError').is(':visible')) {
            return;
        }

        // Determine if it's an update or a new save
        const isUpdate = $('#submitButton').text().trim() === 'Update';

        $.ajax({
            url: '/Masters/SaveManageRole',
            method: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                resetForm();
                fetchRoles(currentPage, pageSize, searchQuery);
                $('#roleError').hide();
                // Show appropriate toast message based on action
                if (isUpdate) {
                    showToast('Success', 'Role updated successfully!', 'success');
                } else {
                    showToast('Success', 'Role saved successfully!', 'success');
                }
            },
            error: function (error) {
                // Show appropriate toast message based on action
                if (isUpdate) {
                    showToast('Error', 'Error updated role. Please try again.', 'danger');
                } else {
                    showToast('Error', 'Error saved role. Please try again.', 'danger');
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

    // Check if the role already exists
    $('#role').on('input', function () {
        var role = $(this).val();
        $('#roleError').hide();
        $('#submitButton').prop('disabled', false);

        if (role.length > 0) {
            $.ajax({
                url: `/Masters/CheckManageRole?role=${role}`,
                method: 'GET',
                data: { role: role },
                success: function (response) {
                    if (!response) {
                        $('#roleError').text('This role is already taken.').show();
                        $('#submitButton').prop('disabled', true);
                    }
                },
                error: function (error) {
                    showToast('Error', 'checking role:', 'danger');
                }
            });
        }
    });

    // Update the search function to work with AJAX
    $('#search').on('keyup', function () {
        searchQuery = $(this).val(); // Update searchQuery based on input
        currentPage = 1; // Reset to the first page
        fetchRoles(currentPage, pageSize, searchQuery); // Fetch roles based on new searchQuery
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
        fetchRoles(currentPage, pageSize, searchQuery);
    });
});
