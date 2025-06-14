if (location.pathname.includes("index.html")) {
  document.getElementById("loginForm").addEventListener("submit", async (e) => {
    e.preventDefault();
    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;

    const res = await fetch("/api/users/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ userName: username, password })
    });

    const data = await res.json();
    if (res.ok && data.token) {
      localStorage.setItem("token", data.token);
      alert("Login successful!");
      window.location.href = "products.html";
    } else {
      alert("Login failed");
    }
  });
}

if (location.pathname.includes("products.html")) {
  const token = localStorage.getItem("token");

  async function loadProducts() {
    const res = await fetch("/api/products", {
      headers: { Authorization: `Bearer ${token}` }
    });
    const products = await res.json();
    const list = document.getElementById("productList");
    list.innerHTML = "";
    products.forEach(p => {
      const item = document.createElement("li");
      item.className = "list-group-item d-flex justify-content-between";
      item.textContent = `${p.name} - ${p.stock} units - $${p.price}`;
      list.appendChild(item);
    });
  }

  window.addProduct = async function () {
    const name = document.getElementById("productName").value;
    const res = await fetch("/api/products", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify({ name, description: "", stock: 0, price: 0 })
    });
    if (res.ok) {
      alert("Product added!");
      loadProducts();
    }
  };

  loadProducts();
}
