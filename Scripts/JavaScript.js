  
        ShowCount();
        $(function () {
            ShowCount();
                $(".btnAddToCart").click(function (event) {
                    event.preventDefault();
                    var productId = $(this).data("id");



                    $.ajax({
                        url: '@Url.Action("AddToCart", "CartItem")',
                        type: "POST",
                        data: JSON.stringify({ productId: productId, quantity: 1 }),
                        contentType: "application/json",
                        dataType: "JSON",
                        success: function (response) {
                            if (response.success) {
                                $('#checkout_items').html(response.Count);
                                alert('Sản phẩm đã được thêm vào giỏ hàng!');
                            } else {
                                alert('Đã xảy ra lỗi khi thêm sản phẩm vào giỏ hàng!');
                            }
                        },
                    });
                });
            });
        function ShowCount() {
            $.ajax({
                url: '/CartItem/ShowCount',
                type: 'GET',
                success: function (rs) {
                    $('#checkout_items').html(rs.Count);
                }
            });
        }
   

    
        document.addEventListener("DOMContentLoaded", function () {
            var checkboxes = document.querySelectorAll('input[type="checkbox"]');

            checkboxes.forEach(function (checkbox) {
                checkbox.addEventListener('change', function () {
                    filterProducts();
                });
            });

            function filterProducts() {
                var checkedCheckboxes = document.querySelectorAll('input[type="checkbox"]:checked');
                var filteredPrices = [];

                checkedCheckboxes.forEach(function (checkbox) {
                    if (checkbox.id !== "price-all") {
                        var priceRange = checkbox.id.split('-').slice(1);
                        filteredPrices.push(priceRange);
                    }
                });

                var products = document.querySelectorAll('.col-sm-12');

                products.forEach(function (product) {
                    var productPrice = parseFloat(product.getAttribute('data-price'));

                    if (filteredPrices.length === 0 || filteredPrices.some(function (range) {
                        return productPrice >= parseFloat(range[0]) && productPrice <= parseFloat(range[1]);
                    })) {
                        product.style.display = 'block';
                    } else {
                        product.style.display = 'none';
                    }
                });
            }
        });
  