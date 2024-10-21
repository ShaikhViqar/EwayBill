$(document).ready(function() {
    $("#forgotPasswordForm").submit(function(event) {
        event.preventDefault(); // Prevent form submission from refreshing the page

        var email = $("#email").val();

        // AJAX request to send the password reset email
        $.ajax({
            url: '/Account/ForgotPassword',  // Replace with your server-side endpoint
            type: 'POST',
            data: JSON.stringify({ email: email }),
            contentType: 'application/json; charset=utf-8',
            success: function(response) {
                if (response.success) {
                    showToast('Success', 'Password reset link has been sent to your email.', 'success');
                } else {
                    showToast('Error', response.message || 'An error occurred', 'danger');
                }
            },
            error: function(xhr, status, error) {
                showToast('Error', 'Error: ' + xhr.responseText, 'danger');
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
            delay: 10000, // Set the delay to 5 seconds
            autohide: true
        });
    
        // Show the toast
        toastElement.show();
    }
});
