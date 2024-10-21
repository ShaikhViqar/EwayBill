$(document).ready(function () {
    // Extract email and reset code from URL query parameters
    const urlParams = new URLSearchParams(window.location.search);
    const email = urlParams.get('email'); // Get the 'email' parameter from the URL
    const resetCode = urlParams.get('resetCode'); // Get the 'resetCode' parameter from the URL

    if (!email || !resetCode) {
        showToast('Error', 'Invalid reset link. Please make sure the link contains both email and reset code.', 'danger');
        return;
    }

    // Handle form submission
    $("#resetPasswordForm").submit(function (event) {
        event.preventDefault(); // Prevent form submission from refreshing the page

        var newPassword = $("#newPassword").val();
        var confirmPassword = $("#confirmPassword").val();

        // Check if passwords match
        if (newPassword !== confirmPassword) {
            showToast('Error', 'Passwords do not match. Please try again.', 'danger');
            return;
        }

        // AJAX request to reset the password
        $.ajax({
            url: '/Account/ResetPassword',  // Replace with your server-side endpoint
            type: 'POST',
            data: JSON.stringify({ email: email, newPassword: newPassword, resetCode: resetCode }), // Include email, new password, and reset code
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                if (response.success) {
                    showToast('Success', 'Your password has been reset successfully. You can now log in with your new password.', 'success');
                } else {
                    showToast('Error', response.message || 'An error occurred while resetting your password. Please try again.', 'danger');
                }
            },
            error: function (xhr, status, error) {
                showToast('Error', 'An unexpected error occurred: ' + xhr.responseText, 'danger');
            }
        });
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
            delay: 10000, // Set the delay to 10 seconds
            autohide: true
        });
    
        // Show the toast
        toastElement.show();
    }
});
