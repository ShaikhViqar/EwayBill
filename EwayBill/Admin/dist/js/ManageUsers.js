$(document).ready(function () {
    let currentPage = 1;
    let pageSize = 5;
    let totalItems = 0;
    let totalPages = 0;
    let deleteUserId = null;
    let searchQuery = '';

    fetchUsers(currentPage, pageSize, searchQuery);

    $('#pageSize').change(function () {
        pageSize = parseInt($(this).val());
        currentPage = 1;
        fetchUsers(currentPage, pageSize, searchQuery);
    });

    function fetchUsers(page, size, search) {
        $.ajax({
            url: `/Account/ManageUsers?page=${page}&pageSize=${size}&searchQuery=${search}`,
            method: 'GET',
            success: function (response) {
                totalItems = response.totalItems;
                totalPages = Math.ceil(totalItems / pageSize);
                renderUserTable(response.users);
                updatePagination();
            },
            error: function (error) {
                showToast('Error', 'Error fetching users:', 'danger');
            }
        });
    }

    function renderUserTable(users) {
        var tableBody = '';
        $.each(users, function (index, user) {
            tableBody += `
            <tr class="align-middle">
                <td>${user.UserID}</td>
                <td>${user.FirstName}</td>
                <td>${user.LastName}</td>
                <td>${user.Email}</td>
                <td>${user.Role}</td>
                <td>
                    <button class="btn btn-primary btn-sm viewUser" data-userid="${user.UserID}">View</button>
                    <button class="btn btn-warning btn-sm editUser" data-userid="${user.UserID}">Edit</button>
                    <button class="btn btn-danger btn-sm deleteUser" data-userid="${user.UserID}">Delete</button>
                </td>
            </tr>`;
        });
        $('#userTable tbody').html(tableBody);
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
        fetchUsers(currentPage, pageSize, searchQuery);
    });

    $('#prevPage').click(function () {
        if (currentPage > 1) {
            currentPage--;
            fetchUsers(currentPage, pageSize, searchQuery);
        }
    });

    $('#nextPage').click(function () {
        if (currentPage < totalPages) {
            currentPage++;
            fetchUsers(currentPage, pageSize, searchQuery);
        }
    });

    $('#lastPage').click(function () {
        currentPage = totalPages;
        fetchUsers(currentPage, pageSize, searchQuery);
    });

    $('#userTable').on('click', '.deleteUser', function () {
        deleteUserId = $(this).data('userid');
        $('#deleteConfirmationModal').modal('show');
    });

    $('#confirmDeleteButton').on('click', function () {
        if (deleteUserId) {
            $.ajax({
                url: `/Account/DeleteManageUsers?userId=${deleteUserId}`,
                method: 'DELETE',
                success: function (response) {
                    if (response.success) {
                        $('#deleteConfirmationModal').modal('hide');
                        showToast('Success', 'User delete successfully!', 'success');
                        fetchUsers(currentPage, pageSize, searchQuery);
                    } else {
                        showToast('Error', 'Error deleting user.', 'danger');
                    }
                },
                error: function (error) {
                    showToast('Error', 'Error deleting user.', 'danger');
                }
            });
        }
    });

    // Event listener for View button
$('#userTable').on('click', '.viewUser', function () {
    const userId = $(this).data('userid');

    // Fetch user data first
    $.ajax({
        url: '/Account/ManageUsers',
        type: 'GET',
        data: { userId: userId },
        success: function(response) {
            if (response.error) {
                showToast('Error', response.error, 'danger');
            } else {
                // Assuming the response contains a single user object
                const user = response.users[0];
                
                // Store user data in session storage to access it on the new page
                sessionStorage.setItem('userData', JSON.stringify(user));

                // Redirect to the new page
                window.location.href = '/Admin/dist/pages/Users/ManageView.html'; // Adjust this path as needed
            }
        },
        error: function(xhr, status, error) {
            showToast('Error', response.error, 'danger');
        }
    });
});

    // Event listener for Edit button
    $('#userTable').on('click', '.editUser', function () {
        const userId = $(this).data('userid');

    // Fetch user data first
    $.ajax({
        url: '/Account/ManageUsers',
        type: 'GET',
        data: { userId: userId },
        success: function(response) {
            if (response.error) {
                showToast('Error', response.error, 'danger');
            } else {
                // Assuming the response contains a single user object
                const user = response.users[0];
                
                // Store user data in session storage to access it on the new page
                sessionStorage.setItem('userData', JSON.stringify(user));

                // Redirect to the new page
                window.location.href = '/Admin/dist/pages/Users/ManageEdit.html'; // Adjust this path as needed
            }
        },
        error: function(xhr, status, error) {
            showToast('Error', response.error, 'danger');
        }
    });
});

    // Redirect to ManageRegister.html on button click
    document.getElementById("addUserButton").addEventListener("click", function () {
        window.location.href = "/Admin/dist/pages/Users/ManageRegister.html";
    });

    // Update the search function to work with AJAX
    $('#search').on('keyup', function () {
        searchQuery = $(this).val(); // Update searchQuery based on input
        currentPage = 1; // Reset to the first page
        fetchUsers(currentPage, pageSize, searchQuery); // Fetch roles based on new searchQuery
    });

    document.getElementById('clearSearch').addEventListener('click', function() {
    document.getElementById('search').value = '';  // Clear the input field
    searchQuery = '';  // Reset the searchQuery variable
    fetchUsers(currentPage, pageSize, searchQuery);
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
            delay: 10000, // Set the delay to 5 seconds
            autohide: true
        });
    
        // Show the toast
        toastElement.show();
    }

    function showRegistrationSuccessToast() {
    if (sessionStorage.getItem('registrationSuccess')) {
        showToast('Success', 'Registration completed successfully!', 'success');
        sessionStorage.removeItem('registrationSuccess'); // Remove the success flag
        }
    }

    function showRegistrationEditSuccessToast() {
    if (sessionStorage.getItem('registrationEditSuccess')) {
        showToast('Success', 'Registration edited successfully!', 'success');
        sessionStorage.removeItem('registrationEditSuccess'); // Remove the success flag
        }
    }

    // Check if the document is already loaded
    if (document.readyState === 'complete' || document.readyState === 'interactive') {
        showRegistrationSuccessToast();
        showRegistrationEditSuccessToast();
    } 
});
