// Sidebar Toggle (mobile)
const sidebar = document.getElementById('sidebar');
const btnHamburger = document.getElementById('btnHamburger');
btnHamburger?.addEventListener('click', () => {
    sidebar.classList.toggle('show');
});

// Dropdowns
const profileBtn = document.getElementById('profileBtn');
const profileMenu = document.getElementById('profileMenu');
profileBtn?.addEventListener('click', () => profileMenu.classList.toggle('show'));

const gearBtn = document.getElementById('gearBtn');
const gearMenu = document.getElementById('gearMenu');
gearBtn?.addEventListener('click', () => gearMenu.classList.toggle('show'));

document.addEventListener('click', (e) => {
    const withinProfile = profileBtn?.contains(e.target) || profileMenu?.contains(e.target);
    const withinGear = gearBtn?.contains(e.target) || gearMenu?.contains(e.target);
    if (!withinProfile) profileMenu?.classList.remove('show');
    if (!withinGear) gearMenu?.classList.remove('show');
});

// Theme demo toggle (light/dark sidebar)
gearMenu?.querySelectorAll('a').forEach(a => {
    a.addEventListener('click', (e) => {
        e.preventDefault();
        const theme = a.dataset.theme;
        if (theme === 'dark') {
            document.documentElement.style.setProperty('--sidebar', '#0b1220');
            document.documentElement.style.setProperty('--sidebar-2', '#050816');
        } else {
            document.documentElement.style.setProperty('--sidebar', '#111827');
            document.documentElement.style.setProperty('--sidebar-2', '#0f172a');
        }
    });
});

// Charts
const ctx = document.getElementById('salesChart');

const gradient = (ctx, colorStop = [['0', 'rgba(37,99,235,.35)'], ['1', 'rgba(37,99,235,0)']]) => {
    const g = ctx.getContext('2d').createLinearGradient(0, 0, 0, 220);
    colorStop.forEach(cs => g.addColorStop(parseFloat(cs[0]), cs[1]));
    return g;
};

const lineData = {
    labels: ['2012', '2013', '2014', '2015', '2016', '2017', '2018', '2019'],
    datasets: [{
        label: 'Revenue',
        data: [3200, 2900, 3600, 4800, 4200, 6100, 5400, 5900],
        fill: true,
        tension: .35,
        backgroundColor: (c) => gradient(ctx),
        borderColor: '#2563eb',
        pointBackgroundColor: '#2563eb',
        pointRadius: 4,
        borderWidth: 2
    }, {
        label: 'Orders',
        data: [2800, 3000, 3300, 4200, 3900, 5200, 4700, 5100],
        fill: true,
        tension: .35,
        backgroundColor: (c) => gradient(ctx, [['0', 'rgba(16,185,129,.35)'], ['1', 'rgba(16,185,129,0)']]),
        borderColor: '#10b981',
        pointBackgroundColor: '#10b981',
        pointRadius: 4,
        borderWidth: 2
    }]
};

const lineOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: { legend: { position: 'bottom' } },
    scales: {
        y: { grid: { color: '#eef2f7' }, ticks: { color: '#6b7280' } },
        x: { grid: { display: false }, ticks: { color: '#6b7280' } }
    }
};

let currentChart = new Chart(ctx, { type: 'line', data: lineData, options: lineOptions });

// Sidebar nav: toggle submenus (accordion style)
document.querySelectorAll('.sidebar .nav .dropdown > a').forEach(link => {
    link.addEventListener('click', e => {
        // If mobile view, toggle the dropdown
        if (window.innerWidth <= 900) {
            e.preventDefault();
            const item = link.parentElement;

            // Close other groups (accordion)
            document.querySelectorAll('.sidebar .nav .dropdown.open').forEach(other => {
                if (other !== item) other.classList.remove('open');
            });

            item.classList.toggle('open');
        }
    });
});


