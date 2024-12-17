(() => {
    'use strict';

    const getStoredTheme = () => localStorage.getItem('theme');
    const setStoredTheme = theme => localStorage.setItem('theme', theme);

    const getPreferredTheme = () => {
        const storedTheme = getStoredTheme();
        if (storedTheme) {
            return storedTheme;
        }

        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    };

    const setTheme = theme => {
        if (theme === 'auto') {
            document.documentElement.setAttribute('data-bs-theme', window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light');
        } else {
            document.documentElement.setAttribute('data-bs-theme', theme);
        }
    };

    setTheme(getPreferredTheme());

    const showActiveTheme = (theme, focus = false) => {
        const themeSwitcher = document.querySelector('#bd-theme');

        if (!themeSwitcher) {
            return;
        }

        const themeSwitcherText = document.querySelector('#bd-theme-text');
        const activeThemeIcon = document.querySelector('.theme-icon-active'); // Chọn icon chính xác
        const btnToActive = document.querySelector(`[data-bs-theme-value="${theme}"]`);

        if (!btnToActive || !activeThemeIcon) {
            console.warn('Theme button or active icon not found.');
            return;
        }

        const svgOfActiveBtn = btnToActive.querySelector('.bi'); // Tìm icon qua class `.bi`
        if (!svgOfActiveBtn) {
            console.warn('Icon not found in button.');
            return;
        }

        // Cập nhật class cho icon chính
        activeThemeIcon.className = `${svgOfActiveBtn.className} theme-icon-active`;

        // Cập nhật trạng thái của các nút
        document.querySelectorAll('[data-bs-theme-value]').forEach(element => {
            element.classList.remove('active');
            element.setAttribute('aria-pressed', 'false');
        });

        btnToActive.classList.add('active');
        btnToActive.setAttribute('aria-pressed', 'true');

        const themeSwitcherLabel = `${themeSwitcherText ? themeSwitcherText.textContent : ''} (${btnToActive.dataset.bsThemeValue})`;
        themeSwitcher.setAttribute('aria-label', themeSwitcherLabel);

        if (focus) {
            themeSwitcher.focus();
        }
    };

    // Sự kiện DOMContentLoaded
    window.addEventListener('DOMContentLoaded', () => {
        showActiveTheme(getPreferredTheme()); // Hiển thị theme hiện tại

        document.querySelectorAll('[data-bs-theme-value]').forEach(toggle => {
            toggle.addEventListener('click', () => {
                const theme = toggle.getAttribute('data-bs-theme-value');
                setStoredTheme(theme);
                setTheme(theme);
                showActiveTheme(theme, true);
            });
        });
    });
})();

document.addEventListener('DOMContentLoaded', () => {
    const navbar = document.querySelector('.navbar');
    const sidebar = document.querySelector('.sidebar');

    const updateSidebarMargin = () => {
        if (navbar && sidebar) {
            const navbarHeight = navbar.offsetHeight;
            sidebar.style.marginTop = `${navbarHeight}px`;
            sidebar.style.height = `calc(100vh - ${navbarHeight}px)`; // Đảm bảo sidebar luôn vừa với viewport
        }
    };

    // Cập nhật margin ngay khi trang tải
    updateSidebarMargin();

    // Cập nhật margin khi cửa sổ thay đổi kích thước
    window.addEventListener('resize', updateSidebarMargin);
});
