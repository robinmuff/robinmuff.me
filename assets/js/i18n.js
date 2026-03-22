function getLanguage() {
  if (window.location.pathname.startsWith('/de')) {
    return 'de';
  }
  return 'en';
}

function applyTranslations(lang) {
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
}

function updateToggle(lang) {
  document.querySelectorAll('.lang-option').forEach(function (el) {
    if (el.getAttribute('data-lang') === lang) {
      el.classList.add('active');
    } else {
      el.classList.remove('active');
    }
  });
}

function setLanguage(lang) {
  localStorage.setItem('lang', lang);

  var onDe = window.location.pathname.startsWith('/de');
  if (lang === 'de' && !onDe) {
    window.location.href = '/de';
    return;
  }
  if (lang === 'en' && onDe) {
    window.location.href = '/';
    return;
  }

  applyTranslations(lang);
  updateToggle(lang);
}

document.addEventListener('DOMContentLoaded', function () {
  var lang = getLanguage();

  // If on root and user prefers German, redirect to /de
  if (lang === 'en') {
    var stored = localStorage.getItem('lang');
    if (stored === 'de') {
      window.location.href = '/de';
      return;
    }
  }

  applyTranslations(lang);
  updateToggle(lang);

  // Attach click handlers to toggle links
  document.querySelectorAll('.lang-option').forEach(function (el) {
    el.addEventListener('click', function (e) {
      e.preventDefault();
      setLanguage(el.getAttribute('data-lang'));
    });
  });
});
