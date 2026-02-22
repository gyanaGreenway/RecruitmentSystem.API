// Custom JavaScript for Swagger UI Enhancements

let swaggerUIInstance = null;
let currentView = 'swagger';
const specUrl = '/swagger/v1/swagger.json';

function showVendorError(msg) {
    const loader = document.getElementById('copilot-loader');
    if (loader) loader.style.display = 'none';
    const e = document.getElementById('copilot-error');
    if (e) {
        e.style.display = 'block';
        e.textContent = msg + ' Check console for errors.';
    }
}

function showDebugBanner(msg) {
    let b = document.getElementById('copilot-debug');
    if (!b) {
        b = document.createElement('div');
        b.id = 'copilot-debug';
        b.style.position = 'fixed';
        b.style.left = '320px';
        b.style.top = '8px';
        b.style.right = '16px';
        b.style.zIndex = 99999;
        b.style.padding = '8px 12px';
        b.style.background = 'rgba(255,255,255,0.95)';
        b.style.border = '1px solid #eee';
        b.style.borderRadius = '6px';
        b.style.boxShadow = '0 2px 8px rgba(0,0,0,0.06)';
        b.style.fontSize = '13px';
        document.body.appendChild(b);
    }
    b.textContent = msg;
}

function checkSpecAvailability() {
    // quick diagnostic fetch to show status
    fetch(specUrl, { cache: 'no-store' })
        .then(async resp => {
            if (resp.ok) {
                showDebugBanner('OpenAPI JSON loaded (200).');
                console.info('OpenAPI JSON OK');
            } else {
                const text = await resp.text().catch(() => '');
                showDebugBanner(`OpenAPI JSON fetch failed: ${resp.status} ${resp.statusText}`);
                console.error('OpenAPI JSON response body:', text);
            }
        })
        .catch(err => {
            showDebugBanner('OpenAPI JSON fetch error: ' + err.message);
            console.error('OpenAPI JSON fetch error', err);
        });
}

function renderFallbackSpec() {
    // Fetch the OpenAPI spec and render a simple readable list
    fetch(specUrl)
        .then(resp => {
            if (!resp.ok) throw new Error('Spec fetch failed: ' + resp.status);
            return resp.json();
        })
        .then(spec => {
            const content = document.querySelector('.content');
            if (!content) return;
            content.innerHTML = '';

            const header = document.createElement('div');
            header.style.marginBottom = '12px';
            header.innerHTML = `<h2 style="margin:0 0 8px 0">${spec.info?.title || 'API'} <small style="font-weight:400;color:#666;margin-left:8px">${spec.info?.version || ''}</small></h2><p style="margin:0 0 12px 0;color:#444">${spec.info?.description || ''}</p>`;
            content.appendChild(header);

            const list = document.createElement('div');
            list.style.display = 'grid';
            list.style.gridTemplateColumns = '1fr';
            list.style.gap = '8px';

            const paths = spec.paths || {};
            Object.keys(paths).forEach(p => {
                const methods = paths[p];
                const card = document.createElement('div');
                card.style.padding = '10px';
                card.style.border = '1px solid #e6e6e6';
                card.style.borderRadius = '6px';
                card.style.background = '#fff';

                const title = document.createElement('div');
                title.innerHTML = `<strong>${p}</strong>`;
                card.appendChild(title);

                const ul = document.createElement('ul');
                ul.style.margin = '6px 0 0 18px';
                ul.style.padding = '0';
                for (const m of Object.keys(methods)) {
                    const li = document.createElement('li');
                    li.style.listStyle = 'none';
                    li.style.margin = '4px 0';
                    li.innerHTML = `<span style="display:inline-block;width:80px;font-weight:600;color:#fff;background:#6c757d;padding:3px 8px;border-radius:4px;text-transform:uppercase">${m}</span> <span style="margin-left:10px;color:#333">${methods[m].summary || methods[m].description || ''}</span>`;
                    ul.appendChild(li);
                }
                card.appendChild(ul);
                list.appendChild(card);
            });

            content.appendChild(list);
            showToast('Rendered fallback API view from OpenAPI JSON');
        })
        .catch(err => {
            console.error('Fallback spec render failed', err);
            showVendorError('Unable to load API spec for fallback view.');
        });
}

