const loadText = document.querySelector(".loading-text");
const container = document.querySelector(".container");

let load = 0;

let interval = setInterval(blurring, 10);

function blurring() {
  load++;
  if (load > 99) {
    clearInterval(interval);
    loadText.parentNode.removeChild(loadText);
  }
  loadText.innerText = `${load}%`;
  loadText.style.opacity = 1 - load / 100;
  loadText.style.width = `${10 + load}%`;
  container.style.filter = `blur(${30 - (load * 30) / 100}px)`;
}
