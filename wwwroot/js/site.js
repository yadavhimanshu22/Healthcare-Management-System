document.addEventListener("DOMContentLoaded", () => {
  document.querySelectorAll("form").forEach(form => {
    form.addEventListener("submit", () => {
      const button = form.querySelector("button[type='submit'], button:not([type])");
      if (button && !button.disabled) {
        button.dataset.originalText = button.innerText;
        button.innerText = "Please wait...";
      }
    });
  });
});
