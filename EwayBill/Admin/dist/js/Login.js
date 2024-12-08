const SELECTOR_SIDEBAR_WRAPPER = ".sidebar-wrapper";
const Default = {
    scrollbarTheme: "os-theme-light",
    scrollbarAutoHide: "leave",
    scrollbarClickScroll: true,
};

document.addEventListener("DOMContentLoaded", function() {
    const sidebarWrapper = document.querySelector(SELECTOR_SIDEBAR_WRAPPER);
    if (sidebarWrapper) {
        OverlayScrollbarsGlobal.OverlayScrollbars(sidebarWrapper, {
            scrollbars: {
                theme: Default.scrollbarTheme,
                autoHide: Default.scrollbarAutoHide,
                clickScroll: Default.scrollbarClickScroll,
            },
        });
    }
});

$(document).ready(function() {
    $("#loginForm").on("submit", function(event) {
        event.preventDefault();

        var formData = {
            UserName: $("input[name='UserName']").val().trim(),
            Password: $("input[name='Password']").val().trim(),
        };

        // Client-side validation
        if (!formData.UserName || !formData.Password) {
            alert("Please enter both username and password.");
            return;
        }

        // Show the loader
        $("#loader").show();

        $.ajax({
            type: "POST",
            url: $(this).attr("action"),
            data: formData,
            success: function(response) {
                if (response.Redirect) {
                    window.location.href = response.Redirect;
                } else {
                    alert(response.message || "Login failed. Please try again.");
                }
            },
            error: function(xhr) {
                let message = "An error occurred. Please try again.";
                if (xhr.status === 401) {
                    message = "Invalid credentials. Please try again.";
                } else if (xhr.status === 500) {
                    message = "Server error. Please contact support.";
                }
                alert(message);
            }
        });
    });
});
