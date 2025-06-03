var OwnerDetail = {
    displayName: "",
    branchAddress: "",
    pincode: "",
}

$(document).ready(function () {
    GetOwnerDetails();
});

$(document).on("input", ".qty-input", function () {
    let row = $(this).closest("tr");
    updateTotal(row);
});

$(document).on("click", ".remove-item", function () {
    const $tableBody = $(this).closest("tbody");
    $(this).closest("tr").remove();
    updateGrandTotal();
    if ($tableBody.children("tr").length === 0) {
        SelectedOrderId = '';
    }
});

function updateTotal(row) {
    let qty = parseFloat(row.find(".qty-input").val()) || 0; // Handle decimals like 0.5
    let price = parseFloat(row.find(".price").text()) || 0; // Handle valid numbers
    let unitValue = parseFloat(row.data("unit-value")) || 1;
    // Calculate total based on the unit value
    let total = (qty / unitValue) * price;

    row.find(".total").text(total.toFixed(2)); // Display total with two decimal places
    updateGrandTotal();
}

function updateGrandTotal() {
    let total = 0;
    $("#billItems tr").each(function () {
        total += parseFloat($(this).find(".total").text());
    });
    $("#totalAmount").text(total);
}


//function addItemToBill(itemId, itemName, itemPrice, itemUnit, unitForPrice) {
//    let existingRow = $("#billItems").find(`tr[data-id='${itemId}']`);

//    if (existingRow.length > 0) {
//        let qtyInput = existingRow.find(".qty-input");
//        qtyInput.val(parseInt(qtyInput.val()) + 1);
//        updateTotal(existingRow);
//    } else {
//        $("#billItems").append(`
//                        <tr data-id="${itemId}" data-unit-value="${unitForPrice}">
//                            <td>${itemName}</td>
//                            <td><input type="number" class="qty-input form-control"  value="${unitForPrice}" min="0" style="width:75px;"></td>
//                            <td class="unit">${itemUnit}</td>
//                            <td class="price">${itemPrice}</td>
//                            <td class="total">${itemPrice}</td>
//                            <td><button class="btn btn-danger btn-sm remove-item">X</button></td>
//                        </tr>
//                    `);
//    }
//    updateGrandTotal();
//}
function addItemToBill(itemId, itemName, itemPrice, itemUnit, unitForPrice, maxValue,orderQuantity) {
    let existingRow = $("#billItems").find(`tr[data-id='${itemId}']`);

    //if (existingRow.length > 0) {
    //    let qtyInput = existingRow.find(".qty-input");
    //    let currentQty = parseInt(qtyInput.val());
    //    let newQty = currentQty + 1;

    //    if (newQty > maxValue) {
    //        newQty = maxValue;
    //        msgPopup('warning',`Only ${maxValue} items available in stock.`);
    //    }

    //    qtyInput.val(newQty);
    //    updateTotal(existingRow);

    //}
    //else {
        let initialQty = maxValue > 0 ? unitForPrice : 0;
        if (orderQuantity) {
            initialQty = orderQuantity;
        }
        $("#billItems").append(`
            <tr data-id="${itemId}" data-unit-value="${unitForPrice}">
                <td>${itemName}</td>
                <td>
                    <input type="number" 
                           class="qty-input form-control"  
                           value="${initialQty}" 
                           min="0" 
                           max="${maxValue}" 
                           style="width:75px;">
                </td>
                <td class="unit">${itemUnit}</td>
                <td class="price">${itemPrice}</td>
                <td class="total">${(initialQty * itemPrice).toFixed(2)}</td>
                <td><button class="btn btn-danger btn-sm remove-item">X</button></td>
            </tr>
        `);
    //}

    updateGrandTotal();
}
$(document).on('input', '.qty-input', function () {
    let maxVal = parseInt(this.max) || 0;
    let enteredQty = parseInt($(this).val());

    if (enteredQty > maxVal) {
        msgPopup('warning', `Maximum available stock is ${maxVal}.`);
        $(this).val(maxVal);
    } else if (enteredQty < 0 || isNaN(enteredQty)) {
        $(this).val(0);
    }

    let row = $(this).closest('tr');
    updateTotal(row);
    updateGrandTotal();
});


function GetItemsDetailsById(id) {
    $.ajax({
        url: '/Billing/GetDetailsOnBarcodeScan',
        type: 'POST',
        data: {
            itemId: id
        },
        success: function (response) {
            if (response != null) {
                addItemToBill(response.itemId, response.itemName, response.pricePerUnit, response.unit, response.priceQuantity, response.maxQuantity);
            } else {
                msgPopup('error', 'Items Is Not Present in the Inventory');
            }
        },
        error: function (xhr, status, error) {
            alert('An error occurred: ' + error);
        }
    });
}

