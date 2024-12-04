
// Dữ liệu giỏ hàng từ Local Storage
function getCartFromLocalStorage() {
    const storedCart = localStorage.getItem("cart");
    return storedCart ? JSON.parse(storedCart) : [];
}
function updateQuantity(productId, newQuantity) {
    if (newQuantity < 1) return; // Không cho phép số lượng nhỏ hơn 1

    const currentCart = getCartFromLocalStorage();
    const updatedCart = currentCart.map((item) =>
        item.id === productId ? { ...item, quantity: newQuantity } : item
    );
    saveCartToLocalStorage(updatedCart);
    renderCart(updatedCart);
}
// Lưu giỏ hàng vào Local Storage
function saveCartToLocalStorage(cartItems) {
    localStorage.setItem("cart", JSON.stringify(cartItems));
}
function addProductToCart(quantity, id, name, price, img) {
    const cart = getCartFromLocalStorage();

    const existingProduct = cart.find((item) => item.id === id);

    if (existingProduct) {
        existingProduct.quantity += quantity;  // Thêm vào số lượng
    } else {
        cart.push({ id, name, price, quantity, img });
    }

    saveCartToLocalStorage(cart);
    renderCart(cart);  // Cập nhật giao diện giỏ hàng
}
// Hiển thị giỏ hàng
function renderCart(cartItems) {
    const cartContainer = document.querySelector("#cart-items");
    cartContainer.innerHTML = ""; // Xóa nội dung cũ

    if (cartItems.length === 0) {
        cartContainer.innerHTML = "<p class='text-center'>Giỏ hàng trống</p>";
        document.querySelector("#total-price").innerText = "0₫"; // Hiển thị tổng tiền là 0₫
        return;
    }

    cartItems.forEach((item) => {
        const row = `
    <div class="row align-items-center border-bottom py-3">
        <div class="col-4 d-flex align-items-center">
            <img src="/images/${item.img}" alt="${item.name}" class="img-thumbnail me-3" style="width: 80px;">
                <p class="mb-0">${item.name}</p>
        </div>
        <div class="col-2 text-center">${item.price.toLocaleString()}₫</div>
        <div class="col-2 text-center">
            <div class="input-group justify-content-center">
                <button class="btn btn-outline-secondary btn-sm" onclick="updateQuantity(${item.id}, ${item.quantity - 1})">-</button>
                <input type="text" class="form-control text-center" value="${item.quantity}" style="width: 50px;" readonly>
                    <button class="btn btn-outline-secondary btn-sm" onclick="updateQuantity(${item.id}, ${item.quantity + 1})">+</button>
            </div>
        </div>
        <div class="col-2 text-center">${(item.price * item.quantity).toLocaleString()}₫</div>
        <div class="col-1 text-center">
            <button class="btn btn-danger btn-sm" onclick="removeProduct(${item.id})">Xóa</button>
        </div>
    </div>
    `;
        cartContainer.innerHTML += row;
    });

    // Cập nhật tổng tiền
    updateTotal();
}

// Hàm cập nhật tổng tiền
function updateTotal() {
    const cart = getCartFromLocalStorage();
    let productIds = "";
    let productQuantities = "";
    const total = cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);

    document.querySelector("#total-price").innerText = total.toLocaleString() + "₫";
    document.querySelector("#total-price-hidden").value = total; // Lưu tổng tiền vào input ẩn

    cart.forEach(item => {
        // Thêm id vào chuỗi productIds
        productIds += `${item.id},`;

        // Thêm quantity vào chuỗi productQuantities
        productQuantities += `${item.quantity},`;
    });

    // Xóa dấu phẩy cuối cùng
    if (productIds.endsWith(',')) {
        productIds = productIds.slice(0, -1);
    }
    if (productQuantities.endsWith(',')) {
        productQuantities = productQuantities.slice(0, -1);
    }

    // Gán chuỗi vào các input hidden
    document.getElementById("product-ids").value = productIds;
    document.getElementById("product-quantities").value = productQuantities;
}

// Hàm xóa tất cả sản phẩm trong giỏ
function clearCart() {
    localStorage.removeItem("cart");
    renderCart([]);
}

// Hàm gửi thông tin đơn hàng

document.getElementById("order-form").addEventListener("submit", function (event) {
    // Trước khi gửi form, cập nhật lại các trường ẩn với thông tin giỏ hàng
    updateTotal();

    clearCart()
});

document.addEventListener("DOMContentLoaded", () => {
    const storedCart = getCartFromLocalStorage();
    renderCart(storedCart);
});
