<script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>



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
            document.getElementById('purchaseForm').submit();
        }
    });
}
    // If user cancels, do nothing (form won't be submitted)