function generatePDF() {
    $.ajax({
        url: '/Billing/GetCustomerDetailsById',
        type: 'GET',
        data: { id: $('#ddlCustomer').val() },
        success: function (response) {
            const { jsPDF } = window.jspdf;
            let doc = new jsPDF();

            // ========================== OWNER DETAILS ==========================
            doc.setFontSize(18).setFont('helvetica', 'bold');
            let ownerTitle = (OwnerDetail?.displayName || 'N/A').toUpperCase();
            doc.text(ownerTitle, 105, 15, { align: 'center' });

            doc.setFontSize(12).setFont('helvetica', 'normal');
            let branchAddress = (OwnerDetail?.branchAddress || 'N/A').toUpperCase();
            doc.text(branchAddress, 105, 25, { align: 'center' });

            let pincodeText = `Pincode: ${(OwnerDetail?.pincode || 'N/A').toUpperCase()}`;
            doc.text(pincodeText, 105, 35, { align: 'center' });

            doc.setLineWidth(0.5).line(10, 40, 200, 40);

            // ========================== CUSTOMER DETAILS ==========================
            doc.setFontSize(14).setFont('helvetica', 'bold').text('CUSTOMER DETAILS', 10, 50);

            doc.setFontSize(12).setFont('helvetica', 'normal');
            let customerName = response?.customerName || 'N/A';
            let phoneNo = response?.phoneNumber || 'N/A';
            let address = response?.address || 'N/A';

            doc.text('Name:', 10, 60);
            doc.text(`${customerName} (${phoneNo})`, 35, 60);

            doc.text('Address:', 10, 70);
            doc.text(address, 35, 70);

            doc.line(10, 75, 200, 75);

            // ========================== BILL SUMMARY ==========================
            doc.setFontSize(14).setFont('helvetica', 'bold').text('BILL SUMMARY', 10, 85);

            let billData = [];
            $("#billItems tr").each(function (index) {
                let itemName = $(this).find("td").eq(0).text().trim() || 'N/A';
                let qty = $(this).find(".qty-input").val() || '0';
                let unit = $(this).find(".unit").text().trim() || 'N/A';
                let price = $(this).find(".price").text().trim() || '0.00';
                let total = $(this).find(".total").text().trim() || '0.00';

                billData.push([index + 1, itemName, `${qty} ${unit}`, price, total]);
            });

            doc.autoTable({
                head: [['No', 'Item Name', 'Qty', 'Price', 'Amount']],
                body: billData,
                startY: 90,
                theme: 'striped',
                headStyles: { fillColor: '#f2f2f2', textColor: 'black' },
                styles: { fontSize: 10, cellPadding: 2 }
            });

            // ========================== GRAND TOTAL ==========================
            let totalAmount = $('#totalAmount').text().trim() || '0.00';
            let finalY = doc.autoTable.previous.finalY + 10;
            doc.setFontSize(14).setFont('helvetica', 'bold')
                .text(`Grand Total: ${totalAmount}`, 105, finalY, { align: 'center' });

            // ========================== PENDING BILLS ==========================
            let billDetails = response?.bills?.map(bill => ({
                date: bill.billDate
                    ? new Date(bill.billDate).toLocaleDateString('en-GB').replace(/\//g, '-')
                    : 'N/A',
                amount: bill.finalAmount || '0.00'
            })) || [];

            doc.setFontSize(12).setFont('helvetica', 'bold').text('Unpaid Bills:', 10, finalY + 10);

            let billStartY = finalY + 20;
            billDetails.forEach((bill, index) => {
                doc.text(`• Date: ${bill.date} | Amount: ₹ ${bill.amount}`, 15, billStartY + (index * 10));
            });

            let totalPending = billDetails.reduce((sum, bill) => sum + parseFloat(bill.amount), 0).toFixed(2);
            doc.setFont('helvetica', 'bold').text('Total Pending Amount:', 10, billStartY + (billDetails.length * 10) + 10);
            doc.text(`₹ ${totalPending}`, 60, billStartY + (billDetails.length * 10) + 10);

            // ========================== FOOTER (Fixed at Bottom) ==========================
            let pageHeight = doc.internal.pageSize.height;
            doc.line(10, pageHeight - 25, 200, pageHeight - 25);

            doc.setFontSize(10);
            let footerMessage = 'Thank you for your business!';
            let footerContact = 'For any queries, contact us at support@example.com';

            doc.text(footerMessage, 105, pageHeight - 15, { align: 'center' });
            doc.text(footerContact, 105, pageHeight - 10, { align: 'center' });

            // ========================== SAVE PDF ==========================
            let formattedDate = new Date().toLocaleDateString('en-GB').replace(/\//g, '-');
            doc.save(`Bill_${formattedDate}.pdf`);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching data:', error);
            alert('An error occurred while generating the PDF. Please try again.');
        }
    });
}

function ValidatePaymentMode(status) {
    if (status == "UnPaid") {
        $('#ddlPaymentMode').val('NA');
        $('#ddlPaymentMode').prop('disabled', true);
    } else {
        $('#ddlPaymentMode').prop('disabled', false);
    } 
}
function GetOwnerDetails() {
    $.ajax({
        url: '/Billing/GetOwnerBranchDetails',
        type: 'GET',
        success: function (response) {
            OwnerDetail.displayName = response.displayName;
            OwnerDetail.branchAddress = response.branchAddress;
            OwnerDetail.pincode = response.pincode;
        },
        error: function (xhr, status, error) {
            alert('An error occurred: ' + error);
        }
    });
}
