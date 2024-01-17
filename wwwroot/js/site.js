function confirmPurchase(qty, event) {

    event.preventDefault();

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

                Swal.fire({
                    icon: 'success',
                    title: 'Order Confirmation!',
                    text: 'Thank you for your order.',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'OK'
                }).then(() => {
                    var purchaseForm = document.getElementById('purchaseForm');
                    if (purchaseForm) {
                        purchaseForm.submit();
                    } else {
                        console.error('Form with ID "purchaseForm" not found.');
                    }
                });
            }
        });
    }
}


function confirmCart(qty, event) {
    event.preventDefault();

    var cartForm = document.getElementById('cartForm');
    var cartQuantity = document.getElementById('cartQuantity');
    var inputQuantity = document.getElementById('quantityInput');

    // Parse the input value as an integer
    var inputValue = parseInt(inputQuantity.value);
    console.log("Sasia e inputit :" + inputValue);

    if (inputValue > qty) {
        Swal.fire({
            icon: 'error',
            title: 'Not Enough Stock!',
            text: 'Sorry, the quantity you selected exceeds the available stock.',
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'OK'
        });

        inputQuantity.value = qty;
        cartQuantity.value = qty;
    } else {
        // Set the cartQuantity value
        cartQuantity.value = inputValue;

        // Manually trigger the form submission
        if (cartForm) {
            cartForm.submit();
        } else {
            console.error('Form with ID "cartForm" not found.');
        }
    }
}


function updateCartQuantity() {
    var cartQuantity = document.getElementById('cartQuantity');
    var quantityInput = document.getElementById('quantityInput');
    
    // Update cartQuantity value when quantityInput changes
    cartQuantity.value = quantityInput.value;
}