function createSwaggerUI() {
    try {
        if (typeof SwaggerUIBundle === 'undefined') throw new Error('SwaggerUIBundle not available');
        const ui = SwaggerUIBundle({
            url: specUrl,
            dom_id: '#swagger-ui',
            deepLinking: true,
            presets: [
                SwaggerUIBundle.presets.apis,
                SwaggerUIStandalonePreset
            ],
            layout: 'BaseLayout',
            docExpansion: 'list',
            requestInterceptor: (req) => {
                // Add auth header automatically if stored
                const token = localStorage.getItem('swagger_jwt');
                if (token) req.headers['Authorization'] = 'Bearer ' + token;
                return req;
            }
        });

        swaggerUIInstance = ui;
        // hide loader and error when UI created
        const loader = document.getElementById('copilot-loader'); if (loader) loader.style.display = 'none';
        const err = document.getElementById('copilot-error'); if (err) err.style.display = 'none';
        return ui;
    } catch (err) {
        console.error('Failed to initialize Swagger UI', err);
        // fallback to rendering spec directly so user can still see API
        renderFallbackSpec();
        return null;
    }
}

function showSwagger() {
    if (currentView === 'swagger') return;
    currentView = 'swagger';
    document.querySelectorAll('.menu-btn').forEach(b => b.classList.remove('active'));
    const swBtn = document.getElementById('show-swagger'); if (swBtn) swBtn.classList.add('active');

    const content = document.querySelector('.content');
    if (!content) return;
    content.innerHTML = '<div id="copilot-loader">Loading documentation...</div><div id="copilot-error" style="display:none"></div><div id="swagger-ui" class="swagger-panel"></div>';
    // Wait for vendor then init
    waitForVendorInit().then(() => {
        const ui = createSwaggerUI();
        if (!ui) showToast('Falling back to basic API view');
        const q = document.getElementById('api-search')?.value.trim();
        if (q) applySearchFilter(q);
    }).catch(err => {
        console.error(err);
        showVendorError('Vendor scripts did not load. Showing fallback.');
        renderFallbackSpec();
    });
}

function showRedoc() {
    if (currentView === 'redoc') return;
    currentView = 'redoc';
    document.querySelectorAll('.menu-btn').forEach(b => b.classList.remove('active'));
    const rdBtn = document.getElementById('show-redoc'); if (rdBtn) rdBtn.classList.add('active');
    const content = document.querySelector('.content');
    if (!content) return;
    content.innerHTML = '<div id="copilot-loader">Loading documentation...</div><div id="copilot-error" style="display:none"></div><div id="redoc-container"></div>';
    waitForVendorInit().then(() => {
        if (typeof Redoc === 'undefined') {
            showVendorError('Failed to load ReDoc scripts. Showing fallback.');
            renderFallbackSpec();
            return;
        }
        // Load ReDoc with lazy fetch
        try {
            Redoc.init(specUrl, { scrollYOffset: 50, theme: { typography: { fontSize: '14px' } } }, document.getElementById('redoc-container'));
            const loader = document.getElementById('copilot-loader'); if (loader) loader.style.display = 'none';
        } catch (err) {
            console.error('Failed to initialize ReDoc', err);
            showVendorError('Failed to initialize ReDoc. Showing fallback.');
            renderFallbackSpec();
        }
    }).catch(err => {
        console.error(err);
        showVendorError('Vendor scripts did not load. Showing fallback.');
        renderFallbackSpec();
    });
}

function waitForVendorInit(timeoutMs = 10000) {
    return new Promise((resolve, reject) => {
        const start = Date.now();
        const iv = setInterval(() => {
            if (typeof SwaggerUIBundle !== 'undefined') {
                clearInterval(iv);
                resolve();
                return;
            }
            if (Date.now() - start > timeoutMs) {
                clearInterval(iv);
                reject(new Error('Vendor scripts did not load in time'));
            }
        }, 200);
    });
}

