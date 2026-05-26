document.querySelector("[data-menu-toggle]")?.addEventListener("click", () => {
  const menu = document.querySelector("[data-menu]");
  menu?.classList.toggle("hidden");
  menu?.classList.toggle("flex");
  menu?.classList.toggle("absolute");
  menu?.classList.toggle("left-4");
  menu?.classList.toggle("right-4");
  menu?.classList.toggle("top-16");
  menu?.classList.toggle("flex-col");
  menu?.classList.toggle("rounded-lg");
  menu?.classList.toggle("bg-black");
  menu?.classList.toggle("p-4");
});

document.querySelectorAll("[data-quantity-input]").forEach((input) => {
  const form = input.closest("form");
  const decrease = form?.querySelector("[data-quantity-decrease]");
  const increase = form?.querySelector("[data-quantity-increase]");

  const step = (change) => {
    const min = Number(input.min || 1);
    const max = Number(input.max || Number.MAX_SAFE_INTEGER);
    const current = Number(input.value || min);
    input.value = Math.min(max, Math.max(min, current + change));
  };

  decrease?.addEventListener("click", () => step(-1));
  increase?.addEventListener("click", () => step(1));
});
