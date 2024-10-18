const SELECTOR_SIDEBAR_WRAPPER = ".sidebar-wrapper";
    const Default = {
    scrollbarTheme: "os-theme-light",
    scrollbarAutoHide: "leave",
    scrollbarClickScroll: true,
    };
    document.addEventListener("DOMContentLoaded", function() {
    const sidebarWrapper = document.querySelector(SELECTOR_SIDEBAR_WRAPPER);
    if (sidebarWrapper && typeof OverlayScrollbarsGlobal?.OverlayScrollbars !== "undefined") {
    OverlayScrollbarsGlobal.OverlayScrollbars(sidebarWrapper, {
    scrollbars: {
    theme: Default.scrollbarTheme,
    autoHide: Default.scrollbarAutoHide,
    clickScroll: Default.scrollbarClickScroll,
    },
    });
    }
    });

    // Redirect to BillNew.html on button click
    document.getElementById("addNewBillButton").addEventListener("click", function () {
        window.location.href = "/Admin/dist/pages/Users/BillNew.html";
    });

    function search() {
    const input = document.getElementById('search');
    const filter = input.value.toLowerCase();
    const table = document.getElementById('userTable');
    const tr = table.getElementsByTagName('tr');

    for (let i = 1; i < tr.length; i++) {
        const td = tr[i].getElementsByTagName('td');
        let rowVisible = false;

        for (let j = 0; j < td.length; j++) {
            if (td[j]) {
                const txtValue = td[j].textContent || td[j].innerText;
                if (txtValue.toLowerCase().indexOf(filter) > -1) {
                    rowVisible = true;
                    break; // No need to check further if one match is found
                }
            }
        }
        tr[i].style.display = rowVisible ? "" : "none"; // Show or hide the row
    }
}