function applySearchFilter(query) {
    // Use Swagger UI filter if available (ui.getSystem().fn) or perform simple DOM search
    if (swaggerUIInstance && swaggerUIInstance.getSystem) {
        try {
            const system = swaggerUIInstance.getSystem();
            if (system && system.fn && typeof system.fn.filterTags === 'function') {
                // prefer built-in tagging filter (some builds expose this)
                system.fn.filterTags(query);
                return;
            }
        } catch (e) {
            // fallback to DOM filtering below
        }
    }

    // Simple DOM-based filter: show operations that match
    const text = query.toLowerCase();
    document.querySelectorAll('.opblock').forEach(block => {
        const summary = block.innerText.toLowerCase();
        if (!text || summary.includes(text)) {
            block.style.display = '';
        } else {
            block.style.display = 'none';
        }
    });
}

function authorizeWithToken(token) {
    // persist
    if (token) {
        localStorage.setItem('swagger_jwt', token);
    } else {
        localStorage.removeItem('swagger_jwt');
    }

    // try to open SwaggerUI auth modal programmatically
    if (swaggerUIInstance && typeof swaggerUIInstance.preauthorizeApiKey === 'function') {
        try {
            swaggerUIInstance.preauthorizeApiKey('Bearer', token ? 'Bearer ' + token : '');
        } catch (e) {
            // older versions may not expose preauthorizeApiKey
        }
    }

    // Also update existing UI: some requests will pick up requestInterceptor
    // Show a quick toast
    showToast(token ? 'Token saved. Requests will include Authorization header.' : 'Token cleared.');
}

function showToast(message, ms = 3000) {
    let t = document.getElementById('copilot-toast');
    if (!t) {
        t = document.createElement('div');
        t.id = 'copilot-toast';
        t.style.position = 'fixed';
        t.style.right = '20px';
        t.style.bottom = '20px';
        t.style.padding = '10px 15px';
        t.style.background = 'rgba(45,55,72,0.9)';
        t.style.color = 'white';
        t.style.borderRadius = '6px';
        t.style.zIndex = 99999;
        document.body.appendChild(t);
    }
    t.textContent = message;
    t.style.opacity = '1';
    setTimeout(() => { t.style.opacity = '0'; }, ms);
}

function applyTheme(dark) {
    if (dark) {
        document.documentElement.classList.add('dark-theme');
        localStorage.setItem('swagger_theme_dark', '1');
    } else {
        document.documentElement.classList.remove('dark-theme');
        localStorage.removeItem('swagger_theme_dark');
    }
}

// Wire up UI controls after DOM ready
window.addEventListener('DOMContentLoaded', () => {
    // show debug of OpenAPI availability immediately
    checkSpecAvailability();

    // Initialize theme from localStorage
    const dark = localStorage.getItem('swagger_theme_dark') === '1';
    const themeToggle = document.getElementById('theme-toggle');
    if (themeToggle) themeToggle.checked = dark;
    applyTheme(dark);

    // Initialize JWT token field
    const storedToken = localStorage.getItem('swagger_jwt') || '';
    const jwtField = document.getElementById('jwt-token');
    if (jwtField) jwtField.value = storedToken;

    // Buttons
    const btn = document.getElementById('btn-authorize');
    if (btn) btn.addEventListener('click', () => {
        const token = document.getElementById('jwt-token')?.value.trim();
        authorizeWithToken(token);
    });

    const swBtn = document.getElementById('show-swagger');
    if (swBtn) swBtn.addEventListener('click', showSwagger);
    const rdBtn = document.getElementById('show-redoc');
    if (rdBtn) rdBtn.addEventListener('click', showRedoc);

    const search = document.getElementById('api-search');
    if (search) search.addEventListener('input', (e) => {
        const q = e.target.value.trim();
        applySearchFilter(q);
    });

    const theme = document.getElementById('theme-toggle');
    if (theme) theme.addEventListener('change', (e) => {
        applyTheme(e.target.checked);
    });

    // Wait for vendor scripts then load initial Swagger UI
    waitForVendorInit().then(() => {
        try {
            createSwaggerUI();
            setTimeout(() => showToast('Tip: paste JWT token and click Authorize to include auth in requests.'), 600);
        } catch (err) {
            console.error(err);
            // if initialization fails, render fallback
            renderFallbackSpec();
        }
    }).catch(err => {
        console.error(err);
        showVendorError('Failed to load vendor scripts (Swagger UI).');
        // render fallback so user can still see API
        renderFallbackSpec();
    });
});

// Also export small API for console debugging
window.CopilotUI = {
    showSwagger,
    showRedoc,
    authorizeWithToken,
    applySearchFilter
};

