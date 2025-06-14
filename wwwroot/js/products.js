document.addEventListener("DOMContentLoaded", async () => {
  const list = document.getElementById("stockList");

  document.getElementById("logOutButton").addEventListener("click",()=>
{
    localStorage.clear;
    window.location.href="/login.html";
});

  try {
    const response = await fetch("/api/Products");
    if (!response.ok) throw new Error("Failed to fetch products");

    const products = await response.json();

    if (products.length === 0) {
      list.innerHTML = "<li>No products found.</li>";
      return;
    }

    products.forEach(product => {
      const li = document.createElement("li");
      li.textContent = `${product.name} - ${product.description} - Stock: ${product.stock} - Price: ₪${product.price}`;
      list.appendChild(li);
    });

  } catch (err) {
    list.innerHTML = `<li>Error: ${err.message}</li>`;
  }
});
