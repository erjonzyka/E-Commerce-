function confirmPurchase() {
    Swal.fire({
        title: 'Are you sure?',
        text: 'Are you sure you want to purchase this product?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, buy it!'
    }).then((result) => {
        if (result.isConfirmed) {
            // If user confirms, submit the form
            var purchaseForm = document.getElementById('purchaseForm');
            if (purchaseForm) {
                purchaseForm.submit();
            } else {
                console.error('Form with ID "purchaseForm" not found.');
            }
        }
    });
}

    // If user cancels, do nothing (form won't be submitted)

