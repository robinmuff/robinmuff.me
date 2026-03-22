function getLanguage() {
  if (window.location.pathname.startsWith('/de')) {
    return 'de';
  }
  var stored = localStorage.getItem('lang');
  if (stored === 'de' || stored === 'en') {
    return stored;
  }
  return 'en';
}

function setLanguage(lang) {
  var strings = translations[lang];
  if (!strings) return;

  document.querySelectorAll('[data-i18n]').forEach(function (el) {
    var key = el.getAttribute('data-i18n');
    if (strings[key] !== undefined) {
      el.textContent = strings[key];
    }
  });

  document.querySelectorAll('[data-i18n-placeholder]').forEach(function (el) {
    var key = el.getAttribute('data-i18n-placeholder');
    if (strings[key] !== undefined) {
      el.placeholder = strings[key];
    }
  });

  document.querySelectorAll('[data-i18n-value]').forEach(function (el) {
    var key = el.getAttribute('data-i18n-value');
    if (strings[key] !== undefined) {
      el.value = strings[key];
    }
  });

  document.documentElement.lang = lang;
  document.title = strings['page.title'] || document.title;
  localStorage.setItem('lang', lang);

  var targetPath = lang === 'de' ? '/de' : '/';
  if (window.location.pathname !== targetPath) {
    history.pushState(null, '', targetPath);
  }
}

document.addEventListener('DOMContentLoaded', function () {
  setLanguage(getLanguage());
});
