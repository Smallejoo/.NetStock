
    document.getElementById("loginForm").addEventListener("submit", async (e) => {
      e.preventDefault();

      const res = await fetch("/api/users/LogIn?userName=" +
        document.getElementById("username").value +
        "&password=" +
        document.getElementById("password").value);

      if (res.ok) {
        const data = await res.json();
        localStorage.setItem("token", data.token); // 🧠 save token
        window.location.href = "products.html";    // go to stock page
      } else {
        alert("Login failed");
      }
    });
  