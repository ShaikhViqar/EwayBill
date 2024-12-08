function collectInvoiceDetails() {
    const supplyType = document.getElementById("supplyType").value;
    const subSupplyType = document.getElementById("subSupplyType").value;
    const docType = document.getElementById("documentType").value;
    const docNo = document.getElementById("documentNumber").value;
    const docDate = document.getElementById("documentDate").value;
    const fromGstin = document.getElementById("fromGSTIN").value;
    const fromTrdName = document.getElementById("fromTraderName").value;
    const fromAddr1 = document.getElementById("fromAddress1").value;
    const fromAddr2 = document.getElementById("fromAddress2").value;
    const fromPlace = document.getElementById("fromPlace").value;
    const actFromStateCode = document.getElementById("actFromStateCode").value;
    const fromPincode = document.getElementById("fromPincode").value;
    const fromStateCode = document.getElementById("fromStateCode").value;
    const toGstin = document.getElementById("toGSTIN").value;
    const toTrdName = document.getElementById("toTraderName").value;
    const toAddr1 = document.getElementById("toAddress1").value;
    const toAddr2 = document.getElementById("toAddress2").value;
    const toPlace = document.getElementById("toPlace").value;
    const toPincode = document.getElementById("toPincode").value;
    const actToStateCode = document.getElementById("actToStateCode").value;
    const toStateCode = document.getElementById("toStateCode").value;
    const transactionType = document.getElementById("transactionType").value;
    const dispatchFromGSTIN = document.getElementById("dispatchFromGSTIN").value;
    const dispatchFromTradeName = document.getElementById("dispatchFromTraderName").value;
    const shipToGSTIN = document.getElementById("shipToGSTIN").value;
    const shipToTradeName = document.getElementById("shipToTradeName").value;
    const totalValue = document.getElementById("totalValue").value;
    const cgstValue = document.getElementById("cgstValue").value;
    const sgstValue = document.getElementById("sgstValue").value;
    const igstValue = document.getElementById("igstValue").value;
    const cessValue = document.getElementById("cessValue").value;
    const cessNonAdvolValue = document.getElementById("cessNonAdvolValue").value;
    const totInvValue = document.getElementById("totalInvoiceValue").value;
    const transMode = document.getElementById("transportMode").value;
    const transDistance = document.getElementById("transportDistance").value;
    const transporterId = document.getElementById("transporterID").value;
    const transDocNo = document.getElementById("transportDocumentNo").value;
    const vehicleNo = document.getElementById("vehicleNo").value;
    const vehicleType = document.getElementById("vehicleType").value;

    const invoiceDetails = {
        supplyType,
        subSupplyType,
        docType,
        docNo,
        docDate,
        fromGstin,
        fromTrdName,
        fromAddr1,
        fromAddr2,
        fromPlace,
        actFromStateCode,
        fromPincode,
        fromStateCode,
        toGstin,
        toTrdName,
        toAddr1,
        toAddr2,
        toPlace,
        toPincode,
        actToStateCode,
        toStateCode,
        transactionType,
        dispatchFromGSTIN,
        dispatchFromTradeName,
        shipToGSTIN,
        shipToTradeName,
        totalValue,
        cgstValue,
        sgstValue,
        igstValue,
        cessValue,
        cessNonAdvolValue,
        totInvValue,
        transMode,
        transDistance,
        transporterId,
        transDocNo,
        vehicleNo,
        vehicleType
    };
    return invoiceDetails;
}

function collectInvoiceItems() {
    const invoiceItems = [];
    const itemRows = document.querySelectorAll(".itemRow");

    itemRows.forEach(row => {
        const productNameElement = row.querySelector(".col-md-4.mb-3 input[name='productName']");
        const productDescriptionElement = row.querySelector(".col-md-4.mb-3 input[name='productDescription']");
        const hsnCodeElement = row.querySelector(".col-md-4.mb-3 input[name='hsnCode']");
        const quantityElement = row.querySelector(".col-md-4.mb-3 input[name='quantity']");
        const quantityUnitElement = row.querySelector(".col-md-4.mb-3 input[name='quantityUnit']");
        const taxableAmountElement = row.querySelector(".col-md-4.mb-3 input[name='taxableAmount']");
        const cgstRateElement = row.querySelector(".col-md-4.mb-3 input[name='cgstRate']");
        const sgstRateElement = row.querySelector(".col-md-4.mb-3 input[name='sgstRate']");
        const igstRateElement = row.querySelector(".col-md-4.mb-3 input[name='igstRate']");
        const cessRateElement = row.querySelector(".col-md-4.mb-3 input[name='cessRate']");

        const ProductName = productNameElement ? productNameElement.value : "";
        const ProductDesc = productDescriptionElement ? productDescriptionElement.value : "";
        const HsnCode = hsnCodeElement ? hsnCodeElement.value : "";
        const Quantity = quantityElement ? quantityElement.value : "";
        const QtyUnit = quantityUnitElement ? quantityUnitElement.value : "";
        const TaxableAmount = taxableAmountElement ? taxableAmountElement.value : "";
        const CgstRate = cgstRateElement ? cgstRateElement.value : "";
        const SgstRate = sgstRateElement ? sgstRateElement.value : "";
        const IgstRate = igstRateElement ? igstRateElement.value : "";
        const CessRate = cessRateElement ? cessRateElement.value : "";

        const item = {
            ProductName,
            ProductDesc,
            HsnCode,
            Quantity,
            QtyUnit,
            TaxableAmount,
            CgstRate,
            SgstRate,
            IgstRate,
            CessRate
        };
        invoiceItems.push(item);
    });
    return invoiceItems;
}

$(document).ready(function() {
    $('#submitAll').click(function(event) {
        event.preventDefault();

        const invoiceDetails = collectInvoiceDetails();
        const invoiceItems = collectInvoiceItems();

        const payload = {
            ...invoiceDetails,
            ItemList: invoiceItems
        };

        fetch('/EwayBill/Generate', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(payload)
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    showToast('Success', data.message || 'Data submitted successfully!', 'success');
                } else {
                    showToast('Error', data.message || 'An error occurred: ' + (data.error || ''), 'danger');
                }
            })
            .catch(error => {
                showToast('Error', 'Error submitting data: ' + error, 'danger');
            });
    });
});

function showToast(title, message, type) {
    $('#toastBody').text(message);

    if (type === 'success') {
        $('#toastMessage').removeClass('bg-danger').addClass('bg-success');
    } else if (type === 'danger') {
        $('#toastMessage').removeClass('bg-success').addClass('bg-danger');
    }

    var toastElement = new bootstrap.Toast(document.getElementById('toastMessage'), {
        delay: 5000,
        autohide: true
    });
    toastElement.show();
}

document.addEventListener('DOMContentLoaded', function() {
    const itemContainer = document.getElementById('itemContainer');
    const addItemButton = document.getElementById('addItemButton');

    addItemButton.addEventListener('click', function() {
        const itemRow = document.querySelector('.itemRow');
        const newItemRow = itemRow.cloneNode(true);
        itemContainer.appendChild(newItemRow);
        clearInputs(newItemRow);
    });

    itemContainer.addEventListener('click', function(event) {
        if (event.target.classList.contains('removeItem')) {
            const itemRows = document.querySelectorAll('.itemRow');
            if (itemRows.length > 1) {
                event.target.closest('.itemRow').remove();
            } else {
                showToast('Error', 'Cannot remove the last item!', 'danger');
            }
        }
    });

    function clearInputs(row) {
        const inputs = row.querySelectorAll('input');
        inputs.forEach(input => input.value = '');
    }
});
