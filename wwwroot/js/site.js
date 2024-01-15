function confirmPurchase(qty) {
    var inputQuantity = document.querySelector('#quantityInput');
    
    // Parse the input value as an integer
    var inputValue = parseInt(inputQuantity.value);

    if (inputValue > qty) {
        Swal.fire({
            icon: 'error',
            title: 'Not Enough Stock!',
            text: 'Sorry, the quantity you selected exceeds the available stock.',
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'OK'
        });

        // Set the input value to the available stock
        inputQuantity.value = qty;
    } else {
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
}
